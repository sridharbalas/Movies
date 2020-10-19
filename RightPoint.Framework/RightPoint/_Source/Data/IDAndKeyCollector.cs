using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for IDAndKeyCollector.
    /// </summary>
    public class IDAndKeyCollector<KeyType, IdType>
    {
		private Dictionary<IdType, KeyType> _idDictionary = new Dictionary<IdType, KeyType>();
		private Dictionary<KeyType, IdType> _keyDictionary = new Dictionary<KeyType, IdType>();

        public IDAndKeyCollector( string connectionKey, string storedProcedureName )
        {
            LoadIDsAndKeys( connectionKey, storedProcedureName );
        }

		public IdType GetIDByKey(KeyType key)
        {
            try
            {
                return _keyDictionary[key];
            }
            catch (Exception e)
            {
                throw new RightPointException(
                    String.Format("Could not retrieve Key: [{0}] from this collection.", key),
                    e);
            }
        }

		public KeyType GetKeyByID(IdType id)
        {
            try
            {
                return _idDictionary[id];
            }
            catch (Exception e)
            {
                throw new RightPointException( 
                    String.Format( "Could not retrieve ID: [{0}] from this collection.", id ),
                    e);
            }

        }

        #region Private Helper Methods

        private object ConvertWithTypeConverterOrNewGuid(Type typeToConvertTo, object valueToConvertFrom)
        {
            if (typeToConvertTo == typeof(Guid) && valueToConvertFrom is string)
            {
				return new Guid((string) valueToConvertFrom);
            }
            else
            {
				TypeConverter typeConverter = TypeDescriptor.GetConverter(typeToConvertTo);
				return typeConverter.ConvertTo(valueToConvertFrom, typeToConvertTo);
            }
        }
        
        /// <summary>
        /// Retrieves the IDAndKey collection (based on specified stored procedure).
        /// </summary>
        /// <param name="connectionKey">The key for the sql connection string hosting the GetIDsAndKeys stored procedure.</param>
        /// <param name="storedProcedureName"></param>
        private void LoadIDsAndKeys( string connectionKey, string storedProcedureName )
        {
            using ( IDbConnection dbConnection = DbFactory.GetConnection( connectionKey ) )
            {
                dbConnection.Open();
                try
                {
                    IDbCommand dbCommand = DbUtility.CreateStoredProcedureCommand( dbConnection, storedProcedureName );

                    DbUtility.AddReturnParameter( dbCommand );
                    IDataReader dbDataReader = dbCommand.ExecuteReader( CommandBehavior.Default );
                    while ( dbDataReader.Read() )
                    {
						IdType id = (IdType)ConvertWithTypeConverterOrNewGuid(typeof(IdType), dbDataReader[0]);
						KeyType key = (KeyType)ConvertWithTypeConverterOrNewGuid(typeof(KeyType), dbDataReader[1]);
                        
                        _keyDictionary.Add( key, id );
                        _idDictionary.Add( id, key );
                    }
                    dbDataReader.Close();

                    int returnCode = 0;

                    if ( ( (IDbDataParameter) dbCommand.Parameters[0] ).Value != DBNull.Value )
                    {
                        returnCode = Convert.ToInt32( ( (IDbDataParameter) dbCommand.Parameters[0] ).Value );
                    }

                    switch ( returnCode )
                    {
                        case 0: // Success
                            return;
                        default: // Invalid ReturnCode
                            throw new InvalidReturnCodeException( returnCode,
                                                                  "Invalid ReturnCode: <" + returnCode.ToString() +
                                                                  "> from '" + storedProcedureName + "'." );
                    }
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        #endregion
    }
}