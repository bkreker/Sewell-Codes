using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoppingFeedValidation.Properties;

namespace ShoppingFeedValidation
{
    public partial class Form1 : Form
    {
        const string FILE_NAME = "Feed File";
        const string G_STRING = "http://base.google.com/ns/1.0";
        const string C_STRING = "http://base.google.com/cns/1.0";


        XName gColor = $"{{{G_STRING}}}color";
        XName cColor = $"{{{C_STRING}}}color";
        XName sku = $"{{{G_STRING}}}id";
        XName title = "title";
        XName item = "item";

        public Form1()
        {
            InitializeComponent();
            LoadXML();
        }
        public void LoadXML()
        {
            try
            {
                var doc = XDocument.Parse(Resources.ShoppingFeed);
                var root = doc.Root;


                var items = doc.Descendants()
                    .Where(x => x.Name == item);

                var i = (from x in items
                         where
                            x.Element(gColor) != null &&
                            x.Element(cColor) != null
                         select x).ToList();

                var inconsistants = (from x in i
                                     where x.Element(cColor).Value != x.Element(gColor).Value
                                     select x);

                using (var writer = new StreamWriter("results.csv"))
                {
                    writer.WriteLine("itemSku,itemTitle,itemCcolor,itemGcolor");
                    foreach (var item in inconsistants)
                    {
                        var itemSku = item.Element(sku).Value;
                        var itemTitle = item.Element(title).Value.Trim('\n', ' ');
                        var itemCcolor = item.Element(cColor).Value;
                        var itemGcolor = item.Element(gColor).Value;

                        Console.WriteLine($"\n{itemSku} - {itemTitle} \n\tcColor: {itemCcolor}, \n\tgColor: {itemGcolor}");
                        writer.WriteLine($"{itemSku},{itemTitle},{itemCcolor},{itemGcolor}");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadXML();
        }
    }
}
