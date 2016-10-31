using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryMining
{
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
