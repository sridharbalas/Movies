using System;

namespace RightPoint.Config
{
	[AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
	public sealed class CDataAttribute : Attribute
	{
	}
}