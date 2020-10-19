#if DOTNET_STANDARD
    using MySql.Data.MySqlClient;
#endif
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for DbFactory.
    /// </summary>
    public sealed class DbFactory
    {
        private DbFactory()
        {
        }

        public static IDbConnection GetConnection( string connectionKey )
        {
            Connection connection = Configuration.GetConnection( connectionKey );

            switch ( connection.ConnectionType )
            {
                case ConnectionType.SqlClient:
                    return CreateSqlClientConnection( connection );

#if !DOTNET_STANDARD
                case ConnectionType.OleDb:
                    return CreateOleDbConnection( connection );
#endif

#if DOTNET_STANDARD
                case ConnectionType.MySqlClient:
                    return CreateMySqlConnection(connection);
#endif
                default:
                    throw new InvalidConnectionTypeException( "The connection type specified: " +
                                                              connection.ConnectionType +
                                                              " is not supported by this version of RightPoint.Data" );
            }
        }

        private static IDbConnection CreateSqlClientConnection( Connection connection )
        {
            SqlConnection newConnection = new SqlConnection( connection.ConnectionString );

            return ( newConnection );
        }

#if !DOTNET_STANDARD
        private static IDbConnection CreateOleDbConnection( Connection connection )
        {
            OleDbConnection newConnection = new OleDbConnection( connection.ConnectionString );

            return ( newConnection );
        }
#endif

#if DOTNET_STANDARD
        private static IDbConnection CreateMySqlConnection(Connection connection)
        {
            MySqlConnection newConnection = new MySqlConnection(connection.ConnectionString);

            return (newConnection);
        }
#endif
    }

}