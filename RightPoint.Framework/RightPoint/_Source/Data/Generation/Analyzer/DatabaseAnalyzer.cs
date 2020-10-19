using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Diagnostics;

namespace RightPoint.Data.Generation.Analyzer
{ 
	public class DatabaseAnalyzer : IDatabaseAnalyzer
    {
		private string LogFile = @"c:\generation.log";

		private IDALTemplate _dalTemplate;

		public DatabaseAnalyzer(IDALTemplate dalTemplate)
		{
			_dalTemplate = dalTemplate;
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

			return (GetStoredProcedureSchema(connectionString, storedProcedure, false));
		}

		public string GetStoredProcedureNameFromDALMethodName(string connectionString, string dalMethodName)
		{
			string returnValue = null;

			// DAL names always start with "Try", so drop the first three chars.
			string partialProcName = dalMethodName.Substring(3);

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				// Get all the input parameters for the stored procedure.
				conn.Open();

				string query = string.Format(@"
					select su.name + '.' + so.name as name from sysobjects so 
					join sysusers su on su.uid = so.uid
					where so.name like '%\_{0}' ESCAPE '\' AND type = 'P'
				",
					partialProcName
					);

				Log("Running query: \r\n" + query);

				SqlCommand comm = new SqlCommand(query, conn);
				comm.CommandType = CommandType.Text;

				SqlDataReader dr = comm.ExecuteReader();

				if (dr.Read() == true)
					returnValue = dr["name"].ToString();
				
				// More than one record found, can't determine the right proc name!
				if (dr.Read() == true)
					returnValue = String.Empty;

				dr.Close();

				if (String.IsNullOrEmpty(returnValue) == true)
				{
					query = string.Format(@"
					select su.name + '.' + so.name as name from sysobjects so 
					join sysusers su on su.uid = so.uid
					where so.name like '%\{0}' AND type = 'P'
				",
					partialProcName
					);

					Log("Running query: \r\n" + query);

					comm = new SqlCommand(query, conn);
					comm.CommandType = CommandType.Text;

					dr = comm.ExecuteReader();

					if (dr.Read() == true)
						returnValue = dr["name"].ToString();
				
					// More than one record found, can't determine the right proc name!
					if (dr.Read() == true)
						returnValue = String.Empty;

					dr.Close();
				}
			}

			return returnValue;
		}

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


		protected StoredProcedureSchema GetStoredProcedureSchema(string connectionString, SysObjectRecord storedProcedure, bool useTransactions)
		{
			StoredProcedureSchema spReturnValue = new StoredProcedureSchema();
			spReturnValue.Name = storedProcedure.ObjectName;
			spReturnValue.Owner = storedProcedure.OwnerName;

			Log("Generating with transactions set to: " + useTransactions);

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				// Get all the input parameters for the stored procedure.
				conn.Open();

				SqlTransaction trans = null;

				if (useTransactions)
					trans = conn.BeginTransaction();

				string query = string.Format(@"
					SELECT 
						spr.name, 
						spr.precision, 
						spr.scale, 
						spr.max_length,
						spr.is_output, 
						st.name AS type,
                        st.is_table_type,
                        (select COUNT(*) from sys.columns c 
                        inner join sys.table_types t 
                        on t.type_table_object_id =c.object_id 
                        where t.name=st.name
						) as table_type_column_count
					FROM 
						sys.parameters spr, 
						sys.procedures sp, 
						sys.types st,
						sys.schemas ss,
                        sys.objects so
					WHERE 
                    so.object_id=sp.object_id
                    AND sp.name = '{0}'
					AND ss.name = '{1}'
					AND spr.object_id = sp.object_id
					AND ss.schema_id = sp.schema_id	
					AND st.system_type_id = spr.system_type_id
					AND	(
                            (st.is_user_defined = 0 AND st.system_type_id = st.user_type_id) 
						    OR ( st.is_table_type = 1  and spr.user_type_id=st.user_type_id)
                        )
					ORDER BY spr.parameter_id
				",
					storedProcedure.ObjectName,
					storedProcedure.OwnerName
					);

				Log("Running query: \r\n" + query);

				SqlCommand comm = new SqlCommand(query, conn);
				comm.CommandType = CommandType.Text;

				if (useTransactions)
					comm.Transaction = trans;

				List<Parameter> parameters = new List<Parameter>();

				//				try
				//				{
				SqlDataReader dr = comm.ExecuteReader();

				while (dr.Read())
				{
					Parameter newParam = new Parameter();
					//newParam.eDbType = DbConvert.ToDbType(dr["type"].ToString());

					newParam.Length = Convert.IsDBNull(dr["max_length"]) == true ? null : Convert.ToInt32(dr["max_length"]) as int?;
					newParam.Precision = Convert.IsDBNull(dr["precision"]) == true ? null : Convert.ToInt32(dr["precision"]) as int?;
					newParam.Scale = Convert.IsDBNull(dr["scale"]) == true ? null : Convert.ToInt32(dr["scale"]) as int?;
					
					newParam.ParameterName = dr["name"].ToString();
					newParam.IsOutput = Convert.ToBoolean(dr["is_output"]);
                    newParam.IsTableType = Convert.ToBoolean(dr["is_table_type"]);
                    newParam.TableTypeColumnCount = Convert.IsDBNull(dr["table_type_column_count"]) == true ? null : Convert.ToInt32(dr["table_type_column_count"]) as int?;
                    newParam.DataType = newParam.IsTableType == true ? "structured" : dr["type"].ToString();
                    newParam.Type = Utility.ToSystemType(dr["type"].ToString(), newParam.IsTableType);
                    

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


				if (!useTransactions)
				{
					comm.CommandText = "SET FMTONLY ON";
					comm.CommandType = CommandType.Text;
					comm.ExecuteNonQuery();
				}

				// Get columns for any returned rows.
				comm.CommandText = spReturnValue.Owner + "." + spReturnValue.Name;
				comm.CommandType = CommandType.StoredProcedure;
				comm.CommandTimeout = 600;

				foreach (Parameter currParam in parameters)
				{
					SqlParameter p = new SqlParameter(); //, currParam.eDbType, currParam.nLength, ParameterDirection.Input, true, currParam.nPrecision, currParam.nScale, null, DataRowVersion.Default, DbUtility.GetSampleParameterValue(currParam));
					p.ParameterName = currParam.ParameterName;
					p.Value = GetSampleParameterValue(currParam);
					p.Size = currParam.Length.GetValueOrDefault(-1);
                    

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

				List<Column> columns = new List<Column>();

				try
				{
					DataTable dt = new DataTable();
					SqlDataAdapter da = new SqlDataAdapter(comm);
					da.Fill(dt);

					int i = 0;
					foreach (DataColumn c in dt.Columns)
					{
						Column newColumn = new Column();
						newColumn.IsIdentity = c.AutoIncrement;
						newColumn.Length = c.MaxLength;
						newColumn.Ordinal = i;
						newColumn.Precision = null;
						newColumn.Scale = null;
						newColumn.ColumnName = c.ColumnName;
						newColumn.DataType = c.DataType.Name; //Utility.ReturnSqlDBType(c.DataType.Name, newColumn.Length, false);
						newColumn.Type = c.DataType; //Utility.ToSystemType(c.DataType.Name);
						newColumn.IsNullable = c.AllowDBNull;

						columns.Add(newColumn);
						i++;
					}
				}
				catch (SqlException ex)
				{
					Log("SQL Exception, useTransactions: " + useTransactions);

					Log(ex.ToString());
				    // A temp table was in the stored procedure, so try again, this time
				    // using transactions instead of 'WITH FMT ONLY'
				    if (!useTransactions)
				    {
				        return (GetStoredProcedureSchema(connectionString, storedProcedure, true));
				    }
				    else
				    {
						trans.Rollback();

						if(_dalTemplate != null)
							_dalTemplate.Error(ex.ToString());

				        throw;
				    }
				}

				spReturnValue.ResultSetColumns = columns;

				if(useTransactions == true)
					trans.Rollback();

				conn.Close();
			}

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

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				// Get all the input parameters for the stored procedure.
				conn.Open();

				SqlCommand comm = new SqlCommand("sp_depends", conn);
				comm.CommandType = CommandType.StoredProcedure;

				comm.Parameters.AddWithValue("@objname", storedProcedureName);

				SqlDataReader dr = comm.ExecuteReader();
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

				string query = string.Format("sp_depends");
				conn.Close();
			}

			return returnValue;
		}

		public static TableSchema GetTableSchema(string connectionString, SysObjectRecord tableObject)
		{
			//DataSet dsReturnValue = new DataSet();
			TableSchema tsReturnValue = new TableSchema();
			tsReturnValue.TableName = tableObject.ObjectName;

			string tableName = string.Format("[{0}].[{1}]", tableObject.OwnerName, tableObject.ObjectName);

			try
			{
				using ( SqlConnection conn = new SqlConnection( connectionString ) )
				{
					conn.Open();

					SqlCommand comm = new SqlCommand( "sp_helpconstraint", conn );
					comm.CommandType = CommandType.StoredProcedure;

					comm.Parameters.AddWithValue( "@objname", tableName );

					DataSet dsConstraints = new DataSet();
					SqlDataAdapter da = new SqlDataAdapter( comm );
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
					comm = new SqlCommand( "sp_columns", conn );
					comm.CommandType = CommandType.StoredProcedure;

					comm.Parameters.AddWithValue( "@table_name", tableObject.ObjectName );

					SqlDataReader dr = comm.ExecuteReader();

					List<Column> columns = new List<Column>();
					while ( dr.Read() )
					{
						Column c = new Column();
						c.ColumnName = dr["COLUMN_NAME"].ToString();

						c.IsIdentity = dr["TYPE_NAME"].ToString().IndexOf( " identity" ) != -1;
						c.Length = Convert.ToInt32( dr["LENGTH"] );
						c.Precision = Convert.IsDBNull( dr["PRECISION"] ) == true ? null : Convert.ToInt32( dr["PRECISION"] ) as int?;
						c.Scale = Convert.IsDBNull( dr["SCALE"] ) == true ? null : Convert.ToInt32( dr["SCALE"] ) as int?;
						c.Ordinal = Convert.ToInt32( dr["ORDINAL_POSITION"] );

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
			catch ( SqlException sqlEx )
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

			using (SqlConnection conn = new SqlConnection(connectionString))
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

				SqlCommand comm = new SqlCommand(query, conn);
				comm.CommandType = CommandType.Text;

				SqlDataReader dr = comm.ExecuteReader();
				List<IndexSchema> indexes = new List<IndexSchema>();
				while (dr.Read())
				{
					IndexSchema ix = new IndexSchema();
					ix.IndexName = dr["name"].ToString();
					ix.IndexID = Convert.ToInt32(dr["indid"]);

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

					comm = new SqlCommand(query, conn);
					comm.CommandType = CommandType.Text;

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

		public string GetDatabaseName(string connectionString)
		{
			string databaseName = String.Empty;

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				string query = "SELECT DB_NAME() AS DataBaseName";


				SqlCommand comm = new SqlCommand(query, conn);
				comm.CommandType = CommandType.Text;

				SqlDataReader dr = comm.ExecuteReader();
				List<IndexSchema> indexes = new List<IndexSchema>();
				if (dr.Read() == true)
				{
					databaseName = dr["DataBaseName"].ToString();
				}

				dr.Close();
				conn.Close();
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
