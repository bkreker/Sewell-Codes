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
using System.Web;

namespace Google_Feed_Test_Display
{
    public class Taxonomy : XmlDocument
    {
        public const string XML_IN_FILE = "Google Shopping Taxonomy.xml";
        public List<XmlNode> AllNodes { get; private set; }

        public List<XmlNode> Level1Nodes { get; private set; }
        public List<XmlNode> Level2Nodes { get; private set; }
        public List<XmlNode> Level3Nodes { get; private set; }
        public List<XmlNode> Level4Nodes { get; private set; }
        public List<XmlNode> Level5Nodes { get; private set; }
        public List<XmlNode> Level6Nodes { get; private set; }
        public List<XmlNode> Level7Nodes { get; private set; }

        public Taxonomy() : base()
        {
            base.Load(XML_IN_FILE);

            var allNodes = base.GetElementsByTagName("item", base.NamespaceURI);
            this.AllNodes = (from XmlNode node in allNodes
                             select node).ToList();

            this.Level1Nodes = (from XmlNode node in this.AllNodes
                                where node.ChildNodes.Count == 1
                                select node).ToList();

            this.Level2Nodes = (from XmlNode node in this.AllNodes
                                where node.ChildNodes.Count == 2
                                select node.LastChild).ToList();

            this.Level3Nodes = (from XmlNode node in this.AllNodes
                                where node.ChildNodes.Count == 3
                                select node.LastChild).ToList();

            this.Level4Nodes = (from XmlNode node in this.AllNodes
                                where node.ChildNodes.Count == 4
                                select node.LastChild).ToList();

            this.Level5Nodes = (from XmlNode node in this.AllNodes
                                where node.ChildNodes.Count == 5
                                select node.LastChild).ToList();
            this.Level6Nodes = (from XmlNode node in this.AllNodes
                                where node.ChildNodes.Count == 6
                                select node.LastChild).ToList();

            this.Level7Nodes = (from XmlNode node in this.AllNodes
                                where node.ChildNodes.Count == 7
                                select node.LastChild).ToList();
        }

        const string
            INDEX_TAG = "<level index=\"",
            ID_TAG = "\" id=\"",
            NAME_TAG = "\" name=\"",
            LVL_START_END = "\">",
            LVL_CLOSE = "</level>";

        public void FillTreeView(ref TreeView treeView1)
        {
            treeView1.BeginUpdate();
            string URI = base.NamespaceURI;
            for (int i_1 = 0; i_1 < Level1Nodes.Count; i_1++)
            {
                var level = Level1Nodes[i_1];
                var aff = level.Attributes.GetNamedItem("index", URI).Value;
                string
                    index = level.Attributes.GetNamedItem("index", URI).Value,
                    id = level.Attributes.GetNamedItem("id", URI).Value,
                    name = level.InnerText;

                TreeNode node = new TreeNode(name);
                node.Tag = id;
                treeView1.Nodes.Add(node);

                var lvl2Nodes = (from XmlNode a in Level2Nodes
                                 where a.PreviousSibling.InnerText == level.InnerText
                                 select a).ToList();
                // Level2Nodes.Where(a => a.PreviousSibling.InnerText == level.InnerText).ToList();

                for (int i_2 = 0; i_2 < lvl2Nodes.Count; i_2++)
                {
                    var level2 = lvl2Nodes[i_2];

                    var index2Node = level2.Attributes.GetNamedItem("index", level2.BaseURI);
                    var id2Node = level2.Attributes.GetNamedItem("id", level2.NamespaceURI);
                    string
                        index2 = index2Node.Value,
                        id2 = id2Node.Value,
                        name2 = level2.InnerText;

                    TreeNode node2 = new TreeNode(name2);
                    node2.Tag = id2;
                    treeView1.Nodes[i_1].Nodes.Add(node2);

                    var lvl3Nodes = Level3Nodes.Where(a => a.PreviousSibling.InnerText == level2.InnerText).ToList();
                    for (int i_3 = 0; i_3 < lvl3Nodes.Count; i_3++)
                    {
                        var level3 = lvl3Nodes[i_3];

                        string
                            index3 = level3.Attributes["index", URI].Value,
                            id3 = level3.Attributes["id", URI].Value,
                            name3 = level3.InnerText;

                        TreeNode node3 = new TreeNode(name3);
                        node3.Tag = id3;
                        treeView1.Nodes[i_1].Nodes[i_2].Nodes.Add(node3);

                        var lvl4Nodes = Level4Nodes.Where(a => a.PreviousSibling.InnerText == level3.InnerText).ToList();
                        for (int i_4 = 0; i_4 < lvl4Nodes.Count; i_4++)
                        {
                            var level4 = lvl4Nodes[i_4];
                            string
                                   index4 = level4.Attributes["index", URI].Value,
                                id4 = level4.Attributes["id", URI].Value,
                                name4 = level4.InnerText;

                            TreeNode node4 = new TreeNode(name4);
                            node4.Tag = id4;
                            treeView1.Nodes[i_1].Nodes[i_2].Nodes[i_3].Nodes.Add(node4);

                            var lvl5Nodes = Level5Nodes.Where(a => a.PreviousSibling.InnerText == level4.InnerText).ToList();
                            for (int i_5 = 0; i_5 < lvl5Nodes.Count; i_5++)
                            {
                                var level5 = lvl5Nodes[i_5];
                                string
                                    index5 = level5.Attributes["index", URI].Value,
                                    id5 = level5.Attributes["id", URI].Value,
                                    name5 = level5.InnerText;

                                TreeNode node5 = new TreeNode(name5);
                                node5.Tag = id5;
                                treeView1.Nodes[i_1].Nodes[i_2].Nodes[i_3].Nodes[i_4].Nodes.Add(node5);

                                var lvl6Nodes = Level6Nodes
                                    .Where(a => a.PreviousSibling.InnerText == level5.InnerText).ToList();
                                for (int i_6 = 0; i_6 < lvl6Nodes.Count; i_6++)
                                {
                                    var level6 = lvl6Nodes[i_6];
                                    string
                                        index6 = level6.Attributes["index", URI].Value,
                                        id6 = level6.Attributes["id", URI].Value,
                                        name6 = level6.InnerText;

                                    TreeNode node6 = new TreeNode(name6);
                                    node6.Tag = id6;
                                    treeView1.Nodes[i_1].Nodes[i_2].Nodes[i_3].Nodes[i_4].Nodes[i_5].Nodes.Add(node6);

                                    var lvl7Nodes = Level7Nodes
                                        .Where(a => a.PreviousSibling.InnerText == level6.InnerText).ToList();
                                    for (int i_7 = 0; i_7 < lvl7Nodes.Count; i_7++)
                                    {
                                        var level7 = lvl7Nodes[i_7];
                                        string
                                            index7 = level7.Attributes["index", URI].Value,
                                            id7 = level7.Attributes["id", URI].Value,
                                            name7 = level7.InnerText;

                                        TreeNode node7 = new TreeNode(name7);
                                        node7.Tag = id7;
                                        treeView1.Nodes[i_1].Nodes[i_2].Nodes[i_3].Nodes[i_4].Nodes[i_5].Nodes[i_6].Nodes.Add(node7);

                                    }
                                }

                            }
                        }
                    }
                }
            }
            treeView1.EndUpdate();
        }
        public string Collapse()
        {
            string URI = base.NamespaceURI;
            StringBuilder outDoc = new StringBuilder();
            foreach (var level in Level1Nodes)
            {
                string
                    index = level.Attributes["index", URI].Value,
                    id = level.Attributes["id", URI].Value,
                    name = level.InnerText;

                outDoc
                   .Append(INDEX_TAG).Append(index)
                   .Append(ID_TAG).Append(id)
                   .Append(NAME_TAG).Append(name)
                   .AppendLine(LVL_START_END);

                foreach (var level2 in Level2Nodes.Where(a => a.PreviousSibling.InnerText == level.InnerText))
                {
                    string
                        index2 = level2.FirstChild.Attributes["index", URI] == null ? null : level2.FirstChild.Attributes["index", URI].Value,
                        id2 = level2.Attributes["id", URI].Value == null ? null : level2.Attributes["id", URI].Value,
                        name2 = level2.InnerText;

                    outDoc
                       .Append(INDEX_TAG).Append(index2)
                       .Append(ID_TAG).Append(id2)
                       .Append(NAME_TAG).Append(name2)
                       .AppendLine(LVL_START_END);

                    foreach (var level3 in Level2Nodes.Where(a => a.PreviousSibling.InnerText == level2.InnerText))
                    {
                        string
                            index3 = level3.Attributes["index", URI].Value,
                            id3 = level3.Attributes["id", URI].Value,
                            name3 = level3.InnerText;

                        outDoc
                           .Append(INDEX_TAG).Append(index3)
                           .Append(ID_TAG).Append(id3)
                           .Append(NAME_TAG).Append(name3)
                           .AppendLine(LVL_START_END);

                        foreach (var level4 in Level2Nodes.Where(a => a.PreviousSibling.InnerText == level3.InnerText))
                        {
                            Console.WriteLine(
                                outDoc.Append("<")
                                .Append(level4.FirstChild.LocalName).Append(">")
                                .Append(level4.InnerText));

                            foreach (var level5 in Level2Nodes.Where(a => a.PreviousSibling.InnerText == level4.InnerText))
                            {
                                Console.WriteLine(
                                    outDoc.Append("<")
                                    .Append(level5.FirstChild.LocalName).Append(">")
                                    .Append(level5.InnerText));

                                foreach (var level6 in Level2Nodes.Where(a => a.PreviousSibling.InnerText == level5.InnerText))
                                {

                                    Console.WriteLine(
                                        outDoc.Append("<")
                                        .Append(level6.FirstChild.LocalName).Append(">")
                                        .Append(level6.InnerText));

                                    foreach (var level7 in Level2Nodes.Where(a => a.PreviousSibling.InnerText == level6.InnerText))
                                    {
                                        Console.WriteLine(
                                           outDoc.Append("<")
                                               .Append(level7.LocalName).Append(">")
                                               .Append(level7.Value).Append("</")
                                               .Append(level7.LocalName).Append(">"));

                                    }

                                    Console.WriteLine(outDoc.Append("</").Append(LVL_CLOSE).Append(">"));
                                }
                                Console.WriteLine(outDoc.Append("</").Append(LVL_CLOSE).Append(">"));

                            }
                            Console.WriteLine(outDoc.AppendLine("</").Append(LVL_CLOSE).AppendLine(">"));

                        }
                        Console.WriteLine(outDoc.AppendLine("</").Append(LVL_CLOSE).AppendLine(">"));

                    }
                    Console.WriteLine(outDoc.AppendLine("</").Append(LVL_CLOSE).AppendLine(">"));

                }

                Console.WriteLine(outDoc.AppendLine("</").Append(LVL_CLOSE).AppendLine(">"));
            }
            return outDoc.ToString();
        }
    }
}
