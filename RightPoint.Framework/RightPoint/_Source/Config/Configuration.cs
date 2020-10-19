using System;
using System.Configuration;

namespace RightPoint.Config
{
	internal class Configuration : ConfigurationSection, ISupportRefresh
	{
		[ConfigurationProperty( "internalLoggingEnabled", IsRequired = false, DefaultValue = false )]
		public Boolean InternalLoggingEnabled
		{
			get
			{
				Boolean? enabled = null;
				switch ( RightPoint.Configuration.MachineType )
				{
					case MachineType.Development:
                        enabled = InternalLoggingEnabled_Development;
						break;
					case MachineType.Local:
						enabled = InternalLoggingEnabled_Local;
						break;
					case MachineType.Preview:
                        enabled = InternalLoggingEnabled_Preview;
						break;
					case MachineType.Production:
						enabled = InternalLoggingEnabled_Production;
						break;
					case MachineType.Qa:
                        enabled = InternalLoggingEnabled_QA;
						break;
					case MachineType.Stage:
                        enabled = InternalLoggingEnabled_Stage;
						break;
					case MachineType.Integration:
						enabled = InternalLoggingEnabled_Integration;
						break;
					default:
						enabled = (Boolean?)this["internalLoggingEnabled"];
						break;
				}
				if ( enabled.HasValue == false )
				{
					enabled = (Boolean?)this["internalLoggingEnabled"] ?? false;
				}
				return enabled.GetValueOrDefault();
			}
		}

        [ConfigurationProperty("internalLoggingEnabled_Development", IsRequired = false, DefaultValue = false)]
        private Boolean InternalLoggingEnabled_Development
        {
            get
            {
                return (Boolean?)this["internalLoggingEnabled_Development"] ?? false;
            }
        }

        [ConfigurationProperty("internalLoggingEnabled_Preview", IsRequired = false, DefaultValue = false)]
        private Boolean InternalLoggingEnabled_Preview
        {
            get
            {
                return (Boolean?)this["internalLoggingEnabled_Preview"] ?? false;
            }
        }

        [ConfigurationProperty("internalLoggingEnabled_Qa", IsRequired = false, DefaultValue = false)]
        private Boolean InternalLoggingEnabled_QA
        {
            get
            {
                return (Boolean?)this["internalLoggingEnabled_Qa"] ?? false;
            }
        }

        [ConfigurationProperty("internalLoggingEnabled_Stage", IsRequired = false, DefaultValue = false)]
        private Boolean InternalLoggingEnabled_Stage
        {
            get
            {
                return (Boolean?)this["internalLoggingEnabled_Stage"] ?? false;
            }
        }

		[ConfigurationProperty( "internalLoggingEnabled_Local", IsRequired = false, DefaultValue = false )]
		private Boolean InternalLoggingEnabled_Local
		{
			get
			{
				return (Boolean?)this["internalLoggingEnabled_Local"] ?? false;
			}
		}

		[ConfigurationProperty("internalLoggingEnabled_Integration", IsRequired = false, DefaultValue = false)]
		private Boolean InternalLoggingEnabled_Integration
		{
			get
			{
				return (Boolean?)this["internalLoggingEnabled_Integration"] ?? false;
			}
		}

		[ConfigurationProperty( "internalLoggingEnabled_Production", IsRequired = false, DefaultValue = true )]
		private Boolean InternalLoggingEnabled_Production
		{
			get
			{
				return (Boolean?)this["internalLoggingEnabled_Production"] ?? true;
			}
		}

		[ConfigurationProperty( "machines", IsRequired = true )]
		public Machines Machines
		{
			get { return (Machines)this["machines"] ?? new Machines(); }
		}

		[ConfigurationProperty( "webHosts", IsRequired = true )]
		public WebHosts WebHosts
		{
			get { return (WebHosts)this["webHosts"] ?? new WebHosts(); }
		}

		[ConfigurationProperty( "applications", IsRequired = true )]
		public Applications Applications
		{
			get { return (Applications)this["applications"] ?? new Applications(); }
		}

		[ConfigurationProperty( "excludedTypes", IsRequired = true )]
		public ExcludedTypes ExcludedTypes
		{
			get { return (ExcludedTypes)this["excludedTypes"] ?? new ExcludedTypes(); }
		}

		#region ISupportRefresh Members

		public void Refresh ()
		{
			ConfigurationManager.RefreshSection( this.GetType().FullName );
		}

		#endregion ISupportRefresh Members
	}

	internal class Machine : ConfigurationElement
	{
		[ConfigurationProperty( "machineName" )]
		public String MachineName
		{
			get { return this["machineName"] as String; }
		}
	}

	internal class Machines : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement ()
		{
			return new Machine();
		}

		protected override object GetElementKey ( ConfigurationElement element )
		{
			return ((Machine)element).MachineName;
		}
	}

	internal class WebHost : ConfigurationElement
	{
		[ConfigurationProperty( "webHostPart" )]
		public String WebHostPart
		{
			get { return this["webHostPart"] as String; }
		}
	}

	internal class WebHosts : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement ()
		{
			return new WebHost();
		}

		protected override object GetElementKey ( ConfigurationElement element )
		{
			return ((WebHost)element).WebHostPart;
		}
	}

	internal class Application : ConfigurationElement
	{
		[ConfigurationProperty( "applicationName" )]
		public String ApplicationName
		{
			get { return this["applicationName"] as String; }
		}
	}

	internal class Applications : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement ()
		{
			return new Application();
		}

		protected override object GetElementKey ( ConfigurationElement element )
		{
			return ((Application)element).ApplicationName;
		}
	}

	internal class ExcludedType : ConfigurationElement
	{
		[ConfigurationProperty( "typeFullName" )]
		public String TypeFullName
		{
			get { return this["typeFullName"] as String; }
		}
	}

	internal class ExcludedTypes : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement ()
		{
			return new ExcludedType();
		}

		protected override object GetElementKey ( ConfigurationElement element )
		{
			return ((ExcludedType)element).TypeFullName;
		}

		public Boolean Contains ( Type type )
		{
			foreach ( ExcludedType exType in this )
			{
				if ( exType.TypeFullName == type.FullName )
				{
					return true;
				}
			}
			return false;
		}
	}
}