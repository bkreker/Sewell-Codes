using System;
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
    public partial class MineTypeSelectorForm : Form
    {
        public MineTypeSelectorForm()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.None;
            Program.AvgAll = true;
            cBoxMineType.Items.Add(MineType.One);
            cBoxMineType.Items.Add(MineType.Two);
            cBoxMineType.Items.Add(MineType.Three);
            cBoxMineType.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Program.MineType = (MineType)cBoxMineType.SelectedItem;
            Program.AvgAll = chkBxAvgAll.Checked;
            this.Close();
        }

    }
}
