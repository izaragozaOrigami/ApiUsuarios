namespace Origami.Identity.Api.Core.Data
{
    /// <summary>
    /// Clase que ayuda a obtener la cadena de conexion.
    /// </summary>
    public class DataHelper
    {
        /// <summary>
        /// Cadena de conexion BD
        /// </summary>
        private readonly IConfiguration _configuration;

        public DataHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString =>_configuration.GetConnectionString("DefaultConnection");


    }
}