using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation
{
	/// <summary>
	/// Identified types of generated data access entities
	/// </summary>
	public enum GeneratedTypes
	{
		/// <summary>
		/// The stored procedure has not been processed.
		/// </summary>
		Undefined = -1,
		/// <summary>
		/// Generates:	Entity (based on resultset row), Collection, Method returns Collection
		/// Signature:	Stored Procedure contains no output parameters, returns result set
		/// </summary>
		ResultSetCollection,
		/// <summary>
		/// Generates:	Entity (based on output paramters), Method returns Entity
		/// Signature:	Stored Procedures has more than one output parameters, does not return resultset
		/// </summary>
		OutputEntity,
		/// <summary>
		/// Generates:	Method returns simple value
		/// Signature:	Stored Procedures has only one output parameters, does not return resultset
		/// </summary>
		SimpleType,
		/// <summary>
		/// Generates:	Method returns Int32 value
		/// Signature:	Method contains no output parameters, does not return resultset, only returns a value
		/// </summary>
		ReturnValue,
		/// <summary>
		/// Generates:	Error, will stop generation and return an error
		/// Signature:	Method returns multiple resultsets and/or (method contains output parameters and returns a resultset)
		/// </summary>
		InvalidComplex
	}
}
