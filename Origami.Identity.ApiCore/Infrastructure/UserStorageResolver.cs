using Microsoft.Extensions.Configuration;
using System;

namespace Origami.Identity.Api.Core.Infrastructure
{
    /// <summary>
    /// Consolidación de blobs (dev): un solo contenedor (origami-dev), carpeta usuarios/, y en BD se guarda RUTA relativa.
    /// Se activa SOLO si existe Storage:ConsolidatedContainer (appsettings.Development.json). En qa/main no existe esa
    /// clave (usan otro ambiente) => todo queda EXACTAMENTE igual que hoy (contenedor por tipo + URL absoluta).
    /// Único lugar donde se arma la URL; el día que se pase a privado/SAS, se cambia solo aquí.
    /// </summary>
    public static class UserStorageResolver
    {
        public const string UsersFolder = "usuarios";

        private static string _blobBaseUri;
        private static string _consolidatedContainer;

        public static void Init(IConfiguration configuration)
        {
            _blobBaseUri = configuration["Storage:BlobBaseUri"];
            _consolidatedContainer = configuration["Storage:ConsolidatedContainer"];
        }

        public static bool ConsolidationEnabled => !string.IsNullOrWhiteSpace(_consolidatedContainer);
        public static string ConsolidatedContainer => _consolidatedContainer;

        // Valor guardado (ruta relativa dev, o URL http heredada/qa/main) => URL usable.
        public static string ResolveUrl(string stored)
        {
            if (string.IsNullOrWhiteSpace(stored)) return stored;

            if (stored.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                stored.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return stored;

            if (string.IsNullOrWhiteSpace(_blobBaseUri) || string.IsNullOrWhiteSpace(_consolidatedContainer))
                return stored;

            return $"{_blobBaseUri.TrimEnd('/')}/{_consolidatedContainer}/{EncodePath(stored)}";
        }

        // Normaliza a ruta relativa para GUARDAR (round-trip: el front reenvía la URL absoluta que recibió al subir).
        public static string ToRelativePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !ConsolidationEnabled)
                return value;

            if (string.IsNullOrWhiteSpace(_blobBaseUri))
                return value;

            var prefix = $"{_blobBaseUri.TrimEnd('/')}/{_consolidatedContainer}/";
            if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return Uri.UnescapeDataString(value.Substring(prefix.Length));

            return value;
        }

        public static string BuildRelativePath(string folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folder)) return fileName;
            return $"{folder.Trim('/')}/{fileName}";
        }

        private static string EncodePath(string relativePath)
        {
            var segments = relativePath.Split('/');
            for (int i = 0; i < segments.Length; i++)
                segments[i] = Uri.EscapeDataString(segments[i]);
            return string.Join("/", segments);
        }
    }
}
