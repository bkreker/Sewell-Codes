using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QueryMining
{
    public class QueryWord
    {

        public string Word { get; set; }
        public string Query { get; set; }
        public decimal Cost { get; set; }
        public decimal GP { get; set; }
        public decimal NetProfit { get; set; }
        public double ROI { get; set; }
        public decimal NPPerConv { get; set; }
        public decimal GPPerConv { get; set; }
        public int Conversions { get; set; }
        public int Clicks { get; set; }
        public int Impressions { get; set; }
        public decimal ConvValPerCost { get; set; }
        public double CTR { get; set; }
        public decimal AvgCPC { get; set; }
        public double AvgPosition { get; set; }
        public decimal CostPerConv { get; set; }
        public double ConvRate { get; set; }
        public int ViewThroughConv { get; set; }

        public List<List<Stat>> Stats { get; set; }

        public QueryWord()
        {
            Stats = new List<List<Stat>>();
            Word = "Not Set";
            Query = "Not Set";
        }
        public QueryWord(string word, string query, List<Stat> list)
        {
            this.Stats = new List<List<Stat>>();
            this.Stats.Add(list);
            this.Word = word;
            this.Query = query;
        }       
    }
}
