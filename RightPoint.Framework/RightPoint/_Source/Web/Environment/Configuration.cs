using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Xml;
using RightPoint.Config;

namespace RightPoint.Web.Environment
{
    /// <summary>
    /// Implements the IConfigurationSectionHandler interface for the user defined configuration sections.
    /// </summary>
//    public sealed class Configuration : IConfigurationSectionHandler
//    {
//        #region Private Static Members

//        private static readonly Configuration _instance;
//        private const String SECTION_NAME = "environmentConfiguration";
//        private static ArrayList _debugMessages = new ArrayList();
//        #endregion

//        #region Properties

//        private static EnvironmentDictionary _environments = new EnvironmentDictionary();

//        /// <summary>
//        /// Gets a dictionary object which contains all available environments from the config file
//        /// </summary>
//        public static EnvironmentDictionary Environments
//        {
//            get { return (_environments); }
//        }

//        #endregion

//        /// <summary>
//        /// Initializes the <see cref="Configuration"/> class.
//        /// </summary>
//        static Configuration()
//        {
//            try
//            {

//                _instance = (Configuration)ConfigurationManager.GetSection(SECTION_NAME);

//            }
//            catch (Exception ex)
//            {
//                Logger.LogError(ex);
//                System.Diagnostics.Debug.WriteLine(ex.ToString());
//                throw;
//            }

//            if (_instance == null)
//            {
//                throw new RightPointException("The '" + SECTION_NAME + "' section not provided in the config file.");
//            }
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Configuration"/> class.
//        /// </summary>
//        private Configuration()
//        {
//        }

//        /// <summary>
//        /// Implemented by all configuration section handlers to parse the XML of the configuration section. 
//        /// </summary>
//        /// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
//        /// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
//        /// <param name="section">he XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
//        /// <returns>A configuration object.</returns>
//        public Object Create(Object parent, Object configContext, XmlNode section)
//        {
//            Configuration returnValue = new Configuration();

//            LoadEnvironments(section);

//            return returnValue;
//        }
//        #region Public Methods


//        public static Environment CurrentEnvironment
//        {
//            get { return GetEnvironment(RightPoint.Configuration.MachineType); }
//        }


//        /// <summary>
//        /// Gets the environment.
//        /// </summary>
//        /// <param name="machineType">The machine type.</param>
//        /// <returns>A connection object.</returns>
//        public static Environment GetEnvironment(MachineType machineType)
//        {
//            if (_environments[machineType] == null)
//            {
//                throw new RightPointException("Environment could not be found for machine type '" + machineType + "'");
//            }

//            return _environments[machineType];
//        }

//        /// <summary>
//        /// Adds the debug message (will store only the first 10 messages)
//        /// </summary>
//        /// <param name="debugMessage">The debug message.</param>
//        public static void AddDebugMessage(string debugMessage)
//        {
//            if (_debugMessages.Count < 10)
//            {
//                _debugMessages.Add(DateTime.Now.ToString(CultureInfo.CurrentCulture) + ": " + debugMessage);
//            }
//        }

//        /// <summary>
//        /// Gets all debug messages.
//        /// </summary>
//        /// <returns>Array of debug messages.</returns>
//        public static string[] GetDebugMessages()
//        {
//            return (string[])_debugMessages.ToArray(typeof(String));
//        }

//        #endregion

//        #region Private Methods

//        /// <summary>
//        /// Loads the environments from the configuration file.
//        /// </summary>
//        /// <param name="section">The section.</param>
//        private static void LoadEnvironments(XmlNode section)
//        {
//            // Remove all environments that were loaded from a config file from the environment list
//            _environments.Clear();

//            XmlNode environmentsNode = section.SelectSingleNode("environments");
//            if (environmentsNode == null)
//            {
//                throw new RightPointException(
//                    String.Format("The 'environments' node not provided in {0} configuration section.", SECTION_NAME));
//            }

//            foreach (XmlNode environmentNode in environmentsNode.SelectNodes("environment"))
//            {
//                string environmentKey = GetAttributeValue(environmentNode, "key");
//                MachineType machineType;
//                string contentServerName = GetAttributeValue(environmentNode, "contentServerName");
//                string contentRootPath = GetAttributeValue(environmentNode, "contentRootPath");
//                string secureContentServerName = GetAttributeValue(environmentNode, "secureContentServerName");
//                string secureContentRootPath = GetAttributeValue(environmentNode, "secureContentRootPath");
//                string webServerName = GetAttributeValue(environmentNode, "webServerName");
//                string webRootPath = GetAttributeValue(environmentNode, "webRootPath");

//                // Replace all localhost server names with the real machine names, so that 
//                // others can view the website under development.
//#if DEBUG == false
//                if ( contentServerName != null && contentServerName.ToLower().Contains( "localhost" ) )
//                    contentServerName = contentServerName.ToLower().Replace( "localhost", System.Environment.MachineName );

//                if ( secureContentServerName != null && secureContentServerName.ToLower().Contains( "localhost" ) )
//                    secureContentServerName = secureContentServerName.ToLower().Replace( "localhost", System.Environment.MachineName );

//                if ( webServerName != null && webServerName.ToLower().Contains( "localhost" ) )
//                    webServerName = webServerName.ToLower().Replace( "localhost", System.Environment.MachineName );
//#endif
//                Dictionary<string, EmailAddressHeader> emailAddresses = new Dictionary<string,EmailAddressHeader>();
                
//                #region Urls
//                XmlNode searchTicketsnowDomainNode = environmentNode.SelectSingleNode("search.ticketsnow.com");
//                string searchTicketsnowComDomain = "";
//                if ( searchTicketsnowDomainNode != null )
//                    searchTicketsnowComDomain = GetAttributeValue(searchTicketsnowDomainNode, "url");

//                XmlNode CustomerServiceDomainNode = environmentNode.SelectSingleNode("customerServiceDomain");
//                string CustomerServiceDomain = "";
//                if(CustomerServiceDomainNode!=null)
//                    CustomerServiceDomain = GetAttributeValue(CustomerServiceDomainNode, "url");

                

//                #endregion
                
//                #region Email Addresses
//                XmlNode emailItemsNode = environmentNode.SelectSingleNode("emailItems");
//                if (emailItemsNode != null)
//                {
//                    foreach (XmlNode emailNode in emailItemsNode.SelectNodes("emailItem"))
//                    {
//                        string emailKey = GetAttributeValue(emailNode, "key");
//                        string toAddress = GetAttributeValue(emailNode, "to");
//                        string fromAddress = GetAttributeValue(emailNode, "from");
//                        string ccAddress = GetAttributeValue(emailNode, "cc");
//                        string bccAddress = GetAttributeValue(emailNode, "bcc");
//                        if (emailKey != null)
//                        {
//                            emailAddresses.Add(emailKey, new EmailAddressHeader(toAddress, fromAddress, ccAddress, bccAddress));
//                        }
//                    }
//                }

//                #endregion

//                switch (environmentKey.ToUpper())
//                {
//                    case "LOCAL":
//                        machineType = MachineType.Local;
//                        break;

//                    case "DEVELOPMENT":
//                        machineType = MachineType.Development;
//                        break;

//                    case "PREVIEW":
//                        machineType = MachineType.Preview;
//                        break;

//                    case "PRODUCTION":
//                        machineType = MachineType.Production;
//                        break;

//                    case "QA":
//                        machineType = MachineType.Qa;
//                        break;

//                    case "STAGE":
//                        machineType = MachineType.Stage;
//                        break;

//                    default:
//                        machineType = MachineType.Local;
//                        break;
//                }

//                Environment environment =
//                    new Environment(machineType, contentServerName, contentRootPath, secureContentServerName, secureContentRootPath, webServerName, webRootPath, emailAddresses, searchTicketsnowComDomain, CustomerServiceDomain );
                
//                if (_environments.Contains(environment.MachineType))
//                {
//                    _environments[environment.MachineType] = environment;
//                }
//                else
//                {
//                    _environments.Add(environment);
//                }
//            }
//        }

//        /// <summary>
//        /// Gets the attribute value.
//        /// </summary>
//        /// <param name="configurationNode">The configuration node.</param>
//        /// <param name="attributeName">Name of the attribute.</param>
//        /// <returns>Value of the attribute.</returns>
//        private static string GetAttributeValue(XmlNode configurationNode, string attributeName)
//        {
//            if (configurationNode.Attributes[attributeName] == null)
//            {
//                throw new RightPointException("The '" + attributeName +
//                                                   "' node not provided in connection node of '" + SECTION_NAME +
//                                                   "' section.");
//            }
//            return configurationNode.Attributes[attributeName].InnerText;
//        }

//        #endregion
//    }

	public class Configuration : ConfigSection<Configuration>
	{
		public readonly EnvironmentCollection Environments;

		private Environment _currentEnvironment = null;
		public Environment CurrentEnvironment
		{
			get 
			{ 
				if(_currentEnvironment == null)
					_currentEnvironment = GetEnvironment(RightPoint.Configuration.MachineType);

				return _currentEnvironment;
			}
		}

        public readonly TmrSiteConfiguration TmrSite;

		/// <summary>
		/// Gets the environment.
		/// </summary>
		/// <param name="machineType">The machine type.</param>
		/// <returns>A connection object.</returns>
		public Environment GetEnvironment ( MachineType machineType )
		{
			if ( Environments[machineType] == null )
			{
				throw new RightPointException( "Environment could not be found for machine type '" + machineType + "'" );
			}

			return Environments[machineType];
		}

        public class TmrSiteConfiguration : RightPoint.Config.ConfigElement
        {
            public readonly string TmrContentRootPath;
            public readonly string TmResaleContentServerNameAU;
        }
	}
}
