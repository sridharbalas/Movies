using System;
using System.Collections.Generic;
using System.Text;
using RightPoint.Data.Generation.Analyzer;

namespace RightPoint.Data.Generation
{
	public partial class DALEntityCollection
	{
		public readonly string EntityName;

		public DALEntityCollection(StoredProcedureSchema storedProcedureSchema)
		{
			var parsedStoredProcedureName = Utility.ParseStoredProcedureName(storedProcedureSchema.Name);
			EntityName = parsedStoredProcedureName.EntityName;
		}
	}
}
