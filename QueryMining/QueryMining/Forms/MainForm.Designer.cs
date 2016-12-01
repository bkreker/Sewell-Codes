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
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvMineResults = new System.Windows.Forms.DataGridView();
            this.outFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.lblTitleOutput = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mineQueriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAvgAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMine1Word = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMine2Words = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMine3Words = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMineResults)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.PerformBackgroundWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(1061, 911);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(472, 28);
            this.progressBar1.TabIndex = 0;
            // 
            // dgvMineResults
            // 
            this.dgvMineResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvMineResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMineResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMineResults.Location = new System.Drawing.Point(0, 0);
            this.dgvMineResults.Margin = new System.Windows.Forms.Padding(4);
            this.dgvMineResults.Name = "dgvMineResults";
            this.dgvMineResults.RowTemplate.DefaultCellStyle.Format = "0.##";
            this.dgvMineResults.RowTemplate.DefaultCellStyle.NullValue = null;
            this.dgvMineResults.Size = new System.Drawing.Size(1549, 874);
            this.dgvMineResults.TabIndex = 1;
            this.dgvMineResults.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvResults_RowsAdded);
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
            this.lblRowCount.Location = new System.Drawing.Point(108, 919);
            this.lblRowCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(16, 17);
            this.lblRowCount.TabIndex = 3;
            this.lblRowCount.Text = "0";
            // 
            // lblTitleOutput
            // 
            this.lblTitleOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTitleOutput.AutoSize = true;
            this.lblTitleOutput.Location = new System.Drawing.Point(16, 919);
            this.lblTitleOutput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleOutput.Name = "lblTitleOutput";
            this.lblTitleOutput.Size = new System.Drawing.Size(93, 17);
            this.lblTitleOutput.TabIndex = 4;
            this.lblTitleOutput.Text = "Output Rows:";
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mineQueriesToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1549, 28);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip2";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.mineToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(129, 26);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // mineToolStripMenuItem
            // 
            this.mineToolStripMenuItem.Name = "mineToolStripMenuItem";
            this.mineToolStripMenuItem.Size = new System.Drawing.Size(129, 26);
            this.mineToolStripMenuItem.Text = "Mine";
            this.mineToolStripMenuItem.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(129, 26);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(129, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // mineQueriesToolStripMenuItem
            // 
            this.mineQueriesToolStripMenuItem.AutoSize = false;
            this.mineQueriesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAvgAll,
            this.tsmiMine1Word,
            this.tsmiMine2Words,
            this.tsmiMine3Words});
            this.mineQueriesToolStripMenuItem.Name = "mineQueriesToolStripMenuItem";
            this.mineQueriesToolStripMenuItem.Size = new System.Drawing.Size(89, 23);
            this.mineQueriesToolStripMenuItem.Text = "Set Mine Type";
            // 
            // tsmiAvgAll
            // 
            this.tsmiAvgAll.Checked = true;
            this.tsmiAvgAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiAvgAll.Name = "tsmiAvgAll";
            this.tsmiAvgAll.Size = new System.Drawing.Size(207, 26);
            this.tsmiAvgAll.Text = "Average All Values";
            this.tsmiAvgAll.Click += new System.EventHandler(this.tsmiAvgAll_Click);
            // 
            // tsmiMine1Word
            // 
            this.tsmiMine1Word.Name = "tsmiMine1Word";
            this.tsmiMine1Word.Size = new System.Drawing.Size(207, 26);
            this.tsmiMine1Word.Tag = "MineType.One";
            this.tsmiMine1Word.Text = "Mine 1 Word";
            this.tsmiMine1Word.Click += new System.EventHandler(this.SetMineType);
            // 
            // tsmiMine2Words
            // 
            this.tsmiMine2Words.Checked = true;
            this.tsmiMine2Words.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiMine2Words.Name = "tsmiMine2Words";
            this.tsmiMine2Words.Size = new System.Drawing.Size(207, 26);
            this.tsmiMine2Words.Tag = "MineType.Two";
            this.tsmiMine2Words.Text = "Mine 2 Words";
            this.tsmiMine2Words.Click += new System.EventHandler(this.SetMineType);
            // 
            // tsmiMine3Words
            // 
            this.tsmiMine3Words.Name = "tsmiMine3Words";
            this.tsmiMine3Words.Size = new System.Drawing.Size(207, 26);
            this.tsmiMine3Words.Tag = "MineType.Three";
            this.tsmiMine3Words.Text = "Mine 3 Words";
            this.tsmiMine3Words.Click += new System.EventHandler(this.SetMineType);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.dgvMineResults);
            this.panel1.Location = new System.Drawing.Point(0, 30);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1549, 874);
            this.panel1.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(931, 911);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1549, 946);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitleOutput);
            this.Controls.Add(this.lblRowCount);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.menuStrip);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QueryWizard";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMineResults)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvMineResults;
        private System.Windows.Forms.SaveFileDialog outFileDialog;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.Label lblTitleOutput;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem mineQueriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiMine1Word;
        private System.Windows.Forms.ToolStripMenuItem tsmiMine2Words;
        private System.Windows.Forms.ToolStripMenuItem tsmiMine3Words;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripMenuItem tsmiAvgAll;
    }
}