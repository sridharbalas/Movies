using System;
using System.Configuration;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace RightPoint.Config
{
	public class ConfigElementCollection<T> : ConfigurationElementCollection where T : ConfigElementCollectionItem, new()
	{
		private List<String> _keys = new List<string>();

		public ConfigElementCollection ()
		{
			foreach ( FieldInfo fieldInfo in this.GetType().GetFields() )
			{
				if ( ConfigManager.ExcludedTypes.Contains( fieldInfo.FieldType ) )
				{
					continue;
				}

				string fieldName = fieldInfo.Name;
				fieldName = Char.ToLower( fieldName[0] ) + fieldName.Substring( 1 );

				this.Properties.Add( new ConfigurationProperty( fieldName.ToString(), fieldInfo.FieldType, fieldInfo.GetValue( this ) ) );

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
				{
					this.Properties.Add( new ConfigurationProperty( fieldName.ToString() + "_" + machineType, fieldInfo.FieldType, null ) );
				}
			}
		}

		public virtual T this[Int32 index]
		{
			get { return base.BaseGet( index ) as T; }
		}

		public new T this[String key]
		{
			get
			{
				T t = default(T);

				if ( String.IsNullOrEmpty( key ) == false )
				{
					if ( ContainsKey( key ) )
					{
						t = base.BaseGet( key ) as T;
					}
					else if ( ContainsKey( key.ToLower() ) )
					{
						t = base.BaseGet( key.ToLower() ) as T;
					}
					else if ( ContainsKey( key.ToUpper() ) )
					{
						t = base.BaseGet( key.ToUpper() ) as T;
					}
					//else
					//{
					//    Logger.LogWarning( @"ConfigElementCollection containing type {0} has no item with key {1}", typeof( T ).FullName, key );
					//}
				}
				
				return t;
			}
		}

		public new Int32 Count
		{
			get { return _keys.Count; }
		}

		public Boolean ContainsKey ( String key )
		{
			return _keys.Contains( key );
		}

		protected override ConfigurationElement CreateNewElement ()
		{
			return new T();
		}

		protected override object GetElementKey ( ConfigurationElement element )
		{
			return ((T)element).Key;

			//return GetKeyValueFromElement((T)element);
		}

		//public void Add(ConfigurationElement element)
		//{
		//    base.BaseAdd(element);
		//}

		//private bool _checkedForCustomKeyField = false;
		//private FieldInfo _customKeyFieldInfo = null;
		//private string GetKeyValueFromElement(T typedElement)
		//{
		//    if (_checkedForCustomKeyField == false)
		//    {
		//        foreach (FieldInfo fieldInfo in typeof(T).GetFields())
		//        {
		//            if (fieldInfo.GetCustomAttributes(typeof(CustomKeyAttribute), true).Length > 0 && fieldInfo.FieldType == typeof(string))
		//            {
		//                _customKeyFieldInfo = fieldInfo;
		//                break;
		//            }
		//        }

		//        _checkedForCustomKeyField = true;
		//    }

		//    if (_customKeyFieldInfo == null)
		//        return typedElement.Key;

		//    return (string)_customKeyFieldInfo.GetValue(typedElement);
		//}
		/// <summary>
		/// DO NOT Override this method to initialize data in your configuration element after it has been loaded from the data source.
		/// OVERRIDE OnLoadComplete instead.
		/// </summary>
		protected override void PostDeserialize ()
		{
			base.PostDeserialize();

			LoadConfiguartionValuesIntoFields( ConfigManager.MachineName, ConfigManager.MachineType );

			OnLoadComplete();
		}

		protected override bool OnDeserializeUnrecognizedAttribute ( String name, String value )
		{
			String fieldName = name[0].ToString().ToUpper() + name.Substring( 1 );
			FieldInfo fieldInfo = this.GetType().GetField( fieldName );

			this.Properties.Add( new ConfigurationProperty( name, fieldInfo.FieldType ) );

			if ( fieldInfo.FieldType.FullName == @"System.Text.RegularExpressions.Regex" )
			{
				base[name] = new Regex( value );
			}

			return true;
		}

		//protected override bool OnDeserializeUnrecognizedElement ( string elementName, XmlReader reader )
		//{
		//    if ( reader.NodeType != XmlNodeType.EndElement )
		//    {
		//        return base.OnDeserializeUnrecognizedElement( elementName, reader );
		//    }

		//    return true;
		//}

		/// <summary>
		/// Override this to initialize data in your configuration element after it has been loaded from the data source.
		/// The framework will call this method automatically after the element's data has been loaded.
		/// </summary>
		public virtual void OnLoadComplete ()
		{
			foreach ( T t in this )
			{
				_keys.Add( t.Key );
			}
		}

		public virtual void LoadConfiguartionValuesIntoFields ( String machineName, MachineType machineType )
		{
			foreach ( FieldInfo fieldInfo in this.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) )
			{
				if ( (fieldInfo.Attributes & FieldAttributes.Private) == FieldAttributes.Private )
					continue;

				string fieldName = fieldInfo.Name;
				fieldName = Char.ToLower( fieldName[0] ) + fieldName.Substring( 1 );

				Object val = null;

				String customKey = fieldName + "_" + machineName;
				if ( this.Properties.Contains( customKey )
					&& fieldInfo.FieldType.GetInterface( typeof( ISupportFieldValueLoading ).Name ) == null
					/*&& fieldInfo.FieldType.IsSubclassOf( typeof( ConfigurationElementCollection ) ) == false*/ )
				{
					val = base[customKey];
				}

				customKey = fieldName + "_" + machineType.ToString();
				if ( this.Properties.Contains( customKey )
					&& fieldInfo.FieldType.GetInterface( typeof( ISupportFieldValueLoading ).Name ) == null
					/*&& fieldInfo.FieldType.IsSubclassOf( typeof( ConfigurationElementCollection ) ) == false*/ )
				{
					val = base[customKey];
				}

				//if (val == null || (val is String && String.IsNullOrEmpty((String)val)))
				if ( val == null || (val is ConfigurationElementCollection && ((ConfigurationElementCollection)val).Count == 0) )
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
				}

				fieldInfo.SetValue( this, val );
			}
		}
	}
}
