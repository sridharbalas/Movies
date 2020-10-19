using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace RightPoint.Config
{
	public abstract class ConfigElement : ConfigurationElement, ISupportFieldValueLoading
	{
		
		private List<String> _cdataFieldNames = new List<String>();

		internal Boolean IsDefault = true;

		public ConfigElement ()
		{
			string keyFieldName = null;

			if ( this is ConfigElementCollectionItem )
			{
				FieldInfo customKeyFieldInfo = null;
				foreach ( FieldInfo fieldInfo in this.GetType().GetFields() )
				{
					if ( fieldInfo.GetCustomAttributes( typeof( CustomKeyAttribute ), true ).Length > 0 )
					{
						customKeyFieldInfo = fieldInfo;
						break;
					}
				}

				if ( customKeyFieldInfo != null )
					keyFieldName = customKeyFieldInfo.Name;
				else
					keyFieldName = "Key";

				keyFieldName = Char.ToLower( keyFieldName[0] ) + keyFieldName.Substring( 1 );
				this.Properties.Add( new ConfigurationProperty( keyFieldName, typeof( string ), null, ConfigurationPropertyOptions.IsKey ) );
			}

			foreach ( FieldInfo fieldInfo in this.GetType().GetFields() )
			{
				if ( ConfigManager.ExcludedTypes.Contains( fieldInfo.FieldType ) )
				{
					continue;
				}

				string fieldName = fieldInfo.Name;
				fieldName = Char.ToLower( fieldName[0] ) + fieldName.Substring( 1 );

				if ( keyFieldName == fieldName || fieldInfo.FieldType == typeof( Regex ) )
					continue;

				ConfigurationPropertyOptions options = ConfigurationPropertyOptions.None;
				if ( fieldInfo.GetCustomAttributes( typeof( OptionalAttribute ), true ).Length == 0 )
					options = ConfigurationPropertyOptions.IsRequired;

				if ( fieldInfo.GetCustomAttributes( typeof( CDataAttribute ), true ).Length > 0 )
				{
					if ( _cdataFieldNames.Count > 0 )
					{
						throw new Exception( @"Only one property may have the CData attribute assigned." );
					}

					_cdataFieldNames.Add( fieldName );

					foreach ( Machine machine in ConfigManager.Machines )
					{
						_cdataFieldNames.Add( fieldName.ToString() + "_" + machine.MachineName );
					}

					foreach ( WebHost webHost in ConfigManager.WebHosts )
					{
						_cdataFieldNames.Add( fieldName.ToString() + "_" + webHost.WebHostPart );
					}

					foreach ( Application application in ConfigManager.Applications )
					{
						_cdataFieldNames.Add( fieldName.ToString() + "_" + application.ApplicationName );
					}

					foreach ( string machineType in Enum.GetNames( typeof( RightPoint.MachineType ) ) )
					{
						_cdataFieldNames.Add( fieldName.ToString() + "_" + machineType );
					}
				}

				this.Properties.Add( new ConfigurationProperty( fieldName.ToString(), fieldInfo.FieldType, fieldInfo.GetValue( this ), options ) );

				foreach ( Machine machine in ConfigManager.Machines )
				{
					this.Properties.Add( new ConfigurationProperty( fieldName.ToString() + "_" + machine.MachineName, fieldInfo.FieldType, null ) );
				}

				foreach ( WebHost webHost in ConfigManager.WebHosts )
				{
					this.Properties.Add( new ConfigurationProperty( fieldName.ToString() + "_" + webHost.WebHostPart, fieldInfo.FieldType, null ) );
				}

				foreach ( Application application in ConfigManager.Applications )
				{
					this.Properties.Add( new ConfigurationProperty( fieldName.ToString() + "_" + application.ApplicationName, fieldInfo.FieldType, null ) );
				}

				foreach ( string machineType in Enum.GetNames( typeof( RightPoint.MachineType ) ) )
					this.Properties.Add( new ConfigurationProperty( fieldName.ToString() + "_" + machineType, fieldInfo.FieldType, null ) );
			}
		}

		protected override void DeserializeElement ( XmlReader reader, Boolean serializeCollectionKey )
		{
			if ( _cdataFieldNames.Count == 0 )
			{
				base.DeserializeElement( reader, serializeCollectionKey );
			}
			else
			{
				foreach ( ConfigurationProperty configurationProperty in Properties )
				{
					String name = configurationProperty.Name;
					if ( _cdataFieldNames.Contains( name ) )
					{
						base[name] = reader.ReadString().Trim();
					}
					else
					{
						String attributeValue = reader.GetAttribute( name );
						if ( String.IsNullOrEmpty( attributeValue ) == false )
						{
							base[name] = configurationProperty.Converter.ConvertFromInvariantString( attributeValue );
						}
						else
						{
							base[name] = configurationProperty.DefaultValue;
						}
					}
				}
				//reader.ReadEndElement();
				PostDeserialize();
			}
		}

		protected override void PostDeserialize ()
		{
			base.PostDeserialize();

			LoadConfiguartionValuesIntoFields( ConfigManager.MachineName, ConfigManager.MachineType );

			IsDefault = false;

			OnLoadComplete();
		}

		protected override bool OnDeserializeUnrecognizedAttribute ( String name, String value )
		{
			String[] nameParts = name.Split( '_' );
			//Int32 length = (name.Contains( "_" ) ? name.IndexOf( '_' ) : name.Length) - 1;
			String fieldName = nameParts[0][0].ToString().ToUpper() + nameParts[0].Substring( 1 );
			FieldInfo fieldInfo = this.GetType().GetField( fieldName );

			this.Properties.Add( new ConfigurationProperty( name, fieldInfo.FieldType, null, new RegexTypeConverter(), new RegexValidator(), ConfigurationPropertyOptions.None ) );

			if ( fieldInfo.FieldType.FullName == @"System.Text.RegularExpressions.Regex" )
			{
				if ( value.Contains( "^" ) || value.Contains( "$" ) )
				{
					base[name] = new Regex( value, RegexOptions.Multiline | RegexOptions.CultureInvariant );
				}
				else
				{
					base[name] = new Regex( value, RegexOptions.CultureInvariant );
				}
			}

			return true;
		}

		protected override bool OnDeserializeUnrecognizedElement ( string elementName, XmlReader reader )
		{
			return base.OnDeserializeUnrecognizedElement( elementName, reader );
		}

		public void LoadConfiguartionValuesIntoFields ( String machineName, MachineType machineType )
		{
			FieldInfo customKeyFieldInfo = null;
			FieldInfo keyFieldInfo = null;

			foreach ( FieldInfo fieldInfo in this.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) )
			{
				if ( (fieldInfo.Attributes & FieldAttributes.Private) == FieldAttributes.Private )
					continue;

				string fieldName = fieldInfo.Name;
				fieldName = Char.ToLower( fieldName[0] ) + fieldName.Substring( 1 );

				Object val = null;

				String customKey = fieldName + "_" + machineName;
				if ( this.Properties.Contains( customKey )
					//&& fieldInfo.FieldType.GetInterface( typeof( ISupportFieldValueLoading ).Name ) == null
					/*&& fieldInfo.FieldType.IsSubclassOf( typeof( ConfigurationElementCollection ) ) == false*/ )
				{
					val = base[customKey];
					if ( val is ConfigElement && ((ConfigElement)val).IsDefault )
					{
						val = null;
					}
				}

				if ( String.IsNullOrEmpty( ConfigManager.Application ) == false )
				{
					customKey = fieldName + "_" + ConfigManager.Application;
					if ( (val == null || (val is String && String.IsNullOrEmpty( val as String ))
					|| (val is CDataConfigElement && ((CDataConfigElement)val).Value == null)
						|| (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0))
						&& this.Properties.Contains( customKey ) && fieldInfo.FieldType.GetInterface( typeof( ISupportFieldValueLoading ).Name ) == null )
					{
						val = base[customKey];
					}
				}

				if ( HttpContext.Current != null )
				{
					try
					{
						foreach ( WebHost webHost in ConfigManager.WebHosts )
						{
							if ( HttpContext.Current.Request.Url.Host.Contains( webHost.WebHostPart ) )
							{
								customKey = fieldName + "_" + webHost.WebHostPart;
								if ( (val == null || (val is String && String.IsNullOrEmpty( val as String ))
									|| (val is CDataConfigElement && ((CDataConfigElement)val).Value == null)
									|| (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0))
									&& this.Properties.Contains( customKey ) && fieldInfo.FieldType.GetInterface( typeof( ISupportFieldValueLoading ).Name ) == null )
								{
									val = base[customKey];
								}
							}
						}
					}
					catch ( System.Web.HttpException)
					{
					}
				}

				customKey = fieldName + "_" + machineType.ToString();
				if ( (val == null || (val is String && String.IsNullOrEmpty( val as String )
					|| (val is CDataConfigElement && ((CDataConfigElement)val).Value == null))
					|| (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0))
					&& this.Properties.Contains( customKey )
					&& (fieldInfo.FieldType.GetInterface( typeof( ISupportFieldValueLoading ).Name ) == null || fieldInfo.FieldType == typeof( CDataConfigElement ))
					/*&& fieldInfo.FieldType.IsSubclassOf( typeof( ConfigurationElementCollection ) ) == false*/ )
				{
					val = base[customKey];
				}

				//if (val == null || (val is String && String.IsNullOrEmpty((String)val)))
				if ( (val == null || (val is String && String.IsNullOrEmpty( val as String ))
					|| (val is CDataConfigElement && ((CDataConfigElement)val).Value == null)
					|| (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0))
					&& this.Properties.Contains( fieldName ) )
				{
					val = base[fieldName];
				}

				if ( fieldName == "key" )
				{
					if ( val == null )
					{
						val = Guid.NewGuid().ToString();
					}
					else
					{
						val = val.ToString().ToLower();
					}

					keyFieldInfo = fieldInfo;
				}

				fieldInfo.SetValue( this, val );

				if ( this is ConfigElementCollectionItem && fieldInfo.GetCustomAttributes( typeof( CustomKeyAttribute ), true ).Length > 0 )
					customKeyFieldInfo = fieldInfo;
			}

			// If the object being loaded is a config element collection item, and it has a custom key attribute on a field,
			// set the internal Key field value to match the value of the custom key field.
			if ( this is ConfigElementCollectionItem && customKeyFieldInfo != null )
			{
				keyFieldInfo.SetValue( this, customKeyFieldInfo.GetValue( this ) );
			}
		}

		/// <summary>
		/// Override this to initialize data in your configuration element after it has been loaded from the data source.
		/// The framework will call this method automatically after the element's data has been loaded.
		/// </summary>
		public virtual void OnLoadComplete ()
		{
		}
	}
}