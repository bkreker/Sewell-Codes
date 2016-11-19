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
            this.btnClose = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtBoxInFile = new System.Windows.Forms.TextBox();
            this.outFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.rBtnAvgAll = new System.Windows.Forms.RadioButton();
            this.rBtnAvgSome = new System.Windows.Forms.RadioButton();
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
            this.btnImport.Location = new System.Drawing.Point(395, 10);
            this.btnImport.Margin = new System.Windows.Forms.Padding(2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(92, 40);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "&Import CSV File";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(395, 87);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2);
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
            this.btnGo.Margin = new System.Windows.Forms.Padding(2);
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
            this.txtBoxInFile.Margin = new System.Windows.Forms.Padding(2);
            this.txtBoxInFile.Name = "txtBoxInFile";
            this.txtBoxInFile.Size = new System.Drawing.Size(383, 20);
            this.txtBoxInFile.TabIndex = 1;
            this.txtBoxInFile.Text = "Select a .csv file to import...";
            // 
            // outFileDialog
            // 
            this.outFileDialog.DefaultExt = "csv";
            this.outFileDialog.Filter = "CSV Files|*.csv";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 128);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.MarqueeAnimationSpeed = 0;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(381, 19);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 2;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_RunWorkerCompleted);
            // 
            // rBtnAvgAll
            // 
            this.rBtnAvgAll.AutoSize = true;
            this.rBtnAvgAll.Checked = true;
            this.rBtnAvgAll.Location = new System.Drawing.Point(35, 50);
            this.rBtnAvgAll.Name = "rBtnAvgAll";
            this.rBtnAvgAll.Size = new System.Drawing.Size(136, 17);
            this.rBtnAvgAll.TabIndex = 4;
            this.rBtnAvgAll.TabStop = true;
            this.rBtnAvgAll.Text = "Average All Aggregates";
            this.rBtnAvgAll.UseVisualStyleBackColor = true;
            this.rBtnAvgAll.CheckedChanged += new System.EventHandler(this.rBtnAvgAll_CheckedChanged);
            // 
            // rBtnAvgSome
            // 
            this.rBtnAvgSome.AutoSize = true;
            this.rBtnAvgSome.Location = new System.Drawing.Point(192, 50);
            this.rBtnAvgSome.Name = "rBtnAvgSome";
            this.rBtnAvgSome.Size = new System.Drawing.Size(154, 17);
            this.rBtnAvgSome.TabIndex = 5;
            this.rBtnAvgSome.Text = "Only Average Avg Columns";
            this.rBtnAvgSome.UseVisualStyleBackColor = true;
            this.rBtnAvgSome.CheckedChanged += new System.EventHandler(this.rBtnAvgAll_CheckedChanged);
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 155);
            this.Controls.Add(this.rBtnAvgSome);
            this.Controls.Add(this.rBtnAvgAll);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtBoxInFile);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(306, 45);
            this.Name = "ImportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QueryWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog inFileDialog;
        private System.Windows.Forms.TextBox txtBoxInFile;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.SaveFileDialog outFileDialog;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.RadioButton rBtnAvgAll;
        private System.Windows.Forms.RadioButton rBtnAvgSome;
    }
}

