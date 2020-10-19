using System;

namespace RightPoint.Config
{
	interface ISupportFieldValueLoading
	{
		void LoadConfiguartionValuesIntoFields ( String machineName, MachineType machineType );
	}
}