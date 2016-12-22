using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using System.Drawing;
using GoogleTaxonomyViewer.Properties;

namespace GoogleTaxonomyViewer
{
    public partial class MainForm : Form
    {
        const string TXT_IN_FILE = "Google Shopping Taxonomy.txt";
        const string LEVEL_NAME = "google_product_category";
        const string ID_NAME = "id";
        const string NAME_NAME = "caption";

        public MainForm()
        {
            InitializeComponent();
            ReadText();
        }

        TaxonomyList<Level> levelList = new TaxonomyList<Level>();

        private void ReadText()
        {
            try
            {
                char
                    delimChar = '>',
                    numDelimChar = '~';

                char[]
                    delim = { delimChar },
                    numDelim = { numDelimChar };
                using (var reader = new StringReader(Resources.Taxonomy))
                {
                    while (reader.Peek() != -1)
                    {
                        string line = reader.ReadLine();
                        if (line[0] == '#') continue;

                        var fullNameAndId = line.Split(numDelim).ToList();
                        var idString = fullNameAndId.First();

                        fullNameAndId.Remove(idString);

                        var fullName = string.Join(delimChar.ToString(), fullNameAndId);

                        var fullNameList = new TaxonomyList<string>(fullName.Split(delim));

                        var lastName = fullNameList.Pop_Last();

                        string ancestor = fullNameList.Count == 0 ? "" : fullNameList.First();

                        fullNameList.Remove(lastName);
                        Level level = new Level()
                        {
                            FullName = fullName.Replace(">", " > "),
                            Id = int.Parse(idString),
                            Name = lastName,
                            Ancestor = ancestor,
                            Parents = new TaxonomyList<string>(fullNameList)
                        };

                        levelList.Push(level);

                    }

                }
                AddNodesToTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Taxonomy from file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AddNodesToTree()
        {
            TreeNode newChild = new TreeNode();
            var nodesNotAdded = new List<TreeNode>();
            while (levelList.Count > 0)
            {
                try
                {
                    var newLevel = levelList.Pop_First();
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
                        var parent = treeView1.Nodes[newLevel.Parents.Pop_First()];
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
                var nextParent = parent.Nodes[newLevel.Parents.Pop_First()];
                RecursiveTreeFill(newLevel, nextParent, ref newChild);
            }

        }


        private void MainForm_Load(object sender, EventArgs e)
        {
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

        private void btnLoadText_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();
            ReadText();
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
            foreach (TreeNode child in treeView1.Nodes)
            {
                if (child.Text.ToUpper().Contains(query.ToUpper()))
                {
                    results.Add(child);
                }
                searchNodes(ref results, query, child);
            }
            return results;
        }

        private List<TreeNode> searchNodes(ref List<TreeNode> results, string query, TreeNode parent)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                if (child.Text.ToUpper().Contains(query.ToUpper()))
                {
                    results.Add(child);
                }
                searchNodes(ref results, query, child);
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
                var treeView = sender as JDTreeView;

                nodeToolTip.Show(node.ToolTipText, treeView);
            }
            catch (Exception)
            {

            }
        }
    }


}
