using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Origami.Identity.Api.Core.Infrastructure.Security
{
    /// <summary>Lo que se toma de la cuenta de Microsoft una vez validado su id_token.</summary>
    public sealed record EntraIdentity(string Email, string Name, string ObjectId);

    /// <summary>
    /// Valida el id_token que Microsoft Entra ID entrega al front en el inicio de sesión con Microsoft.
    ///
    /// Revisa la firma contra las llaves públicas del tenant (se descargan y se refrescan solas),
    /// el emisor, la audiencia (el ClientId del registro de la app), la vigencia y que el token
    /// venga del tenant configurado. Configuración: <c>Sso:TenantId</c> y <c>Sso:ClientId</c>;
    /// ninguno es secreto. Si falta alguno, el inicio de sesión con Microsoft queda apagado.
    /// </summary>
    public sealed class EntraIdTokenValidator
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly ConfigurationManager<OpenIdConnectConfiguration>? _metadata;

        public EntraIdTokenValidator(IConfiguration configuration)
        {
            _tenantId = (configuration["Sso:TenantId"] ?? string.Empty).Trim();
            _clientId = (configuration["Sso:ClientId"] ?? string.Empty).Trim();

            if (Habilitado)
            {
                _metadata = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"https://login.microsoftonline.com/{_tenantId}/v2.0/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = true });
            }
        }

        public bool Habilitado => !string.IsNullOrEmpty(_tenantId) && !string.IsNullOrEmpty(_clientId);

        /// <summary>Devuelve la identidad si el token es válido; null si no lo es.</summary>
        public async Task<EntraIdentity?> ValidarAsync(string? idToken, CancellationToken cancellationToken)
        {
            if (!Habilitado || _metadata == null || string.IsNullOrWhiteSpace(idToken))
                return null;

            try
            {
                return await ValidarConLlavesAsync(idToken, cancellationToken);
            }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                // Microsoft rotó sus llaves: se piden de nuevo y se reintenta una vez.
                _metadata.RequestRefresh();
                try
                {
                    return await ValidarConLlavesAsync(idToken, cancellationToken);
                }
                catch (Exception ex) when (ex is SecurityTokenException || ex is ArgumentException)
                {
                    return null;
                }
            }
            catch (Exception ex) when (ex is SecurityTokenException || ex is ArgumentException)
            {
                return null;
            }
        }

        private async Task<EntraIdentity?> ValidarConLlavesAsync(string idToken, CancellationToken cancellationToken)
        {
            OpenIdConnectConfiguration oidc = await _metadata!.GetConfigurationAsync(cancellationToken);

            var parametros = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = oidc.Issuer,
                ValidateAudience = true,
                ValidAudience = _clientId,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2),
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = oidc.SigningKeys
            };

            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(idToken, parametros, out _);

            string tid = principal.FindFirst("tid")?.Value ?? string.Empty;
            if (!string.Equals(tid, _tenantId, StringComparison.OrdinalIgnoreCase))
                return null;

            // preferred_username es el correo con el que la persona entra a Microsoft; email y upn
            // quedan de respaldo porque no siempre vienen.
            string email = principal.FindFirst("preferred_username")?.Value
                ?? principal.FindFirst("email")?.Value
                ?? principal.FindFirst("upn")?.Value
                ?? string.Empty;
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return null;

            return new EntraIdentity(
                email.Trim(),
                principal.FindFirst("name")?.Value ?? string.Empty,
                principal.FindFirst("oid")?.Value ?? string.Empty);
        }
    }
}
