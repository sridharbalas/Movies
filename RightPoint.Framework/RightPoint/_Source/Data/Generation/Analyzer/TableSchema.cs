using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation.Analyzer
{
	/// <summary>
	/// An object representing a SQL Server table.
	/// </summary>
	public class TableSchema
	{
		/// <summary>
		/// The name of the table.
		/// </summary>
		public string TableName;

		/// <summary>
		/// A string array of column names that are primary keys.
		/// </summary>
		public List<string> PrimaryKeyColumns;

		/// <summary>
		/// A Column object array of columns that belong to this table.
		/// </summary>
		public List<Column> TableColumns;

		/// <summary>
		/// An array of unique indexes that are tied to this table schema.
		/// </summary>
		public List<IndexSchema> UniqueIndexes;

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("\r\n========================\r\n\tsTableName: {0}\r\n", TableName);
			sb.AppendFormat("\tPrimaryKeys:\r\n");
			foreach (string s in PrimaryKeyColumns)
			{
				sb.AppendFormat("\t\t{0}", s);
			}

			foreach (Column c in TableColumns)
				sb.AppendFormat("\tColumn: {0}", c.ToString());

			foreach (IndexSchema i in this.UniqueIndexes)
				sb.Append(i.ToString());

			return (sb.ToString());
		}

	}
}
