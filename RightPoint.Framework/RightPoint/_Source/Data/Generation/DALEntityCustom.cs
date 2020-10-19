using System;
using System.Collections.Generic;
using System.Text;
using RightPoint.Data.Generation.Analyzer;
using System.Globalization;
using System.Diagnostics;

namespace RightPoint.Data.Generation
{
	public partial class DALEntity
	{
		public readonly StoredProcedureSchema StoredProcedureSchema;
		public readonly string EntityName;
		public readonly string ContainingName;
		public readonly string HelperMethodName;
		public readonly string EntityCode;

		public DALEntity(StoredProcedureSchema storedProcedureSchema)
		{
			StoredProcedureSchema = storedProcedureSchema;

			var parsedStoredProcedureName = Utility.ParseStoredProcedureName(storedProcedureSchema.Name);
			EntityName = parsedStoredProcedureName.EntityName;
			ContainingName = parsedStoredProcedureName.ContainingName;
			HelperMethodName = parsedStoredProcedureName.HelperMethodName;

			StringBuilder entityCode = new StringBuilder();

			if (storedProcedureSchema.StoredProcedureType == StoredProcedureType.MultiRow ||
				storedProcedureSchema.StoredProcedureType == StoredProcedureType.BothSingleAndMultiRow ||
				storedProcedureSchema.StoredProcedureType == StoredProcedureType.SingleRecordSet)
			{
				// GenerateMembersFromResults
				entityCode.AppendLine(GenerateMembersFromResults());
				entityCode.AppendLine(GenerateFullConstructorFromResults());
			}
			else if (storedProcedureSchema.StoredProcedureType == StoredProcedureType.SingleRow ||
				storedProcedureSchema.StoredProcedureType == StoredProcedureType.SingleValue) 
			{
				// GenerateMembersFromOutputParameters
				entityCode.AppendLine(GenerateMembersFromOutputParameters());
				entityCode.AppendLine(GenerateFullConstructorFromOutputParameters());
			}

			EntityCode = entityCode.ToString();
		}


		public String GenerateMembersFromResults()
		{
			if (StoredProcedureSchema.ResultSetColumns.Count == 0)
				return string.Empty;

			StringBuilder builder = new StringBuilder();

			builder.AppendLine("// GenerateMembersFromResults");

			Dictionary<String, int> usedNames = new Dictionary<String, int>();
			foreach (var column in StoredProcedureSchema.ResultSetColumns)
			{
				builder.Append(TypeMembers(usedNames, column));
				builder.Append(Environment.NewLine);
			}
			return builder.ToString();
		}

		public String GenerateMembersFromOutputParameters()
		{
			if (StoredProcedureSchema.Parameters.Count == 0) { return String.Empty; }

			StringBuilder builder = new StringBuilder();

			builder.AppendLine("/* GenerateMembersFromOutputParameters */");

			Dictionary<String, int> usedNames = new Dictionary<String, int>();
			foreach (Parameter parameter in StoredProcedureSchema.Parameters)
			{
				if (parameter.IsOutput == false)
					continue;

				builder.Append(TypeMembers(usedNames, parameter));
				builder.Append(Environment.NewLine);
			}
			return builder.ToString();
		}

		public String GenerateFullConstructorFromOutputParameters()
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				//only generate the constructor if there is a need
				//if there are no parameters then don't generate a custom constructor
				if (StoredProcedureSchema.Parameters.Count == 0) { return String.Empty; }

				//Generate full public constructor
				builder.Append("/// <summary>" + Environment.NewLine);
				builder.AppendFormat("/// Initializes a new instance of the <see cref=\"{0}Record\"/> class." + Environment.NewLine + "", EntityName);
				builder.Append("/// </summary>" + Environment.NewLine);
				builder.AppendFormat("\t\tpublic {0}Record (", EntityName);
				Int32 iterator = 0;

				Dictionary<String, int> usedNames = new Dictionary<String, int>();
				foreach (var parameter in StoredProcedureSchema.Parameters)
				{
					if (parameter.IsOutput == false)
						continue;

					if (iterator > 0)
						builder.Append("," + Environment.NewLine);
	
					string fixedName = GetFixedName(usedNames, parameter.ParameterName);
					builder.AppendFormat(" {0}" + Utility.MakeNullable(parameter.Type.ToString()) + " {1}",
										  parameter.Type.ToString(),
										  Utility.FormatMemberName(fixedName));
					iterator++;
				}

				builder.Append(" )" + Environment.NewLine + "\t\t{" + Environment.NewLine);
				usedNames = new Dictionary<String, int>();
				foreach (var parameter in StoredProcedureSchema.Parameters)
				{
					if (parameter.IsOutput == false)
						continue;

					string fixedName = GetFixedName(usedNames, parameter.ParameterName);
					builder.AppendFormat("\t\t\t{0} = {1};" + Environment.NewLine + "", Utility.FormatPrivateName(fixedName), Utility.FormatMemberName(fixedName));
				}
				builder.Append(Environment.NewLine + "\t\t}" + Environment.NewLine);
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
			}
			return builder.ToString();
		}

		public String GenerateFullConstructorFromResults()
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				//only generate the constructor if there is a need
				//if there are no parameters then don't generate a custom constructor
				if (StoredProcedureSchema.ResultSetColumns.Count == 0) { return String.Empty; }

				//Generate full public constructor
				builder.Append("/// <summary>" + Environment.NewLine);
				builder.AppendFormat("/// Initializes a new instance of the <see cref=\"{0}Record\"/> class." + Environment.NewLine + "", EntityName);
				builder.Append("/// </summary>" + Environment.NewLine);
				builder.AppendFormat("\t\tpublic {0}Record (", EntityName);
				Int32 iterator = 0;

				Dictionary<String, int> usedNames = new Dictionary<String, int>();
				foreach (var column in StoredProcedureSchema.ResultSetColumns)
				{
					string fixedName = GetFixedName(usedNames, column.ColumnName);

					builder.AppendFormat(" {0}" + Utility.MakeNullable(column.Type.ToString()) + " {1}",
										  column.Type.ToString(),
										  Utility.FormatMemberName(fixedName));
					++iterator;
					if (iterator < StoredProcedureSchema.ResultSetColumns.Count)
					{
						builder.Append("," + Environment.NewLine);
					}
				}
				builder.Append(" )" + Environment.NewLine + "\t\t{" + Environment.NewLine);

				usedNames = new Dictionary<String, int>();
				foreach (var column in StoredProcedureSchema.ResultSetColumns)
				{
					string fixedName = GetFixedName(usedNames, column.ColumnName);

					builder.AppendFormat("\t\t\t{0} = {1};" + Environment.NewLine, Utility.FormatPrivateName(fixedName), Utility.FormatMemberName(fixedName));
				}
				builder.Append(Environment.NewLine + "\t\t}" + Environment.NewLine);
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
			}
			return builder.ToString();
		}

		public String TypeMembers(Dictionary<String, int> usedNames, Analyzer.Column column)
		{
			return TypeMembers(usedNames, column.ColumnName.Trim(), column.Type.ToString());
		}

		public String TypeMembers(Dictionary<String, int> usedNames, Analyzer.Parameter parameter)
		{
			return TypeMembers(usedNames, parameter.ParameterName.Trim(), parameter.Type.ToString());
		}

		public String TypeMembers(Dictionary<String, int> usedNames, String name, String typeOf)
		{
			name = GetFixedName(usedNames, name);

			StringBuilder builder = new StringBuilder();
			try
			{
				if (name == null && name.Length == 0)
				{
					throw new ApplicationException("A name must be provided.");
				}

				String publicName = Utility.FormatPublicName(name);
				String privateName = Utility.FormatPrivateName(name);

				builder.Append("/// <summary>" + Environment.NewLine);
				builder.Append("/// Gets the " + publicName + " (" + typeOf + ")" + Environment.NewLine);
				builder.Append("/// </summary>" + Environment.NewLine);
				builder.Append("/// <value>The " + privateName + ".</value>" + Environment.NewLine);
				builder.Append("\t\tprivate " + typeOf + Utility.MakeNullable(typeOf) + " " + privateName + ";" + Environment.NewLine + Environment.NewLine);
				builder.Append("\t\tpublic " + typeOf + Utility.MakeNullable(typeOf) + " " + publicName + Environment.NewLine + "");
				builder.Append("\t\t{" + Environment.NewLine);
				builder.Append("\t\t\tget { return " + privateName + "; }" + Environment.NewLine);
				builder.Append("\t\t\tset { " + privateName + " = value; }" + Environment.NewLine);
				builder.Append("\t\t}" + Environment.NewLine);
			}
			catch (Exception ex)
			{
				builder.AppendLine(Environment.NewLine + "/*" + Environment.NewLine + ex.ToString() + Environment.NewLine + "*/" + Environment.NewLine);
				System.Diagnostics.Debugger.Break();
				Trace.WriteLine(ex.ToString());
			}
			return builder.ToString();
		}

		private object _syncObject = new object();

		public string GetFixedName(Dictionary<String, int> usedNames, string nameToTest)
		{
			if (usedNames.ContainsKey(nameToTest) == false)
			{
				lock (_syncObject)
				{
					if (usedNames.ContainsKey(nameToTest) == false)
					{
						usedNames.Add(nameToTest, 0);
						return nameToTest;
					}
				}
			}

			int usedCounter;

			lock (_syncObject)
			{
				usedCounter = ++usedNames[nameToTest];
			}

			return nameToTest + usedCounter.ToString();

			//int usedCounter = 0;
			//foreach (string usedName in usedNames)
			//{
			//	if (usedName.Equals(nameToTest) == true)
			//		usedCounter++;
			//}

			//usedNames.Add(nameToTest);

			//string fixedName = nameToTest;

			//if (usedCounter > 0)
			//	fixedName += usedCounter.ToString();

			//return fixedName;
		}


	}
}
