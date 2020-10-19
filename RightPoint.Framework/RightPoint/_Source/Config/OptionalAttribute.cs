using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Config
{
	/// <summary>
	/// Optional allows for declaring a Field as being optional in the config file.  Default value must be provided.
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public sealed class OptionalAttribute : Attribute
	{
	}
}
