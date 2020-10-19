using System;
using RightPoint;

namespace RightPoint.Data
{
	/// <summary>
	/// Summary description for ParseFromDbType.
	/// </summary>
	public sealed class ParseFromDbType
	{
		public static System.Boolean ToBoolean(object Value)
		{
			if( Value == DBNull.Value || Value == null )
				return(false);
			else
				return (System.Boolean) Value;
		}

		public static System.Byte ToByte(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.Byte;
			else
				return (System.Byte) Value;
		}

		public static System.Int16 ToInt16(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.Int16;
			else
				return (System.Int16) Value;
		}

		public static System.Int32 ToInt32(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.Int32;
			else
				return (System.Int32) Value;
		}

		public static System.Int64 ToInt64(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.Int64;
			else
				return (System.Int64) Value;
		}

		public static System.Decimal ToDecimal(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.Decimal;
			else
				return Convert.ToDecimal(Value);
		}

		public static System.String ToString(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.String;
			else
				return (System.String) Value;
		}

		public static System.DateTime ToDateTime(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.DateTime;
			else
				return (System.DateTime) Value;
		}

		public static System.Double ToDouble(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.Double;
			else
				return (System.Double) Value;
		}

		public static Byte [] ToByteArray(object Value)
		{
			if ( Value == DBNull.Value || Value == null )
				return NullValue.ByteArray;
			else
				return (Byte []) Value;
		}
	}
}
