using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
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
    public partial class MainForm : Form
    {
        public TreeView Tree { get { return this.treeView1; } }
        const string TXT_IN_FILE = "New taxonomy inFile.txt";
        //New - "New taxonomy inFile.txt"
        //Old - "Google Shopping Taxonomy.txt"
        const string XML_FILE = "Google Shopping Taxonomy.xml";
        const string ROOT_ELEMENT_NAME = "taxonomy";
        const string LEVEL_NAME = "google_product_category";
        const string ID_NAME = "id";
        const string NAME_NAME = "caption";

        XmlDocument xmlDocument = new XmlDocument();
        XElement RootElement;

        public MainForm()
        {
            InitializeComponent();
            ReadText();

        }

        TaxonomyList<Level> levelList = new TaxonomyList<Level>();
        private void ReadText()
        {
            Console.WriteLine("Hello");
            char
                delimChar = '>',
                numDelimChar = '~';

            char[]
                delim = { delimChar },
                numDelim = { numDelimChar };

            using (var reader = File.OpenText(TXT_IN_FILE))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line[0] == '#') continue;

                    var fullNameAndId = line.Split(numDelim).ToList();
                    var idString = fullNameAndId.Last();

                    fullNameAndId.Remove(idString);

                    var fullName = string.Join(delimChar.ToString(), fullNameAndId);
                    var fullNameList = fullName.Split(delim).ToList();

                    var lastName = fullNameList.Last();
                    var firstName = fullNameList.First();

                    string ancestor = lastName == firstName ? "" : firstName;

                    fullNameList.Remove(lastName);
                    Level level = new Level()
                    {
                        FullName = fullName.Replace(">", " > "),
                        Id = int.Parse(idString),
                        Name = lastName,
                        Ancestor = ancestor,
                        Parents = new TaxonomyList<string>(fullNameList)
                    };


                    levelList.Enqueue(level);
                }

            }
            AddNodesToTree();
        }

        private void AddNodesToTree()
        {
            TreeNode newChild = new TreeNode();
            var nodesNotAdded = new List<TreeNode>();
            while (levelList.Count > 0)
            {
                try
                {
                    var newLevel = levelList.Dequeue();
                    newChild = new TreeNode()
                    {
                        Text = newLevel.Name,
                        Name = newLevel.Name,
                        Tag = newLevel.Id,
                        ToolTipText = newLevel.ToolTipText
                    };

                    if (newLevel.Parents.Count == 0)
                    {
                        treeView1.Nodes.Add(newChild);
                    }
                    else
                    {
                        var parent = treeView1.Nodes[newLevel.Parents.Dequeue()];
                        RecursiveTreeFill(newLevel, parent, ref newChild);
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating TreeView: {ex.Message}, node not added.");

                }
            }

            //  return newChild;
        }
        private void RecursiveTreeFill(Level newLevel, TreeNode parent, ref TreeNode newChild)
        {
            if (newLevel.Parents.Count == 0)
            {
                parent.Nodes.Add(newChild);
            }
            else if (parent.Nodes.Count > 0)
            {
                var nextParent = parent.Nodes[newLevel.Parents.Dequeue()];
                RecursiveTreeFill(newLevel, nextParent, ref newChild);
            }

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
                        var node = new TreeNode()
                        {
                            Name = name,
                            Text = name,
                            Tag = id
                        };
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var query = txtBoxQuery.Text;
            treeView1.CollapseAll();
            if (query == "")
            {
                return;
            }
            else
            {
                var results = searchNodes(query);
                if (results.Count == 0)
                {
                    MessageBox.Show("Query not found.");
                }
                treeView1.SelectedNodes = results;
            }
        }

        private List<TreeNode> searchNodes(string query)
        {
            var results = new List<TreeNode>();
            foreach (TreeNode item in treeView1.Nodes)
            {
                if (item.Text.ToUpper().Contains(query.ToUpper()))
                {
                    results.Add(item);
                }
                searchNodes(ref results, query, item);
            }
            return results;
        }

        private List<TreeNode> searchNodes(ref List<TreeNode> results, string query, TreeNode parent)
        {
            foreach (TreeNode item in parent.Nodes)
            {
                string target = $@".*{item.Text}.*";

                if (item.Text.ToUpper().Contains(query.ToUpper()))
                {
                    results.Add(item);
                }
                searchNodes(ref results, query, item);
            }
            return results;
        }

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            this.treeView1.CollapseAll();
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            this.treeView1.ExpandAll();
        }

        private void treeView1_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            var node = e.Node; try
            {

                nodeToolTip.Show(node.ToolTipText, sender as Control);

            }
            catch (Exception)
            {
                
            }
        }
    }

    public class Level
    {
        public TaxonomyList<string> Parents { get; set; }
        public string Name { get; set; }
        public string Ancestor { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ToolTipText
        {
            get
            {
                return $"Full Name: {this.FullName}\nId: {this.Id}";
            }
        }

        public Level()
        {
            this.Name = "";
            this.Id = -1;
            this.Ancestor = "";
            this.Parents = new TaxonomyList<string>();
            this.FullName = "";

        }
        public Level(string name, int id, string ancestor, TaxonomyList<string> parents, string fullName)
        {
            this.Name = name;
            this.Id = id;
            this.Ancestor = ancestor;
            this.Parents = parents;
            this.FullName = fullName;
        }


        public override string ToString()
        {
            return this.Name;
        }
    }


    public class TaxonomyList<T> : LinkedList<T>
    {
        public TaxonomyList() : base()
        {
        }
        public TaxonomyList(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                this.Push(item);
            }

        }
        /// <summary>
        /// Adds the new object to the End of the list
        /// </summary>
        /// <param name="newVal">The object to push onto the Stack. The value can be null for reference types.</param>            
        public void Push(T newVal)
        {
            base.AddLast(newVal);
        }

        /// <summary>
        /// Adds the new object to the End of the list
        /// </summary>
        /// <param name="newVal">
        /// The object to add to the Queue. The value can be null for reference types
        /// </param>            
        public void Enqueue(T newVal)
        {
            base.AddLast(newVal);
        }

        /// <summary>
        /// Removes and returns the Last Object in the list
        /// </summary>
        /// <returns>The object removed from the top of the Stack</returns>
        /// <exception cref="InvalidOperationException">The Stack is empty.</exception>
        public T Pop()
        {
            var last = base.Last.Value;
            base.RemoveLast();
            return last;
        }


        /// <summary>
        /// Removes and returns the First object in the list
        /// </summary>
        /// <returns>
        /// The object that is removed from the beginning of the Queue
        /// </returns>
        /// <exception cref="InvalidOperationException">The Queue is empty.</exception>
        public T Dequeue()
        {
            var first = base.First.Value;
            base.RemoveFirst();
            return first;
        }

    }

}
