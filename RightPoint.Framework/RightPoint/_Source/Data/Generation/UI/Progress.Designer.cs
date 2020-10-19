namespace RightPoint.Data.Generation.UI
{
	partial class Progress
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label_StoredProcName = new System.Windows.Forms.Label();
			this.progressBar_Progress = new System.Windows.Forms.ProgressBar();
			this.label_ProcessedProcCount = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Generating: ";
			// 
			// label_StoredProcName
			// 
			this.label_StoredProcName.AutoSize = true;
			this.label_StoredProcName.Location = new System.Drawing.Point(107, 13);
			this.label_StoredProcName.Name = "label_StoredProcName";
			this.label_StoredProcName.Size = new System.Drawing.Size(154, 17);
			this.label_StoredProcName.TabIndex = 1;
			this.label_StoredProcName.Text = "label_StoredProcName";
			// 
			// progressBar_Progress
			// 
			this.progressBar_Progress.Location = new System.Drawing.Point(12, 65);
			this.progressBar_Progress.Name = "progressBar_Progress";
			this.progressBar_Progress.Size = new System.Drawing.Size(845, 23);
			this.progressBar_Progress.TabIndex = 2;
			// 
			// label_ProcessedProcCount
			// 
			this.label_ProcessedProcCount.AutoSize = true;
			this.label_ProcessedProcCount.Location = new System.Drawing.Point(17, 42);
			this.label_ProcessedProcCount.Name = "label_ProcessedProcCount";
			this.label_ProcessedProcCount.Size = new System.Drawing.Size(179, 17);
			this.label_ProcessedProcCount.TabIndex = 3;
			this.label_ProcessedProcCount.Text = "label_ProcessedProcCount";
			// 
			// Progress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(869, 111);
			this.ControlBox = false;
			this.Controls.Add(this.label_ProcessedProcCount);
			this.Controls.Add(this.progressBar_Progress);
			this.Controls.Add(this.label_StoredProcName);
			this.Controls.Add(this.label1);
			this.Name = "Progress";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "DAL Generation Progress";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.Label label_StoredProcName;
		public System.Windows.Forms.Label label_ProcessedProcCount;
		public System.Windows.Forms.ProgressBar progressBar_Progress;
	}
}