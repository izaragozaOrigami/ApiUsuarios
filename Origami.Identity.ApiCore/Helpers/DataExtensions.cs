using System.ComponentModel;
using System.Data;
using System.Reflection;


namespace Origami.Identity.Api.Core.Infrastructure
{
    /// <summary>
    /// Clase de ayuda para objetos data sql
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Convierte una lista generica en una tabla
        /// </summary>
        public static DataTable AsDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));

            var table = new DataTable();

            // Crear columnas
            foreach (PropertyDescriptor prop in properties)
            {
                if (!prop.Attributes.OfType<DataTableIgnoreAttribute>().Any())
                {
                    table.Columns.Add(
                        prop.Name,
                        Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType
                    );
                }
            }

            // Crear filas
            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    if (!prop.Attributes.OfType<DataTableIgnoreAttribute>().Any())
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Convierte una tabla generica a una lista
        /// </summary>
        public static List<T> AsObject<T>(DataTable table)
        {
            List<T> data = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                data.Add(GetItem<T>(row));
            }

            return data;
        }

        /// <summary>
        /// Crea un objeto generico del tipo T de un registro de tabla
        /// </summary>
        private static T GetItem<T>(DataRow dataRow)
        {
            T obj = Activator.CreateInstance<T>();
            Type type = typeof(T);

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                PropertyInfo prop = type.GetProperty(column.ColumnName);
                if (prop != null && dataRow[column] != DBNull.Value)
                {
                    prop.SetValue(obj, dataRow[column]);
                }
            }

            return obj;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DataTableIgnoreAttribute : Attribute
    {
    }
}