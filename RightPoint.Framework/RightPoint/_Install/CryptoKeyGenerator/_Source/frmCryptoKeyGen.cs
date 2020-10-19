/***************************************************************************************
 * This application is considered a SECRET. This application should not be distributed
 * to developers or placed in a public area. 
 * 
 * This application is used to generate the strong encryption keys that are linked to
 * the machine type (dev, beta, stage, production) that are embedded in the 
 * EventInventory.Installer.Installer class. This class is then used by the setup merge module 
 * to place the strong encryption keys into the registry. The strong encryption keys 
 * are stored locked onto the machine so that keys taken from one machine will not 
 * work on any other. If a machine doesn't have keys installed, then the development
 * keys and machine type are used by default.
 * 
 * IMPORTANT: Development and Beta must have different check GUIDs but use the same
 * strong encryption key. 
 * 
 * Check guids are used to determine if the correct password has been specified for 
 * a given encryption method. When generating strong encryption blocks for dev and beta,
 * specify a blank password. If a prime key is entered into the prime key box, then that 
 * prime key will be used, otherwise a new random prime key is generated. The prime key
 * is the machine type specific strong encryption key. The prime key should not be stored
 * anywhere else but in the location that the installer class places it. 
 * 
 * Changing the passwords for production and stage
 * You'll need to retrieve the prime key for production and stage by running the installer,
 * changing the machine type to stage or production, and then click the "show primary key"
 * button. Then, you can cut and paste the primary key to a new window. Next, click the 
 * "New Guid" button which gives you a new check guid. Then, paste the primary key into 
 * the the primay key box on the main form. If you are creating a key for beta or dev, leave
 * the password blank, otherwise type in the new password. Then, you can cut and paste 
 * the values from the "encrypted key and guid" field and the 
 * "GUID contained in encrypted key" fields into the appropriate places in EventInventory.Constants. 
 * 
 * 
 * **************************************************************************************/


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace CryptoKeyGenerator
{
	/// <summary>
	/// Summary description for frmCryptoKeyGen.
	/// </summary>
	public class frmCryptoKeyGen : System.Windows.Forms.Form
	{
		private const string RSA_PRIVATE_KEY = "<RSAKeyValue><Modulus>ufZweIhcy8yH2PlXy7gFRA+vRjmr0Rt06+EC91C1JmF8aPcvO5qr9HCwgl98t3k+ob2i9O2IoJcPFBWYKvN3hldys598ZdVdqcPsLKJEzko6f6if/bwY6/SNevU5Yn6P3Phxfqfpyx0Kh5RT0x/dG4f2YqBYcxvtbLWSwWndoLgeMrXcfFfh+t/9gmYqRbzpENrblq0XDIQkWy4wDN/FM4i0VNYL9urN2Tc7CbM+ODJKrJeXMwWKNblbK9nqFyjwo6l5yf5R5h/cI0zpXdAokJmajIAbB4HZazHOzJzu3iZhWE+arPcULtR9R0hZaEJrs7i4SKWFwG0hjvPcQgL3wm87</Modulus><Exponent>AQAB</Exponent><P>692WABnGWgDqtM/3q3MazuBbUs/DxJan5U+N+EubpvuPe8uh19c3+ci7QvIlk46f38jyziuGTlyr9TPfYIMwlReXDmkjmbjRkcaLXMOG7E//smAyO3aAagKEzLAsha80Uo7dlM8C4/x3Z7Smluv8LX8AAjNK5evXRQbH2mSTaI9Z</P><Q>ydZQ0lyvnSpNXu9lvsu+x5aA136U3lg1mPdN/osBtQgnXaFwWf2JNuHdcmIz3YOuwOlqUz3AeVUVFspV/Ih2s21TetEPRHs9C3WE03jRs7mXSKQ5pe3XsdCHIOQCVCz3jbjkCuzBKdOx46Mn39CHih3EJq2fAmdc5wCt1yS85VSz</Q><DP>numVVY4IGL3DZH011frgVf/NJ7c5o+sJRbNa6K4wOcoFexfQe/bSDOXUXR9+d7NBPepKxOOIQqWnabZASus4wUq6iIgH51shP4EwhhhqgC7KDOlJDs0hacs2pB4ScSOBMhp1afwra2W/IU0SbF6kcha9772VOeGzERunbRoUQv9J</DP><DQ>oOhJwJsBqgMqcW/bJlNaAdXbW3Dfj6609ADcXfxF3SPl9kAYyLVWabmBCIp/nK45aDPtfvUw98m7qVKfYFcOgpa6waPfTxIMpZEH1mtwip5/m/rItp65oVmEENWVgbPjm0l2C7uZdxX8sGTHCig9lf+K++bYaFss9kBVVrpEGXP/</DQ><InverseQ>M9pYgKKtVCVwydqPLfGb6/lh8P6vC73mtu1/CsTki6iblO2H0hAX+OfIv2U7uUm8NlyFRrLhLg3MFVsLbTrhXq2LTZdsPLAGkI66gFChYdyrS22rtJCN0X083+fBlcLQJutHq9rCowYgd24YqLMwMkYlB4eB/APtjtm5jiHnqhYN</InverseQ><D>BLAFncQyMqDxO7LtV6FH4LNUg7yobBj5FYkSfhF2v4TtkefJNQNtGjhM+rxVdHqF7aXOGcZzDzk/lA4HS+3fMW+64HRAuoBqYAEkTuu4OEr+toEVp1u6dRgxCMxtqeHu1m8czUiGSCxvO4OtqUDAXhVLTKkmEaC+l2LsVBvLDmYVscD+5E1eX6mK4IMr3syKr1cd9SleO2Mb6i2anQOWEYS0rwuyYBlyMD17Sg0qTyqrO9GHaTI7MObYJ71uDNL4Kmf9hHBtJ9H12E9Mai2zrCi1LlGj1J4hyFJnLyAVVwZX2rK38fFwz6RASbWCixwJAP3jZXmbRBNDPJ8g5rtqUTHB</D></RSAKeyValue>";
		

		private System.Windows.Forms.TextBox txtEncryptionKey;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Label lblPublicKey;
		private System.Windows.Forms.TextBox txtRandomCryptoKey;
		private System.Windows.Forms.Splitter splitter3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtClearTextGuid;
		private System.Windows.Forms.Button btnNewGuid;
		private System.Windows.Forms.Splitter splitter4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Splitter splitter5;
		private System.Windows.Forms.TextBox txtPrimeKey;
		private System.Windows.Forms.Button btnGetCurrentPrimeKey;
		private System.Windows.Forms.Button btnTestInstaller;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmCryptoKeyGen()
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
				if (components != null) 
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
			this.txtEncryptionKey = new System.Windows.Forms.TextBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.lblPublicKey = new System.Windows.Forms.Label();
			this.txtRandomCryptoKey = new System.Windows.Forms.TextBox();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this.label3 = new System.Windows.Forms.Label();
			this.txtClearTextGuid = new System.Windows.Forms.TextBox();
			this.btnNewGuid = new System.Windows.Forms.Button();
			this.btnTestInstaller = new System.Windows.Forms.Button();
			this.splitter4 = new System.Windows.Forms.Splitter();
			this.txtPrimeKey = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.splitter5 = new System.Windows.Forms.Splitter();
			this.btnGetCurrentPrimeKey = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtEncryptionKey
			// 
			this.txtEncryptionKey.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtEncryptionKey.Location = new System.Drawing.Point(10, 33);
			this.txtEncryptionKey.Multiline = true;
			this.txtEncryptionKey.Name = "txtEncryptionKey";
			this.txtEncryptionKey.ReadOnly = true;
			this.txtEncryptionKey.Size = new System.Drawing.Size(596, 104);
			this.txtEncryptionKey.TabIndex = 0;
			this.txtEncryptionKey.Text = "";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(10, 204);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(596, 23);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(596, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "Encrypted Key and GUID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Location = new System.Drawing.Point(10, 227);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(596, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Password for Key";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtPassword
			// 
			this.txtPassword.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtPassword.Location = new System.Drawing.Point(10, 250);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '?';
			this.txtPassword.Size = new System.Drawing.Size(596, 20);
			this.txtPassword.TabIndex = 4;
			this.txtPassword.Text = "";
			this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(528, 584);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 5;
			this.btnOk.Text = "&Ok";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter2.Location = new System.Drawing.Point(10, 270);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(596, 21);
			this.splitter2.TabIndex = 6;
			this.splitter2.TabStop = false;
			// 
			// lblPublicKey
			// 
			this.lblPublicKey.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblPublicKey.Location = new System.Drawing.Point(10, 291);
			this.lblPublicKey.Name = "lblPublicKey";
			this.lblPublicKey.Size = new System.Drawing.Size(596, 23);
			this.lblPublicKey.TabIndex = 7;
			this.lblPublicKey.Text = "Random Public Crypto Key";
			this.lblPublicKey.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtRandomCryptoKey
			// 
			this.txtRandomCryptoKey.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtRandomCryptoKey.Location = new System.Drawing.Point(10, 314);
			this.txtRandomCryptoKey.Multiline = true;
			this.txtRandomCryptoKey.Name = "txtRandomCryptoKey";
			this.txtRandomCryptoKey.Size = new System.Drawing.Size(596, 70);
			this.txtRandomCryptoKey.TabIndex = 8;
			this.txtRandomCryptoKey.Text = "";
			// 
			// splitter3
			// 
			this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter3.Location = new System.Drawing.Point(10, 137);
			this.splitter3.Name = "splitter3";
			this.splitter3.Size = new System.Drawing.Size(596, 24);
			this.splitter3.TabIndex = 9;
			this.splitter3.TabStop = false;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Top;
			this.label3.Location = new System.Drawing.Point(10, 161);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(596, 23);
			this.label3.TabIndex = 10;
			this.label3.Text = "GUID Contained In Encrypted Key";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtClearTextGuid
			// 
			this.txtClearTextGuid.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtClearTextGuid.Location = new System.Drawing.Point(10, 184);
			this.txtClearTextGuid.Name = "txtClearTextGuid";
			this.txtClearTextGuid.Size = new System.Drawing.Size(596, 20);
			this.txtClearTextGuid.TabIndex = 11;
			this.txtClearTextGuid.Text = "";
			// 
			// btnNewGuid
			// 
			this.btnNewGuid.Location = new System.Drawing.Point(448, 584);
			this.btnNewGuid.Name = "btnNewGuid";
			this.btnNewGuid.TabIndex = 12;
			this.btnNewGuid.Text = "&New Guid";
			this.btnNewGuid.Click += new System.EventHandler(this.btnNewGuid_Click);
			// 
			// btnTestInstaller
			// 
			this.btnTestInstaller.Location = new System.Drawing.Point(8, 576);
			this.btnTestInstaller.Name = "btnTestInstaller";
			this.btnTestInstaller.Size = new System.Drawing.Size(104, 23);
			this.btnTestInstaller.TabIndex = 13;
			this.btnTestInstaller.Text = "&Test Installer";
			this.btnTestInstaller.Click += new System.EventHandler(this.btnTestInstaller_Click);
			// 
			// splitter4
			// 
			this.splitter4.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter4.Location = new System.Drawing.Point(10, 384);
			this.splitter4.Name = "splitter4";
			this.splitter4.Size = new System.Drawing.Size(596, 21);
			this.splitter4.TabIndex = 14;
			this.splitter4.TabStop = false;
			// 
			// txtPrimeKey
			// 
			this.txtPrimeKey.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtPrimeKey.Location = new System.Drawing.Point(10, 428);
			this.txtPrimeKey.Multiline = true;
			this.txtPrimeKey.Name = "txtPrimeKey";
			this.txtPrimeKey.Size = new System.Drawing.Size(596, 44);
			this.txtPrimeKey.TabIndex = 16;
			this.txtPrimeKey.Text = "";
			// 
			// label4
			// 
			this.label4.Dock = System.Windows.Forms.DockStyle.Top;
			this.label4.Location = new System.Drawing.Point(10, 405);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(596, 23);
			this.label4.TabIndex = 15;
			this.label4.Text = "Prime Key";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// splitter5
			// 
			this.splitter5.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter5.Location = new System.Drawing.Point(10, 472);
			this.splitter5.Name = "splitter5";
			this.splitter5.Size = new System.Drawing.Size(596, 21);
			this.splitter5.TabIndex = 17;
			this.splitter5.TabStop = false;
			// 
			// btnGetCurrentPrimeKey
			// 
			this.btnGetCurrentPrimeKey.Location = new System.Drawing.Point(120, 576);
			this.btnGetCurrentPrimeKey.Name = "btnGetCurrentPrimeKey";
			this.btnGetCurrentPrimeKey.Size = new System.Drawing.Size(152, 23);
			this.btnGetCurrentPrimeKey.TabIndex = 20;
			this.btnGetCurrentPrimeKey.Text = "&Get Current Prime Key";
			this.btnGetCurrentPrimeKey.Click += new System.EventHandler(this.btnGetCurrentPrimeKey_Click);
			// 
			// frmCryptoKeyGen
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(616, 605);
			this.Controls.Add(this.btnGetCurrentPrimeKey);
			this.Controls.Add(this.splitter5);
			this.Controls.Add(this.txtPrimeKey);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.splitter4);
			this.Controls.Add(this.btnTestInstaller);
			this.Controls.Add(this.btnNewGuid);
			this.Controls.Add(this.txtRandomCryptoKey);
			this.Controls.Add(this.lblPublicKey);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.txtClearTextGuid);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.splitter3);
			this.Controls.Add(this.txtEncryptionKey);
			this.Controls.Add(this.label1);
			this.DockPadding.All = 10;
			this.Name = "frmCryptoKeyGen";
			this.Text = "Crypto Key Generator";
			this.Load += new System.EventHandler(this.frmCryptoKeyGen_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmCryptoKeyGen());
		}

		private void frmCryptoKeyGen_Load(object sender, System.EventArgs e)
		{
			//GenerateRsaPairs();

			ResetGuid();

			RijndaelManaged encryptorProvider = new RijndaelManaged();
			encryptorProvider.GenerateKey();
			txtRandomCryptoKey.Text = Util.ByteArrayToString( encryptorProvider.Key );
		}



		private void btnOk_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void txtPassword_TextChanged(object sender, System.EventArgs e)
		{
			RegenerateEncryptedKey();
		}

		private void btnNewGuid_Click(object sender, System.EventArgs e)
		{
			ResetGuid();
		}

		private void ResetGuid()
		{
			this.txtClearTextGuid.Text = Guid.NewGuid().ToString();
			this.txtPrimeKey.Text = "";

			RegenerateEncryptedKey();
		}
		
		private void RegenerateEncryptedKey()
		{
			byte [] iv = EventInventory.Cryptography.CryptoUtility.GenerateIvFromPassword( txtPassword.Text );
			byte [] key = EventInventory.Configuration.PUBLIC_ENCRYPTION_KEY;
			string guidToEncrypt = this.txtClearTextGuid.Text;

			byte [] encryptedGuid = EventInventory.Cryptography.CryptoUtility.EncryptStringToBytes( guidToEncrypt, key, iv );

			System.Security.Cryptography.RijndaelManaged rm = new RijndaelManaged();

			if( this.txtPrimeKey.Text.Length >  0)
			{
				rm.Key = Util.StringToByteArray( this.txtPrimeKey.Text );
			}
			else
			{
				rm.GenerateKey();
				this.txtPrimeKey.Text = Util.ByteArrayToString( rm.Key );
			}

			

			byte [] encryptedDataKey = EventInventory.Cryptography.CryptoUtility.EncryptBytesToBytes( rm.Key, key, iv );
			
			byte [] multiplexedEncryptedData = EventInventory.Cryptography.PseudoRandomCrypto.MultiplexByteArrays( encryptedGuid, encryptedDataKey );

			//byte [] encryptedDataKey = EventInventory.Cryptography.CryptoUtility.En

			this.txtEncryptionKey.Text = Util.ByteArrayToString( multiplexedEncryptedData );
		}

		
		private void btnTestInstaller_Click(object sender, System.EventArgs e)
		{
			EventInventory.Install.InstallCustomActions.FormSelectMachineType fsmt = new EventInventory.Install.InstallCustomActions.FormSelectMachineType();
			fsmt.ShowDialog();
		}

		/*
		private void GenerateRsaPairs()
		{
			System.Security.Cryptography.RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2064);

			csp.FromXmlString( RSA_PRIVATE_KEY );

			string privateXml = csp.ToXmlString(true);
			string publicXml = csp.ToXmlString(false);

			this.txtPublicCSP.Text = publicXml;
			this.txtPrivateCSP.Text = privateXml;
		}
		*/

		private void btnGetCurrentPrimeKey_Click(object sender, System.EventArgs e)
		{
			frmPrimeKeyDisplay fpkd = new frmPrimeKeyDisplay();
			fpkd.ShowDialog();
		}
	}
}
