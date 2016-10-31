using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryMining
{
    public partial class AnalyzeForm : Form
    {

        public AnalyzeForm()
        {
            InitializeComponent();
        }

        public AnalyzeForm(StringWriter outPutStringStream, int wordColumn, int queryColumn) : this()
        {
            string k = outPutStringStream.ToString();
            var l = k.Split('\n').ToList();
            var headers = l[0];
            l.RemoveAt(0);
            var data = new Dictionary<string[], List<List<double>>>();
            foreach (string fullRow in l)
            {
                var rowStats = fullRow.Split(',').ToList();
                string[] key = { rowStats[wordColumn], rowStats[queryColumn] };
                rowStats.RemoveAt(queryColumn);
                rowStats.RemoveAt(wordColumn);
                List<double> newList = new List<double>();
                foreach (string item in rowStats)
                {
                    double stat;
                    if (double.TryParse(item, out stat))
                    {
                        newList.Add(stat);

                    }
                }
                try
                {
                    data[key].Add(newList);
                }
                catch (KeyNotFoundException)
                {
                    data[key] = new List<List<double>>();
                    data[key].Add(newList);
                }
            }
        }

        private void Analyze()
        {

        }
    }
}
