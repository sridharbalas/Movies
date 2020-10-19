using System;
using System.Collections.Generic;
using System.Text;
using RightPoint.Config;

namespace RightPoint.Data
{
	public class Config : ConfigSection<Config>
	{
		public readonly Int32 CacheDurationDefault = 60;
        public readonly int DefaultSuperMarketId ;

		public readonly ConfigConnectionCollection Connections;

        public readonly String DbMaintenanceServiceInternalURL;

        public Connection GetConnection ( String key )
		{
			return new Connection( key, ConnectionType.SqlClient, Connections[key].ConnectionString, false );
		}
	}

	public class ConfigConnection : ConfigElementCollectionItem
	{
		public readonly String ConnectionString;
	}

	public class ConfigConnectionCollection : ConfigElementCollection<ConfigConnection>
	{
	}
}
