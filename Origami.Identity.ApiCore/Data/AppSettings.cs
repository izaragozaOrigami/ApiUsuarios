


using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Origami.Identity.Api.Core.Data
{
    public class AppSettings
    {
        private static AppSettings appSettings;
        private static IConfiguration _configuration;

        public string Value { get; set; }

        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Obtiene un valor de forma genérica
        public static string GetUrlBase(string section, string key)
        {
            if (_configuration == null)
                throw new InvalidOperationException("AppSettings no ha sido inicializado. Llama AppSettings.Init(builder.Configuration) en Program.cs");

            return _configuration.GetSection(section)[key];
        }

        //public AppSettings(IConfiguration config, string key)
        //{
        //    this.Value = config.GetValue<string>(key);
        //}

        //public static AppSettings GetUrlBase(string section, string key)
        //{
        //    var builder = new ConfigurationBuilder()
        //                    .SetBasePath(Directory.GetCurrentDirectory())
        //                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //                    .AddEnvironmentVariables();

        //    IConfigurationRoot configuration = builder.Build();

        //    var settings = new AppSettings(configuration.GetSection(section), key);

        //    return settings;
        //}
    }
}
