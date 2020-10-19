using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;

namespace RightPoint.Config
{
	public abstract class ConfigSection<T> : ConfigurationSection, ISupportFieldValueLoading, ISupportRefresh where T : ConfigurationSection, ISupportRefresh
	{
		private Boolean _configSectionLoaded = false;
		private List<String> _cdataFieldNames = new List<String>();
		private String _cacheKey = String.Empty;

		public String CacheKey
		{
			get { return _cacheKey; }
			set { _cacheKey = value; }
		}
		public ConfigSection ()
		{
			if ( _configSectionLoaded == false )
			{
				foreach ( FieldInfo fieldInfo in this.GetType().GetFields() )
				{
					if (ConfigManager.ExcludedTypes.Contains(fieldInfo.FieldType))
					{
						continue;
					}
					string fieldName = fieldInfo.Name;
					fieldName = Char.ToLower(fieldName[0]) + fieldName.Substring(1);

					if (fieldInfo.FieldType.IsSubclassOf(typeof(ConfigElement))
						|| fieldInfo.FieldType.IsSubclassOf(typeof(ConfigurationElementCollection)))
				{
						this.Properties.Add(new ConfigurationProperty(fieldName.ToString(), fieldInfo.FieldType));

					foreach ( Machine machine in ConfigManager.Machines )
					{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + machine.MachineName, fieldInfo.FieldType));
						
					}

					foreach ( WebHost webHost in ConfigManager.WebHosts )
					{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + webHost.WebHostPart, fieldInfo.FieldType, null));
					}

						foreach (Application application in ConfigManager.Applications)
						{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + application.ApplicationName, fieldInfo.FieldType, null));
						}

						foreach (string machineType in Enum.GetNames(typeof(RightPoint.MachineType)))
						{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + machineType, fieldInfo.FieldType));

					}
				}
				else
				{
					ConfigurationPropertyOptions options = ConfigurationPropertyOptions.None;
					if ( fieldInfo.GetCustomAttributes( typeof( OptionalAttribute ), true ).Length == 0 )
						options = ConfigurationPropertyOptions.IsRequired;

						this.Properties.Add(new ConfigurationProperty(fieldName.ToString(), fieldInfo.FieldType, fieldInfo.GetValue(this), options));

					foreach ( Machine machine in ConfigManager.Machines )
					{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + machine.MachineName, fieldInfo.FieldType, null));
					}

					foreach ( WebHost webHost in ConfigManager.WebHosts )
					{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + webHost.WebHostPart, fieldInfo.FieldType, null));
					}

						foreach (Application application in ConfigManager.Applications)
						{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + application.ApplicationName, fieldInfo.FieldType, null));
						}

						foreach (string machineType in Enum.GetNames(typeof(RightPoint.MachineType)))
						{
							this.Properties.Add(new ConfigurationProperty(fieldName.ToString() + "_" + machineType, fieldInfo.FieldType, null));
						}
					}
				}
			}
			_configSectionLoaded = true;
		}

		protected static T _instance = null;

		public static T Instance
		{
			get
			{
				if ( _instance == null )
				{                    
					    _instance = RightPoint.Config.ConfigManager.GetSection<T>();
				}
				return _instance;
			}
		}

		public static void SetInstance<U> () where U : T
		{
			_instance = RightPoint.Config.ConfigManager.GetSection<U>();
		}

		public static void ClearInstance ()
		{
			_instance = null;
		}

#if DEBUG

		/// <summary>
		/// UNIT TEST USE ONLY - wrap calls in #if DEBUG: This method enables unit tests to get config values for a specific environment type.
		/// </summary>
		/// <param name="machineType"></param>
		public static T GetInstance(String machineName, MachineType machineType)
		{
			return RightPoint.Config.ConfigManager.GetSection<T>( machineName, machineType );
		}

#endif

		public void Refresh ()
		{
			ConfigurationManager.RefreshSection( this.GetType().FullName );
			_instance = null;
		}

		public void LoadConfiguartionValuesIntoFields(String machineName, MachineType machineType)
		{
			foreach ( FieldInfo fieldInfo in this.GetType().GetFields() )
			{
				string fieldName = fieldInfo.Name;
				fieldName = Char.ToLower(fieldName[0]) + fieldName.Substring(1);

				Object val = null;

				String customKey = fieldName + "_" + machineName;
				if (this.Properties.Contains(customKey)
					&& (fieldInfo.FieldType.Equals(typeof(CDataConfigElement)) || fieldInfo.FieldType.GetInterface(typeof(ISupportFieldValueLoading).Name) == null))
				{
					val = base[customKey];
				}

				if (String.IsNullOrEmpty(ConfigManager.Application) == false)
				{
					customKey = fieldName + "_" + ConfigManager.Application;
					if ((val == null || (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0) || (val is CDataConfigElement && ((CDataConfigElement)val).Value == null))
						&& this.Properties.Contains(customKey)
						&& (fieldInfo.FieldType.Equals(typeof(CDataConfigElement)) || fieldInfo.FieldType.GetInterface(typeof(ISupportFieldValueLoading).Name) == null))
					{
						val = base[customKey];
					}
				}


				if (HttpContext.Current != null)
				{
					try
				{
						foreach (WebHost webHost in ConfigManager.WebHosts)
					{
							if (HttpContext.Current.Request.Url.Host.Contains(webHost.WebHostPart))
						{
							customKey = fieldName + "_" + webHost.WebHostPart;
								if ((val == null || (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0) || (val is CDataConfigElement && ((CDataConfigElement)val).Value == null))
									&& this.Properties.Contains(customKey)
									&& (fieldInfo.FieldType.Equals(typeof(CDataConfigElement)) || fieldInfo.FieldType.GetInterface(typeof(ISupportFieldValueLoading).Name) == null))
							{
								val = base[customKey];
							}
						}
						}
					}
					catch (System.Web.HttpException)
					{

					}
				}

				customKey = fieldName + "_" + machineType.ToString();
				if ((val == null || (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0) || (val is CDataConfigElement && ((CDataConfigElement)val).Value == null))
					&& this.Properties.Contains(customKey)
					&& (fieldInfo.FieldType.Equals(typeof(CDataConfigElement)) || fieldInfo.FieldType.GetInterface(typeof(ISupportFieldValueLoading).Name) == null))
				{
					val = base[customKey];
				}

				if ((val == null || (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0) || (val is CDataConfigElement && ((CDataConfigElement)val).Value == null))
					&& this.Properties.Contains(fieldName))
				{
					val = base[fieldName];
				}

				fieldInfo.SetValue( this, val );
			}
		}

		protected override void DeserializeElement(XmlReader reader, Boolean serializeCollectionKey)
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
					if (_cdataFieldNames.Contains(name))
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

        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            LoadConfiguartionValuesIntoFields(ConfigManager.MachineName, ConfigManager.MachineType);

            OnLoadComplete();
        }

        /// <summary>
        /// Override this to initialize data in your configuration element after it has been loaded from the data source.
        /// The framework will call this method automatically after the element's data has been loaded.
        /// </summary>
        public virtual void OnLoadComplete()
        {
        }

		protected override bool OnDeserializeUnrecognizedAttribute(String name, String value)
		{
			try
			{
				String[] nameParts = name.Split('_');
				//Int32 length = (name.Contains( "_" ) ? name.IndexOf( '_' ) : name.Length) - 1;
				String fieldName = nameParts[0][0].ToString().ToUpper() + nameParts[0].Substring(1);
				FieldInfo fieldInfo = this.GetType().GetField(fieldName);

				this.Properties.Add(new ConfigurationProperty(name, fieldInfo.FieldType));

				if ( fieldInfo.FieldType.FullName == @"System.Text.RegularExpressions.Regex" )
				{
					base[name] = new Regex( value );
				}
			}
			catch ( Exception ex )
			{
				ex.Data.Add("name", name);
				ex.Data.Add("value", value);
				ex.Data.Add("Type", this.GetType().FullName);
				throw;
			}

			return true;
		}
	}
}
