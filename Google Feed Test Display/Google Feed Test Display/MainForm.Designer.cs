namespace GoogleTaxonomyViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnLoadText = new System.Windows.Forms.Button();
            this.txtBoxQuery = new System.Windows.Forms.TextBox();
            this.btnCollapseAll = new System.Windows.Forms.Button();
            this.btnExpandAll = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.nodeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.treeView1 = new GoogleTaxonomyViewer.JDTreeView();
            this.SuspendLayout();
            // 
            // btnLoadText
            // 
            this.btnLoadText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoadText.Location = new System.Drawing.Point(11, 711);
            this.btnLoadText.Name = "btnLoadText";
            this.btnLoadText.Size = new System.Drawing.Size(121, 20);
            this.btnLoadText.TabIndex = 3;
            this.btnLoadText.Text = "Load From &Text";
            this.btnLoadText.UseVisualStyleBackColor = true;
            this.btnLoadText.Visible = false;
            this.btnLoadText.Click += new System.EventHandler(this.btnLoadText_Click);
            // 
            // txtBoxQuery
            // 
            this.txtBoxQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxQuery.Location = new System.Drawing.Point(11, 684);
            this.txtBoxQuery.Name = "txtBoxQuery";
            this.txtBoxQuery.Size = new System.Drawing.Size(262, 20);
            this.txtBoxQuery.TabIndex = 4;
            // 
            // btnCollapseAll
            // 
            this.btnCollapseAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseAll.Location = new System.Drawing.Point(154, 711);
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Size = new System.Drawing.Size(121, 20);
            this.btnCollapseAll.TabIndex = 5;
            this.btnCollapseAll.Text = "&Collapse All";
            this.btnCollapseAll.UseVisualStyleBackColor = true;
            this.btnCollapseAll.Click += new System.EventHandler(this.btnCollapseAll_Click);
            // 
            // btnExpandAll
            // 
            this.btnExpandAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExpandAll.Location = new System.Drawing.Point(291, 711);
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Size = new System.Drawing.Size(121, 20);
            this.btnExpandAll.TabIndex = 6;
            this.btnExpandAll.Text = "&Expand All";
            this.btnExpandAll.UseVisualStyleBackColor = true;
            this.btnExpandAll.Click += new System.EventHandler(this.btnExpandAll_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(290, 684);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(121, 20);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // nodeToolTip
            // 
            this.nodeToolTip.AutoPopDelay = 5000;
            this.nodeToolTip.InitialDelay = 500;
            this.nodeToolTip.ReshowDelay = 0;
            this.nodeToolTip.ShowAlways = true;
            this.nodeToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.nodeToolTip.ToolTipTitle = "Level Info:";
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(11, 13);
            this.treeView1.Margin = new System.Windows.Forms.Padding(2);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeView1.SelectedNodes")));
            this.treeView1.Size = new System.Drawing.Size(401, 666);
            this.treeView1.TabIndex = 1;
            this.treeView1.NodeMouseHover += new System.Windows.Forms.TreeNodeMouseHoverEventHandler(this.treeView1_NodeMouseHover);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 743);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnExpandAll);
            this.Controls.Add(this.btnCollapseAll);
            this.Controls.Add(this.txtBoxQuery);
            this.Controls.Add(this.btnLoadText);
            this.Controls.Add(this.treeView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(439, 781);
            this.Name = "MainForm";
            this.Text = "Google Product Taxonomy";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JDTreeView treeView1;
        private System.Windows.Forms.Button btnLoadText;
        private System.Windows.Forms.TextBox txtBoxQuery;
        private System.Windows.Forms.Button btnCollapseAll;
        private System.Windows.Forms.Button btnExpandAll;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ToolTip nodeToolTip;
    }
}

