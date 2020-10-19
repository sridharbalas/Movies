using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using RightPoint.Data.Generation.Analyzer;
using System.Collections;

namespace RightPoint.Data.Generation
{
	public class Utility
	{
		private Utility()
		{

		}

		/// <summary>
		/// Formats the name of the public.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static String FormatPublicName(String name)
		{
			return FormatMemberName(name);
		}

		/// <summary>
		/// Formats the name of the private.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static String FormatPrivateName(String name)
		{
			return "_" + FormatMemberName(name);
		}

		private static object _syncObject = new object();
		private static Dictionary<string, string> _memberNameCache = new Dictionary<string, string>();

		/// <summary>
		/// Formats the name of the member.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static String FormatMemberName(String name)
		{
			string memberName;

			if (_memberNameCache.TryGetValue(name, out memberName) == false)
			{
				lock (_syncObject)
				{
					if (_memberNameCache.TryGetValue(name, out memberName) == false)
					{
						memberName = name;

						if (name[0] == '@')
						{
							memberName = name.Substring(1, name.Length - 1);
						}

						//while (Type.GetType(memberName, false, true) != null || IsNameReservedString(memberName) == true)
						//	memberName = memberName + "_";

						if (IsNameReservedString(memberName) == true)
							memberName = memberName + "_";

						if (memberName.Contains("#") == true)
							memberName = memberName.Replace("#", "Number");

						if (memberName.Contains(" ") == true)
							memberName = memberName.Replace(" ", "_");

						if (memberName.Contains(".") == true)
							memberName = memberName.Replace(".", "_");

						_memberNameCache.Add(name, memberName);
					}
				}
			}

			return memberName;
		}

		//private static Regex reservedNameFinder = new Regex(
		//	  "(?:^|(?<= ))(int|bool|double|string|float|char|decimal)(?:(?= )|$)",
		//	RegexOptions.IgnoreCase
		//	| RegexOptions.CultureInvariant
		//	| RegexOptions.Compiled
		//	);


		//private SortedList<string, string> _reservedNames = null;
		//private static string [] _reservedNames = new string[] { "int", "bool", "double", "string", "float", "char", "decimal" };
		//private static List<String> reservedNames = new List<string>(new string [] { "int", "bool", "double", "string", "float", "char", "decimal" });
		//private static HashSet<String> = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private static Hashtable _reservedNames = null;
		private static Hashtable ReservedNames
		{
			get
			{
				if (_reservedNames == null)
				{
					_reservedNames = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
					_reservedNames.Add("int", "int");
					_reservedNames.Add("bool", "bool");
					_reservedNames.Add("double", "double");
					_reservedNames.Add("string", "string");
					_reservedNames.Add("float", "float");
					_reservedNames.Add("char", "char");
					_reservedNames.Add("decimal", "decimal");
				}

				return _reservedNames;
			}
		}

		public static bool IsNameReservedString(String name)
		{
			
			return ReservedNames.ContainsKey(name);

		}

		///// <summary>
		///// Parses the command names.
		///// </summary>
		///// <param name="storedProcedure">The stored procedure.</param>
		///// <returns></returns>
		//public static ParsedStoredProcedureName ParseStoredProcedureName(CommandSchema storedProcedure)
		//{
		//    return ParseStoredProcedureName(storedProcedure.Name);
		//}

		///// <summary>
		///// Evaluates the stored procedure.
		///// </summary>
		///// <param name="storedProcedure">The stored procedure.</param>
		///// <returns></returns>
		//public static GeneratedTypes EvaluateStoredProcedure(CommandSchema storedProcedure)
		//{
		//    //Debugger.Break();
		//    if ((storedProcedure.AllOutputParameters.Count > 0 && storedProcedure.CommandResults.Count > 0) ||
		//         storedProcedure.CommandResults.Count > 1)
		//    {
		//        return GeneratedTypes.InvalidComplex;
		//    }
		//    else if ((storedProcedure.AllOutputParameters.Count > 1 && storedProcedure.CommandResults.Count == 0))
		//    {
		//        return GeneratedTypes.OutputEntity;
		//    }
		//    else if ((storedProcedure.AllOutputParameters.Count == 0 && storedProcedure.CommandResults.Count == 1))
		//    {
		//        return GeneratedTypes.ResultSetCollection;
		//    }
		//    else if (storedProcedure.AllOutputParameters.Count == 1 && storedProcedure.CommandResults.Count == 0)
		//    {
		//        return GeneratedTypes.SimpleType;
		//    }
		//    else if (storedProcedure.AllOutputParameters.Count == 0 && storedProcedure.CommandResults.Count == 0)
		//    {
		//        return GeneratedTypes.ReturnValue;
		//    }
		//    else
		//    {
		//        return GeneratedTypes.Undefined;
		//    }
		//}

		private static List<String> NullableTypeExclusions = new List<string>(new string [] 
		{
 			"System.String",
			"System.Byte[]",
			"System.Object",
			"System.Data.DataTable"
		});

		public static String MakeNullable(String systemTypeName)
		{
			if (NullableTypeExclusions.Contains(systemTypeName) == true)
			{
				return String.Empty;
			}
			else
			{
				return "?";
			}
		}

		///// <summary>
		///// Evaluates the stored procedure.
		///// </summary>
		///// <param name="database">The database.</param>
		///// <param name="storedProcedure">The stored procedure.</param>
		///// <param name="databaseObjectOwner">The database object owner.</param>
		///// <returns></returns>
		//public static GeneratedTypes EvaluateStoredProcedure(DatabaseSchema database, String storedProcedure,
		//                                                      String databaseObjectOwner)
		//{
		//    return EvaluateStoredProcedure(new CommandSchema(database, storedProcedure, databaseObjectOwner, DateTime.Now));
		//}

		/// <summary>
		/// Parses the command names.
		/// </summary>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns>
		public static ParsedStoredProcedureName ParseStoredProcedureName(String storedProcedure)
		{
			//Debugger.Break();
			//Email_GetMessages
			//(FUNCTIONGROUP)_(METHODNAME(RETURNTYPE))

			//trim valid prefixes from the head of the string
			storedProcedure = Regex.Replace(storedProcedure, @"^(?:m|a|e)p_", String.Empty);

			Match m =
				Regex.Match(storedProcedure,
							 @"(?<Main>(?<FunctionGroup>(^[^_]+))(_+)(?<MethodName>((?:g|s)et|(?:upda|dele)te|add|select)?_?(?<ReturnType>(\w*)))$)",
							 RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline |
							 RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
			if (m.Groups["Main"] == null || m.Groups["Main"].ToString().Length == 0)
			{
				return new ParsedStoredProcedureName(storedProcedure, storedProcedure, storedProcedure);
			}
			else
			{
				return
					new ParsedStoredProcedureName(m.Groups["FunctionGroup"].Value, m.Groups["ReturnType"].Value,
												   m.Groups["MethodName"].Value);
			}
		}

		///// <summary>
		///// Gets the size of the parameter.
		///// </summary>
		///// <param name="parameterSchema">The parameter schema.</param>
		///// <returns></returns>
		public static String GetParameterSize(Parameter parameterSchema)
		{
			DbType dataType;

			if (Enum.IsDefined(typeof(System.Data.DbType), parameterSchema.DataType) == true)
				dataType = (DbType)Enum.Parse(typeof(System.Data.DbType), parameterSchema.DataType);
			else
				dataType = ToDbType(parameterSchema.DataType);

			switch (parameterSchema.DataType.ToLower())
			{
				case "uniqueidentifier":
				case "bit":
				case "datetime":
				case "datetime2":
				case "smalldatetime":
				case "decimal":
				case "double":
				case "money":
				case "smallmoney":
				case "real":
				case "tinyint":
				case "float":
				case "numeric":
                case "structured":
					return "0";

				case "nchar":
				case "nvarchar":
					if (parameterSchema.Length.GetValueOrDefault(0) == -1)
						return "1073741823";
					else
						return (parameterSchema.Length.GetValueOrDefault(0) / 2).ToString();

				case "ntext":
				case "xml":
					return "1073741823";

				case "text":
					return "2147483647";

				default:
					var tempLength = parameterSchema.Length.GetValueOrDefault(2147483647);
					if(tempLength < 0)
						tempLength = 2147483647;

					return tempLength.ToString();
			}
		}

		public static string ReturnSqlDBType(string dbType, int? size, Boolean returnSize)
		{
			String delimiter = ", ";
			switch (dbType)
			{
				case "bigint":
					return "Int64";
				case "binary":
					return "Binary";
				case "bit":
					return "Boolean";
				case "char":
					return "AnsiStringFixedLength" + (returnSize ? delimiter + size : String.Empty);
				case "datetime":
					return "DateTime";
				case "datetime2":
					return "DateTime";
				case "decimal":
					return "Decimal";
				case "float":
					return "Double";
				case "image":
					return "Image";
				case "int":
					return "Int32";
				case "money":
					return "Currency";
				case "nchar":
					return "StringFixedLength" + (returnSize ? delimiter + size : String.Empty);
				case "ntext":
					return "String";
				case "numeric":
					return "Decimal";
				case "nvarchar":
					return "String" + (returnSize ? delimiter + size : String.Empty);
				case "real":
					return "Single";
				case "smalldatetime":
					return "DateTime";
				case "smallint":
					return "Int16";
				case "smallmoney":
					return "Currency";
				case "text":
					return "AnsiString";
				case "timestamp":
					return "Timestamp";
				case "tinyint":
					return "Byte";
				case "uniqueidentifier":
					return "Guid";
				case "varbinary":
					return "Binary";
				case "varchar":
					return "AnsiString" + (returnSize ? delimiter + size : String.Empty);
				case "xml":
					return "Object";
				case "variant":
					return "Variant";
                case "structured":
                    return "Structured";
				default:
					return "***Error*** - " + dbType;
			}
		}


		public static System.Data.DbType ToDbType(string value)
		{
			System.Data.DbType returnValue;

			switch (value.ToLower().Trim())
			{
				case "binary":
					returnValue = DbType.Binary;
					break;

				case "char":
					returnValue = DbType.AnsiString;
					break;

				case "varbinary":
					returnValue = DbType.Binary;
					break;

				case "varchar":
					returnValue = DbType.AnsiString;
					break;

				case "bit":
					returnValue = DbType.Boolean;
					break;

				case "datetime":
					returnValue = DbType.DateTime;
					break;

				case "datetime2":
					returnValue = DbType.DateTime2;
					break;

				case "smalldatetime":
					returnValue = DbType.Date;
					break;

				case "decimal":
					returnValue = DbType.Decimal;
					break;

				case "real":
					returnValue = DbType.Single;
					break;

				case "float":
					returnValue = DbType.Double;
					break;

				case "image":
					returnValue = DbType.Binary;
					break;

				case "tinyint":
					returnValue = DbType.Byte;
					break;

				case "smallint":
					returnValue = DbType.Int16;
					break;

				case "int":
					returnValue = DbType.Int32;
					break;

				case "smallmoney":
					returnValue = DbType.Currency;
					break;

				case "money":
					returnValue = DbType.Currency;
					break;

				case "nchar":
					returnValue = DbType.String;
					break;

				case "ntext":
					returnValue = DbType.String;
					break;

				case "numeric":
					returnValue = DbType.Decimal;
					break;

				case "nvarchar":
					returnValue = DbType.String;
					break;

				case "text":
					returnValue = DbType.AnsiString;
					break;

				case "bigint":
					returnValue = DbType.Int64;
					break;

				case "uniqueidentifier":
					returnValue = DbType.Guid;
					break;

				case "xml":
					returnValue = DbType.Object;
					break;

				default:
					returnValue = DbType.AnsiString;
					break;
				//throw new Exception("An unspecified db type: " + value + " was passed, please add support for this type.");
			}

			return (returnValue);
		}

		public static System.Type ToSystemType(string value, bool isTableType)
		{
			System.Type returnValue;

            if (isTableType == true)
                return typeof(System.Data.DataTable);

			switch (value.ToLower().Trim())
			{
				case "binary":
					returnValue = typeof(System.Byte[]);
					break;

				case "char":
					returnValue = typeof(System.String);
					break;

				case "varbinary":
					returnValue = typeof(System.Byte[]);
					break;

				case "varchar":
					returnValue = typeof(System.String);
					break;

				case "bit":
					returnValue = typeof(System.Boolean);
					break;

				case "datetime":
					returnValue = typeof(System.DateTime);
					break;

				case "datetime2":
					returnValue = typeof( System.DateTime );
					break;

				case "smalldatetime":
					returnValue = typeof(System.DateTime);
					break;

				case "decimal":
					returnValue = typeof(System.Decimal);
					break;

				case "real":
					returnValue = typeof(System.Single);
					break;

				case "float":
					returnValue = typeof(System.Double);
					break;

				case "image":
					returnValue = typeof(System.Byte[]);
					break;

				case "tinyint":
					returnValue = typeof(System.Byte);
					break;

				case "smallint":
					returnValue = typeof(System.Int16);
					break;

                case "int":
                case "int identity":
					returnValue = typeof(System.Int32);
					break;

				case "smallmoney":
					returnValue = typeof(System.Decimal);
					break;

				case "money":
					returnValue = typeof(System.Decimal);
					break;

				case "nchar":
					returnValue = typeof(System.String);
					break;

				case "ntext":
					returnValue = typeof(System.String);
					break;

				case "numeric":
					returnValue = typeof(System.Decimal);
					break;

				case "nvarchar":
					returnValue = typeof(System.String);
					break;

				case "text":
					returnValue = typeof(System.String);
					break;

				case "bigint":
					returnValue = typeof(System.Int64);
					break;

				case "uniqueidentifier":
					returnValue = typeof(System.Guid);
					break;

				case "xml":
					returnValue = typeof(System.Object);
					break;

				default:
					returnValue = typeof(System.String);
					break;
				//throw new Exception("An unspecified db type: " + value + " was passed, please add support for this type.");
			}

			return (returnValue);
		}

	}
}
