namespace QueryMining
{
    partial class ImportForm
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
            this.btnGo = new System.Windows.Forms.Button();
            this.txtBoxInFile = new System.Windows.Forms.TextBox();
            this.outFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.cboxAvgAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // inFileDialog
            // 
            this.inFileDialog.DefaultExt = "csv";
            this.inFileDialog.Filter = "CSV Files|*.csv";
            this.inFileDialog.Title = "Select the File To Mine From";
            this.inFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.inFileDialog_FileOk);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(161, 42);
            this.btnImport.Margin = new System.Windows.Forms.Padding(2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(98, 22);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "&Select CSV File";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Location = new System.Drawing.Point(285, 41);
            this.btnGo.Margin = new System.Windows.Forms.Padding(2);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(98, 22);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "&Begin Import";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtBoxInFile
            // 
            this.txtBoxInFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxInFile.Location = new System.Drawing.Point(10, 12);
            this.txtBoxInFile.Margin = new System.Windows.Forms.Padding(2);
            this.txtBoxInFile.Name = "txtBoxInFile";
            this.txtBoxInFile.Size = new System.Drawing.Size(375, 20);
            this.txtBoxInFile.TabIndex = 1;
            this.txtBoxInFile.Text = "Select a .csv file to import...";
            this.txtBoxInFile.DoubleClick += new System.EventHandler(this.txtBoxInFile_DoubleClick);
            // 
            // outFileDialog
            // 
            this.outFileDialog.DefaultExt = "csv";
            this.outFileDialog.Filter = "CSV Files|*.csv";
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 68);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.MarqueeAnimationSpeed = 0;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(395, 10);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 2;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_RunWorkerCompleted);
            // 
            // cboxAvgAll
            // 
            this.cboxAvgAll.AutoSize = true;
            this.cboxAvgAll.Checked = true;
            this.cboxAvgAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxAvgAll.Location = new System.Drawing.Point(12, 45);
            this.cboxAvgAll.Name = "cboxAvgAll";
            this.cboxAvgAll.Size = new System.Drawing.Size(123, 17);
            this.cboxAvgAll.TabIndex = 5;
            this.cboxAvgAll.Text = "Average All Columns";
            this.cboxAvgAll.UseVisualStyleBackColor = true;
            this.cboxAvgAll.CheckedChanged += new System.EventHandler(this.rBtnAvgAll_CheckedChanged);
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 78);
            this.Controls.Add(this.cboxAvgAll);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtBoxInFile);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(306, 45);
            this.Name = "ImportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog inFileDialog;
        private System.Windows.Forms.TextBox txtBoxInFile;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.SaveFileDialog outFileDialog;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.CheckBox cboxAvgAll;
    }
}

