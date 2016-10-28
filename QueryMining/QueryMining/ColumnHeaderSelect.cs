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
        public ColumnHeaderSelect()
        {
            InitializeComponent();
        }

        public ColumnHeaderSelect(List<string> headerRow) :this()
        {
            foreach (var item in headerRow)
            {
                lstBxColumnNames.Items.Add(item.Trim());

            }
        }

        private void lvHeaderSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _index = lstBxColumnNames.SelectedIndex;
        }
        public int SelectedIndex { get { return _index; } }
    }
}
