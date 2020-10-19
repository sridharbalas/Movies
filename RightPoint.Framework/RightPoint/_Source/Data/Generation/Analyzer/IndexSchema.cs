using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation.Analyzer
{
	/// <summary>
	/// Summary description for IndexSchema.
	/// </summary>
	public class IndexSchema
	{
		public string IndexName;
		public int IndexID;
		public List<String> ColumnNames;

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("IndexSchema: {0}\r\n", this.IndexName);

			foreach (string c in this.ColumnNames)
			{
				sb.Append("\t" + c.ToString() + "\r\n");
			}

			return (sb.ToString());
		}

	}
}
