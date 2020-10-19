using System;

namespace RightPoint.Config
{
	/// <summary>
	/// Allows the adding of CData elements to the config files.  Only one CData element per class.
	/// </summary>
	public class CDataConfigElement : ConfigElement
	{
		[CData]
		public readonly String Value;
	}
}