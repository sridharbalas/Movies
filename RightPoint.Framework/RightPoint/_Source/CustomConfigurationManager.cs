using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Web;

namespace EventInventory
{
	public static class CustomConfigurationManager<T> where T : CustomConfigurationSection
	{
		private static FileSystemWatcher _fsw;
		private static Dictionary<String, CustomConfigurationSection> _sectionsLoaded = new Dictionary<String, CustomConfigurationSection>();

		static CustomConfigurationManager()
		{
			String configFilesPath = ConfigurationManager.AppSettings["configFilesPath"];
			if ( String.IsNullOrEmpty( configFilesPath ) )
			{
				// if it's a web app, then the default business config will be in the root web.
				if ( HttpContext.Current != null )
				{
					configFilesPath = HttpContext.Current.Server.MapPath( "~/" );
				}
				else // It's in a the same path as the application's main entry point.
				{
					configFilesPath = AppDomain.CurrentDomain.BaseDirectory;
				}
			}

			_fsw = new FileSystemWatcher( configFilesPath, "*.config" );
			_fsw.NotifyFilter = NotifyFilters.LastWrite;
			_fsw.Changed += new FileSystemEventHandler( configChanged );
			_fsw.EnableRaisingEvents = true;
		}

		public static T GetSection ()
		{
			String sectionName = typeof( T ).FullName;
			T section = null;
			if ( _sectionsLoaded.ContainsKey( sectionName ) )
			{
				section = (T)_sectionsLoaded[sectionName];
			}
			if ( section == null )
			{
				section = (T)ConfigurationManager.GetSection( sectionName );
				if ( _sectionsLoaded.ContainsKey( sectionName ) == false )
				{
					_sectionsLoaded.Add( sectionName, section );
				}
				else if ( _sectionsLoaded[sectionName] == null )
				{
					_sectionsLoaded[sectionName] = section;
				}
			}
			return section;
		}

		private static void configChanged ( Object sender, FileSystemEventArgs e )
		{
			String section = e.Name.Substring( 0, e.Name.LastIndexOf( '.' ) ) + ".Configuration";
			if ( _sectionsLoaded.ContainsKey( section ) )
			{
				_sectionsLoaded[section].Refresh();
				_sectionsLoaded.Remove( section );
			}
		}
	}
}
