namespace Rolling13Week
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.calCurrent = new System.Windows.Forms.MonthCalendar();
            this.calPrev = new System.Windows.Forms.MonthCalendar();
            this.gbPast = new System.Windows.Forms.GroupBox();
            this.gbCurrent = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbPast.SuspendLayout();
            this.gbCurrent.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbCurrent);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbPast);
            this.splitContainer1.Size = new System.Drawing.Size(261, 403);
            this.splitContainer1.SplitterDistance = 201;
            this.splitContainer1.TabIndex = 1;
            // 
            // calCurrent
            // 
            this.calCurrent.Location = new System.Drawing.Point(16, 25);
            this.calCurrent.MaxSelectionCount = 365;
            this.calCurrent.Name = "calCurrent";
            this.calCurrent.TabIndex = 1;
            // 
            // calPrev
            // 
            this.calPrev.Location = new System.Drawing.Point(16, 23);
            this.calPrev.MaxSelectionCount = 365;
            this.calPrev.Name = "calPrev";
            this.calPrev.TabIndex = 1;
            // 
            // gbPast
            // 
            this.gbPast.Controls.Add(this.calPrev);
            this.gbPast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbPast.Location = new System.Drawing.Point(0, 0);
            this.gbPast.Name = "gbPast";
            this.gbPast.Size = new System.Drawing.Size(261, 198);
            this.gbPast.TabIndex = 2;
            this.gbPast.TabStop = false;
            this.gbPast.Text = "Prev: 14 Wks - 1 Wk";
            // 
            // gbCurrent
            // 
            this.gbCurrent.Controls.Add(this.calCurrent);
            this.gbCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbCurrent.Location = new System.Drawing.Point(0, 0);
            this.gbCurrent.Name = "gbCurrent";
            this.gbCurrent.Size = new System.Drawing.Size(261, 201);
            this.gbCurrent.TabIndex = 2;
            this.gbCurrent.TabStop = false;
            this.gbCurrent.Text = "Current: 13 Wks - Today";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 403);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rolling 13 Weeks";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbPast.ResumeLayout(false);
            this.gbCurrent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbCurrent;
        private System.Windows.Forms.MonthCalendar calCurrent;
        private System.Windows.Forms.GroupBox gbPast;
        private System.Windows.Forms.MonthCalendar calPrev;
    }
}

