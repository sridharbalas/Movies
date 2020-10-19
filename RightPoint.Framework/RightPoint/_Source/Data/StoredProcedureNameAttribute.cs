using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data
{
	public class StoredProcedureNameAttribute : Attribute
	{
		public string StoredProcedureName { get; set; }

		public StoredProcedureNameAttribute(string StoredProcedureName)
		{
			this.StoredProcedureName = StoredProcedureName;
		}
	}
}
