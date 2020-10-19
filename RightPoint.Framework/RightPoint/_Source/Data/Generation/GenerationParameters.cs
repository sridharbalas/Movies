using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using RightPoint.Data.Generation.Analyzer;

namespace RightPoint.Data.Generation
{
    public class GenerationParameters
    {
        public GenerationParameters(string connectionString, string generatedNamespace)
        {
            Initialize(DatabaseType.SqlServer, connectionString, generatedNamespace);
        }

        public GenerationParameters(DatabaseType databaseType, string connectionString, string generatedNamespace)
        {
            Initialize(databaseType, connectionString, generatedNamespace);
        }

        public void Initialize(DatabaseType databaseType, string connectionString, string generatedNamespace)
		{
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            GeneratedNamespace = generatedNamespace;

			
            string databaseName = String.Empty;

            switch (databaseType)
            {
                case DatabaseType.SqlServer:
            var databaseAnalzer = new DatabaseAnalyzer(null);
                    databaseName = databaseAnalzer.GetDatabaseName(connectionString);
                    break;

                case DatabaseType.MySQL:
                    var mysqlDatabaseAnalzer = new MySqlDatabaseAnalyzer(null);
                    databaseName = mysqlDatabaseAnalzer.GetDatabaseName(connectionString);
                    break;

                default:
                    throw new NotSupportedException(databaseType.ToString());
            }
            

			//SqlConnection connection = new SqlConnection(connectionString);

            // These can be overridden by the templates, these values are just the defaults.
            _generatedClassName = databaseName;
            ConnectionKey = databaseName;
        }

        public string ConnectionString;
        public string GeneratedNamespace;
        public DatabaseType DatabaseType;


        private string _generatedClassName;
        public string GeneratedClassname
        {
            get
            {
                return _generatedClassName;
            }

            set
            {
                _generatedClassName = value;
                HasManuallySpecifiedGeneratedClassName = true;
            }
        }


        public bool HasManuallySpecifiedGeneratedClassName = false;
        public string DatabaseObjectOwner = "dbo";
        public string ConnectionKey;
        public string CommandTimeout = null;
        public bool MarkSerializable = true;
        public bool EnableTransactionSupport = false;

        public List<string> StoredProcedures = new List<string>();

    }
}
