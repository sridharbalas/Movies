using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Config
{
	/// <summary>
	/// CustomKey allows for overriding the default key name of Key.  Field must be of Type String.
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public sealed class CustomKeyAttribute : Attribute
	{
	}
}
