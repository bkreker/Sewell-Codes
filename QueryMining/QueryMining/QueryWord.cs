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
        private List<List<double>> _stats;

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

        public List<List<double>> Stats
        {
            get { return _stats; }
            set { _stats = value; }
        }

        public QueryWord()
        {
            Stats = new List<List<double>>();
            Word = "Not Set";
            Query = "Not Set";
        }
        public QueryWord(string word, string query, List<double> list)
        {
            this.Stats = new List<List<double>>();
            this.Stats.Add(list);
            this.Word = word;
            this.Query = query;
        }

        static public QueryWord operator +(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();

            result.Word = left.Word + " " + left.Word;
            result.Query = left.Query + " / " + left.Query;
            result.Cost = left.Cost + right.Cost;
            result.GP = left.GP + right.GP;
            result.NetProfit = left.NetProfit + right.NetProfit;
            result.ROI = left.ROI + right.ROI;
            result.NPPerConv = left.NPPerConv + right.NPPerConv;
            result.GPPerConv = left.GPPerConv + right.GPPerConv;
            result.Conversions = left.Conversions + right.Conversions;
            result.Clicks = left.Clicks + right.Clicks;
            result.Impressions = left.Impressions + right.Impressions;
            result.ConvValPerCost = left.ConvValPerCost + right.ConvValPerCost;
            result.CTR = left.CTR + right.CTR;
            result.AvgCPC = left.AvgCPC + right.AvgCPC;
            result.AvgPosition = left.AvgPosition + right.AvgPosition;
            result.CostPerConv = left.CostPerConv + right.CostPerConv;
            result.ConvRate = left.ConvRate + right.ConvRate;
            result.ViewThroughConv = left.ViewThroughConv + right.ViewThroughConv;

            return result;
        }

        static public QueryWord operator *(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();
            result.Cost = left.Cost * right.Cost;
            result.GP = left.GP * right.GP;
            result.NetProfit = left.NetProfit * right.NetProfit;
            result.ROI = left.ROI * right.ROI;
            result.NPPerConv = left.NPPerConv * right.NPPerConv;
            result.GPPerConv = left.GPPerConv * right.GPPerConv;
            result.Conversions = left.Conversions * right.Conversions;
            result.Clicks = left.Clicks * right.Clicks;
            result.Impressions = left.Impressions * right.Impressions;
            result.ConvValPerCost = left.ConvValPerCost * right.ConvValPerCost;
            result.CTR = left.CTR * right.CTR;
            result.AvgCPC = left.AvgCPC * right.AvgCPC;
            result.AvgPosition = left.AvgPosition * right.AvgPosition;
            result.CostPerConv = left.CostPerConv * right.CostPerConv;
            result.ConvRate = left.ConvRate * right.ConvRate;
            result.ViewThroughConv = left.ViewThroughConv * right.ViewThroughConv;

            return result;
        }


        static public QueryWord operator /(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();
            result.Cost = left.Cost / right.Cost;
            result.GP = left.GP / right.GP;
            result.NetProfit = left.NetProfit / right.NetProfit;
            result.ROI = left.ROI / right.ROI;
            result.NPPerConv = left.NPPerConv / right.NPPerConv;
            result.GPPerConv = left.GPPerConv / right.GPPerConv;
            result.Conversions = left.Conversions / right.Conversions;
            result.Clicks = left.Clicks / right.Clicks;
            result.Impressions = left.Impressions / right.Impressions;
            result.ConvValPerCost = left.ConvValPerCost / right.ConvValPerCost;
            result.CTR = left.CTR / right.CTR;
            result.AvgCPC = left.AvgCPC / right.AvgCPC;
            result.AvgPosition = left.AvgPosition / right.AvgPosition;
            result.CostPerConv = left.CostPerConv / right.CostPerConv;
            result.ConvRate = left.ConvRate / right.ConvRate;
            result.ViewThroughConv = left.ViewThroughConv / right.ViewThroughConv;

            return result;
        }

        static public QueryWord operator -(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();
            result.Cost           /**/ = left.Cost              /**/   - right.Cost;
            result.GP             /**/ = left.GP                /**/   - right.GP;
            result.NetProfit      /**/ = left.NetProfit         /**/   - right.NetProfit;
            result.ROI            /**/ = left.ROI               /**/   - right.ROI;
            result.NPPerConv      /**/ = left.NPPerConv         /**/   - right.NPPerConv;
            result.GPPerConv      /**/ = left.GPPerConv         /**/   - right.GPPerConv;
            result.Conversions    /**/ = left.Conversions       /**/   - right.Conversions;
            result.Clicks         /**/ = left.Clicks            /**/   - right.Clicks;
            result.Impressions    /**/ = left.Impressions       /**/   - right.Impressions;
            result.ConvValPerCost /**/ = left.ConvValPerCost    /**/   - right.ConvValPerCost;
            result.CTR            /**/ = left.CTR               /**/   - right.CTR;
            result.AvgCPC         /**/ = left.AvgCPC            /**/   - right.AvgCPC;
            result.AvgPosition    /**/ = left.AvgPosition       /**/   - right.AvgPosition;
            result.CostPerConv    /**/ = left.CostPerConv       /**/   - right.CostPerConv;
            result.ConvRate       /**/ = left.ConvRate          /**/   - right.ConvRate;
            result.ViewThroughConv/**/ = left.ViewThroughConv   /**/   - right.ViewThroughConv;

            return result;
        }
    }
}
