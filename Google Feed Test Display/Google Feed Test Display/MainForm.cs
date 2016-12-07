using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Web;


namespace Google_Feed_Test_Display
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var urlstring = "Google Shopping Taxonomy.xml":

            var reader = new XmlTextReader(urlstring);
            var str = new StringBuilder();
            while (reader.Read())
            {
                var name = reader.LocalName;
                var val = reader.Value;
                str.Append(name).Append(": ").Append(val).Append("\n");

            }
            Console.Write(str.ToString());
        }
    }
}
