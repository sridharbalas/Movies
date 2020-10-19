using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation.Analyzer
{
	/// <summary>
	/// This indicates the type of data that the stored procedure will 
	/// return when it is called. The "BothSingleAndMultiRow" indicate
	/// that the stored procedure defined both output parameters, 
	/// and returns a result set, which is unsupported by CodeGenerator,
	/// and will be treated as if the stored procedure were returning
	/// multiple rows. A single row stored procedure is one that defines
	/// more than one output parameter. A single value stored procedure
	/// has only one output parameter defined. A SingleRecordSet is a stored
	/// procedure that is returning only one row as a RecordSet instead of
	/// via output parameters.
	/// </summary>
	public enum StoredProcedureType
	{
		Unknown,
		NoResultSet,
		SingleRow,
		MultiRow,
		SingleValue,
		BothSingleAndMultiRow,
		SingleRecordSet
	}
}
