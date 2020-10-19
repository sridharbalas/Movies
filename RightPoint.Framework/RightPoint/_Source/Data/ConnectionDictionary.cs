using System.Collections.Specialized;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for ConnectionDictionary.
    /// </summary>
    public class ConnectionDictionary : HybridDictionary
    {
        public void Add( Connection value )
        {
            base.Add( value.ConnectionKey.ToUpper(), value );
        }

        private new void Add( object key, object value )
        {
            base.Add( key, value );
        }

        public Connection this[ string connectionKey ]
        {
            get { return ( (Connection) base[connectionKey.ToUpper()] ); }
            set { base[connectionKey.ToUpper()] = value; }
        }

        private new object this[ object key ]
        {
            get { return ( base[key] ); }
            set { base[key] = value; }
        }

        public bool Contains( string connectionKey )
        {
            return ( base.Contains( connectionKey.ToUpper() ) );
        }

        private new bool Contains( object key )
        {
            return ( base.Contains( key ) );
        }
    }
}