using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Text.RegularExpressions;

namespace RightPoint.Config
{
	public abstract class ConfigElementCollectionItem : ConfigElement
	{
		[Optional]
		public readonly string Key = Guid.NewGuid().ToString();

		public ConfigElementCollectionItem () : base() { }

		public ConfigElementCollectionItem ( String key )
			: base()
		{
			Key = key;
		}
	}
}
