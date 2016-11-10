namespace QueryMining
{
    partial class AnalyzeForm
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
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.outFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtn1Word = new System.Windows.Forms.RadioButton();
            this.rbtn2Words = new System.Windows.Forms.RadioButton();
            this.rbtn3Words = new System.Windows.Forms.RadioButton();
            this.btnBegin = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(605, 562);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(354, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // dgvResults
            // 
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(0, 0);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.Size = new System.Drawing.Size(1250, 556);
            this.dgvResults.TabIndex = 1;
            this.dgvResults.VirtualMode = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1163, 562);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(1070, 562);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // outFileDialog
            // 
            this.outFileDialog.DefaultExt = "CSV(*.csv)";
            this.outFileDialog.Filter = "CSV Files|*.csv";
            this.outFileDialog.Title = "Where To Export the File?";
            this.outFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.outFileDialog_FileOk);
            // 
            // lblRowCount
            // 
            this.lblRowCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRowCount.AutoSize = true;
            this.lblRowCount.Location = new System.Drawing.Point(76, 569);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(13, 13);
            this.lblRowCount.TabIndex = 3;
            this.lblRowCount.Text = "0";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 569);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Rows:";
            // 
            // rbtn1Word
            // 
            this.rbtn1Word.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbtn1Word.AutoSize = true;
            this.rbtn1Word.Location = new System.Drawing.Point(188, 565);
            this.rbtn1Word.Name = "rbtn1Word";
            this.rbtn1Word.Size = new System.Drawing.Size(101, 17);
            this.rbtn1Word.TabIndex = 5;
            this.rbtn1Word.Text = "Mine for 1 Word";
            this.rbtn1Word.UseVisualStyleBackColor = true;
            this.rbtn1Word.CheckedChanged += new System.EventHandler(this.rbtnWordCount_CheckedChanged);
            // 
            // rbtn2Words
            // 
            this.rbtn2Words.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbtn2Words.AutoSize = true;
            this.rbtn2Words.Checked = true;
            this.rbtn2Words.Location = new System.Drawing.Point(336, 565);
            this.rbtn2Words.Name = "rbtn2Words";
            this.rbtn2Words.Size = new System.Drawing.Size(106, 17);
            this.rbtn2Words.TabIndex = 5;
            this.rbtn2Words.TabStop = true;
            this.rbtn2Words.Text = "Mine for 2 Words";
            this.rbtn2Words.UseVisualStyleBackColor = true;
            this.rbtn2Words.CheckedChanged += new System.EventHandler(this.rbtnWordCount_CheckedChanged);
            // 
            // rbtn3Words
            // 
            this.rbtn3Words.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbtn3Words.AutoSize = true;
            this.rbtn3Words.Enabled = false;
            this.rbtn3Words.Location = new System.Drawing.Point(484, 565);
            this.rbtn3Words.Name = "rbtn3Words";
            this.rbtn3Words.Size = new System.Drawing.Size(106, 17);
            this.rbtn3Words.TabIndex = 5;
            this.rbtn3Words.Text = "Mine for 3 Words";
            this.rbtn3Words.UseVisualStyleBackColor = true;
            this.rbtn3Words.CheckedChanged += new System.EventHandler(this.rbtnWordCount_CheckedChanged);
            // 
            // btnBegin
            // 
            this.btnBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBegin.Location = new System.Drawing.Point(978, 562);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(75, 23);
            this.btnBegin.TabIndex = 2;
            this.btnBegin.Text = "Mine Queries";
            this.btnBegin.UseVisualStyleBackColor = true;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // AnalyzeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1250, 591);
            this.Controls.Add(this.rbtn3Words);
            this.Controls.Add(this.rbtn2Words);
            this.Controls.Add(this.rbtn1Word);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblRowCount);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.progressBar1);
            this.Name = "AnalyzeForm";
            this.Text = "AnalyzeForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog outFileDialog;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbtn1Word;
        private System.Windows.Forms.RadioButton rbtn2Words;
        private System.Windows.Forms.RadioButton rbtn3Words;
        private System.Windows.Forms.Button btnBegin;
    }
}