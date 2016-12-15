using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization.Advanced;
using System.Xml.Serialization.Configuration;
using System.Xml.Resolvers;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl;


namespace Google_Feed_Test_Display
{
    public struct Level
    {
        public Level(string name, int id, string ancestor, Stack<string> parents)
        {
            this.Name = name;
            this.Id = id;
            this.Ancestor = ancestor;
            this.Parents = parents;
        }
        public Stack<string> Parents { get; set; }
        public string Name { get; set; }
        public string Ancestor { get; set; }
        public int Id { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
    

    public partial class MainForm : Form
    {
        public TreeView Tree { get { return this.treeView1; } }
        const string TXT_IN_FILE = "Google Shopping Taxonomy.txt";
        const string XML_FILE = "Google Shopping Taxonomy.xml";
        const string ROOT_ELEMENT_NAME = "taxonomy";
        const string LEVEL_NAME = "google_product_category";
        const string ID_NAME = "id";
        const string NAME_NAME = "caption";

        public MainForm()
        {
            InitializeComponent();
            
            
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
                int id = 0;
                string name = "", idString = "";
                while (!reader.EndOfStream)
                {
                    Level level = new Level();
                    var lastNameAndId = new Stack<string>();
                    string line = reader.ReadLine();
                    var levelsStrings = line.Split(delim).ToList();
                    var lineStack = new Stack<string>();
                    foreach (var lvl in levelsStrings)
                    {
                        var lvlName = lvl.Trim();
                        if (level.Parents.Count == 0)
                            level.Ancestor = lvlName;

                        level.Parents.Push(lvlName);
                    }


                    idString = lineStack.Pop();
                    idString.Split(numDelim).ToList().ForEach(a => lastNameAndId.Push(a.Trim()));

                    id = int.Parse(lastNameAndId.Pop());
                    name = string.Join(" ", lastNameAndId);


                    levelStack.Enqueue(level);

                }
            }
            var top = levelStack.Dequeue();
            TreeNode node = new TreeNode();
            List<TreeNode> nodesNotAdded = new List<TreeNode>();
            string p1 = "", p2 = "", p3 = "", p4 = "", p5 = "", p6 = "", p7 = "";
            var parent = treeView1.Nodes;
            node = new TreeNode(top.Name) { Tag = top.Id, Name = top.Name };
            treeView1.Nodes.Add(node);
            while (levelStack.Count > 0)
            {
                node = new TreeNode(top.Name) { Tag = top.Id, Name = top.Name };
                int parentCount = top.Parents.Count;

                AddNodesToTree(ref top, treeView1.Nodes[top.Ancestor], node);
                try
                {

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

        /*
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
                  }*/
        private void AddNodesToTree(ref Level top, TreeNode parent, TreeNode newChild)
        {
            if (top.Parents.Count == 0)
            {
                AddNodesToTree(ref top, parent.Nodes[top.Parents.Pop()], newChild);
            }
            else
            {
                parent.Nodes.Add(newChild);
            }
            //  return newChild;


        }

        private void ReadXml()
        {

            var path = "newOutfile.xml";
            var xdoc = new XDocument();

            using (var inFile = File.OpenText(XML_FILE))
            {
                //    doc.Load(inFile);
                xdoc = XDocument.Load(inFile);
            }
            RootElement = xdoc.Root;
            var xmlns = xdoc.Root.GetDefaultNamespace();

            foreach (XElement x in xdoc.Descendants())
            {
                try
                {
                    var xname = x.Name;
                    if (x.HasAttributes)
                    {
                        var name = x.FirstAttribute.Value;
                        var id = x.LastAttribute.Value;
                        var node = new TreeNode() { Name = name, Text = name, Tag = id };

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        XElement RootElement;
        private void AddNodes(TreeNode node, XElement x)
        {
            if (x.Parent == RootElement)
            {
                treeView1.Nodes.Add(node);
            }
            else
            {

            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            //TreeViewToXml(treeView1, path);
            try
            {
                using (var stream = File.OpenWrite(XML_FILE))
                {
                    var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true, WriteEndDocumentOnClose = true, CheckCharacters = true });
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement(ROOT_ELEMENT_NAME);
                    SaveNodes(treeView1.Nodes, ref xmlWriter);
                    //xmlWriter.WriteEndElement();
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

                    xmlWriter.WriteStartElement(LEVEL_NAME);

                    xmlWriter.WriteAttributeString(NAME_NAME, value);
                    xmlWriter.WriteAttributeString(ID_NAME, node.Tag.ToString());
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

        private void btnLoadText_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();
            ReadText();
        }

        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();
            ReadXml();
        }
    }
}
