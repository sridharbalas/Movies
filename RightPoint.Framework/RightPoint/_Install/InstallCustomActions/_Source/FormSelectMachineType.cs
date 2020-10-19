using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace EventInventory.Install.InstallCustomActions
{
	/// <summary>
	/// Summary description for SelectMachineType.
	/// </summary>
	public class FormSelectMachineType : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbStage;
		private System.Windows.Forms.RadioButton rbProduction;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Panel pPassword;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		private byte [] _encryptedMachineType = null;
		private System.Windows.Forms.RadioButton rbDevelopment;
		private System.Windows.Forms.RadioButton rbLocal;
		private System.Windows.Forms.RadioButton rbQa;
		private System.Windows.Forms.RadioButton rbPreview;
		private Panel pCountersign;
		private TextBox txtCountersignPassword;
		private Label label2;
		private Button btnSkip;
		private RadioButton rbIntegration;
	
		public byte [] EncryptedMachineType
		{
			get { return _encryptedMachineType; }
		}

		private byte [] _encryptedPrimeKey = null;
		public byte [] EncryptedPrimeKey
		{
			get { return _encryptedPrimeKey; }
		}


		// For debugging.
		public static void Main()
		{
			FormSelectMachineType fsmt = new FormSelectMachineType();
			fsmt.ShowDialog();
		}


		public FormSelectMachineType()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.pCountersign = new System.Windows.Forms.Panel();
			this.txtCountersignPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.rbPreview = new System.Windows.Forms.RadioButton();
			this.rbQa = new System.Windows.Forms.RadioButton();
			this.pPassword = new System.Windows.Forms.Panel();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.rbProduction = new System.Windows.Forms.RadioButton();
			this.rbStage = new System.Windows.Forms.RadioButton();
			this.rbDevelopment = new System.Windows.Forms.RadioButton();
			this.rbLocal = new System.Windows.Forms.RadioButton();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSkip = new System.Windows.Forms.Button();
			this.rbIntegration = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.pCountersign.SuspendLayout();
			this.pPassword.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbIntegration);
			this.groupBox1.Controls.Add(this.pCountersign);
			this.groupBox1.Controls.Add(this.rbPreview);
			this.groupBox1.Controls.Add(this.rbQa);
			this.groupBox1.Controls.Add(this.pPassword);
			this.groupBox1.Controls.Add(this.rbProduction);
			this.groupBox1.Controls.Add(this.rbStage);
			this.groupBox1.Controls.Add(this.rbDevelopment);
			this.groupBox1.Controls.Add(this.rbLocal);
			this.groupBox1.Location = new System.Drawing.Point(19, 18);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(429, 358);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Select Machine Type";
			// 
			// pCountersign
			// 
			this.pCountersign.Controls.Add(this.txtCountersignPassword);
			this.pCountersign.Controls.Add(this.label2);
			this.pCountersign.Location = new System.Drawing.Point(10, 283);
			this.pCountersign.Name = "pCountersign";
			this.pCountersign.Size = new System.Drawing.Size(412, 66);
			this.pCountersign.TabIndex = 7;
			// 
			// txtCountersignPassword
			// 
			this.txtCountersignPassword.Location = new System.Drawing.Point(10, 28);
			this.txtCountersignPassword.Name = "txtCountersignPassword";
			this.txtCountersignPassword.PasswordChar = '?';
			this.txtCountersignPassword.Size = new System.Drawing.Size(211, 22);
			this.txtCountersignPassword.TabIndex = 5;
			this.txtCountersignPassword.TextChanged += new System.EventHandler(this.OnPassphraseChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(163, 19);
			this.label2.TabIndex = 4;
			this.label2.Text = "Countersign Password";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// rbPreview
			// 
			this.rbPreview.Location = new System.Drawing.Point(16, 104);
			this.rbPreview.Name = "rbPreview";
			this.rbPreview.Size = new System.Drawing.Size(128, 28);
			this.rbPreview.TabIndex = 8;
			this.rbPreview.Text = "Preview";
			this.rbPreview.CheckedChanged += new System.EventHandler(this.OnMachineTypeChanged);
			// 
			// rbQa
			// 
			this.rbQa.Location = new System.Drawing.Point(16, 80);
			this.rbQa.Name = "rbQa";
			this.rbQa.Size = new System.Drawing.Size(128, 28);
			this.rbQa.TabIndex = 7;
			this.rbQa.Text = "Qa";
			this.rbQa.CheckedChanged += new System.EventHandler(this.OnMachineTypeChanged);
			// 
			// pPassword
			// 
			this.pPassword.Controls.Add(this.txtPassword);
			this.pPassword.Controls.Add(this.label1);
			this.pPassword.Location = new System.Drawing.Point(10, 210);
			this.pPassword.Name = "pPassword";
			this.pPassword.Size = new System.Drawing.Size(412, 66);
			this.pPassword.TabIndex = 6;
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(10, 28);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '?';
			this.txtPassword.Size = new System.Drawing.Size(211, 22);
			this.txtPassword.TabIndex = 5;
			this.txtPassword.TextChanged += new System.EventHandler(this.OnPassphraseChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(163, 19);
			this.label1.TabIndex = 4;
			this.label1.Text = "Machine Type Password";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// rbProduction
			// 
			this.rbProduction.Location = new System.Drawing.Point(16, 176);
			this.rbProduction.Name = "rbProduction";
			this.rbProduction.Size = new System.Drawing.Size(128, 28);
			this.rbProduction.TabIndex = 3;
			this.rbProduction.Text = "Production";
			this.rbProduction.CheckedChanged += new System.EventHandler(this.OnMachineTypeChanged);
			// 
			// rbStage
			// 
			this.rbStage.Location = new System.Drawing.Point(16, 128);
			this.rbStage.Name = "rbStage";
			this.rbStage.Size = new System.Drawing.Size(128, 28);
			this.rbStage.TabIndex = 2;
			this.rbStage.Text = "Stage";
			this.rbStage.CheckedChanged += new System.EventHandler(this.OnMachineTypeChanged);
			// 
			// rbDevelopment
			// 
			this.rbDevelopment.Location = new System.Drawing.Point(16, 57);
			this.rbDevelopment.Name = "rbDevelopment";
			this.rbDevelopment.Size = new System.Drawing.Size(128, 27);
			this.rbDevelopment.TabIndex = 1;
			this.rbDevelopment.Text = "Development";
			this.rbDevelopment.CheckedChanged += new System.EventHandler(this.OnMachineTypeChanged);
			// 
			// rbLocal
			// 
			this.rbLocal.Checked = true;
			this.rbLocal.Location = new System.Drawing.Point(16, 32);
			this.rbLocal.Name = "rbLocal";
			this.rbLocal.Size = new System.Drawing.Size(128, 28);
			this.rbLocal.TabIndex = 0;
			this.rbLocal.TabStop = true;
			this.rbLocal.Text = "Local";
			this.rbLocal.CheckedChanged += new System.EventHandler(this.OnMachineTypeChanged);
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(245, 383);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(90, 26);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "&Ok";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(358, 383);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 26);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSkip
			// 
			this.btnSkip.Location = new System.Drawing.Point(137, 382);
			this.btnSkip.Name = "btnSkip";
			this.btnSkip.Size = new System.Drawing.Size(90, 27);
			this.btnSkip.TabIndex = 3;
			this.btnSkip.Text = "&Skip";
			this.btnSkip.UseVisualStyleBackColor = true;
			this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
			// 
			// rbIntegration
			// 
			this.rbIntegration.Location = new System.Drawing.Point(16, 152);
			this.rbIntegration.Name = "rbIntegration";
			this.rbIntegration.Size = new System.Drawing.Size(128, 28);
			this.rbIntegration.TabIndex = 9;
			this.rbIntegration.Text = "Integration";
			// 
			// FormSelectMachineType
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(503, 432);
			this.ControlBox = false;
			this.Controls.Add(this.btnSkip);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.groupBox1);
			this.Name = "FormSelectMachineType";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Machine Type";
			this.Load += new System.EventHandler(this.FormSelectMachineType_Load);
			this.groupBox1.ResumeLayout(false);
			this.pCountersign.ResumeLayout(false);
			this.pCountersign.PerformLayout();
			this.pPassword.ResumeLayout(false);
			this.pPassword.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion


		private void FormSelectMachineType_Load(object sender, System.EventArgs e)
		{
			//Debugger.Break();

			switch (Configuration.MachineType)
			{
				case MachineType.Development:
					rbDevelopment.Checked = true;
					break;

				case MachineType.Local:
					rbLocal.Checked = true;
					break;

				case MachineType.Preview:
					rbPreview.Checked = true;
					break;

				case MachineType.Production:
					rbProduction.Checked = true;
					break;

				case MachineType.Qa:
					rbQa.Checked = true;
					break;

				case MachineType.Stage:
					rbStage.Checked = true;
					break;

				case MachineType.Integration:
					rbIntegration.Checked = true;
					break;

				default:
					rbLocal.Checked = true;
					break;
			}

			txtPassword.Text = String.Empty;
			txtCountersignPassword.Text = String.Empty;

			ResetControls();
		}

		private void OnMachineTypeChanged(object sender, System.EventArgs e)
		{
			ResetControls();
		}

		private void ResetControls()
		{
			MachineType machineType = GetMachineType();

			if(machineType != Configuration.MachineType)
			{
				this.txtPassword.Text = "";
				txtCountersignPassword.Text = "";
			}

			SetOkButtonEnabled(machineType);

			SetPasswordVisible(machineType);
		}

		private MachineType GetMachineType()
		{
			MachineType machineType = MachineType.Local;

			if( this.rbLocal.Checked )
				machineType = MachineType.Local;
			else if( this.rbDevelopment.Checked )
				machineType = MachineType.Development;
			else if( this.rbQa.Checked )
				machineType = MachineType.Qa;
			else if( this.rbPreview.Checked )
				machineType = MachineType.Preview;
			else if( this.rbStage.Checked )
				machineType = MachineType.Stage;
			else if (this.rbIntegration.Checked)
				machineType = MachineType.Integration;
			else if( this.rbProduction.Checked )
				machineType = MachineType.Production;

			return machineType;
		}

		private void SetOkButtonEnabled(MachineType machineType)
		{
			bool machineTypeDoesntRequirePassword = machineType == MachineType.Local || machineType == MachineType.Development;
			bool passwordIsValidForMachineType = IsPasswordValidForMachineType( machineType );

			if( machineTypeDoesntRequirePassword || passwordIsValidForMachineType )
				this.btnOk.Enabled = true;
			else
				this.btnOk.Enabled = false;
		}

		private void SetPasswordVisible(MachineType machineType)
		{
			if (machineType == MachineType.Local || machineType == MachineType.Development)
			{
				pPassword.Visible = false;
				pCountersign.Visible = false;
			}
			else
			{
				if (machineType == MachineType.Production)
					pCountersign.Visible = true;
				else
					pCountersign.Visible = false;

				pPassword.Visible = true;
			}
		}

		private bool IsPasswordValidForMachineType(MachineType machineType)
		{
			bool returnValue = false;

			switch( machineType )
			{
				case MachineType.Qa:
				case MachineType.Preview:
				case MachineType.Stage:
				case MachineType.Integration:
				case MachineType.Production:
					returnValue = IsPasswordValid( machineType );
					break;
			}

			return returnValue;
		}

		private byte [] GetEncryptedMultiplexedData(MachineType machineType)
		{
			byte [] returnValue = null;

			switch( machineType )
			{
				case MachineType.Local:
					returnValue = Configuration.ENCRYPTED_LOCAL_GUID;
					break;

				case MachineType.Development:
					returnValue = Configuration.ENCRYPTED_DEVELOPMENT_GUID;
					break;

				case MachineType.Qa:
					returnValue = Configuration.ENCRYPTED_QA_GUID;
					break;

				case MachineType.Preview:
					returnValue = Configuration.ENCRYPTED_PREVIEW_GUID;
					break;

				case MachineType.Stage:
					returnValue = Configuration.ENCRYPTED_STAGE_GUID;
					break;

				case MachineType.Integration:
					returnValue = Configuration.ENCRYPTED_INTEGRATION_GUID;
					break;

				case MachineType.Production:
					returnValue = Configuration.ENCRYPTED_PRODUCTION_GUID;
					break;

				default:
					throw new NotSupportedException("MachineType: " + machineType + " is not supported.");
			}

			return returnValue;
		}

		private string GetMachineTypeGuid(MachineType machineType)
		{
			string returnValue = null;

			switch( machineType )
			{
				case MachineType.Local:
					returnValue = Configuration.LOCAL_GUID;
					break;

				case MachineType.Development:
					returnValue = Configuration.DEVELOPMENT_GUID;
					break;

				case MachineType.Qa:
					returnValue = Configuration.QA_GUID;
					break;

				case MachineType.Preview:
					returnValue = Configuration.PREVIEW_GUID;
					break;

				case MachineType.Stage:
					returnValue = Configuration.STAGE_GUID;
					break;

				case MachineType.Integration:
					returnValue = Configuration.INTEGRATION_GUID;
					break;

				case MachineType.Production:
					returnValue = Configuration.PRODUCTION_GUID;
					break;

				default:
					throw new NotSupportedException("MachineType: " + machineType + " is not supported.");
			}

			return returnValue;
		}

		private bool IsPasswordValid(MachineType machineType)
		{
			byte [] encryptedMultiplexedData = GetEncryptedMultiplexedData( machineType );
			string machineGuid = GetMachineTypeGuid( machineType );

			return GetDecryptedGuid(encryptedMultiplexedData, machineType) == machineGuid;
		}

		private string GetPassphrase(MachineType machineType)
		{
			string passphrase = txtPassword.Text;

			if (machineType == MachineType.Production)
				passphrase += "_" + txtCountersignPassword.Text;

			return passphrase;
		}

		private string GetDecryptedGuid(byte [] encryptedMultiplexedData, MachineType machineType)
		{
			string passphrase = GetPassphrase(machineType);

			byte[] iv = Cryptography.CryptoUtility.GenerateIvFromPassword(passphrase);
			byte [] key = Configuration.PUBLIC_ENCRYPTION_KEY;

			byte [] encryptedGuid = Cryptography.PseudoRandomCrypto.DemultiplexByteArray( encryptedMultiplexedData, Cryptography.PseudoRandomCrypto.MultiplexedByteArrayPosition.FirstByteArray );
			string decryptedGuid = Cryptography.CryptoUtility.DecryptStringFromBytes( encryptedGuid, key, iv );
			
			return decryptedGuid;
		}

		private byte [] GetDecryptedKey(byte [] encryptedMultiplexedData, MachineType machineType)
		{
			string passphrase = GetPassphrase(machineType);

			byte[] iv = Cryptography.CryptoUtility.GenerateIvFromPassword(passphrase);
			byte [] key = Configuration.PUBLIC_ENCRYPTION_KEY;

			byte [] encryptedKey = Cryptography.PseudoRandomCrypto.DemultiplexByteArray( encryptedMultiplexedData, Cryptography.PseudoRandomCrypto.MultiplexedByteArrayPosition.SecondByteArray );

			byte [] decryptedKey = Cryptography.CryptoUtility.DecryptBytesFromBytes( encryptedKey, key, iv );

			return decryptedKey;
		}

		private void OnPassphraseChanged(object sender, System.EventArgs e)
		{
			MachineType machineType = GetMachineType();


			SetOkButtonEnabled( machineType );
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			MachineType machineType = GetMachineType();
			byte [] encryptedMultiplexedData = GetEncryptedMultiplexedData( machineType );

			string machineName = Environment.MachineName;
			byte [] iv = Cryptography.CryptoUtility.GenerateIvFromPassword( machineName );
			byte [] key = Configuration.PUBLIC_ENCRYPTION_KEY;

			byte[] decryptedKey = GetDecryptedKey(encryptedMultiplexedData, machineType);

			// Encrypt the prime key with the public encryption key using the machineName as
			// the IV. 
			this._encryptedPrimeKey = Cryptography.CryptoUtility.EncryptBytesToBytes( decryptedKey, key, iv );
			this._encryptedMachineType = Cryptography.CryptoUtility.EncryptStringToBytes( machineType.ToString(), key, iv );

			/*
			using( RegistryKey rk = Registry.LocalMachine.CreateSubKey( Configuration.EVENTINVENTORY_REGISTRY_KEY ) )
			{
				rk.SetValue( Configuration.EVENTINVENTORY_REGISTRY_VALUE_NAME_STRONG_KEY, encryptedKey );
				rk.SetValue( Configuration.EVENTINVENTORY_REGISTRY_VALUE_NAME_MACHINE_TYPE, encryptedMachineType );

				rk.Close();
			}
			*/

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnSkip_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Ignore;
			this.Close();
		}
	}
}
