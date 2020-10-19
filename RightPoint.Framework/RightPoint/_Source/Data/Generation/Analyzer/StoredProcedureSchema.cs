using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation.Analyzer
{
	/// <summary>
	/// An object that represents all the known facts about a particular
	/// stored procedure. 
	/// </summary>
	public class StoredProcedureSchema
	{
		/// <summary>
		/// The user id of this objects owner in SQL server. Typically, 
		/// this value will be "dbo" unless otherwise specified at 
		/// stored procedure creation time.
		/// </summary>
		public string Owner;

		/// <summary>
		/// The name of the stored procedure.
		/// </summary>
		public string Name;

		/// <summary>
		/// An ordered array of Parameter objects that represent
		/// the parameters of the stored procedure. This property 
		/// may be an empty array.
		/// </summary>
		public List<Parameter> Parameters;

		/// <summary>
		/// An ordered array of Column objects that represent the
		/// columns that may be returned by the stored procedure after 
		/// execution. This property may be an empty array.
		/// </summary>
		public List<Column> ResultSetColumns;

		

		/// <summary>
		/// An array of table schemas that are used by this stored procedure.
		/// </summary>
		//public List<TableSchema> TableSchemas;

		private int _outputParameterCount = -1;

		/// <summary>
		/// Discovers how many output parameters are in the stored
		/// procedure definition.
		/// </summary>
		/// <returns>A count of output parameters defined for this stored procedure.</returns>
		public int OutputParameterCount
		{
			get
			{
				if (_outputParameterCount > -1)
					return (_outputParameterCount);

				_outputParameterCount = 0;
				foreach (Parameter p in Parameters)
				{
					if (p.IsOutput)
					{
						_outputParameterCount++;
					}
				}

				return (_outputParameterCount);
			}
		}

		/// <summary>
		/// Tests to see if the stored procedure has any output parameters defined.
		/// </summary>
		/// <returns>Returns true if the stored procedure has at least one output
		/// parameter defined.</returns>
		public bool HasOutputParameters
		{
			get
			{
				bool returnValue = false;

				if (this.OutputParameterCount > 0)
					returnValue = true;

				return (returnValue);
			}
		}

		private StoredProcedureType _storedProcedureType = StoredProcedureType.Unknown;
		/// <summary>
		/// Determines the type of stored procedure that this object is representing.
		/// </summary>
		/// <returns>They type of this object's stored procedure. See the comments for StoredProcedureType for more info.</returns>
		public StoredProcedureType StoredProcedureType
		{
			get
			{
				if (_storedProcedureType != StoredProcedureType.Unknown)
					return (_storedProcedureType);

				if (ResultSetColumns.Count > 0)
				{
					if (this.HasOutputParameters == true)
					{
						_storedProcedureType = StoredProcedureType.BothSingleAndMultiRow;
					}
					else
					{
						//// If there are any parameters, then there is a chance that
						//// this sp is selecting data based on a primary key, 
						//// or by an identity. If it is, then the stored procedure 
						//// type will be SingleRecordSet, otherwise it will be MultiRow.
						//if (this.Parameters.Count > 0)
						//{
						//	if (AreAllParametersUniqueKeys == true)
						//		_storedProcedureType = StoredProcedureType.SingleRecordSet;
						//	else
						//		_storedProcedureType = StoredProcedureType.MultiRow;
						//}
						//else
						//{
							_storedProcedureType = StoredProcedureType.MultiRow;
						//}
					}
				}
				else
				{
					if (this.OutputParameterCount == 0)
					{
						_storedProcedureType = StoredProcedureType.NoResultSet;
					}
					else if (this.OutputParameterCount == 1)
					{
						_storedProcedureType = StoredProcedureType.SingleValue;
					}
					else if (this.OutputParameterCount > 1)
					{
						_storedProcedureType = StoredProcedureType.SingleRow;
					}
				}

				return (_storedProcedureType);
			}
		}

		/// <summary>
		/// If there is only one table in the depending tables array, 
		/// and all the stored procedure parameters are either PKs or 
		/// Identities in the depending table, then this function returns true. 
		/// </summary>
		/// <returns></returns>
		//private bool AreAllParametersUniqueKeys
		//{
		//	get
		//	{
		//		bool returnValue = false;

		//		// There must be only one referenced table in the sp, any
		//		// joins will make unique indexes irrelevant.
		//		if (this.TableSchemas.Count == 1)
		//		{
		//			TableSchema t = this.TableSchemas[0];

		//			foreach (IndexSchema uniqueIndex in t.UniqueIndexes)
		//			{
		//				int matchingKeyCount = 0;

		//				foreach (Parameter p in this.Parameters)
		//				{
		//					//bool matchedPrimaryKeyParameter = false;
		//					string parameterName = p.ParameterName.Replace("@", "");

		//					foreach (string keyName in uniqueIndex.ColumnNames)
		//					{
		//						if (keyName == parameterName)
		//						{
		//							matchingKeyCount++;
		//							break;
		//						}
		//					}
		//				}

		//				// We found an unique index that matches all columns to 
		//				// all the parameters for this stored procedure,
		//				// so we know that we will only get one row back from this
		//				// sp call.
		//				if (matchingKeyCount == uniqueIndex.ColumnNames.Count)
		//				{
		//					returnValue = true;
		//					break;
		//				}
		//			}

		//			// If we didn't find an exact match on a unique index, then
		//			// check all the parameters to see if one lines up with an 
		//			// identity column
		//			if (returnValue == false)
		//			{
		//				bool hasIdentityParameter = false;
		//				foreach (Parameter p in this.Parameters)
		//				{
		//					string parameterName = p.ParameterName.Replace("@", "");
		//					foreach (Column c in t.TableColumns)
		//					{
		//						if (c.IsIdentity == true)
		//						{
		//							if (c.ColumnName == parameterName)
		//							{
		//								hasIdentityParameter = true;
		//								break;
		//							}
		//						}
		//					}

		//					if (hasIdentityParameter)
		//						break;
		//				}

		//				if (hasIdentityParameter)
		//					returnValue = true;
		//			}

		//		}

		//		return (returnValue);
		//	}
		//}
	}
}
