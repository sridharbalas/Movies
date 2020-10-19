using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Xml;
using RightPoint.Config;

namespace RightPoint.Data
{
    /// <summary>
    /// Implements the IConfigurationSectionHandler interface for the user defined configuration sections.
    /// </summary>
    public sealed class Configuration : IConfigurationSectionHandler
    {
        #region Private Static Members

        private static readonly Configuration _instance;
        private const String SECTION_NAME = "dataSettings";
        // This is used by the encrypted store when it's initializing a custom 
        // connection key into the connection array. This is used to determine the
        // initialization order of the configuration information.
        private static ArrayList _debugMessages = new ArrayList();

        #endregion

        #region Properties

        private static ConnectionDictionary _connections = new ConnectionDictionary();

        /// <summary>
        /// Gets a dictionary object which contains all available connections from the config file
        /// </summary>
        public static ConnectionDictionary Connections
        {
            get { return ( _connections ); }
        }

        #endregion

        /// <summary>
        /// Initializes the <see cref="Configuration"/> class.
        /// </summary>
        static Configuration()
        {
            _instance = (Configuration) ConfigurationManager.GetSection( SECTION_NAME );

            if ( _instance == null )
            {
				_instance = new Configuration();
				AddDebugMessage("The '" + SECTION_NAME + "' section not provided in the config file.");

                //throw new RightPointException( "The '" + SECTION_NAME + "' section not provided in the config file." );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        private Configuration()
        {
        }

        /// <summary>
        /// Implemented by all configuration section handlers to parse the XML of the configuration section. 
        /// </summary>
        /// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
        /// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
        /// <param name="section">he XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
        /// <returns>A configuration object.</returns>
        public Object Create( Object parent, Object configContext, XmlNode section )
        {
            Configuration returnValue = new Configuration();

            LoadConnections( section );

            return returnValue;
        }

        #region Public Methods

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns>A connection string.</returns>
        public static string GetConnectionString( string connectionKey )
        {
            return GetConnection( connectionKey.ToUpper() ).ConnectionString;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns>A connection object.</returns>
        public static Connection GetConnection( string connectionKey )
        {
            if ( _connections[connectionKey.ToUpper( CultureInfo.CurrentCulture )] == null )
            {
                throw new RightPointException( "Connection string could not be found for key '" + connectionKey +
                                                   "'" );
            }

            return _connections[connectionKey];
        }

        /// <summary>
        /// Adds the debug message (will store only the first 10 messages)
        /// </summary>
        /// <param name="debugMessage">The debug message.</param>
        public static void AddDebugMessage( string debugMessage )
        {
            if ( _debugMessages.Count < 10 )
            {
                _debugMessages.Add( DateTime.Now.ToString( CultureInfo.CurrentCulture ) + ": " + debugMessage );
            }
        }

        /// <summary>
        /// Gets all debug messages.
        /// </summary>
        /// <returns>Array of debug messages.</returns>
        public static string[] GetDebugMessages()
        {
            return (string[]) _debugMessages.ToArray( typeof ( String ) );
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the connection strings from the configuration file.
        /// </summary>
        /// <param name="section">The section.</param>
        private static void LoadConnections( XmlNode section )
        {
            // Remove all connections that were loaded from a config file from the connection 
            // connection, leaving all the manual connections. 
            ArrayList keysToRemove = new ArrayList( _connections.Keys );
            foreach ( string key in keysToRemove )
            {
                Connection currentConnection = _connections[key];

                if ( currentConnection.ManuallyAdded == false )
                {
                    _connections.Remove( key );
                }
            }

            XmlNode connectionsNode = section.SelectSingleNode( "connections" );
            if ( connectionsNode == null )
            {
                throw new RightPointException(
                    String.Format( "The 'connections' node not provided in {0} configuration section.", SECTION_NAME ) );
            }

            foreach ( XmlNode connectionStringNode in connectionsNode.SelectNodes( "connection" ) )
            {
                string connectionTypeName = GetAttributeValue( connectionStringNode, "type" );
                string connectionKey = GetAttributeValue( connectionStringNode, "key" ).ToUpper();
                string connectionString = GetAttributeValue( connectionStringNode, "connectionString" );

                ConnectionType connectionType =
                    (ConnectionType) Enum.Parse( typeof ( ConnectionType ), connectionTypeName, true );
                Connection newConnection = new Connection( connectionKey, connectionType, connectionString, false );

                if ( _connections.Contains( newConnection.ConnectionKey ) )
                {
                    _connections[newConnection.ConnectionKey] = newConnection;
                }
                else
                {
                    _connections.Add( newConnection );
                }
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="configurationNode">The configuration node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>Value of the attribute.</returns>
        private static string GetAttributeValue( XmlNode configurationNode, string attributeName )
        {
            if ( configurationNode.Attributes[attributeName] == null )
            {
                throw new RightPointException( "The '" + attributeName +
                                                   "' node not provided in connection node of '" + SECTION_NAME +
                                                   "' section." );
            }
            return configurationNode.Attributes[attributeName].InnerText;
        }

        #endregion
    }

	public sealed class Configuration_New : ConfigSection<Configuration_New>
	{
		public readonly Int32 CacheDurationDefault = 60;
        public readonly int DefaultSuperMarketId = 1;

        public readonly String DbMaintenanceServiceInternalURL;
    }
}