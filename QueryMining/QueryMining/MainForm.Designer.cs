namespace QueryMining
{
    partial class MainForm
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
            this.inFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtBoxInFIle = new System.Windows.Forms.TextBox();
            this.txtBoxOutFile = new System.Windows.Forms.TextBox();
            this.outFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // inFileDialog
            // 
            this.inFileDialog.DefaultExt = "csv";
            this.inFileDialog.Title = "Select the File To Mine From";
            this.inFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.inFileDialog_FileOk);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(234, 13);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(123, 29);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "&Import CSV File";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(234, 63);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(123, 29);
            this.btnSelectFolder.TabIndex = 0;
            this.btnSelectFolder.Text = "Select";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(234, 107);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(123, 43);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(12, 107);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(216, 43);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "&Go!";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtBoxInFIle
            // 
            this.txtBoxInFIle.Location = new System.Drawing.Point(12, 16);
            this.txtBoxInFIle.Name = "txtBoxInFIle";
            this.txtBoxInFIle.ReadOnly = true;
            this.txtBoxInFIle.Size = new System.Drawing.Size(216, 22);
            this.txtBoxInFIle.TabIndex = 1;
            this.txtBoxInFIle.Text = "Select a .csv file to import...";
            // 
            // txtBoxOutFile
            // 
            this.txtBoxOutFile.Location = new System.Drawing.Point(12, 66);
            this.txtBoxOutFile.Name = "txtBoxOutFile";
            this.txtBoxOutFile.ReadOnly = true;
            this.txtBoxOutFile.Size = new System.Drawing.Size(216, 22);
            this.txtBoxOutFile.TabIndex = 1;
            this.txtBoxOutFile.Text = "Select where to save the new file...";
            // 
            // outFileDialog
            // 
            this.outFileDialog.DefaultExt = "csv";
            this.outFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.outFileFolderDialog_FileOk);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 161);
            this.Controls.Add(this.txtBoxOutFile);
            this.Controls.Add(this.txtBoxInFIle);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.btnImport);
            this.Name = "MainForm";
            this.Text = "QueryWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog inFileDialog;
        private System.Windows.Forms.TextBox txtBoxOutFile;
        private System.Windows.Forms.TextBox txtBoxInFIle;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.SaveFileDialog outFileDialog;
    }
}

