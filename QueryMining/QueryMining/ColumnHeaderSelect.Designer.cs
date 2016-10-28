namespace QueryMining
{
    partial class ColumnHeaderSelect
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
            this.lstBxColumnNames = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstBxColumnNames
            // 
            this.lstBxColumnNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstBxColumnNames.FormattingEnabled = true;
            this.lstBxColumnNames.Location = new System.Drawing.Point(0, 0);
            this.lstBxColumnNames.Name = "lstBxColumnNames";
            this.lstBxColumnNames.Size = new System.Drawing.Size(147, 171);
            this.lstBxColumnNames.TabIndex = 0;
            // 
            // ColumnHeaderSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(147, 171);
            this.Controls.Add(this.lstBxColumnNames);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColumnHeaderSelect";
            this.Text = "ColumnHeaderSelect";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstBxColumnNames;
    }
}