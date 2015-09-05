namespace PartManagerPlugin
{
	partial class PartViewer
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
			this.PartViewTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// PartViewTextBox
			// 
			this.PartViewTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.PartViewTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PartViewTextBox.Location = new System.Drawing.Point(0, 0);
			this.PartViewTextBox.Multiline = true;
			this.PartViewTextBox.Name = "PartViewTextBox";
			this.PartViewTextBox.ReadOnly = true;
			this.PartViewTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.PartViewTextBox.Size = new System.Drawing.Size(520, 389);
			this.PartViewTextBox.TabIndex = 0;
			// 
			// PartViewer
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(520, 389);
			this.Controls.Add(this.PartViewTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "PartViewer";
			this.Text = "Part Viewer";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox PartViewTextBox;
	}
}