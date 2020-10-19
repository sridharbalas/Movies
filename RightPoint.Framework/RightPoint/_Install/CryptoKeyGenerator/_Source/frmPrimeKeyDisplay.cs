using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CryptoKeyGenerator
{
	/// <summary>
	/// Summary description for frmPrimeKeyDisplay.
	/// </summary>
	public class frmPrimeKeyDisplay : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtMachineType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtPrimeKey;
		private System.Windows.Forms.Button Ok;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmPrimeKeyDisplay()
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtMachineType = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPrimeKey = new System.Windows.Forms.TextBox();
			this.Ok = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Machine Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtMachineType
			// 
			this.txtMachineType.Location = new System.Drawing.Point(40, 32);
			this.txtMachineType.Name = "txtMachineType";
			this.txtMachineType.ReadOnly = true;
			this.txtMachineType.Size = new System.Drawing.Size(224, 20);
			this.txtMachineType.TabIndex = 1;
			this.txtMachineType.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 72);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Text = "Prime Key";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtPrimeKey
			// 
			this.txtPrimeKey.Location = new System.Drawing.Point(40, 96);
			this.txtPrimeKey.Multiline = true;
			this.txtPrimeKey.Name = "txtPrimeKey";
			this.txtPrimeKey.ReadOnly = true;
			this.txtPrimeKey.Size = new System.Drawing.Size(648, 136);
			this.txtPrimeKey.TabIndex = 3;
			this.txtPrimeKey.Text = "";
			// 
			// Ok
			// 
			this.Ok.Location = new System.Drawing.Point(616, 264);
			this.Ok.Name = "Ok";
			this.Ok.TabIndex = 4;
			this.Ok.Text = "&Ok";
			this.Ok.Click += new System.EventHandler(this.Ok_Click);
			// 
			// frmPrimeKeyDisplay
			// 
			this.AcceptButton = this.Ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(720, 301);
			this.Controls.Add(this.Ok);
			this.Controls.Add(this.txtPrimeKey);
			this.Controls.Add(this.txtMachineType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPrimeKeyDisplay";
			this.ShowInTaskbar = false;
			this.Text = "Prime Key Display";
			this.Load += new System.EventHandler(this.frmPrimeKeyDisplay_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmPrimeKeyDisplay_Load(object sender, System.EventArgs e)
		{
			this.txtPrimeKey.Text = Util.ByteArrayToString( EventInventory.Configuration.StrongEncryptionKey );
			this.txtMachineType.Text = EventInventory.Configuration.MachineType.ToString();
		}

		private void Ok_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
