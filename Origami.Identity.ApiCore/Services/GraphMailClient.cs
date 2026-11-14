using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Origami.Identity.Api.Core.Services
{
    /// <summary>
    /// Envío de correo por Microsoft Graph (<c>POST /users/{remitente}/sendMail</c>) con permiso de
    /// aplicación <c>Mail.Send</c>. Sustituye al SMTP con usuario y contraseña, que los tenants de
    /// Microsoft 365 tienen apagado.
    ///
    /// Configuración, sección <c>GraphMail</c>: TenantId, ClientId, ClientSecret y Sender (el buzón
    /// desde el que sale el correo). El secreto vive en los App Settings o en appsettings.Local.json,
    /// nunca en el repo.
    /// </summary>
    public static class GraphMailClient
    {
        // Graph acepta adjuntos en el mismo envío hasta ~3 MB; más grandes requieren otra forma de subida.
        private const int MaxAdjuntoBytes = 3 * 1024 * 1024;

        private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        private static readonly SemaphoreSlim CandadoToken = new SemaphoreSlim(1, 1);
        private static string? _token;
        private static string? _tokenClave;
        private static DateTimeOffset _tokenVence = DateTimeOffset.MinValue;

        public sealed record Configuracion(string TenantId, string ClientId, string ClientSecret, string Sender);

        public sealed record Adjunto(string Nombre, string ContentType, byte[] Contenido);

        public static Configuracion LeerConfiguracion(IConfiguration configuration)
        {
            string Requerido(string clave) =>
                configuration[$"GraphMail:{clave}"] is { Length: > 0 } valor
                    ? valor.Trim()
                    : throw new InvalidOperationException($"Falta GraphMail:{clave} para enviar correo por Microsoft Graph.");

            return new Configuracion(Requerido("TenantId"), Requerido("ClientId"), Requerido("ClientSecret"), Requerido("Sender"));
        }

        public static async Task EnviarAsync(Configuracion config, string para, string asunto, string cuerpoHtml,
            Adjunto? adjunto = null, CancellationToken cancellationToken = default)
        {
            if (adjunto != null && adjunto.Contenido.Length > MaxAdjuntoBytes)
                throw new InvalidOperationException($"El adjunto {adjunto.Nombre} pesa {adjunto.Contenido.Length / 1024} KB; Microsoft Graph acepta hasta 3 MB en un solo envío.");

            var mensaje = new JsonObject
            {
                ["subject"] = asunto,
                ["body"] = new JsonObject { ["contentType"] = "HTML", ["content"] = cuerpoHtml },
                ["toRecipients"] = new JsonArray(new JsonObject { ["emailAddress"] = new JsonObject { ["address"] = para } })
            };
            if (adjunto != null)
            {
                mensaje["attachments"] = new JsonArray(new JsonObject
                {
                    ["@odata.type"] = "#microsoft.graph.fileAttachment",
                    ["name"] = adjunto.Nombre,
                    ["contentType"] = adjunto.ContentType,
                    ["contentBytes"] = Convert.ToBase64String(adjunto.Contenido)
                });
            }
            var cuerpo = new JsonObject { ["message"] = mensaje, ["saveToSentItems"] = false };

            string token = await ObtenerTokenAsync(config, cancellationToken);
            using var solicitud = new HttpRequestMessage(HttpMethod.Post,
                $"https://graph.microsoft.com/v1.0/users/{Uri.EscapeDataString(config.Sender)}/sendMail")
            {
                Content = new StringContent(cuerpo.ToJsonString(), Encoding.UTF8, "application/json")
            };
            solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var respuesta = await Http.SendAsync(solicitud, cancellationToken);
            if (!respuesta.IsSuccessStatusCode)
            {
                string detalle = await LeerErrorAsync(respuesta, cancellationToken);
                throw new InvalidOperationException($"Microsoft Graph rechazó el correo ({(int)respuesta.StatusCode}): {detalle}");
            }
        }

        private static async Task<string> ObtenerTokenAsync(Configuracion config, CancellationToken cancellationToken)
        {
            string clave = config.TenantId + "|" + config.ClientId;
            if (_token != null && _tokenClave == clave && DateTimeOffset.UtcNow < _tokenVence)
                return _token;

            await CandadoToken.WaitAsync(cancellationToken);
            try
            {
                if (_token != null && _tokenClave == clave && DateTimeOffset.UtcNow < _tokenVence)
                    return _token;

                using var formulario = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = config.ClientId,
                    ["client_secret"] = config.ClientSecret,
                    ["scope"] = "https://graph.microsoft.com/.default",
                    ["grant_type"] = "client_credentials"
                });
                using var respuesta = await Http.PostAsync(
                    $"https://login.microsoftonline.com/{Uri.EscapeDataString(config.TenantId)}/oauth2/v2.0/token",
                    formulario, cancellationToken);
                if (!respuesta.IsSuccessStatusCode)
                {
                    string detalle = await LeerErrorAsync(respuesta, cancellationToken);
                    throw new InvalidOperationException($"No se obtuvo el token de Microsoft para enviar correo ({(int)respuesta.StatusCode}): {detalle}");
                }

                using var json = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync(cancellationToken));
                string token = json.RootElement.GetProperty("access_token").GetString()
                    ?? throw new InvalidOperationException("Microsoft no devolvió el token para enviar correo.");
                int segundos = json.RootElement.TryGetProperty("expires_in", out var vence) && vence.TryGetInt32(out int s) ? s : 3600;

                _token = token;
                _tokenClave = clave;
                _tokenVence = DateTimeOffset.UtcNow.AddSeconds(Math.Max(60, segundos - 300));
                return token;
            }
            finally
            {
                CandadoToken.Release();
            }
        }

        // Solo el código y el mensaje del error; nunca se registran encabezados ni el secreto.
        private static async Task<string> LeerErrorAsync(HttpResponseMessage respuesta, CancellationToken cancellationToken)
        {
            string texto = await respuesta.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                using var json = JsonDocument.Parse(texto);
                var raiz = json.RootElement;
                if (raiz.TryGetProperty("error", out var error))
                {
                    if (error.ValueKind == JsonValueKind.Object)
                        return $"{error.GetProperty("code").GetString()}: {(error.TryGetProperty("message", out var m) ? m.GetString() : string.Empty)}";
                    return $"{error.GetString()}: {(raiz.TryGetProperty("error_description", out var d) ? d.GetString()?.Split('\n')[0] : string.Empty)}";
                }
            }
            catch (JsonException)
            {
            }
            return texto.Length > 300 ? texto[..300] : texto;
        }
    }
}
