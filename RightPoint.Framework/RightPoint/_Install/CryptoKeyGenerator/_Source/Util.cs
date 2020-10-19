using System;
using System.Collections;

namespace CryptoKeyGenerator
{
	/// <summary>
	/// Summary description for Util.
	/// </summary>
	public class Util
	{
		private Util()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		public static int GetHashValue(string value)
		{
			int returnValue = 0;

			foreach(char c in value)
			{
				returnValue += (byte) c ^ returnValue;
			}

			return returnValue;
		}

		public static string ByteArrayToString(byte [] bytes)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach( byte b in bytes )
			{
				if( sb.Length > 0 )
					sb.Append( ", " );

				sb.Append( "0x" + string.Format( "{0:X}", b ).PadLeft( 2, '0' ) );
			}

			return sb.ToString();
		}

		// TODO: TEST!!!!, get beta same as dev
		public static byte [] StringToByteArray(string bytes)
		{
			ArrayList returnValue = new ArrayList();

			foreach( string s in bytes.Split( ',' ) )
			{
				string hexValue = s.Replace( "0x", "" );
				returnValue.Add( (byte) byte.Parse( hexValue, System.Globalization.NumberStyles.HexNumber ) );
			}

			return (byte []) returnValue.ToArray( typeof(byte) );
		}
	}
}
