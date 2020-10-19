namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for Connection.
    /// </summary>
    public class Connection
    {
        private string _connectionKey;
        private ConnectionType _connectionType;
        private string _connectionString;
        private bool _manuallyAdded;

        public Connection( string connectionKey, ConnectionType connectionType, string connectionString )
        {
            _connectionKey = connectionKey;
            _connectionType = connectionType;
            _connectionString = connectionString;
            _manuallyAdded = true;
        }

        internal Connection( string connectionKey, ConnectionType connectionType, string connectionString,
                             bool manuallyAdded )
        {
            _connectionKey = connectionKey;
            _connectionType = connectionType;
            _connectionString = connectionString;
            _manuallyAdded = manuallyAdded;
        }

        public string ConnectionKey
        {
            get { return ( _connectionKey ); }
        }

        public ConnectionType ConnectionType
        {
            get { return ( _connectionType ); }
        }

        public string ConnectionString
        {
            get { return ( _connectionString ); }
        }

        public bool ManuallyAdded
        {
            get { return _manuallyAdded; }
        }
    }
}