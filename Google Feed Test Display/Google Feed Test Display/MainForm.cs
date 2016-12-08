using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization.Advanced;
using System.Xml.Serialization.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Text.RegularExpressions;
using System.Web;


namespace Google_Feed_Test_Display
{
    public partial class MainForm : Form
    {
        public TreeView Tree { get { return this.treeView1; } }
        public MainForm()
        {
            InitializeComponent();

            Console.WriteLine("Hello");
            XmlDocument doc = new XmlDocument();
            doc.Load(Taxonomy.XML_IN_FILE);
            var docText = doc.OuterXml;
            string nameMatch = @"\s*<(name)>(.+)</name>\s*";
            string allItemMatches = $@"<item id=""(\d*)"" index=""(\d*)"">({nameMatch})+\s*</item>";
            var items = Regex.Matches(docText, allItemMatches);

            foreach (Match item in items)
            {
                var index = item.Groups[1];
                var id = item.Groups[2];
                var itemNames = Regex.Matches(item.ToString(), nameMatch);

                foreach (Match name in itemNames)
                {
                    var nameTag = name.Groups[0];
                    var nameVal = name.Groups[1];
                    var s = name.Groups;
                    Console.WriteLine();
                }
            }

         //   Taxonomy tax = new Taxonomy();
            //   var outpur = tax.Collapse();
            //this.Tree.Nodes.Add("Top");
     
          //  tax.FillTreeView(ref this.treeView1);
            //this.treeView1.Nodes.Add(tax.AllNodes)
        }

 

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
            //var urlstring = "Google Shopping Taxonomy.xml";
            //XmlDocument doc = new XmlDocument();
            //doc.Load(urlstring);
            //StringBuilder outDoc = new StringBuilder();

            //outDoc.Append(doc.FirstChild.InnerXml);
            //var s = doc.DocumentElement;
            //string allItems = "//item";
            //var allItemNodes = doc.SelectNodes(allItems);


            //Console.Write(s);
            //var allNodes = doc.GetElementsByTagName("item", doc.NamespaceURI);

            //List<XmlNode> nodesList = new List<XmlNode>();
            //foreach (XmlNode item in allNodes)
            //{
            //    nodesList.Add(item);
            //}

            //var baseNodes = (from XmlNode node in allNodes
            //                 where node.ChildNodes.Count == 1
            //                 select node).ToList();

            //var topNames = (from XmlNode topNode in baseNodes
            //                select (from XmlNode a in baseNodes
            //                        where a.FirstChild.InnerText == topNode.FirstChild.InnerText
            //                        select a).First()).Distinct().ToList();

            //foreach (XmlNode topLevel in topNames)
            //{
            //    try
            //    {


            //        outDoc.AppendLine("<").Append(topLevel.LocalName).Append(">").Append(topLevel.Value);

            //        var att = topLevel.Attributes;
            //        string levelname = att["level", doc.NamespaceURI].Value;
            //        var matches = (from XmlAttribute a in att
            //                       where a.LocalName == "level" && a.Value == levelname
            //                       select a.OwnerElement).ToList();

            //        foreach (var match in matches)
            //        {
            //            outDoc.AppendLine(match.OuterXml);
            //            Console.WriteLine(match.OuterXml);
            //            nodesList.Add(match);
            //        }
            //        //var xpath = $@"//item[@level='{levelname}'";
            //        //var results = doc.SelectNodes(xpath);
            //        //nodesList.Add(item);
            //        outDoc.AppendLine("</").Append(topLevel.LocalName).AppendLine(">");
            //    }
            //    catch (XPathException ex)
            //    {
            //        var msg = ex.Message;

            //   }
            //}


            //  var levelOnes = nodesList.Where(x => x.a)



            //foreach (var item in topNames)
            //{
            //    var children = (from XmlNode node in baseNodes
            //                    where node.FirstChild.InnerText == item.InnerText
            //                    select node).ToList();

            //    var innerList = (from XmlNode child in children
            //                     select child.InnerXml).ToList();

            //    outDoc.Append(s);
            //}
            //List<XmlNode> items = GetNodes(doc);

            //foreach (XmlNode item in items)
            //{
            //    var attributes = item.Attributes;
            //    var first = item.FirstChild;
            //    var last = item.LastChild;

            //}
            ////var list = (from node in nodes
            ////            select node.ChildNodes).ToList();

            ////list.Sort((a, b) =>
            ////{
            ////    return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            ////});
            ////  Stack<string> stack = list;


            //if (doc.HasChildNodes)
            //{

            //}

     //   }

        private List<XmlNode> GetNodes(XmlDocument doc)
        {
            var allNodes = doc.GetElementsByTagName("item", doc.NamespaceURI);
            var baseNodes = (from XmlNode node in allNodes
                             where node.ChildNodes.Count == 1
                             select node).ToList();

            //var baseNames = (from XmlNode topNode in baseNodes
            //                 select (from XmlNode a in baseNodes
            //                         where a.FirstChild.InnerText == topNode.FirstChild.InnerText
            //                         select a.InnerText).First()).ToList();
            return baseNodes;

        }

        private List<XmlNode> nodesHelper(XmlNode node, ref List<XmlNode> nodeList)
        {

            nodeList.Add(node);

            foreach (XmlNode item in node.ChildNodes)
            {

                nodeList.Add(item);
            }

            return nodesHelper(node.NextSibling, ref nodeList);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Tree.Nodes.Clear();
            Random rand = new Random(0);
            for (int i = 0; i < 10; i++)
            {
                this.Tree.Nodes.Add("Top");
                for (int j = 0; j < rand.Next(10); j++)
                {
                    this.Tree.Nodes[i].Nodes.Add("j");
                    for (int k = 0; k < rand.Next(6); k++)
                    {
                        this.Tree.Nodes[i].Nodes[j].Nodes.Add("k");

                    }

                }

            }
        }
    }
}
