using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RightPoint.Config
{
	public class RegexTypeConverter : TypeConverter
	{
		public override Boolean CanConvertFrom ( ITypeDescriptorContext context, Type sourceType )
		{

			if ( sourceType == typeof( string ) )
			{
				return true;
			}
			return base.CanConvertFrom( context, sourceType );
		}
		
		public override object ConvertFrom ( ITypeDescriptorContext context, CultureInfo culture, object value )
		{
			if ( value is string )
			{
				return new Regex( (String)value );
			}
			return base.ConvertFrom( context, culture, value );
		}
		
		public override object ConvertTo ( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
		{
			if ( destinationType == typeof( string ) )
			{
				return ((Regex)value).ToString();
			}
			return base.ConvertTo( context, culture, value, destinationType );
		}
	}

	public class RegexValidator : ConfigurationValidatorBase
	{
		public override void Validate ( object value )
		{
			//throw new NotImplementedException();
		}

		public override bool CanValidate ( Type type )
		{
			return type.Equals( typeof( Regex ) );
		}
	}
}
