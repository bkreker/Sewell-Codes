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
    public struct Level
    {
        public Level(string name, int id, Stack<string> parents)
        {
            this.Name = name;
            this.Id = id;
            this.Parents = parents;
        }

        public Stack<string> Parents { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }

    public partial class MainForm : Form
    {
        public TreeView Tree { get { return this.treeView1; } }
        public const string TXT_IN_FILE = "Google Shopping Taxonomy.txt";

        public MainForm()
        {
            InitializeComponent();

            ReadText();

            //   Taxonomy tax = new Taxonomy();
            //   var outpur = tax.Collapse();
            //this.Tree.Nodes.Add("Top");

            //  tax.FillTreeView(ref this.treeView1);
            //this.treeView1.Nodes.Add(tax.AllNodes)
        }

        private void ReadText()
        {
            Console.WriteLine("Hello");
            string taxonomy = "";
            Queue<Level> levelStack = new Queue<Level>();
            char[] delim = { '>' },
                numDelim = { '~' };

            using (var reader = File.OpenText(TXT_IN_FILE))
            {
                int id;
                string name, idString;
                while (!reader.EndOfStream)
                {
                    Stack<string> lastNameAndId = new Stack<string>();
                    string line = reader.ReadLine();
                    var levelsStrings = line.Split(delim).ToList();
                    var lineStack = new Stack<string>();
                    levelsStrings.ForEach(lvl => lineStack.Push(lvl.Trim()));

                    idString = lineStack.Pop();
                    idString.Split(numDelim).ToList().ForEach(a => lastNameAndId.Push(a.Trim()));

                    id = int.Parse(lastNameAndId.Pop());
                    name = string.Join(" ", lastNameAndId);

                    Level level = new Level(name, id, lineStack);

                    levelStack.Enqueue(level);

                }
            }
            var top = levelStack.Dequeue();
            TreeNode node = new TreeNode();
            List<TreeNode> nodesNotAdded = new List<TreeNode>();
            string p1 = "", p2 = "", p3 = "", p4 = "", p5 = "", p6 = "", p7 = "";
            while (levelStack.Count > 0)
            {
                node = new TreeNode(top.Name) { Tag = top.Id, Name = top.Name };
                int parentCount = top.Parents.Count;
                try
                {
                    switch (parentCount)
                    {
                        case 0:
                            treeView1.Nodes.Add(node);
                            break;
                        case 1:
                            p1 = top.Parents.Pop();
                            treeView1.Nodes[p1].Nodes.Add(node);
                            break;
                        case 2:
                            p1 = top.Parents.Pop();
                            p2 = top.Parents.Pop();
                            treeView1.Nodes[p2].Nodes[p1].Nodes.Add(node);
                            break;
                        case 3:
                            p1 = top.Parents.Pop();
                            p2 = top.Parents.Pop();
                            p3 = top.Parents.Pop();
                            treeView1.Nodes[p3].Nodes[p2].Nodes[p1].Nodes.Add(node);
                            break;
                        case 4:
                            p1 = top.Parents.Pop();
                            p2 = top.Parents.Pop();
                            p3 = top.Parents.Pop();
                            p4 = top.Parents.Pop();
                            treeView1.Nodes[p4].Nodes[p3].Nodes[p2].Nodes[p1].Nodes.Add(node);
                            break;
                        case 5:
                            p1 = top.Parents.Pop();
                            p2 = top.Parents.Pop();
                            p3 = top.Parents.Pop();
                            p4 = top.Parents.Pop();
                            p5 = top.Parents.Pop();
                            treeView1.Nodes[p5].Nodes[p4].Nodes[p3].Nodes[p2].Nodes[p1].Nodes.Add(node);
                            break;
                        case 6:
                            p1 = top.Parents.Pop();
                            p2 = top.Parents.Pop();
                            p3 = top.Parents.Pop();
                            p4 = top.Parents.Pop();
                            p5 = top.Parents.Pop();
                            p6 = top.Parents.Pop();
                            treeView1.Nodes[p6].Nodes[p5].Nodes[p4].Nodes[p3].Nodes[p2].Nodes[p1].Nodes.Add(node);
                            break;
                        case 7:
                            p1 = top.Parents.Pop();
                            p2 = top.Parents.Pop();
                            p3 = top.Parents.Pop();
                            p4 = top.Parents.Pop();
                            p5 = top.Parents.Pop();
                            p6 = top.Parents.Pop();
                            p7 = top.Parents.Pop();
                            treeView1.Nodes[p7].Nodes[p6].Nodes[p5].Nodes[p4].Nodes[p3].Nodes[p2].Nodes[p1].Nodes.Add(node);
                            break;
                        default:
                            break;
                    }
                    top = levelStack.Dequeue();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating TreeView: {ex.Message}, node {top.Name} not added. Initial ParentCount: {parentCount}");
                    //        node.Tag = $"{node.Tag} {p7} {p6} {p5} {p4} {p3} {p2} {p1}";


                    //   nodesNotAdded.Add(node);
                }
            }

        }

        private void LoadXml()
        {
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

        XmlDocument xmlDocument = new XmlDocument();
        private void button1_Click(object sender, EventArgs e)
        {
            var path = "newOutfile.xml";
            //TreeViewToXml(treeView1, path);
            try
            {
                using (var stream = File.OpenWrite(path))
                {
                    var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true, WriteEndDocumentOnClose = true, CheckCharacters = true });
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("taxonomy");
                    SaveNodes(treeView1.Nodes, ref xmlWriter);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Close();

                }
                Console.WriteLine("Finished saving document.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving document: {ex.Message}");
            }
        }
        private void SaveNodes(TreeNodeCollection nodesCollection, ref XmlWriter xmlWriter)
        {
            //foreach (var node in nodesCollection.OfType<TreeNode>().Where(x => x.Nodes.Count == 0))
            //{
            //    xmlWriter.WriteAttributeString("id", node.Tag.ToString());
            //}
            foreach (TreeNode node in nodesCollection/*.OfType<TreeNode>().Where(x => x.Nodes.Count > 0)*/)
            {
                try
                {
                    string value = node.Name;
                    if(value.Contains("&amp;"))
                    {

                    }
                    if (value.Contains("&") && !value.Contains("&amp;"))
                    {
                   //     value = value.Replace(" & ", " &amp; ");

                    }
                    xmlWriter.WriteStartElement("level");

                    xmlWriter.WriteAttributeString("id", node.Tag.ToString());
                    xmlWriter.WriteAttributeString("name", value);
                    //  xmlWriter.WriteValue(value);
                    //xmlWriter.WriteEndElement();
                    if (node.Nodes.Count > 0)
                    {
                        SaveNodes(node.Nodes, ref xmlWriter);
                    }
                    xmlWriter.WriteEndElement();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void TreeViewToXml(TreeView treeView1, string path)
        {
            xmlDocument = new XmlDocument();
            TreeNodeCollection nodes = null;
            foreach (TreeNode treeNode in treeView1.Nodes)
            {
                xmlDocument.AppendChild(xmlDocument.CreateElement(treeNode.Text));
                nodes = treeNode.Nodes;
            }
            XmlExport(xmlDocument.DocumentElement, nodes);
            xmlDocument.Save(path);
        }
        private XmlNode XmlExport(XmlNode nodeElement, TreeNodeCollection treeNodeCollection)
        {
            XmlNode xmlNode = null;
            foreach (TreeNode treeNode in treeNodeCollection)
            {
                xmlNode = xmlDocument.CreateElement(treeNode.Text);
                string[] node = xmlNode.Name.Split(':');
                if (node[0] == "ATTRIBUTE")
                {
                    if (node[0] != null && node[1] != null)
                    {
                        XmlAttribute newAttribute = xmlDocument.CreateAttribute(node[1]);
                        nodeElement.Attributes.Append(newAttribute);
                    }
                }
                else
                {
                    if (nodeElement != null) nodeElement.AppendChild(xmlNode);
                }
                if (treeNode.Nodes.Count > 0)
                {
                    XmlExport(xmlNode, treeNode.Nodes);
                }
            }
            return xmlNode;
        }


    }
}
