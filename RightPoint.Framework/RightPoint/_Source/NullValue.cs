using System;

namespace RightPoint
{
	/// <summary>
	/// Summary description for NullValue.
	/// </summary>
	public sealed class NullValue
	{

		public static System.Byte Byte
		{
			get
			{
				return System.Byte.MinValue;
			}
		}

		public static System.Int16 Int16
		{
			get
			{
				return System.Int16.MinValue;
			}
		}

		public static System.Int32 Int32
		{
			get
			{
				return System.Int32.MinValue;
			}
		}

		public static System.Int64 Int64
		{
			get
			{
				return System.Int64.MinValue;
			}
		}

		public static System.Decimal Decimal
		{
			get
			{
				return System.Decimal.MinValue;
			}
		}

		public static System.String String
		{
			get
			{
				return null;
			}
		}

		public static System.DateTime DateTime
		{
			get
			{
				return System.DateTime.MinValue;
			}
		}


		public static System.Double Double
		{
			get
			{
				return System.Double.MinValue;
			}
		}

		public static System.Byte [] ByteArray
		{
			get
			{
				return null;
			}
		}

		public static System.Boolean Boolean
		{
			get
			{
				// use false as a MinValue
				return false;		
			}	
		}

		public static System.Guid Guid
		{
			get
			{
				return System.Guid.Empty;		
			}	
		}

	}
}
