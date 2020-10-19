using System;
using System.Collections.Generic;
using System.Text;
//using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Newtonsoft608.Json;

namespace RightPoint.Data.Generation.Analyzer
{
	public class MySqlDatabaseAnalyzer : IDatabaseAnalyzer
	{
		private string LogFile = @"c:\generation.log";

		private IDALTemplate _dalTemplate;

		public MySqlDatabaseAnalyzer(IDALTemplate dalTemplate)
		{
			_dalTemplate = dalTemplate;
		}

		

		//public string GetStoredProcedureNameFromDALMethodName(string connectionString, string dalMethodName)
		//{
		//	string returnValue = null;

		//	// DAL names always start with "Try", so drop the first three chars.
		//	string partialProcName = dalMethodName.Substring(3);

		//	using (MySqlConnection conn = new MySqlConnection(connectionString))
		//	{
		//		// Get all the input parameters for the stored procedure.
		//		conn.Open();

		//		string query = string.Format(@"
		//			select su.name + '.' + so.name as name from sysobjects so 
		//			join sysusers su on su.uid = so.uid
		//			where so.name like '%\_{0}' ESCAPE '\' AND type = 'P'
		//		",
		//			partialProcName
		//			);

		//		Log("Running query: \r\n" + query);

		//		MySqlCommand comm = new MySqlCommand(query, conn);
		//		comm.CommandType = CommandType.Text;

		//		MySqlDataReader dr = comm.ExecuteReader();

		//		if (dr.Read() == true)
		//			returnValue = dr["name"].ToString();
				
		//		// More than one record found, can't determine the right proc name!
		//		if (dr.Read() == true)
		//			returnValue = String.Empty;

		//		dr.Close();

		//		if (String.IsNullOrEmpty(returnValue) == true)
		//		{
		//			query = string.Format(@"
		//			select su.name + '.' + so.name as name from sysobjects so 
		//			join sysusers su on su.uid = so.uid
		//			where so.name like '%\{0}' AND type = 'P'
		//		",
		//			partialProcName
		//			);

		//			Log("Running query: \r\n" + query);

		//			comm = new MySqlCommand(query, conn);
		//			comm.CommandType = CommandType.Text;

		//			dr = comm.ExecuteReader();

		//			if (dr.Read() == true)
		//				returnValue = dr["name"].ToString();
				
		//			// More than one record found, can't determine the right proc name!
		//			if (dr.Read() == true)
		//				returnValue = String.Empty;

		//			dr.Close();
		//		}
		//	}

		//	return returnValue;
		//}

		private void Log(string message)
		{
			string[] values = {};

			Log(message, values);
		}

		private void Log(string format,  params string[] values)
		{
			try
			{
				File.AppendAllText(LogFile, String.Format(format + "\r\n", values));
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Error opening log file: " + LogFile + " " + ex.ToString());
				Debugger.Log(999, "Logging", "Error opening log file: " + LogFile + " " + ex.ToString());
			}
		}


		public StoredProcedureSchema GetStoredProcedureSchema(string connectionString, SysObjectRecord storedProcedure)
		{
            try
            {
                if (File.Exists(LogFile) == true)
                    File.Delete(LogFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            Log("Starting generation for: {0}", storedProcedure.ObjectName);

            StoredProcedureSchema spReturnValue = new StoredProcedureSchema
            {
                Name = storedProcedure.ObjectName,
                Owner = storedProcedure.OwnerName
            };

            //Log("Generating with transactions set to: " + useTransactions);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				// Get all the input parameters for the stored procedure.
				conn.Open();

                MySqlTransaction trans = null;

				//if (useTransactions)
					trans = conn.BeginTransaction();

                string query = string.Format(@"
			SELECT  
	            PARAMETER_NAME as 'name',
                NUMERIC_PRECISION as 'precision',
                NUMERIC_SCALE as scale,
                CHARACTER_MAXIMUM_LENGTH as max_length,
                CASE WHEN PARAMETER_MODE = 'OUT' THEN true ELSE false END as is_output,
                DATA_TYPE as type,
                false as is_table_type, -- Not supported in MySQL
                0 as table_type_column_count -- Not supported in MySQL
            FROM information_schema.parameters
            WHERE SPECIFIC_NAME = '{0}'
            AND SPECIFIC_SCHEMA = '{1}'
				",
                    storedProcedure.ObjectName,
                    GetDatabaseName(connectionString)
                    );

				Log("Running query: \r\n" + query);

                MySqlCommand comm = new MySqlCommand(query, conn)
                {
                    CommandType = CommandType.Text,
                    Transaction = trans
                };

                List<Parameter> parameters = new List<Parameter>();

                //				try
                //				{
                MySqlDataReader dr = comm.ExecuteReader();

                bool containsOutputParameter = false;

				while (dr.Read())
				{
                    Parameter newParam = new Parameter
                    {
                        Length = Convert.IsDBNull(dr["max_length"]) == true ? null : Convert.ToInt32(dr["max_length"]) as int?,
                        Precision = Convert.IsDBNull(dr["precision"]) == true ? null : Convert.ToInt32(dr["precision"]) as int?,
                        Scale = Convert.IsDBNull(dr["scale"]) == true ? null : Convert.ToInt32(dr["scale"]) as int?,

                        ParameterName = dr["name"].ToString(),
                        IsOutput = Convert.ToBoolean(dr["is_output"]),
                        IsTableType = Convert.ToBoolean(dr["is_table_type"]),
                        TableTypeColumnCount = Convert.IsDBNull(dr["table_type_column_count"]) == true ? null : Convert.ToInt32(dr["table_type_column_count"]) as int?
                    };
                    newParam.DataType = newParam.IsTableType == true ? "structured" : dr["type"].ToString();
                    newParam.Type = Utility.ToSystemType(dr["type"].ToString(), newParam.IsTableType);

                    // In MySQL, all INTs are actually LONGLONG, so override those to UINT32.
                    if (newParam.Type == typeof(System.Int32))
                        newParam.Type = typeof(System.UInt32);


                    if (newParam.IsOutput == true)
                        containsOutputParameter = true;


                    parameters.Add(newParam);

					Log("Parameter added: " + newParam.ParameterName);
				}

				spReturnValue.Parameters = parameters;

				dr.Close();
				//				}
				//				catch(SqlException ex)
				//				{
				//					// A temp table was in the stored procedure, so try again, this time
				//					// using transactions instead of 'WITH FMT ONLY'
				//					if(!useTransactions)
				//					{		
				//						return(GetStoredProcedureSchema(targetServer, database, storedProcedure, true));
				//					}
				//					else
				//					{
				//						throw(ex);
				//					}
				//				}

				// Get all the table schemas of any tables that this sp may be 
				// depending on.
				//List<SysObjectRecord> tableObjects = GetDependingTablesForStoredProcedure(connectionString, "[" + storedProcedure.OwnerName + "].[" + storedProcedure.ObjectName + "]");
				//List<TableSchema> tableSchemas = new List<TableSchema>();
				//foreach (SysObjectRecord tableObject in tableObjects)
				//{
				//	//Database.SysObjectRecord tableObject = new SysObjectRecord();
				//	//tableObject.sOwnerName = 
				//	tableSchemas.Add(GetTableSchema(connectionString, tableObject));

				//	Log("Table schema added: " + tableObject.ObjectName);
				//}

				//spReturnValue.TableSchemas = tableSchemas;


				//if (!useTransactions)
				//{
				//	comm.CommandText = "SET FMTONLY ON";
				//	comm.CommandType = CommandType.Text;
				//	comm.ExecuteNonQuery();
				//}

				// Get columns for any returned rows.
				comm.CommandText = GetDatabaseName(connectionString) + "." + spReturnValue.Name;
				comm.CommandType = CommandType.StoredProcedure;
				comm.CommandTimeout = 600;

                List<Column> columns = new List<Column>();

                if (containsOutputParameter == false)
                {
                    foreach (Parameter currParam in parameters)
                    {
                        MySqlParameter p = new MySqlParameter
                        {
                            ParameterName = currParam.ParameterName,
                            Value = GetSampleParameterValue(currParam),
                            Size = currParam.Length.GetValueOrDefault(-1)
                        }; //, currParam.eDbType, currParam.nLength, ParameterDirection.Input, true, currParam.nPrecision, currParam.nScale, null, DataRowVersion.Default, DbUtility.GetSampleParameterValue(currParam));


                        if (currParam.IsOutput == true)
                            p.Direction = ParameterDirection.InputOutput;
                        else
                            p.Direction = ParameterDirection.Input;

                        comm.Parameters.Add(p);

                        Log("Command Parameter added: " + p.ParameterName);

                        //	comm.Parameters.Add(currParam.sParameterName, DbUtility.GetSampleParameterValue(currParam));
                        //else
                        //	comm.Parameters.Add(currParam.sParameterName);
                    }
                    
                    try
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter(comm);
                        da.Fill(dt);

                        int i = 0;
                        foreach (DataColumn c in dt.Columns)
                        {
                            Column newColumn = new Column
                            {
                                IsIdentity = c.AutoIncrement,
                                Length = c.MaxLength,
                                Ordinal = i,
                                Precision = null,
                                Scale = null,
                                ColumnName = c.ColumnName,
                                DataType = c.DataType.Name, //Utility.ReturnSqlDBType(c.DataType.Name, newColumn.Length, false);
                                Type = c.DataType, //Utility.ToSystemType(c.DataType.Name);
                                IsNullable = c.AllowDBNull
                            };

                            columns.Add(newColumn);
                            i++;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        //Log("SQL Exception, useTransactions: " + useTransactions);

                        Log(ex.ToString());
                        // A temp table was in the stored procedure, so try again, this time
                        // using transactions instead of 'WITH FMT ONLY'
                        //if (!useTransactions)
                        //{
                        //    return (GetStoredProcedureSchema(connectionString, storedProcedure, true));
                        //}
                        //else
                        //{
                        trans.Rollback();

                        if (_dalTemplate != null)
                            _dalTemplate.Error(ex.ToString());

                        throw;
                        //}
                    }
                }

				spReturnValue.ResultSetColumns = columns;

				//if(useTransactions == true)
					trans.Rollback();

				conn.Close();
			}

            Log("\n\n" + JsonConvert.SerializeObject(spReturnValue, Formatting.Indented) + "\n\n");


            return (spReturnValue);
		}

		/// <summary>
		/// Gets a list of tables that the passed stored procedure is dependant on.
		/// </summary>
		/// <param name="storedProcedureName">The name of the stored procedure, can be in format of [dbo].sp_MySP, dbo.sp_MySP, or sp_MySP</param>
		/// <returns>An ArrayList of string table names.</returns>
		public List<SysObjectRecord> GetDependingTablesForStoredProcedure(string connectionString, string storedProcedureName)
		{
			List<SysObjectRecord> returnValue = new List<SysObjectRecord>();

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				// Get all the input parameters for the stored procedure.
				conn.Open();

                MySqlCommand comm = new MySqlCommand("sp_depends", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                comm.Parameters.AddWithValue("@objname", storedProcedureName);

                MySqlDataReader dr = comm.ExecuteReader();
				while (dr.Read())
				{
					if (dr["type"].ToString() == "user table")
					{
						SysObjectRecord sysObjectRecord = new SysObjectRecord(dr["name"].ToString().Split('.')[1], dr["name"].ToString().Split('.')[0]);

						if (returnValue.Contains(sysObjectRecord) == false)
						{
							returnValue.Add(sysObjectRecord);
						}
					}
				}

				conn.Close();
			}

			return returnValue;
		}

		public static TableSchema GetTableSchema(string connectionString, SysObjectRecord tableObject)
		{

            TableSchema tsReturnValue = new TableSchema
            {
                TableName = tableObject.ObjectName
            };

            string tableName = string.Format("[{0}].[{1}]", tableObject.OwnerName, tableObject.ObjectName);

			try
			{
				using (MySqlConnection conn = new MySqlConnection( connectionString ) )
				{
					conn.Open();

                    MySqlCommand comm = new MySqlCommand("sp_helpconstraint", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    comm.Parameters.AddWithValue( "@objname", tableName );

					DataSet dsConstraints = new DataSet();
                    MySqlDataAdapter da = new MySqlDataAdapter( comm );
					da.Fill( dsConstraints );

					DataTable pkTable;
					if ( dsConstraints.Tables.Count > 1 )
						pkTable = dsConstraints.Tables[1];
					else
						pkTable = dsConstraints.Tables[0];

					List<String> primaryKeys = new List<String>();
					foreach ( DataRow currRow in pkTable.Rows )
					{
						if ( currRow[0].ToString().IndexOf( "PRIMARY KEY" ) != -1 )
						{
							foreach ( string currPK in currRow[6].ToString().Split( ',' ) )
							{
								primaryKeys.Add( currPK.Trim() );
							}
						}
					}

                    // Get the column info.
                    comm = new MySqlCommand("sp_columns", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    comm.Parameters.AddWithValue( "@table_name", tableObject.ObjectName );

                    MySqlDataReader dr = comm.ExecuteReader();

					List<Column> columns = new List<Column>();
					while ( dr.Read() )
					{
                        Column c = new Column
                        {
                            ColumnName = dr["COLUMN_NAME"].ToString(),

                            IsIdentity = dr["TYPE_NAME"].ToString().IndexOf(" identity") != -1,
                            Length = Convert.ToInt32(dr["LENGTH"]),
                            Precision = Convert.IsDBNull(dr["PRECISION"]) == true ? null : Convert.ToInt32(dr["PRECISION"]) as int?,
                            Scale = Convert.IsDBNull(dr["SCALE"]) == true ? null : Convert.ToInt32(dr["SCALE"]) as int?,
                            Ordinal = Convert.ToInt32(dr["ORDINAL_POSITION"])
                        };

                        if ( c.IsIdentity )
							c.DataType = Utility.ReturnSqlDBType( dr["TYPE_NAME"].ToString().Substring( 0, dr["TYPE_NAME"].ToString().IndexOf( " identity" ) ).Trim(), c.Length, false );
                        else
							c.DataType = Utility.ReturnSqlDBType( dr["TYPE_NAME"].ToString(), c.Length, false );

                        c.SQLDataType = dr["TYPE_NAME"].ToString();

                        //throw new NotSupportedException("Figure out how to handle table types before using this code.");
						c.Type = Utility.ToSystemType( dr["TYPE_NAME"].ToString(), false );

						columns.Add( c );
					}

					conn.Close();

					tsReturnValue.PrimaryKeyColumns = primaryKeys;
					tsReturnValue.TableColumns = columns;
				}
			}
			catch (MySqlException sqlEx )
			{
				throw new Exception( String.Format( "tableName is {0}", tableName ), sqlEx );
			}

			tsReturnValue.UniqueIndexes = GetUniqueIndexesForTable(connectionString, tableObject);

			return (tsReturnValue);
		}

		public static List<IndexSchema> GetUniqueIndexesForTable(string connectionString, SysObjectRecord tableObject)
		{
			string tableName = string.Format("[{0}].[{1}]", tableObject.OwnerName, tableObject.ObjectName);

			List<IndexSchema> returnValue;

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				// Get all the unique indexes that are associated with the 
				// passed table name
				conn.Open();

				string query = string.Format(@"
					select 
						indid,	
						name
					from sysindexes
					where id = object_id('{0}') 
					and indid > 0 
					and indid < 255 
					and (status & 64) = 0  -- Not a statistic
					and (status & 2) = 2 -- Unique Index
					order by indid

				",
					tableName
					);

                MySqlCommand comm = new MySqlCommand(query, conn)
                {
                    CommandType = CommandType.Text
                };

                MySqlDataReader dr = comm.ExecuteReader();
				List<IndexSchema> indexes = new List<IndexSchema>();
				while (dr.Read())
				{
                    IndexSchema ix = new IndexSchema
                    {
                        IndexName = dr["name"].ToString(),
                        IndexID = Convert.ToInt32(dr["indid"])
                    };

                    indexes.Add(ix);
				}

				dr.Close();

				// Get the columns for each index.
				foreach (IndexSchema ix in indexes)
				{
					query = string.Format(@"
						select name
						from sysindexkeys ik
						join syscolumns c on
						c.colid = ik.colid
						where ik.id = object_id('{0}') 
						and c.id = object_id('{0}') 
						and indid = {1}

					",
						tableName,
						ix.IndexID
					);

                    comm = new MySqlCommand(query, conn)
                    {
                        CommandType = CommandType.Text
                    };

                    dr = comm.ExecuteReader();

					List<String> indexNames = new List<String>();
					while (dr.Read())
					{
						indexNames.Add(dr["name"].ToString());
					}

					dr.Close();

					ix.ColumnNames = indexNames;
				}

				conn.Close();

				returnValue = indexes;
			}

			return (returnValue);
		}

        private static object _syncObject = new object();
        private Dictionary<string, string> _databaseNamesByConnectionString = new Dictionary<string, string>();

		public string GetDatabaseName(string connectionString)
		{
            if (_databaseNamesByConnectionString.ContainsKey(connectionString) == true)
                return _databaseNamesByConnectionString[connectionString];

            string databaseName = String.Empty;

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				conn.Open();

				string query = "SELECT DATABASE() AS DataBaseName";


                MySqlCommand comm = new MySqlCommand(query, conn)
                {
                    CommandType = CommandType.Text
                };

                MySqlDataReader dr = comm.ExecuteReader();
				List<IndexSchema> indexes = new List<IndexSchema>();
				if (dr.Read() == true)
				{
					databaseName = dr["DataBaseName"].ToString();
				}

				dr.Close();
				conn.Close();
			}

            lock (_syncObject)
            {
                if (_databaseNamesByConnectionString.ContainsKey(connectionString) == false)
                    _databaseNamesByConnectionString.Add(connectionString, databaseName);
            }

            return databaseName;
		}

		public object GetSampleParameterValue(Parameter parameter)
		{
			object returnValue = null;

			byte[] b;
			string s;

			switch (parameter.DataType.ToLower().Trim())
			{
				case "binary":
					b = new byte[parameter.Length.GetValueOrDefault(-1) < 1 ? 1 : parameter.Length.Value];

					for (int i = 0; i < parameter.Length; i++)
						b[i] = 0x00;

					returnValue = b;
					break;

				case "char":
					s = "";
					s.PadLeft(parameter.Length.GetValueOrDefault(-1) < 1 ? 1 : parameter.Length.Value, 'A');
					returnValue = s;
					break;

				case "varbinary":
					b = new byte[parameter.Length.GetValueOrDefault(-1) < 1 ? 1 : parameter.Length.Value];

					for (int i = 0; i < parameter.Length; i++)
						b[i] = 0x00;

					returnValue = b;
					break;

				case "varchar":
					s = "";
					s.PadLeft(parameter.Length.GetValueOrDefault(-1) < 1 ? 1 : parameter.Length.Value, 'A');
					returnValue = s;
					break;

				case "bit":
					returnValue = true;
					break;

				case "datetime":
					returnValue = DateTime.Now;
					break;

				case "smalldatetime":
					returnValue = DateTime.Now;
					break;

				case "decimal":
					returnValue = 1d;
					break;

				case "real":
					returnValue = 1f;
					break;

				case "float":
					returnValue = 1f;
					break;

				case "image":
					b = new byte[100];

					for (int i = 0; i < 100; i++)
						b[i] = 0x00;

					returnValue = b;
					break;

				case "tinyint":
					returnValue = 1;
					break;

				case "smallint":
					returnValue = 1;
					break;

				case "int":
					returnValue = 1;
					break;

				case "smallmoney":
					returnValue = 1;
					break;

				case "money":
					returnValue = 1;
					break;

				case "nchar":
					s = "";
					s.PadLeft(parameter.Length.GetValueOrDefault(-1) < 1 ? 1 : parameter.Length.Value, 'A');
					returnValue = s;
					break;

				case "ntext":
					s = "";
					s.PadLeft(100, 'A');
					returnValue = s;
					break;

				case "numeric":
					returnValue = 1d;
					break;

				case "nvarchar":
					s = "";
					s.PadLeft(parameter.Length.GetValueOrDefault(-1) < 1 ? 1 : parameter.Length.Value, 'A');
					returnValue = s;
					break;

				case "text":
					s = "";
					s.PadLeft(100, 'A');
					returnValue = s;
					break;

				case "bigint":
					returnValue = 1;
					break;

				case "uniqueidentifier":
					returnValue = Guid.NewGuid();
					break;
                    
                case "structured":
                    var dataTable = new DataTable();
                    for (int i = 0; i < parameter.TableTypeColumnCount; i++)
                    {
                        dataTable.Columns.Add(i.ToString());
                    }

                    returnValue = dataTable;
					break;

				default:
					returnValue = "";
					break;
				//throw new Exception("An unspecified db type: " + parameter.sDataType + " was passed, please add support for this type.");
			}

			return (returnValue);
		}
	}
}
