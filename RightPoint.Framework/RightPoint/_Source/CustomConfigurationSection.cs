using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EventInventory
{
	public abstract class CustomConfigurationSection : ConfigurationSection
	{
		protected static CustomConfigurationSection _instance = null;

		protected new Object this[String key]
		{
			get
			{				
				String customKey = key + "_" + Configuration.MachineType.ToString();
				Object val = null;
				if ( this.Properties.Contains( customKey ) )
				{
					val = base[customKey];
				}
				if ( val == null || ((ConfigurationElement)val).ElementInformation.IsPresent == false )
				{
					val = base[key];
				}
				return val;
			}
			set { base[key + "_" + Configuration.MachineType.ToString()] = value; }
		}

		public void Refresh ()
		{
			ConfigurationManager.RefreshSection( this.GetType().FullName );
			_instance = null;
		}
	}

	public abstract class CustomConfigurationElement : ConfigurationElement
	{
		protected new Object this[String key]
		{
			get
			{
				String customKey = key + "_" + Configuration.MachineType.ToString();
				Object val = null;
				if ( this.Properties.Contains( customKey ) )
				{
					val = base[customKey];
				}
				if ( val == null || ( val is String && String.IsNullOrEmpty( (String)val ) ) )
				{
					val = base[key];
				}
				return val;
			}
		}

		public abstract String Name { get; }
		public abstract MachineType Environment { get; }
	}

	public class CustomConfigurationElementCollection<T> : ConfigurationElementCollection where T : CustomConfigurationElement, new()
	{
		public T this[Int32 index]
		{
			get { return base.BaseGet( index ) as T; }
		}

		public new T this[String key]
		{
			get { return base.BaseGet( key ) as T; }
		}

		public T this[MachineType environment]
		{
			get
			{
				foreach ( T t in this )
				{
					if ( t.Environment == environment )
					{
						return t;
					}
				}
				return null;
			}
		}

		protected override ConfigurationElement CreateNewElement ()
		{
			return new T();
		}

		protected override object GetElementKey ( ConfigurationElement element )
		{
			return ( (T)element ).Name;
		}

		public Boolean Contains ( String elementName )
		{
			foreach ( T t in this )
			{
				if ( t.Name == elementName )
				{
					return true;
				}
			}
			return false;
		}

		public Boolean Contains ( MachineType environment )
		{
			foreach ( T t in this )
			{
				if ( t.Environment == environment )
				{
					return true;
				}
			}
			return false;
		}
	}
}
