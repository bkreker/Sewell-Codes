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
            this.dgvInFile = new System.Windows.Forms.DataGridView();
            this.outFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mineQueriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMine1Word = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMine2Words = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMine3Words = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabOutFile = new System.Windows.Forms.TabControl();
            this.tabPageInFile = new System.Windows.Forms.TabPage();
            this.tabPageOutFile = new System.Windows.Forms.TabPage();
            this.dgvOutFile = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInFile)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabOutFile.SuspendLayout();
            this.tabPageInFile.SuspendLayout();
            this.tabPageOutFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutFile)).BeginInit();
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
            this.progressBar1.Location = new System.Drawing.Point(796, 740);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(354, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // dgvInFile
            // 
            this.dgvInFile.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvInFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInFile.Location = new System.Drawing.Point(3, 3);
            this.dgvInFile.Name = "dgvInFile";
            this.dgvInFile.RowTemplate.DefaultCellStyle.Format = "0.##";
            this.dgvInFile.RowTemplate.DefaultCellStyle.NullValue = null;
            this.dgvInFile.Size = new System.Drawing.Size(1148, 678);
            this.dgvInFile.TabIndex = 1;
            this.dgvInFile.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvResults_RowsAdded);
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
            this.lblRowCount.Location = new System.Drawing.Point(76, 747);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(13, 13);
            this.lblRowCount.TabIndex = 3;
            this.lblRowCount.Text = "0";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 747);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Output Rows:";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mineQueriesToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1162, 27);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // mineToolStripMenuItem
            // 
            this.mineToolStripMenuItem.Name = "mineToolStripMenuItem";
            this.mineToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.mineToolStripMenuItem.Text = "Mine";
            this.mineToolStripMenuItem.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // mineQueriesToolStripMenuItem
            // 
            this.mineQueriesToolStripMenuItem.AutoSize = false;
            this.mineQueriesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMine1Word,
            this.tsmiMine2Words,
            this.tsmiMine3Words});
            this.mineQueriesToolStripMenuItem.Name = "mineQueriesToolStripMenuItem";
            this.mineQueriesToolStripMenuItem.Size = new System.Drawing.Size(89, 23);
            this.mineQueriesToolStripMenuItem.Text = "Set Mine Type";
            // 
            // tsmiMine1Word
            // 
            this.tsmiMine1Word.Name = "tsmiMine1Word";
            this.tsmiMine1Word.Size = new System.Drawing.Size(147, 22);
            this.tsmiMine1Word.Tag = "MineType.One";
            this.tsmiMine1Word.Text = "Mine 1 Word";
            this.tsmiMine1Word.Click += new System.EventHandler(this.SetMineType);
            // 
            // tsmiMine2Words
            // 
            this.tsmiMine2Words.Name = "tsmiMine2Words";
            this.tsmiMine2Words.Size = new System.Drawing.Size(147, 22);
            this.tsmiMine2Words.Tag = "MineType.Two";
            this.tsmiMine2Words.Text = "Mine 2 Words";
            this.tsmiMine2Words.Click += new System.EventHandler(this.SetMineType);
            // 
            // tsmiMine3Words
            // 
            this.tsmiMine3Words.Name = "tsmiMine3Words";
            this.tsmiMine3Words.Size = new System.Drawing.Size(147, 22);
            this.tsmiMine3Words.Tag = "MineType.Three";
            this.tsmiMine3Words.Text = "Mine 3 Words";
            this.tsmiMine3Words.Click += new System.EventHandler(this.SetMineType);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 23);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.tabOutFile);
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1162, 710);
            this.panel1.TabIndex = 9;
            // 
            // tabOutFile
            // 
            this.tabOutFile.Controls.Add(this.tabPageInFile);
            this.tabOutFile.Controls.Add(this.tabPageOutFile);
            this.tabOutFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabOutFile.Location = new System.Drawing.Point(0, 0);
            this.tabOutFile.Name = "tabOutFile";
            this.tabOutFile.SelectedIndex = 0;
            this.tabOutFile.Size = new System.Drawing.Size(1162, 710);
            this.tabOutFile.TabIndex = 2;
            // 
            // tabPageInFile
            // 
            this.tabPageInFile.Controls.Add(this.dgvInFile);
            this.tabPageInFile.Location = new System.Drawing.Point(4, 22);
            this.tabPageInFile.Name = "tabPageInFile";
            this.tabPageInFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInFile.Size = new System.Drawing.Size(1154, 684);
            this.tabPageInFile.TabIndex = 0;
            this.tabPageInFile.Text = "Original Queries";
            this.tabPageInFile.UseVisualStyleBackColor = true;
            // 
            // tabPageOutFile
            // 
            this.tabPageOutFile.Controls.Add(this.dgvOutFile);
            this.tabPageOutFile.Location = new System.Drawing.Point(4, 22);
            this.tabPageOutFile.Name = "tabPageOutFile";
            this.tabPageOutFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOutFile.Size = new System.Drawing.Size(1154, 684);
            this.tabPageOutFile.TabIndex = 1;
            this.tabPageOutFile.Text = "Mine Results";
            this.tabPageOutFile.UseVisualStyleBackColor = true;
            // 
            // dgvOutFile
            // 
            this.dgvOutFile.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvOutFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOutFile.Location = new System.Drawing.Point(3, 3);
            this.dgvOutFile.Name = "dgvOutFile";
            this.dgvOutFile.RowTemplate.DefaultCellStyle.Format = "0.##";
            this.dgvOutFile.RowTemplate.DefaultCellStyle.NullValue = null;
            this.dgvOutFile.Size = new System.Drawing.Size(1148, 678);
            this.dgvOutFile.TabIndex = 2;
            // 
            // AnalyzeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1162, 769);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblRowCount);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.menuStrip);
            this.Name = "AnalyzeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QueryWizard";
            ((System.ComponentModel.ISupportInitialize)(this.dgvInFile)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabOutFile.ResumeLayout(false);
            this.tabPageInFile.ResumeLayout(false);
            this.tabPageOutFile.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutFile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvInFile;
        private System.Windows.Forms.SaveFileDialog outFileDialog;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.TabControl tabOutFile;
        private System.Windows.Forms.TabPage tabPageInFile;
        private System.Windows.Forms.TabPage tabPageOutFile;
        private System.Windows.Forms.DataGridView dgvOutFile;
    }
}