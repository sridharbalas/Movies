using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation.Analyzer
{
	/// <summary>
	/// An object to represent a column in a SQL Server table.
	/// </summary>
	public class Column
	{
		/// <summary>
		/// The name of the column in the database.
		/// </summary>
		public string ColumnName;

		/// <summary>
		/// The length of the parameter. This is how many bytes this 
		/// parameter will need to store it's data.
		/// </summary>
		public int Length;

		/// <summary>
		/// The precision of the parameter. This is how many digits appear
		/// to the left of the decimal point.
		/// </summary>
		public int? Precision;

		/// <summary>
		/// The scale of the parameter. This is how many digits appear to
		/// the right of the decimal point.
		/// </summary>
		public int? Scale;

		/// <summary>
		/// A flag to indicate if the column is an identity 
		/// (Auto Incrementing) column. 
		/// </summary>
		public bool IsIdentity;

		/// <summary>
		/// A number to indicate this columns position in the column list.
		/// </summary>
		public int Ordinal;

		/// <summary>
		/// A string indicating the datatype of this colum.
		/// </summary>
		public string DataType;

        /// <summary>
        /// A string indicating the SQL datatype of this colum.
        /// </summary>
        public string SQLDataType;

		/// <summary>
		/// A System.Type that the SQL data type of this column maps to.
		/// </summary>
		public System.Type Type;

		/// <summary>
		/// If the column is nullable, then this value is true.
		/// </summary>
		public bool IsNullable;

		/// <summary>
		/// Converts the object into a string value for debug output.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			string returnValue = string.Format(@"
				public string sColumnName: {0}
				public int nLength: {1}
				public int nPrecision: {2}
				public bool bIsIdentity: {3}
				public int nOrdinal: {4}
				public int nScale: {5}
				public string sDataType: {6}
				public System.Type tDataType: {7}
					",
						ColumnName,
						Length,
						Precision,
						IsIdentity,
						Ordinal,
						Scale,
						DataType,
						Type
						);

			return (returnValue);
		}
	}
}
