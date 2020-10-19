using System;
using System.Data;

using RightPoint;

namespace RightPoint.Data
{
	/// <summary>
	/// Summary description for ParseToDbType.
	/// </summary>
	public sealed class ParseToDbType
	{
		public static object FromType(DbType valueType, object Value)
		{
			object returnValue = null;

			switch(valueType)
			{
				case DbType.Boolean:
					returnValue = FromBoolean((bool) Value);
					break;

				case DbType.Byte:
					returnValue = FromByte((System.Byte) Value);
					break;

				case DbType.Int16:
					returnValue = FromInt16((System.Int16) Value);
					break;

				case DbType.Int32:
					returnValue = FromInt32((System.Int32) Value);
					break;

				case DbType.Int64:
					returnValue = FromInt64((System.Int64) Value);
					break;

				case DbType.Decimal:
					returnValue = FromDecimal((System.Decimal) Value);
					break;

				case DbType.String:
					returnValue = FromString((System.String) Value);
					break;

				case DbType.DateTime:
					returnValue = FromDateTime((System.DateTime) Value);
					break;

				case DbType.Binary:
					returnValue = FromByteArray((Byte []) Value);
					break;

				case DbType.Double:
					returnValue = FromDouble((System.Double) Value);
					break;

				default: 
					throw new NotSupportedException("The valueType specified is not supported at this time. Please add implementation for this type.");
			}
			
			return(returnValue);
		}


		public static object FromBoolean(System.Boolean Value)
		{
			return (object) Value;
		}
		
		public static object FromByte(System.Byte Value)
		{
			if ( Value == NullValue.Byte )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromInt16(System.Int16 Value)
		{
			if ( Value == NullValue.Int16 )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromGuid(System.Guid Value)
		{
			if ( Value == NullValue.Guid )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromInt32(System.Int32 Value)
		{
			if ( Value == NullValue.Int32 )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromInt64(System.Int64 Value)
		{
			if ( Value == NullValue.Int64 )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromDecimal(System.Decimal Value)
		{
			if ( Value == NullValue.Decimal )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromString(System.String Value)
		{
			if ( Value == NullValue.String )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromDateTime(System.DateTime Value)
		{
			if ( Value == NullValue.DateTime )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromByteArray(Byte [] Value)
		{
			if ( Value == NullValue.ByteArray )
				return DBNull.Value;
			else
				return (object) Value;
		}

		public static object FromDouble(System.Double Value)
		{
			if ( Value == NullValue.Double )
				return DBNull.Value;
			else
				return (object) Value;
		}

	}
}
