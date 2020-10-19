using System;
using System.Collections.Generic;
using System.Text;
using RightPoint.Data.Generation.Analyzer;
using System.Diagnostics;
using System.Data.SqlClient;

namespace RightPoint.Data.Generation
{
	public partial class DALHelperMethod
	{
		public static System.Collections.Generic.Dictionary<string, int> _memberNameCounter = new System.Collections.Generic.Dictionary<string, int>();

		private StoredProcedureSchema _storedProcedureSchema;

		public readonly string ConnectionKey;
		public readonly string DatabaseObjectOwner;
		public readonly GeneratedTypes EntityType;
		public readonly string MethodName;
		public readonly string CommandTimeout;
		public readonly string Database;
		public readonly string EntityName;
		public readonly string StoredProcedureName;
		public readonly bool EnableTransactionSupport;

		public static void ResetMemberNameCounter()
		{
			_memberNameCounter = new System.Collections.Generic.Dictionary<string, int>();
		}

		public DALHelperMethod(GenerationParameters generationParameters, StoredProcedureSchema storedProcedureSchema, GeneratedTypes entityType)
		{
			_storedProcedureSchema = storedProcedureSchema;

			var parsedNames = Utility.ParseStoredProcedureName(storedProcedureSchema.Name);
			//var sqlConnection = new SqlConnection(generationParameters.ConnectionString);

			ConnectionKey = generationParameters.ConnectionKey;
			DatabaseObjectOwner = generationParameters.DatabaseObjectOwner;
			EntityType = entityType;
			MethodName = parsedNames.HelperMethodName;
			CommandTimeout = generationParameters.CommandTimeout;
			EntityName = parsedNames.EntityName;
			Database = generationParameters.ConnectionKey;
			StoredProcedureName = storedProcedureSchema.Name;
			EnableTransactionSupport = generationParameters.EnableTransactionSupport;
		}

		public String GenerateEntityConstructorParametersFromResults()
		{
			StringBuilder builder = new StringBuilder();
			
			try
			{
				if (_storedProcedureSchema.ResultSetColumns.Count == 0)
				{
					return String.Empty;
				}

				Int32 iterator = 0;

				List<String> usedNames = new List<String>();
				foreach (Column column in _storedProcedureSchema.ResultSetColumns)
				{
					string fixedName = GetFixedName(usedNames, column.ColumnName);

					try
					{
						builder.AppendFormat(" ({0}{1}) (dbDataReader.FieldCount < {2} || dbDataReader[{3}] == System.DBNull.Value ? null : dbDataReader[{3}] ) /* {4} */ ",
							column.Type.FullName,
							Utility.MakeNullable(column.Type.ToString()),
							iterator + 1,
							iterator,
							Utility.FormatMemberName(fixedName));
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debugger.Break();
						Trace.WriteLine(ex.ToString());
						builder.AppendFormat("/* {0} */", ex.ToString());
						throw;
					}
					++iterator;
					if (iterator < _storedProcedureSchema.ResultSetColumns.Count)
					{
						builder.Append("," + Environment.NewLine);
					}
				}
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
				builder.AppendFormat("/* {0} */", ex.ToString());
				throw;
			}
			return builder.ToString();
		}

		public String GenerateEntityConstructorParametersFromOutput()
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				if (_storedProcedureSchema.Parameters.Count == 0)
				{
					return String.Empty;
				}
				Int32 iterator = 0;

				List<String> usedNames = new List<String>();
				foreach (Parameter parameter in _storedProcedureSchema.Parameters)
				{
					if (parameter.IsOutput == false)
						continue;

					if (iterator > 0)
						builder.Append("," + Environment.NewLine);
					
					string fixedName = GetFixedName(usedNames, parameter.ParameterName);
					try
					{
						if(parameter.IsOutput == true)
						{
							builder.AppendFormat(" ({1}{2}) ( ( (System.Data.IDbDataParameter) dbCommand.Parameters[ \"{0}\" ] ).Value == System.DBNull.Value ? null : ((System.Data.IDbDataParameter) dbCommand.Parameters[ \"{0}\" ]).Value)",
									fixedName,
									parameter.Type.FullName,
									Utility.MakeNullable(parameter.Type.ToString()));
						}

						//switch (parameter.Direction)
						//{
						//    case System.Data.ParameterDirection.InputOutput:
						//    case System.Data.ParameterDirection.Output:
						//        builder.AppendFormat(" ({1}{2}) ( ( (System.Data.IDbDataParameter) dbCommand.Parameters[ \"{0}\" ] ).Value == System.DBNull.Value ? null : ((System.Data.IDbDataParameter) dbCommand.Parameters[ \"{0}\" ]).Value)",
						//            fixedName,
						//            parameterSchema.SystemType.FullName,
						//            TemplateHelper.MakeNullable(parameterSchema.SystemType.ToString()));
						//        break;
						//    case System.Data.ParameterDirection.Input:
						//    case System.Data.ParameterDirection.ReturnValue:
						//    default:
						//        throw new ApplicationException("Invalid parameter direction.");
						//}
					}
					catch (Exception ex)
					{
						builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
						System.Diagnostics.Debugger.Break();
						Trace.WriteLine(ex.ToString());
						builder.AppendFormat("/* {0} */", ex.ToString());
						throw;
					}
					
					iterator++;
				}
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
				builder.AppendFormat("/* {0} */", ex.ToString());
				throw;
			}
			return builder.ToString();
		}

		public String GetReturnValueTypeForSimpleType()
		{
			StringBuilder builder = new StringBuilder();

			try
			{
				if (_storedProcedureSchema.OutputParameterCount == 0) { return String.Empty; }

				foreach (var parameter in _storedProcedureSchema.Parameters)
				{
					if (parameter.IsOutput == true)
					{
						builder.Append(parameter.Type.ToString());
						builder.Append(Utility.MakeNullable(parameter.Type.ToString()));
						break;
					}
				}
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
				builder.AppendFormat("/* {0} */", ex.ToString());
				throw;
			}

			return builder.ToString();
		}

		public String GenerateReturnValueForSimpleType()
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				if (_storedProcedureSchema.OutputParameterCount == 0) { return String.Empty; }

				foreach (var parameter in _storedProcedureSchema.Parameters)
				{
					if (parameter.IsOutput == true)
					{
						builder.AppendFormat(" ({1}{2}) (((System.Data.IDbDataParameter) dbCommand.Parameters[\"{0}\"]).Value==System.DBNull.Value ? null : ((System.Data.IDbDataParameter) dbCommand.Parameters[\"{0}\"]).Value)",
						parameter.ParameterName,
						parameter.Type.FullName,
						Utility.MakeNullable(parameter.Type.ToString()));
						break;
					}
				}
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
				builder.AppendFormat("/* {0} */", ex.ToString());
				throw;
			}
			return builder.ToString();
		}

		public String GenerateStoredProcedureParameters()
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				if (_storedProcedureSchema.Parameters.Count == 0)
					return String.Empty;

				List<String> usedNames = new List<String>();
				foreach (Parameter parameter in _storedProcedureSchema.Parameters)
				{
					string fixedName = GetFixedName(usedNames, parameter.ParameterName);

					try
					{
						if (parameter.IsOutput == true)
						{
							builder.AppendFormat("RightPoint.Data.DbUtility.AddOutputParameter( dbCommand, \"{0}\", System.Data.DbType.{1}, {2} );" + Environment.NewLine,
								fixedName,
								Utility.ReturnSqlDBType(parameter.DataType, parameter.Length, false),
								Utility.GetParameterSize(parameter));
						}
						else
						{
                            string dbTypeMarker = "System.Data.DbType";
                            if(parameter.IsTableType == true)
                                dbTypeMarker = "System.Data.SqlDbType";

                            builder.AppendFormat("RightPoint.Data.DbUtility.AddInputParameter( dbCommand, \"{0}\", {1}.{2}, {3}, ({4}==null?System.DBNull.Value:(System.Object){4}) );" + Environment.NewLine,
                                parameter.ParameterName,
                                dbTypeMarker,
                                Utility.ReturnSqlDBType(parameter.DataType, parameter.Length, false),
                                Utility.GetParameterSize(parameter),
                                Utility.FormatMemberName(fixedName));
						}


						//switch (parameterSchema.Direction)
						//{
						//    case System.Data.ParameterDirection.Input:
						//        builder.AppendFormat("RightPoint.Data.DbUtility.AddInputParameter( dbCommand, \"{0}\", System.Data.DbType.{1}, {2}, ({3}==null?System.DBNull.Value:(System.Object){3}) );" + Environment.NewLine,
						//            parameterSchema.Name,
						//            parameterSchema.DataType.ToString(),
						//            TemplateHelper.GetParameterSize(parameterSchema),
						//            TemplateHelper.FormatMemberName(fixedName));
						//        break;
						//    case System.Data.ParameterDirection.InputOutput:
						//    case System.Data.ParameterDirection.Output:
						//        builder.AppendFormat("RightPoint.Data.DbUtility.AddOutputParameter( dbCommand, \"{0}\", System.Data.DbType.{1}, {2} );" + Environment.NewLine,
						//            fixedName,
						//            parameterSchema.DataType.ToString(),
						//            TemplateHelper.GetParameterSize(parameterSchema));
						//        break;
						//    case System.Data.ParameterDirection.ReturnValue:
						//    default:
						//        throw new ApplicationException("Invalid parameter direction.");
						//}
					}
					catch (Exception ex)
					{
						builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
						System.Diagnostics.Debugger.Break();
						Trace.WriteLine(ex.ToString());
						builder.AppendFormat("/* {0} */", ex.ToString());
						throw;
					}
				}
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
				builder.AppendFormat("/* {0} */", ex.ToString());
				throw;
			}
			return builder.ToString();
		}

		public String GenerateMethodParameters(bool includeParameterTypesAndNullability, bool includeTrailingCommaIfRequired)
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				if (_storedProcedureSchema.Parameters.Count == 0)
					return String.Empty;
				
				Int32 iterator = 0;
				List<String> usedNames = new List<String>();
				foreach (Parameter parameter in _storedProcedureSchema.Parameters)
				{
					if (parameter.IsOutput == true)
						continue;

					if (iterator > 0)
						builder.Append("," + Environment.NewLine);
	
					string fixedName = GetFixedName(usedNames, parameter.ParameterName);
					try
					{
						if (parameter.IsOutput == true)
						{
							builder.AppendFormat(" {0}",
							Utility.FormatMemberName(fixedName));
						}
						else
						{
							if (includeParameterTypesAndNullability == true)
							{
								builder.AppendFormat(" {0}{2} {1}",
									parameter.Type.ToString(),
									Utility.FormatMemberName(fixedName),
									Utility.MakeNullable(parameter.Type.ToString()));
							}
							else
							{
								builder.AppendFormat(" {0}",
									Utility.FormatMemberName(fixedName));
							}
						}

						//switch (parameterSchema.Direction)
						//{
						//    case System.Data.ParameterDirection.Input:
						//        if (includeParameterTypesAndNullability == true)
						//        {
						//            builder.AppendFormat(" {0}{2} {1}",
						//                parameterSchema.SystemType.ToString(),
						//                TemplateHelper.FormatMemberName(fixedName),
						//                TemplateHelper.MakeNullable(parameterSchema.SystemType.ToString()));
						//        }
						//        else
						//        {
						//            builder.AppendFormat(" {0}",
						//            TemplateHelper.FormatMemberName(fixedName));
						//        }

						//        break;
						//    case System.Data.ParameterDirection.InputOutput:
						//    case System.Data.ParameterDirection.Output:
						//    case System.Data.ParameterDirection.ReturnValue:
						//    default:
						//        throw new ApplicationException("Invalid parameter direction.");
						//}
					}
					catch (Exception ex)
					{
						builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
						System.Diagnostics.Debugger.Break();
						Trace.WriteLine(ex.ToString());
						builder.AppendFormat("/* {0} */", ex.ToString());
						throw;
					}

					iterator++;
				}
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
				builder.AppendFormat("/* {0} */", ex.ToString());
				throw;
			}

			if (builder.Length > 0 && includeTrailingCommaIfRequired == true)
				builder.Append(", ");

			return builder.ToString();
		}


		public String GenerateUnitTestParameters()
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				if (_storedProcedureSchema.Parameters.Count == 0)
					return String.Empty;
				
				Int32 iterator = 0;
				foreach (Parameter parameter in _storedProcedureSchema.Parameters)
				{
					try
					{
						if (parameter.IsOutput == false)
						{
							builder.AppendFormat(" {0}",
									GenerateUnitTestValue(parameter));
						}

						//switch (parameterSchema.Direction)
						//{
						//    case System.Data.ParameterDirection.Input:
						//        builder.AppendFormat(" {0}",
						//            GenerateUnitTestValue(parameterSchema));
						//        break;
						//    case System.Data.ParameterDirection.InputOutput:
						//    case System.Data.ParameterDirection.Output:
						//    case System.Data.ParameterDirection.ReturnValue:
						//    default:
						//        throw new ApplicationException("Invalid parameter direction.");
						//}
					}
					catch (Exception ex)
					{
						builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
						System.Diagnostics.Debugger.Break();
						Trace.WriteLine(ex.ToString());
						builder.AppendFormat("/* {0} */", ex.ToString());
						throw;
					}
					++iterator;

					if (iterator < _storedProcedureSchema.Parameters.Count)
						builder.Append("," + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
				builder.AppendFormat("/* {0} */", ex.ToString());
				throw;
			}
			return builder.ToString();
		}

		public string GenerateUnitTestValue(Parameter parameter)
		{
			switch (parameter.Type.Name)
			{
				case "Int16":
				case "Int32":
				case "Int64":
				case "Decimal":
				case "Double":
				case "Single":
					return "1";
				case "String":
					return "\"sampleString\"";
				case "DateTime":
					return "System.DateTime.Now";
				case "Boolean":
					return "true";
				case "Guid":
					return "System.Guid.NewGuid()";
				case "Byte":
					return "System.Byte.MaxValue";
				case "Byte[]":
					return "(new byte[] { System.Byte.MaxValue, System.Byte.MinValue, System.Byte.MaxValue, System.Byte.MinValue })";
				default:
					throw new Exception("Invalid value: " + parameter.Type.Name);
			}
		}

		public string GetFixedName(List<String> usedNames, string nameToTest)
		{
			int usedCounter = 0;
			foreach (string usedName in usedNames)
			{
				if (usedName.Equals(nameToTest) == true)
					usedCounter++;
			}

			usedNames.Add(nameToTest);

			string fixedName = nameToTest;

			if (usedCounter > 0)
				fixedName += usedCounter.ToString();

			return fixedName;
		}
	}
}
