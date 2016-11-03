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
    public enum StatType
    {
        Word, Query, Cost, NetProfit, GP, GPPerConv, ROI, NPPerConv, Conversions, Clicks, Impressions, ConvValPerCost, CTR, AvgCPC, AvgPosition, CostPerConv, ConvRate, ViewThroughConv
    }
    public class QueryWord
    {

        private List<List<decimal>> _stats;

        public string Word { get; set; }
        public string Query { get; set; }
        public decimal Cost { get; set; }
        public decimal GP { get; set; }
        public decimal NetProfit { get; set; }
        public decimal NPPerConv { get; set; }
        public decimal GPPerConv { get; set; }
        public decimal ConvValPerCost { get; set; }
        public decimal CostPerConv { get; set; }
        public decimal AvgCPC { get; set; }
        public double ROI { get; set; }
        public double CTR { get; set; }
        public double AvgPosition { get; set; }
        public double ConvRate { get; set; }
        public double Conversions { get; set; }
        public double Clicks { get; set; }
        public double Impressions { get; set; }
        public double ViewThroughConv { get; set; }

        public List<List<decimal>> Stats
        {
            get { return _stats; }
            set { _stats = value; }
        }

        public QueryWord()
        {
            Stats = new List<List<decimal>>();
            Word = "Not Set";
            Query = "Not Set";
        }
        public QueryWord(string word, string query, List<decimal> list)
        {
            this.Stats = new List<List<decimal>>();
            this.Stats.Add(list);
            this.Word = word;
            this.Query = query;
        }

        public void Fill(string word, string query, List<decimal> list)
        {
            this.Stats.Add(list);
            this.Word = word;
            this.Query = query;
        }

        public void SetStat(decimal num, StatType statType)
        {
            bool set = false;
            switch (statType)
            {
                case StatType.Cost:
                    this.Cost = num;
                    set = true;
                    break;
                case StatType.NetProfit:
                    this.NetProfit = num;
                    set = true;
                    break;
                case StatType.GP:
                    this.GP = num;
                    set = true;
                    break;
                case StatType.GPPerConv:
                    this.GPPerConv = num;
                    set = true;
                    break;
                case StatType.NPPerConv:
                    this.NPPerConv = num;
                    set = true;
                    break;
                case StatType.ConvValPerCost:
                    this.ConvValPerCost = num;
                    set = true;
                    break;
                case StatType.AvgCPC:
                    this.AvgCPC = num;
                    set = true;
                    break;
                case StatType.CostPerConv:
                    this.CostPerConv = num;
                    set = true;
                    break;
                default:
                    break;
            }
            if (!set)
            {
                SetStat(num.ToString(), statType);
            }


        }
        public void SetStat(double num, StatType statType)
        {
            bool set = false;
            switch (statType)
            {
                case StatType.ROI:
                    this.ROI = num;
                    set = true;
                    break;
                case StatType.Conversions:
                    this.Conversions = num;
                    set = true;
                    break;
                case StatType.Clicks:
                    this.Clicks = num;
                    set = true;
                    break;
                case StatType.Impressions:
                    this.Impressions = num;
                    set = true;
                    break;
                case StatType.CTR:
                    set = true;
                    this.CTR = num;
                    break;
                case StatType.AvgPosition:
                    this.AvgPosition = num;
                    set = true;
                    break;
                case StatType.ConvRate:
                    this.ConvRate = num;
                    set = true;
                    break;
                case StatType.ViewThroughConv:
                    set = true;
                    break;
                default:
                    break;
            }
            if (!set)
            {
                SetStat(num.ToString(), statType);
            }
        }

        public void SetStat(string str, StatType statType)
        {
            try
            {
                if (statType == StatType.Word)
                {
                    this.Word = str;
                }
                else if (statType == StatType.Query)
                {
                    this.Query = str;
                }
                else
                {
                    double dub;
                    decimal dec;
                    if (decimal.TryParse(str, out dec))
                    {
                        SetStat(dec, statType);
                    }
                    else if (double.TryParse(str, out dub))
                    {
                        SetStat(dub, statType);
                    }
                    else
                    {
                        throw new Exception($"Error Setting {statType} for QueryWord: could not parst the string to a valid format.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting {statType} for QueryWord: {ex.Message}");
            }
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
