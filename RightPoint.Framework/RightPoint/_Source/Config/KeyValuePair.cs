using System;

namespace RightPoint.Config
{
	public class KeyValuePair : ConfigElementCollectionItem
	{
		public readonly String Value;
	}

	public class KeyValuePairCollection : ConfigElementCollection<KeyValuePair> { }
}