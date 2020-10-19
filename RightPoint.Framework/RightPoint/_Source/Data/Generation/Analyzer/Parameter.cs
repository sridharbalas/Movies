using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation.Analyzer
{
	/// <summary>
	/// A representation of a stored procedure parameter.
	/// </summary>
	public class Parameter
	{
		/// <summary>
		/// The name of the parameter.
		/// </summary>
		public string ParameterName;

		/// <summary>
		/// The name of the SQL data type of the parameter.
		/// </summary>
		public string DataType;

		/// <summary>
		/// The System.Type that maps to the data type of the paramater.
		/// </summary>
		public System.Type Type;

		/// <summary>
		/// The length of the parameter. This is how many bytes this 
		/// parameter will need to store it's data.
		/// </summary>
		public int? Length;

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
		/// If the parameter is marked as an output parameter, then this
		/// flag will be set to true. 
		/// </summary>
		public bool IsOutput;


        /// <summary>
        /// If the parameter is a table type variable, then this will be true.
        /// </summary>
        public bool IsTableType;

        /// <summary>
        /// Number of columns if a table type.
        /// </summary>
        public int? TableTypeColumnCount;

        /// <summary>
		/// Dumps this object into a string for debug printing.
		/// </summary>
		/// <returns>A string representation of the parameter object.</returns>
		public override string ToString()
		{
			string returnValue = string.Format(@"
					public string sParameterName: {0}
					public string sDataType: {1}
					public int nLength: {2}
					public int nPrecision: {3}
					public int nScale: {4}
					public bool bIsOutput: {5}
					public bool IsTableType: {6}
					public int nTableTypeColumnCount: {7}
				",
				ParameterName,
				DataType,
				Length,
				Precision,
				Scale,
				IsOutput,
                IsTableType,
                TableTypeColumnCount
				);

			return (returnValue);
		}
	}
}
