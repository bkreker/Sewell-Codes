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
            this.txtBoxInFile = new System.Windows.Forms.TextBox();
            this.txtBoxOutFile = new System.Windows.Forms.TextBox();
            this.outFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backGroundWorker = new System.ComponentModel.BackgroundWorker();
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
            this.btnImport.Location = new System.Drawing.Point(395, 10);
            this.btnImport.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(92, 24);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "&Import CSV File";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(395, 50);
            this.btnSelectFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(92, 24);
            this.btnSelectFolder.TabIndex = 0;
            this.btnSelectFolder.Text = "Select";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectOutFile_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(395, 87);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 59);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(9, 87);
            this.btnGo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(382, 35);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "&Go!";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtBoxInFile
            // 
            this.txtBoxInFile.Location = new System.Drawing.Point(9, 12);
            this.txtBoxInFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtBoxInFile.Name = "txtBoxInFile";
            this.txtBoxInFile.Size = new System.Drawing.Size(383, 20);
            this.txtBoxInFile.TabIndex = 1;
            this.txtBoxInFile.Text = "Select a .csv file to import...";
            // 
            // txtBoxOutFile
            // 
            this.txtBoxOutFile.Location = new System.Drawing.Point(9, 53);
            this.txtBoxOutFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtBoxOutFile.Name = "txtBoxOutFile";
            this.txtBoxOutFile.Size = new System.Drawing.Size(383, 20);
            this.txtBoxOutFile.TabIndex = 1;
            this.txtBoxOutFile.Text = "Select where to save the new file...";
            // 
            // outFileDialog
            // 
            this.outFileDialog.DefaultExt = "csv";
            this.outFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.outFileFolderDialog_FileOk);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 128);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressBar1.MarqueeAnimationSpeed = 0;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(381, 19);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 2;
            // 
            // backGroundWorker
            // 
            this.backGroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_DoWork);
            this.backGroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 154);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtBoxOutFile);
            this.Controls.Add(this.txtBoxInFile);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.btnImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(306, 45);
            this.Name = "MainForm";
            this.Text = "QueryWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog inFileDialog;
        private System.Windows.Forms.TextBox txtBoxOutFile;
        private System.Windows.Forms.TextBox txtBoxInFile;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.SaveFileDialog outFileDialog;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backGroundWorker;
    }
}

