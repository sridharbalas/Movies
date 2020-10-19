using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;
using Microsoft.Win32;

namespace EventInventory.Install.InstallCustomActions
{
	/// <summary>
	/// Summary description for Installer.
	/// </summary>
	[RunInstaller(true)]
	public class Installer : System.Configuration.Install.Installer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Installer()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		public override void Install(IDictionary stateSaver)
		{
			FormSelectMachineType fsmt = new FormSelectMachineType();

			DialogResult result = fsmt.ShowDialog();

			if (result == DialogResult.Cancel)
				throw new InstallException( "The installation has been canceled." );

			if (result == DialogResult.OK)
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

			base.Install (stateSaver);
		}

		public override void Uninstall(IDictionary savedState)
		{
			CleanUpRegistry();

			base.Uninstall (savedState);
		}

		public override void Rollback(IDictionary savedState)
		{
			//CleanUpRegistry();

			base.Rollback (savedState);
		}

		private void CleanUpRegistry()
		{
			using( RegistryKey rk = Registry.LocalMachine.OpenSubKey( Configuration.EVENTINVENTORY_REGISTRY_KEY ) )
			{
				if( rk != null )
				{
					rk.Close();
					Registry.LocalMachine.DeleteSubKeyTree( Configuration.EVENTINVENTORY_REGISTRY_KEY );
				}
			}
		}



		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
