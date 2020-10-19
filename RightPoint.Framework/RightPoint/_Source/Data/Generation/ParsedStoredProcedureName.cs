using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation
{

	public class ParsedStoredProcedureName
	{
		String _containingName;
		String _entityName;
		String _helperMethodName;

		/// <summary>
		/// Gets or sets the name of the record.
		/// </summary>
		/// <value>The name of the record.</value>
		public String EntityName
		{
			get { return _entityName; }
			set { _entityName = value; }
		}

		/// <summary>
		/// Gets or sets the name of the method.
		/// </summary>
		/// <value>The name of the method.</value>
		public String HelperMethodName
		{
			get { return _helperMethodName; }
			set { _helperMethodName = value; }
		}

		public string ContainingName
		{
			get { return _containingName; }
			set { _containingName = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParsedStoredProcedureName"/> class.
		/// </summary>
		/// <param name="entityName">Name of the record.</param>
		/// <param name="helperMethodName">Name of the method.</param>
		public ParsedStoredProcedureName(String containingName, String entityName, String helperMethodName)
		{
			_containingName = containingName;
			_entityName = entityName;
			_helperMethodName = helperMethodName;
		}
	}
}
