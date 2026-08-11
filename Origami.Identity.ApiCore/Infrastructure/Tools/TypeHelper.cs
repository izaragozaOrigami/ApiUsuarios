using System.Data;
using System.Reflection;

namespace Origami.Identity.Api.Core.Infrastructure
{



	/// <summary>
	/// Auxliliar de tipos
	/// </summary>
	public static class TypeHelper
	{
		/// <summary>
		/// Valida valores enteros de 64 bits
		/// </summary>
		/// <param name="valueLong">Enteros de 64 bits</param>
		/// <returns>Enteros de 64 bits o nulo</returns>
		public static long? ValidateLong(this long valueLong)
		{
			Int64? value = null;
			return valueLong > 0 ? Convert.ToInt64(valueLong) : value;
		}

		/// <summary>
		/// Valida valores enteros de 64 bits
		/// </summary>
		/// <param name="valueLong">Enteros de 64 bits</param>
		/// <returns>Enteros de 64 bits o nulo</returns>
		public static long? ValidateLong(this long? valueLong)
		{
			Int64? value = null;
			return valueLong > 0 ? Convert.ToInt64(valueLong) : value;
		}
		/// <summary>
		/// Valida valores enteros de 32 bits
		/// </summary>
		/// <param name="valueLong">Enteros de 32 bits</param>
		/// <returns>Enteros de 32 bits o nulo</returns>
		public static int? ValidateInt(this int valueInt)
		{
			int? value = null;
			return valueInt > -1 ? Convert.ToInt32(valueInt) : value;
		}

		/// <summary>
		/// Valida valores enteros de 16 bits
		/// </summary>
		/// <param name="valueLong">Enteros de 16 bits</param>
		/// <returns>Enteros de 16 bits o nulo</returns>
		public static short? ValidateShort(this short valueShort)
		{
			short? value = null;
			return valueShort > 0 ? Convert.ToInt16(valueShort) : value;
		}

		/// <summary>
		/// Valida valores enteros de 16 bits
		/// </summary>
		/// <param name="valueLong">Enteros de 16 bits</param>
		/// <returns>Enteros de 16 bits o nulo</returns>
		public static decimal? ValidateDecimal(this decimal valueDecimal)
		{
			decimal? value = 0;
			return valueDecimal > 0 ? Convert.ToDecimal(valueDecimal) : value;
		}

		/// <summary>
		/// Valida valores dobles decimales
		/// </summary>
		/// <param name="valueDouble">Valor doble</param>
		/// <returns>Valor doble</returns>
		public static double? ValidateDouble(this double valueDouble)
		{
			double? value = null;
			return valueDouble > 0 ? Convert.ToDouble(valueDouble) : value;
		}

		/// <summary>
		/// Valida valores de fecha 
		/// </summary>
		/// <param name="valueLong">Valor de fecha</param>
		/// <returns>Valor o nulo de fecha</returns>
		internal static DateTime? ValidateDateTime(this DateTime? valueDate)
		{
			DateTime? value = null;
			return valueDate > DateTime.MinValue ? valueDate : value;
		}

		/// <summary>
		/// Valida el tiempo
		/// </summary>
		/// <param name="valueTime">Valor de tiempo</param>
		/// <returns>Valor o nulo de tiempo</returns>
		internal static TimeSpan? ValidateTime(this TimeSpan? valueTime)
		{
			TimeSpan? value = null;
			return valueTime > TimeSpan.MinValue ? valueTime : value;
		}

		/// <summary>
		/// Valida valores de string 
		/// </summary>
		/// <param name="value">Valor de string</param>
		/// <returns>Valor o nulo de string</returns>
		internal static string ValidateString(this string value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : value;
		}

		/// <summary>
		/// Valida una cadena y la convierta a fecha tipo sql
		/// </summary>
		/// <param name="value">Valor de string</param>
		/// <returns>Valor o nulo de string</returns>
		internal static string ValidateDateSql(this DateTime value)
		{
			string dateSql = value.ToString("yyyy-MM-dd HH:mm:ss");
			return string.IsNullOrEmpty(dateSql) ? null : dateSql;
		}

		/// <summary>
		/// Valida valores boleanos 
		/// </summary>
		/// <param name="value">Valor  boleano</param>
		/// <returns>Valor o nulo de boleano</returns>
		internal static bool? ValidateBool(this bool? boolValue)
		{
			bool? value = null;
			return boolValue != null ? boolValue : value;
		}

	}
}