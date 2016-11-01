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
        static RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;// | RegexOptions.ExplicitCapture;

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
        private string Conversions_regex = @"Conversion(s)?";
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
            Fill(ref stream);
        }
        public void Fill(ref StringWriter stream)
        {
            string fileString = stream.ToString();
            List<string> statsList = fileString.Split('\n').ToList();

            setHeaders(statsList[0].Split(',').ToList());
            Fill(ref statsList);

        }
        public void setHeaders(List<string> headerRow)
        {
            List<string> newHeaderRow = (from s in headerRow
                                         select s.Trim()).ToList();
            this.Headers = newHeaderRow;

            string rowString = string.Join(",", newHeaderRow);

            this.Word_ColIndex = SetStatColumns(Word_regex, ref rowString, ref newHeaderRow);
            this.Query_ColIndex = SetStatColumns(Query_regex, ref rowString, ref newHeaderRow);
            this.Cost_ColIndex = SetStatColumns(Cost_regex, ref rowString, ref newHeaderRow);
            this.GP_ColIndex = SetStatColumns(GP_regex, ref rowString, ref newHeaderRow);
            this.NetProfit_ColIndex = SetStatColumns(NetProfit_regex, ref rowString, ref newHeaderRow);
            this.ROI_ColIndex = SetStatColumns(ROI_regex, ref rowString, ref newHeaderRow);
            this.NPPerConv_ColIndex = SetStatColumns(NPPerConv_regex, ref rowString, ref newHeaderRow);
            this.GPPerConv_ColIndex = SetStatColumns(GPPerConv_regex, ref rowString, ref newHeaderRow);
            this.Conversions_ColIndex = SetStatColumns(Conversions_regex, ref rowString, ref newHeaderRow);
            this.Clicks_ColIndex = SetStatColumns(Clicks_regex, ref rowString, ref newHeaderRow);
            this.Impressions_ColIndex = SetStatColumns(Impressions_regex, ref rowString, ref newHeaderRow);
            this.ConvValPerCost_ColIndex = SetStatColumns(ConvValPerCost_regex, ref rowString, ref newHeaderRow);
            this.CTR_ColIndex = SetStatColumns(CTR_regex, ref rowString, ref newHeaderRow);
            this.AvgCPC_ColIndex = SetStatColumns(AvgCPC_regex, ref rowString, ref newHeaderRow);
            this.AvgPosition_ColIndex = SetStatColumns(AvgPosition_regex, ref rowString, ref newHeaderRow);
            this.CostPerConv_ColIndex = SetStatColumns(CostPerConv_regex, ref rowString, ref newHeaderRow);
            this.ConvRate_ColIndex = SetStatColumns(ConvRate_regex, ref rowString, ref newHeaderRow);
            this.ViewThroughConv_ColIndex = SetStatColumns(ViewThroughConv_regex, ref rowString, ref newHeaderRow);

        }

        private int SetStatColumns(string regex, ref string rowString, ref List<string> headerRow)
        {
            try
            {
                if (Regex.IsMatch(rowString, regex, options))
                {
                    string wordMatch = Regex.Match(rowString, regex, options).ToString();
                    int index = headerRow.IndexOf(wordMatch);
                    return index;
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
            statsList.RemoveAt(0);

            foreach (string fullRow in statsList)
            {
                try
                {
                    var rowStats = fullRow.Split(',').ToList();

                    string word = rowStats[this.Word_ColIndex];
                    string query = rowStats[this.Query_ColIndex];
                    //, rowStats[_queryColumn] };


                    var newList = new List<Stat>();
                    for (int i = 0; i < rowStats.Count; i++)
                    {
                        if (i != this.Word_ColIndex && i != this.Query_ColIndex)
                        {
                            StatType statType = getStatType(i);
                            string value = rowStats[i];
                            Stat stat = new Stat(value, getStatType(i));
                            SetStat(value, statType);
                            newList.Add(stat);
                        }
                    }

                    try
                    {
                        this[word].Stats.Add(newList);
                    }
                    catch (KeyNotFoundException)
                    {
                        this[word] = new QueryWord(word, query, newList);
                        this[word].Stats.Add(newList);
                    }

                    Console.WriteLine($"Key: {word}. Rows: {this[word].Stats.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error filling StatsList: {ex.Message}");
                }
            }

            Console.WriteLine("Sort Finished.");
        }

        private void SetStat(string item, StatType statType)
        {
            throw new NotImplementedException();
        }

        private StatType getStatType(int columnIndex)
        {

            if (columnIndex == Cost_ColIndex)
            {
                return StatType.Cost;
            }
            else if (columnIndex == Word_ColIndex)
            {
                return StatType.Word;

            }
            else if (columnIndex == Query_ColIndex)
            {
                return StatType.Query;
            }
            else if (columnIndex == Clicks_ColIndex)
            {
                return StatType.Clicks;
            }
            else if (columnIndex == AvgCPC_ColIndex)
            {
                return StatType.AvgCPC;
            }
            else if (columnIndex == AvgPosition_ColIndex)
            {
                return StatType.AvgPosition;
            }
            else if (columnIndex == Conversions_ColIndex)
            {
                return StatType.Conversions;
            }
            else if (columnIndex == ConvRate_ColIndex)
            {
                return StatType.ConvRate;
            }
            else if (columnIndex == ConvValPerCost_ColIndex)
            {
                return StatType.ConvValPerCost;
            }
            else if (columnIndex == CostPerConv_ColIndex)
            {
                return StatType.CostPerConv;
            }
            else if (columnIndex == CTR_ColIndex)
            {
                return StatType.CTR;
            }
            else if (columnIndex == GPPerConv_ColIndex)
            {
                return StatType.GPPerConv;
            }
            else if (columnIndex == GP_ColIndex)
            {
                return StatType.GP;
            }
            else if (columnIndex == Impressions_ColIndex)
            {
                return StatType.Impressions;
            }
            else if (columnIndex == NetProfit_ColIndex)
            {
                return StatType.NetProfit;
            }
            else if (columnIndex == NPPerConv_ColIndex)
            {
                return StatType.NPPerConv;
            }
            else if (columnIndex == ROI_ColIndex)
            {
                return StatType.ROI;
            }
            else if (columnIndex == ViewThroughConv_ColIndex)
            {
                return StatType.ViewThroughConv;
            }
            else
            {
                throw new Exception($"Stat type not found at column {columnIndex}");
            }


        }
    }
}
