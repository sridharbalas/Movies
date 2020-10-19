using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EventInventory.Install.InstallCustomActions;
using Microsoft.Win32;
using EventInventory;

namespace StandAloneConfiguration
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			FormSelectMachineType fsmt = new FormSelectMachineType();

			if (fsmt.ShowDialog() == DialogResult.OK)
			{

				using (RegistryKey rk = Registry.LocalMachine.CreateSubKey(Configuration.EVENTINVENTORY_REGISTRY_KEY))
				{
					rk.SetValue(Configuration.EVENTINVENTORY_REGISTRY_VALUE_NAME_STRONG_KEY, fsmt.EncryptedPrimeKey);
					rk.SetValue(Configuration.EVENTINVENTORY_REGISTRY_VALUE_NAME_MACHINE_TYPE, fsmt.EncryptedMachineType);

					rk.Close();
				}

				if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node") != null)
				{
					using (RegistryKey rk = Registry.LocalMachine.CreateSubKey(Configuration.EVENTINVENTORY_64BIT_REGISTRY_KEY))
					{
						rk.SetValue(Configuration.EVENTINVENTORY_REGISTRY_VALUE_NAME_STRONG_KEY, fsmt.EncryptedPrimeKey);
						rk.SetValue(Configuration.EVENTINVENTORY_REGISTRY_VALUE_NAME_MACHINE_TYPE, fsmt.EncryptedMachineType);

						rk.Close();
					}
				}

			}
		}
	}
}
