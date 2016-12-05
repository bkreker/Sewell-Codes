namespace QueryMining.Forms
{
    partial class MineTypeSelectorForm
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
            this.cBoxMineType = new System.Windows.Forms.ComboBox();
            this.chkBxAvgAll = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cBoxMineType
            // 
            this.cBoxMineType.FormattingEnabled = true;
            this.cBoxMineType.Location = new System.Drawing.Point(167, 11);
            this.cBoxMineType.Name = "cBoxMineType";
            this.cBoxMineType.Size = new System.Drawing.Size(121, 21);
            this.cBoxMineType.TabIndex = 0;
            // 
            // chkBxAvgAll
            // 
            this.chkBxAvgAll.AutoSize = true;
            this.chkBxAvgAll.Checked = true;
            this.chkBxAvgAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxAvgAll.Location = new System.Drawing.Point(14, 13);
            this.chkBxAvgAll.Name = "chkBxAvgAll";
            this.chkBxAvgAll.Size = new System.Drawing.Size(115, 17);
            this.chkBxAvgAll.TabIndex = 1;
            this.chkBxAvgAll.Text = "Average All Values";
            this.chkBxAvgAll.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(190, 49);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(34, 49);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MineTypeSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 84);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkBxAvgAll);
            this.Controls.Add(this.cBoxMineType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MineTypeSelectorForm";
            this.Text = "MineTypeSelectorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cBoxMineType;
        private System.Windows.Forms.CheckBox chkBxAvgAll;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}