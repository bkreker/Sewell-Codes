using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public string Word { get; set; }
        public string Query { get; set; }

        public decimal? Cost
        {
            get { return CostStat.Value; }
            set
            {
                if (value != null)
                {
                    CostStat = new Stat<decimal>((decimal)value, StatType.Cost);
                }
            }
        }
        public decimal? GP
        {
            get { return GPStat.Value; }
            set
            {
                if (value != null)
                {
                    GPStat = new Stat<decimal>((decimal)value, StatType.GP);
                }
            }
        }
        public decimal? NetProfit
        {
            get { return NetProfitStat.Value; }
            set
            {
                if (value != null)
                {
                    NetProfitStat = new Stat<decimal>((decimal)value, StatType.NetProfit);
                }
            }
        }
        public decimal? NPPerConv
        {
            get { return NPPerConvStat.Value; }
            set
            {
                if (value != null)
                {
                    NPPerConvStat = new Stat<decimal>((decimal)value, StatType.NPPerConv);
                }
            }
        }
        public decimal? GPPerConv
        {
            get { return GPPerConvStat.Value; }
            set
            {
                if (value != null)
                {
                    GPPerConvStat = new Stat<decimal>((decimal)value, StatType.GPPerConv);
                }
            }
        }
        public decimal? ConvValPerCost
        {
            get { return ConvValPerCostStat.Value; }
            set
            {
                if (value != null)
                {
                    ConvValPerCostStat = new Stat<decimal>((decimal)value, StatType.ConvValPerCost);
                }
            }
        }
        public decimal? CostPerConv
        {
            get { return CostPerConvStat.Value; }
            set
            {
                if (value != null)
                {
                    CostPerConvStat = new Stat<decimal>((decimal)value, StatType.CostPerConv);
                }
            }
        }
        public decimal? AvgCPC
        {
            get { return AvgCPCStat.Value; }
            set
            {
                if (value != null)
                {
                    AvgCPCStat = new Stat<decimal>((decimal)value, StatType.AvgCPC);
                }
            }
        }
        public double? ROI
        {
            get { return ROIStat.Value; }
            set
            {
                if (value != null)
                {
                    ROIStat = new Stat<double>((double)value, StatType.ROI);
                }
            }
        }
        public double? CTR
        {
            get { return CTRStat.Value; }
            set
            {
                if (value != null)
                {
                    CTRStat = new Stat<double>((double)value, StatType.CTR);
                }
            }
        }
        public double? AvgPosition
        {
            get { return AvgPositionStat.Value; }
            set
            {
                if (value != null)
                {
                    AvgPositionStat = new Stat<double>((double)value, StatType.AvgPosition);
                }
            }
        }
        public double? ConvRate
        {
            get { return ConvRateStat.Value; }
            set
            {
                if (value != null)
                {
                    ConvRateStat = new Stat<double>((double)value, StatType.ConvRate);
                }
            }
        }
        public double? Conversions
        {
            get { return ConversionsStat.Value; }
            set
            {
                if (value != null)
                {
                    ConversionsStat = new Stat<double>((double)value, StatType.Conversions);
                }
            }
        }
        public double? Clicks
        {
            get { return ClicksStat.Value; }
            set
            {
                if (value != null)
                {
                    ClicksStat = new Stat<double>((double)value, StatType.Clicks);
                }
            }
        }
        public double? Impressions
        {
            get { return ImpressionsStat.Value; }
            set
            {
                if (value != null)
                {
                    ImpressionsStat = new Stat<double>((double)value, StatType.Impressions);
                }
            }
        }
        public double? ViewThroughConv
        {
            get { return ViewThroughConvStat.Value; }
            set
            {
                if (value != null)
                {
                    ViewThroughConvStat = new Stat<double>((double)value, StatType.ViewThroughConv);
                }
            }
        }

        public Stat<decimal> CostStat { get; set; }
        public Stat<decimal> GPStat { get; set; }
        public Stat<decimal> NetProfitStat { get; set; }
        public Stat<decimal> NPPerConvStat { get; set; }
        public Stat<decimal> GPPerConvStat { get; set; }
        public Stat<decimal> ConvValPerCostStat { get; set; }
        public Stat<decimal> CostPerConvStat { get; set; }
        public Stat<decimal> AvgCPCStat { get; set; }
        public Stat<double> ROIStat { get; set; }
        public Stat<double> CTRStat { get; set; }
        public Stat<double> AvgPositionStat { get; set; }
        public Stat<double> ConvRateStat { get; set; }
        public Stat<double> ConversionsStat { get; set; }
        public Stat<double> ClicksStat { get; set; }
        public Stat<double> ImpressionsStat { get; set; }
        public Stat<double> ViewThroughConvStat { get; set; }

        public List<List<decimal>> Rows { get; set; }

        public string RowString()
        {
            Cost = null;
            return $"{Word},{Query}{Cost},{GP},{NetProfit},{ROI},{NPPerConv},{GPPerConv},{Conversions},{Clicks},{Impressions},{ConvValPerCost},{CTR},{AvgCPC},{AvgPosition},{CostPerConv},{ConvRate},{ViewThroughConv}";
        }

        public object[] FullRowItems()
        {
            var result = new List<object>();
            if (this.Cost != null)              /**/result.Add(this.Cost);
            if (this.GP != null)                /**/result.Add(this.GP);
            if (this.NetProfit != null)         /**/result.Add(this.NetProfit);
            if (this.ROI != null)               /**/result.Add(this.ROI);
            if (this.NPPerConv != null)         /**/result.Add(this.NPPerConv);
            if (this.GPPerConv != null)         /**/result.Add(this.GPPerConv);
            if (this.Conversions != null)       /**/result.Add(this.Conversions);
            if (this.Clicks != null)            /**/result.Add(this.Clicks);
            if (this.Impressions != null)       /**/result.Add(this.Impressions);
            if (this.ConvValPerCost != null)    /**/result.Add(this.ConvValPerCost);
            if (this.CTR != null)               /**/result.Add(this.CTR);
            if (this.AvgCPC != null)            /**/result.Add(this.AvgCPC);
            if (this.AvgPosition != null)       /**/result.Add(this.AvgPosition);
            if (this.CostPerConv != null)       /**/result.Add(this.CostPerConv);
            if (this.ConvRate != null)          /**/result.Add(this.ConvRate);
            if (this.ViewThroughConv != null)   /**/result.Add(this.ViewThroughConv);


            return result.ToArray();

        }

        public List<TopStat> StatsList()
        {
            try
            {
                List<TopStat> result = new List<TopStat>();
                if (this.Cost != null)             /**/result.Add(this.CostStat);
                if (this.GP != null)               /**/result.Add(this.GPStat);
                if (this.NetProfit != null)        /**/result.Add(this.NetProfitStat);
                if (this.ROI != null)              /**/result.Add(this.NPPerConvStat);
                if (this.NPPerConv != null)        /**/result.Add(this.GPPerConvStat);
                if (this.GPPerConv != null)        /**/result.Add(this.ConvValPerCostStat);
                if (this.Conversions != null)      /**/result.Add(this.CostPerConvStat);
                if (this.Clicks != null)           /**/result.Add(this.AvgCPCStat);
                if (this.Impressions != null)      /**/result.Add(this.ROIStat);
                if (this.ConvValPerCost != null)   /**/result.Add(this.CTRStat);
                if (this.CTR != null)              /**/result.Add(this.AvgPositionStat);
                if (this.AvgCPC != null)           /**/result.Add(this.ConvRateStat);
                if (this.AvgPosition != null)      /**/result.Add(this.ConversionsStat);
                if (this.CostPerConv != null)      /**/result.Add(this.ClicksStat);
                if (this.ConvRate != null)         /**/result.Add(this.ImpressionsStat);
                if (this.ViewThroughConv != null)  /**/result.Add(this.ViewThroughConvStat);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Filling StatsList: {ex.Message}");
            }

        }

        public List<string> GetTotalRowString()
        {
            throw new NotImplementedException();
        }

        public QueryWord(string word = "Not Set", string query = "Not Set")
        {
            this.Rows = new List<List<decimal>>();
            CostStat = new Stat<decimal>(0, StatType.Cost);
            this.Word = word;
            this.Query = query;
            this.Query = query;
            this.Cost = 0;
            this.GP = 0;
            this.NetProfit = 0;
            this.ROI = 0;
            this.NPPerConv = 0;
            this.GPPerConv = 0;
            this.Conversions = 0;
            this.Clicks = 0;
            this.Impressions = 0;
            this.ConvValPerCost = 0;
            this.CTR = 0;
            this.AvgCPC = 0;
            this.AvgPosition = 0;
            this.CostPerConv = 0;
            this.ConvRate = 0;
            this.ViewThroughConv = 0;
        }

        /// <summary>
        /// Initializes a QueryWord, but doesn't set the individual Stats since it doesn't have access to the column indexes
        /// </summary>
        /// <param name="word"></param>
        /// <param name="query"></param>
        /// <param name="statsRow"></param>
        public QueryWord(string word, string query, List<decimal> statsRow)
        {
            this.Rows = new List<List<decimal>>();
            this.Word = word;
            this.Query = query;
            this.Rows.Add(statsRow);
        }

        /// <summary>
        /// Initializes this QueryWord, after the fact
        /// </summary>
        /// <param name="word"></param>
        /// <param name="query"></param>
        /// <param name="list"></param>
        public void Fill(string word, string query, List<decimal> list)
        {
            this.Rows.Add(list);
            this.Word = word;
            this.Query = query;
        }

        public void SetStat(decimal num, StatType statType)
        {
            bool set = false;
            try
            {
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
                        SetStat((double)num, statType);
                        set = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting {statType} for QueryWord: {ex.Message}");
            }
            if (!set)
            {
                throw new Exception($"Error Setting {statType} for QueryWord");
            }
        }

        public void SetStat(double num, StatType statType)
        {
            bool set = false;
            try
            {
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
                        this.CTR = num > 1 ? num / 100 : num;
                        set = true;
                        break;
                    case StatType.AvgPosition:
                        this.AvgPosition = num;
                        set = true;
                        break;
                    case StatType.ConvRate:
                        this.ConvRate = num > 1 ? num / 100 : num;
                        set = true;
                        break;
                    case StatType.ViewThroughConv:
                        set = true;
                        break;
                    default:
                        SetStat((decimal)num, statType);
                        set = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting {statType} for QueryWord: {ex.Message}");
            }
            if (!set)
            {
                throw new Exception($"Error Setting {statType} for QueryWord");
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
                    str = Regex.Match(str, @"-?[0-9]*(.?[0-9]*)?").Value;
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
                        throw new Exception($"Error Setting {statType} for QueryWord: could not parse the string to a valid format.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting {statType} for QueryWord: {ex.Message}");
            }
        }

        public List<string> GetRow()
        {
            return (RowString().Split(',').ToList());
        }


        /// <summary>
        /// Adds every single value of the QueryWord
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public QueryWord operator +(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();

            result.Word = left.Word + " " + right.Word;
            result.Query = left.Query + " / " + right.Query;

            result.Cost           /**/  = left.Cost            /**/ + right.Cost           /**/ ;
            result.GP             /**/  = left.GP              /**/ + right.GP             /**/ ;
            result.NetProfit      /**/  = left.NetProfit       /**/ + right.NetProfit      /**/ ;
            result.ROI            /**/  = (left.ROI            /**/ + right.ROI            /**/ ) / 2;
            result.NPPerConv      /**/  = (left.NPPerConv      /**/ + right.NPPerConv      /**/ ) / 2;
            result.GPPerConv      /**/  = (left.GPPerConv      /**/ + right.GPPerConv      /**/ ) / 2;
            result.Conversions    /**/  = left.Conversions     /**/ + right.Conversions    /**/ ;
            result.Clicks         /**/  = left.Clicks          /**/ + right.Clicks         /**/ ;
            result.Impressions    /**/  = left.Impressions     /**/ + right.Impressions    /**/ ;
            result.ConvValPerCost /**/  = left.ConvValPerCost  /**/ + right.ConvValPerCost /**/ ;
            result.CTR            /**/  = (left.CTR            /**/ + right.CTR            /**/ ) / 2;
            result.AvgCPC         /**/  = (left.AvgCPC         /**/ + right.AvgCPC         /**/ ) / 2;
            result.AvgPosition    /**/  = (left.AvgPosition    /**/ + right.AvgPosition    /**/ ) / 2;
            result.CostPerConv    /**/  = (left.CostPerConv    /**/ + right.CostPerConv    /**/ ) / 2;
            result.ConvRate       /**/  = (left.ConvRate       /**/ + right.ConvRate       /**/ ) / 2;
            result.ViewThroughConv/**/  = left.ViewThroughConv /**/ + right.ViewThroughConv/**/ ;

            return result;
        }

        /// <summary>
        /// Multiplies every single value of the QueryWord
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public QueryWord operator *(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();
            result.Cost           /**/  = left.Cost            /**/  * right.Cost           /**/  ;
            result.GP             /**/  = left.GP              /**/  * right.GP             /**/  ;
            result.NetProfit      /**/  = left.NetProfit       /**/  * right.NetProfit      /**/  ;
            result.ROI            /**/  = left.ROI             /**/  * right.ROI            /**/  ;
            result.NPPerConv      /**/  = left.NPPerConv       /**/  * right.NPPerConv      /**/  ;
            result.GPPerConv      /**/  = left.GPPerConv       /**/  * right.GPPerConv      /**/  ;
            result.Conversions    /**/  = left.Conversions     /**/  * right.Conversions    /**/  ;
            result.Clicks         /**/  = left.Clicks          /**/  * right.Clicks         /**/  ;
            result.Impressions    /**/  = left.Impressions     /**/  * right.Impressions    /**/  ;
            result.ConvValPerCost /**/  = left.ConvValPerCost  /**/  * right.ConvValPerCost /**/  ;
            result.CTR            /**/  = left.CTR             /**/  * right.CTR            /**/  ;
            result.AvgCPC         /**/  = left.AvgCPC          /**/  * right.AvgCPC         /**/  ;
            result.AvgPosition    /**/  = left.AvgPosition     /**/  * right.AvgPosition    /**/  ;
            result.CostPerConv    /**/  = left.CostPerConv     /**/  * right.CostPerConv    /**/  ;
            result.ConvRate       /**/  = left.ConvRate        /**/  * right.ConvRate       /**/  ;
            result.ViewThroughConv/**/  = left.ViewThroughConv /**/  * right.ViewThroughConv/**/  ;

            return result;
        }

        /// <summary>
        /// Divides every single value of the QueryWord
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public QueryWord operator /(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();
            result.Cost           /**/ = left.Cost            /**/  / right.Cost           /**/ ;
            result.GP             /**/ = left.GP              /**/  / right.GP             /**/ ;
            result.NetProfit      /**/ = left.NetProfit       /**/  / right.NetProfit      /**/ ;
            result.ROI            /**/ = left.ROI             /**/  / right.ROI            /**/ ;
            result.NPPerConv      /**/ = left.NPPerConv       /**/  / right.NPPerConv      /**/ ;
            result.GPPerConv      /**/ = left.GPPerConv       /**/  / right.GPPerConv      /**/ ;
            result.Conversions    /**/ = left.Conversions     /**/  / right.Conversions    /**/ ;
            result.Clicks         /**/ = left.Clicks          /**/  / right.Clicks         /**/ ;
            result.Impressions    /**/ = left.Impressions     /**/  / right.Impressions    /**/ ;
            result.ConvValPerCost /**/ = left.ConvValPerCost  /**/  / right.ConvValPerCost /**/ ;
            result.CTR            /**/ = left.CTR             /**/  / right.CTR            /**/ ;
            result.AvgCPC         /**/ = left.AvgCPC          /**/  / right.AvgCPC         /**/ ;
            result.AvgPosition    /**/ = left.AvgPosition     /**/  / right.AvgPosition    /**/ ;
            result.CostPerConv    /**/ = left.CostPerConv     /**/  / right.CostPerConv    /**/ ;
            result.ConvRate       /**/ = left.ConvRate        /**/  / right.ConvRate       /**/ ;
            result.ViewThroughConv/**/ = left.ViewThroughConv /**/  / right.ViewThroughConv/**/ ;

            return result;
        }

        /// <summary>
        /// Subtracts every single value of the QueryWord
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public QueryWord operator -(QueryWord left, QueryWord right)
        {
            QueryWord result = new QueryMining.QueryWord();
            result.Cost           /**/ = left.Cost            /**/  - right.Cost           /**/  ;
            result.GP             /**/ = left.GP              /**/  - right.GP             /**/  ;
            result.NetProfit      /**/ = left.NetProfit       /**/  - right.NetProfit      /**/  ;
            result.ROI            /**/ = left.ROI             /**/  - right.ROI            /**/  ;
            result.NPPerConv      /**/ = left.NPPerConv       /**/  - right.NPPerConv      /**/  ;
            result.GPPerConv      /**/ = left.GPPerConv       /**/  - right.GPPerConv      /**/  ;
            result.Conversions    /**/ = left.Conversions     /**/  - right.Conversions    /**/  ;
            result.Clicks         /**/ = left.Clicks          /**/  - right.Clicks         /**/  ;
            result.Impressions    /**/ = left.Impressions     /**/  - right.Impressions    /**/  ;
            result.ConvValPerCost /**/ = left.ConvValPerCost  /**/  - right.ConvValPerCost /**/  ;
            result.CTR            /**/ = left.CTR             /**/  - right.CTR            /**/  ;
            result.AvgCPC         /**/ = left.AvgCPC          /**/  - right.AvgCPC         /**/  ;
            result.AvgPosition    /**/ = left.AvgPosition     /**/  - right.AvgPosition    /**/  ;
            result.CostPerConv    /**/ = left.CostPerConv     /**/  - right.CostPerConv    /**/  ;
            result.ConvRate       /**/ = left.ConvRate        /**/  - right.ConvRate       /**/  ;
            result.ViewThroughConv/**/ = left.ViewThroughConv /**/  - right.ViewThroughConv/**/  ;

            return result;
        }

        /// <summary>
        /// Only compares the word and full query of the QueryWord, not the values 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public bool operator ==(QueryWord left, QueryWord right)
        {
            return (left.Word == right.Word && left.Query == right.Query);
        }

        /// <summary>
        /// Only compares the word and full query of the QueryWord, not the values 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public bool operator !=(QueryWord left, QueryWord right)
        {
            return (left.Word != right.Word && left.Query != right.Query);
        }

        public override bool Equals(object obj)
        {
            var right = obj as QueryWord;
            return this == right;
        }

    }
}
