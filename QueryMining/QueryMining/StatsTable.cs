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
    public class StatsTable : Dictionary<string, QueryWord>
    {
        static RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture;
        public List<string> Headers { get; set; }

        public int Word_ColIndex { get; set; }
        public int Query_ColIndex { get; set; }
        public int Cost_ColIndex { get; set; }
        public int GP_ColIndex { get; set; }
        public int NetProfit_ColIndex { get; set; }
        public int ROI_ColIndex { get; set; }
        public int NPPerConv_ColIndex { get; set; }
        public int GPPerConv_ColIndex { get; set; }
        public int Conversions_ColIndex { get; set; }
        public int Clicks_ColIndex { get; set; }
        public int Impressions_ColIndex { get; set; }
        public int ConvValPerCost_ColIndex { get; set; }
        public int CTR_ColIndex { get; set; }
        public int AvgCPC_ColIndex { get; set; }
        public int AvgPosition_ColIndex { get; set; }
        public int CostPerConv_ColIndex { get; set; }
        public int ConvRate_ColIndex { get; set; }
        public int ViewThroughConv_ColIndex { get; set; }

        private string Word_regex = @"Word";
        private string Query_regex = @"Query|Search ?term";
        private string Cost_regex = @"Cost";
        private string GP_regex = @"GP|Gross ?Profit|Total ?(Conv\.?|Conversion) (value|val\.?)";
        private string NetProfit_regex = @"Net\s?Profit";
        private string ROI_regex = @"ROI|ROAS";
        private string NPPerConv_regex = @"NP ?\/ ?Conv|NPPerConv";
        private string GPPerConv_regex = @"GP ?\/ ?Conv\.?|GPPerConv";
        private string Conversions_regex = @"Conversions|Conv\.?";
        private string Clicks_regex = @"Clicks";
        private string Impressions_regex = @"Impressions|Imp\.?";
        private string ConvValPerCost_regex = @"Conv\.? ?value ?\/ ?cost|ConvValPerCost";
        private string CTR_regex = @"CTR|Clickthrough ?rate";
        private string AvgCPC_regex = @"Avg\.? ?CPC";
        private string AvgPosition_regex = @"Avg\.? ?Position";
        private string CostPerConv_regex = @"Cost\.? ?\/ ?Conv\.?|CostPerConv(ersion)?";
        private string ConvRate_regex = @"(Conv\.?|Conversion) ?Rate";
        private string ViewThroughConv_regex = @"View\-?through ?Conv\.?";


        public StatsTable()
        {
        }

        public StatsTable(ref StringWriter stream)
        {
            string fileString = stream.ToString();
            List<string> statsList = fileString.Split('\n').ToList();
            Fill(ref statsList);
        }

        public void setHeaders(List<string> headerRow)
        {
            this.Headers = headerRow;
            string rowString = headerRow.ToString();

            this.Word_ColIndex = SetStatColumns(Word_regex, ref rowString, ref headerRow);
            this.Query_ColIndex = SetStatColumns(Query_regex, ref rowString, ref headerRow);
            this.Cost_ColIndex = SetStatColumns(Cost_regex, ref rowString, ref headerRow);
            this.GP_ColIndex = SetStatColumns(GP_regex, ref rowString, ref headerRow);
            this.NetProfit_ColIndex = SetStatColumns(NetProfit_regex, ref rowString, ref headerRow);
            this.ROI_ColIndex = SetStatColumns(ROI_regex, ref rowString, ref headerRow);
            this.NPPerConv_ColIndex = SetStatColumns(NPPerConv_regex, ref rowString, ref headerRow);
            this.GPPerConv_ColIndex = SetStatColumns(GPPerConv_regex, ref rowString, ref headerRow);
            this.Conversions_ColIndex = SetStatColumns(Conversions_regex, ref rowString, ref headerRow);
            this.Clicks_ColIndex = SetStatColumns(Clicks_regex, ref rowString, ref headerRow);
            this.Impressions_ColIndex = SetStatColumns(Impressions_regex, ref rowString, ref headerRow);
            this.ConvValPerCost_ColIndex = SetStatColumns(ConvValPerCost_regex, ref rowString, ref headerRow);
            this.CTR_ColIndex = SetStatColumns(CTR_regex, ref rowString, ref headerRow);
            this.AvgCPC_ColIndex = SetStatColumns(AvgCPC_regex, ref rowString, ref headerRow);
            this.AvgPosition_ColIndex = SetStatColumns(AvgPosition_regex, ref rowString, ref headerRow);
            this.CostPerConv_ColIndex = SetStatColumns(CostPerConv_regex, ref rowString, ref headerRow);
            this.ConvRate_ColIndex = SetStatColumns(ConvRate_regex, ref rowString, ref headerRow);
            this.ViewThroughConv_ColIndex = SetStatColumns(ViewThroughConv_regex, ref rowString, ref headerRow);

        }

        private int SetStatColumns(string regex, ref string rowString, ref List<string> headerRow)
        {
            try
            {
                if (Regex.IsMatch(rowString, regex, options))
                {
                    string wordMatch = Regex.Match(rowString, regex, options).ToString();
                    return headerRow.IndexOf(wordMatch);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting stat info: {ex.Message}");
                return -1;
            }
        }

        public void Fill(ref List<string> statsList)
        {
            setHeaders(statsList[0].Split(',').ToList());
            statsList.RemoveAt(0);

            foreach (string fullRow in statsList)
            {
                var rowStats = fullRow.Split(',').ToList();
                string word = rowStats[this.Word_ColIndex];
                string query = rowStats[this.Query_ColIndex];
                string key = word; //, rowStats[_queryColumn] };
                rowStats.RemoveAt(this.Query_ColIndex);
                rowStats.RemoveAt(this.Word_ColIndex);
                QueryWord newWord = new QueryWord();
                List<decimal> newList = new List<decimal>();
                for (int i = 0; i < rowStats.Count; i++)
                {
                    string item = rowStats[i];
                    StatType statType = getStatType(i);
                    SetStat(ref newList, item, statType);
                }
                newWord.Fill(word, query, newList);
                try
                {
                    this[key].Stats.Add(newList);
                }
                catch (KeyNotFoundException)
                {
                    this[key] = newWord;

                    this[key].Stats.Add(newList);
                }
                Console.WriteLine($"Key: {word}. Rows: {this[key].Stats.Count}");
            }
            Console.WriteLine("Sort Finished.");
        }

        private void SetStat(ref List<decimal> newList, string item, StatType statType)
        {
            decimal stat;
            if (decimal.TryParse(item, out stat))
            {
                newList.Add(stat);
                newList.AddStat(item, statType);
            }
            else
            {
                newList.Add(0);
            }
        }

        private StatType getStatType(int i)
        {
            if (i >= 0)
            {
                if (i == AvgCPC_ColIndex)
                {
                    return StatType.AvgCPC;
                }
                else if (i == AvgPosition_ColIndex)
                {
                    return StatType.AvgPosition;
                }
                else if (i == Clicks_ColIndex)
                {
                    return StatType.Clicks;
                }
                else if (i == Conversions_ColIndex)
                {
                    return StatType.Conversions;
                }
                else if (i == ConvRate_ColIndex)
                {
                    return StatType.ConvRate;
                }
                else if (i == ConvValPerCost_ColIndex)
                {
                    return StatType.ConvValPerCost;
                }
                else if (i == CostPerConv_ColIndex)
                {
                    return StatType.CostPerConv;
                }
                else if (i == Cost_ColIndex)
                {
                    return StatType.Cost;
                }
                else if (i == CTR_ColIndex)
                {
                    return StatType.CTR;
                }
                else if (i == GPPerConv_ColIndex)
                {
                    return StatType.GPPerConv;
                }
                else if (i == GP_ColIndex)
                {
                    return StatType.GP;
                }
                else if (i == Impressions_ColIndex)
                {
                    return StatType.Impressions;
                }
                else if (i == NetProfit_ColIndex)
                {
                    return StatType.NetProfit;
                }
                else if (i == NPPerConv_ColIndex)
                {
                    return StatType.NPPerConv;
                }
                else if (i == Query_ColIndex)
                {
                    return StatType.Query;
                }
                else if (i == ROI_ColIndex)
                {
                    return StatType.ROI;
                }
                else if (i == ViewThroughConv_ColIndex)
                {
                    return StatType.ViewThroughConv;
                }
                else if (i == Word_ColIndex)
                {
                    return StatType.Word;
                }
                else
                {
                    throw new Exception($"The index {i} does not correspond to any existing stat column.");
                }
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
