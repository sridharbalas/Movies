using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace RightPoint.Config
{
	public sealed class ConfigManager
	{
		private static FileSystemWatcher _fileSystemWatcher = null;
		private static Dictionary<String, ConfigurationSection> _sectionsLoaded = new Dictionary<String, ConfigurationSection>();
		private static Boolean _refreshingSections = false;

		private static DateTime _lastConfigurationLoadTime = DateTime.Now;
		
		private static string _application = null;
		public static string Application
		{
			get
			{
				return _application;
			}

			set
			{
				if ( String.IsNullOrEmpty( value ) )
				{
					_application = null;
					return;
				}
				Configuration config = ConfigManager.GetSection<Configuration>();
				if ( config != null )
				{
					foreach ( Application application in config.Applications )
					{
						if ( application.ApplicationName == value )
						{
							_application = value;
							return;
						}
					}

					throw new Exception( value + " is not in the <applications> section in core.config." );
				}

				throw new Exception( "Couldn't read applications section from core.config." );
			}
		}

		/// <summary>
		/// This property is updated any time a config file is reloaded from the config directory.
		/// </summary>
		public static DateTime LastConfigurationLoadTime
		{
			get
			{
				return _lastConfigurationLoadTime;
			}
		}

		public static String CoreConfigFileName
		{
			get
			{
				//if ( String.IsNullOrEmpty( ConfigurationManager.AppSettings["coreConfigFileName"] ) == false )
				//{
				//    _coreConfigFileName = ConfigurationManager.AppSettings["coreConfigFileName"];
				//}
				return @"Core.config";
			}
		}

		public static String ConfigFilesPath
		{
			get
			{
				String basePath = AppDomain.CurrentDomain.BaseDirectory;
				String webConfigPath = String.Empty;
				String configPath = Path.Combine( basePath, @"Config" );
				if (HttpContext.Current != null || Directory.Exists(configPath) == false)
				{
					webConfigPath = Path.Combine( basePath, @"bin\Config" );
					if ( Directory.Exists( webConfigPath ) )
					{
						return webConfigPath;
					}
				}
				return configPath;
			}//ConfigurationManager.AppSettings["configFilesPath"] ?? String.Empty; }
		}

		private static MachineType _machineType;

		/// <summary>
		/// This property is used to pass the machine type to the ConfigElement and ConfigElementCollection objects
		/// as they are reading the values after deserialization. Normally, this would be set to
		/// RightPoint.Configuratoin.MachineType, but to support unit tests, this extra functionality is needed
		/// to override the machine type during unit test runs.
		/// </summary>
		public static MachineType MachineType
		{
			get
			{
				return _machineType;
			}
		}

		private static String _machineName;

		public static String MachineName
		{
			get 
			{
				return ConfigManager._machineName; 
			}
		}

		internal static Boolean InternalLoggingEnabled
		{
			get
			{
				Configuration config = ConfigManager.GetSection<Configuration>( _machineName, _machineType );
				if ( config != null )
				{
					return config.InternalLoggingEnabled;
				}
				return false;
			}
		}

		internal static Machines Machines
		{
			get
			{
				Configuration config = ConfigManager.GetSection<Configuration>(_machineName, _machineType);
				if ( config != null )
				{
					return config.Machines;
				}
				return new Machines();
			}
		}

		internal static WebHosts WebHosts
		{
			get
			{
				Configuration config = ConfigManager.GetSection<Configuration>(_machineName, _machineType);
				if ( config != null )
				{
					return config.WebHosts;
				}
				return new WebHosts();
			}
		}

		internal static Applications Applications
		{
			get
			{
				Configuration config = ConfigManager.GetSection<Configuration>(_machineName, _machineType);
				if ( config != null )
				{
					return config.Applications;
				}
				return new Applications();
			}
		}

		internal static ExcludedTypes ExcludedTypes
		{
			get
			{
				Configuration config = ConfigManager.GetSection<Configuration>(_machineName, _machineType);
				if ( config != null )
				{
					return config.ExcludedTypes;
				}
				return new ExcludedTypes();
			}
		}

		static ConfigManager ()
		{
			if ( _fileSystemWatcher == null )
			{
				string configFilesPath = GetConfigurationPath();

                _fileSystemWatcher = new FileSystemWatcher(configFilesPath, "*.config")
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite
                };
                _fileSystemWatcher.Changed += new FileSystemEventHandler( OnConfigChanged );
				_fileSystemWatcher.EnableRaisingEvents = true;
			}
		}

		public static T GetSection<T> ( String machineName, MachineType machineType ) where T : ConfigurationSection, ISupportRefresh
		{
			String sectionName = typeof( T ).FullName;

            //section.AttachEnvironmentProperties(typeof(T));
            string sectionCacheKey = BuildCacheKey( sectionName, machineName, machineType.ToString() );
            T section = (T)GetConfigurationSection(sectionCacheKey);

            if ( section == null )
			{
				lock ( _sectionsLoaded ) // Only lock if there was no settings available.
				{
					// Because this is set inside the lock, there will only be one accessor to this value
					// while the config is loading, so that way ConfigSection, ConfigElement and ConfigElementCollection
					// can read the right values for the environment during their PostSerialize steps.
					_machineType = machineType;
					_machineName = machineName;

					// Check it again, to prevent the race condition.
					section = (T)GetConfigurationSection( sectionCacheKey );

					if ( section == null ) // If it's still null, then we can go ahead and load it.
					{
						string configFile = GetConfigurationPath( CoreConfigFileName );

                        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                        {
                            ExeConfigFilename = configFile
                        };

                        System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration( fileMap, ConfigurationUserLevel.None );

						try
						{
							section = (T)configuration.GetSection( sectionName );
						}
						catch ( ConfigurationErrorsException ceEx )
						{
							ceEx.Data.Add( "sectionName", sectionName );
							//Logger.LogError( ceEx );
							throw ceEx;
						}

//#if DEBUG
//                        if ( ConfigManager.InternalLoggingEnabled )
//                        Logging.Logger.LogDebug( "RightPoint.Config.ConfigManager::GetSection<" + typeof( T ).Name + ">(): Section Loaded: " + sectionName + " type: " + ((section == null) ? "null" : section.GetType().Name) );
//#endif

						UpdateConfigurationDictionary( sectionCacheKey, section );
					}
				}
			}

			return section;
		}

		public static T GetSection<T> () where T : ConfigurationSection, ISupportRefresh
		{
			return GetSection<T>(System.Environment.MachineName, RightPoint.Configuration.MachineType);
		}

		private static void UpdateConfigurationDictionary ( String sectionCacheKey, ConfigurationSection section )
		{
			if ( _sectionsLoaded.ContainsKey( sectionCacheKey ) == false )
				_sectionsLoaded.Add( sectionCacheKey, section );
			else
				_sectionsLoaded[sectionCacheKey] = section;
		}

		public static void RefeshSection ( ISupportRefresh sectionToRefresh )
		{
			if ( sectionToRefresh != null )
			{
				sectionToRefresh.Refresh();
			}
		}

		public static void RefreshAllSections ()
		{
			foreach ( String key in _sectionsLoaded.Keys )
			{
                if (_sectionsLoaded[key] is ISupportRefresh section)
                {
                    section.Refresh();
                }
            }
		}

		public static void ResetConfigManager()
		{
			_sectionsLoaded = new Dictionary<string, ConfigurationSection>();
		}


		private static ConfigurationSection GetConfigurationSection ( String sectionCacheKey )
		{
			ConfigurationSection section = null;

			if ( _sectionsLoaded.ContainsKey( sectionCacheKey ) == true )
				section = _sectionsLoaded[sectionCacheKey];

			return section;
		}

		private static string GetConfigurationPath ()
		{
			return GetConfigurationPath( String.Empty );
		}

		private static string GetConfigurationPath ( string configFilename )
		{
			//string basePath = AppDomain.CurrentDomain.BaseDirectory;
			//string configPath = Path.Combine( basePath, ConfigFilesPath );

			return Path.Combine( ConfigFilesPath, configFilename );
		}

		private static void OnConfigChanged ( Object sender, FileSystemEventArgs e )
		{
			if ( _refreshingSections )
			{
				return;
			}
			lock ( _sectionsLoaded )
			{

				_refreshingSections = true;
				Byte startIndex = (Byte)(e.Name.LastIndexOf( '\\' ) + 1);
				String section = e.Name.Substring( startIndex ).Replace( ".config", "" );

				foreach ( string machineTypeName in Enum.GetNames( typeof( MachineType ) ) )
				{
					string sectionCacheKey = BuildCacheKey( section, ConfigManager.MachineName, machineTypeName );

					if ( _sectionsLoaded.ContainsKey( sectionCacheKey ) )
					{
						if ( _sectionsLoaded[sectionCacheKey] != null )
						{
							RefeshSection( _sectionsLoaded[sectionCacheKey] as ISupportRefresh );

							_sectionsLoaded[sectionCacheKey] = null;
						}
					}
				}

				_lastConfigurationLoadTime = DateTime.Now;
				_refreshingSections = false;
			}
		}

		private static String BuildCacheKey ( String sectionName, String machineName, String machineTypeName )
		{
			if ( sectionName.EndsWith( ".Config" ) || sectionName.EndsWith( ".Configuration" ) )
			{
				sectionName = sectionName.Substring( 0, sectionName.LastIndexOf( '.' ) );
			}

            return sectionName + "_" + machineName + "_" + machineTypeName;
        }
	}
}
