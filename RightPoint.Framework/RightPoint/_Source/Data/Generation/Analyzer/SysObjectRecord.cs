using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation.Analyzer
{
	public class SysObjectRecord
	{
		public string ObjectName;
		public string OwnerName;

		public SysObjectRecord(string sObjectName, string sOwnerName)
		{
			this.ObjectName = sObjectName;
			this.OwnerName = sOwnerName;

			if (sOwnerName != "dbo")
				Console.WriteLine("");
		}

		public override bool Equals(object obj)
		{
			SysObjectRecord compObj = (SysObjectRecord)obj;

			return (this.ObjectName == compObj.ObjectName && this.OwnerName == compObj.OwnerName);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
