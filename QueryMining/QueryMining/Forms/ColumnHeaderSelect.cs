﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryMining.Forms
{
    public enum ColType
    {
        Query, Keyword
    }
    public partial class ColumnHeaderSelect : Form
    {
        private int _index;

        public int SelectedIndex { get { return _index; } }
        public ColumnHeaderSelect()
        {
            InitializeComponent();
        }

        public ColumnHeaderSelect(List<string> headerRow) : this()
        {
            foreach (string item in headerRow)
            {
                lstBxColumnNames.Items.Add(item.Trim());
            }
            lstBxColumnNames.SelectedIndex = 0;
        }

        public ColumnHeaderSelect(DataColumnCollection columns, ColType colType = ColType.Query) : this()
        {
            if (colType == ColType.Query)
            {
                this.Text = "Which Column Contains the Queries?";
            }
            else
            {
                this.Text = "Which Column Contains the Matched Keyword?";

            }
            foreach (DataColumn column in columns)
            {
                try
                {
                    lstBxColumnNames.Items.Add(column.Caption);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            try
            {
                var s = (from DataColumn c in columns
                         where Regexes.IsMatch(c.Caption, Regexes.Query)
                         select c.Caption).ToList();
                if (s.Count > 0)
                {
                    lstBxColumnNames.SelectedIndex = columns.IndexOf(s[0]);
                }

            }
            catch (Exception)
            {
                lstBxColumnNames.SelectedIndex = 0;

            }

        }

        private void lvHeaderSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _index = lstBxColumnNames.SelectedIndex;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            _index = lstBxColumnNames.SelectedIndex;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            throw new OperationCanceledException();
        }
    }

}
