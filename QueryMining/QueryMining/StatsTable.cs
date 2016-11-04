﻿using System;
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
        public const string numberRegex = @"-?[0-9]*(.?[0-9]*)?";
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

        public List<string> Columns { get; set; }

        public StatsTable()
        {
            this.Columns = new List<string>();
            this.Headers = new List<string>();
        }

        public StatsTable(ref StringWriter stream)
        {
            this.Columns = new List<string>();
            this.Headers = new List<string>();
            Fill(ref stream);
        }

        public void Fill(ref StringWriter stream)
        {
            string fileString = stream.ToString();
            List<string> rows = fileString.Split('\n').ToList();

            List<string> headerRow = (from s in rows[0].Split(',')
                                      select s.Trim()).ToList();
            setHeaders(headerRow);
            rows.RemoveAt(0);
            Fill(ref rows);
        }

        public void setHeaders(List<string> headerRow)
        {
            this.Headers = headerRow;
            string rowString = string.Join(",", headerRow);

            this.Word_ColIndex = SetStatColumn(Word_regex, ref rowString, ref headerRow);
            this.Query_ColIndex = SetStatColumn(Query_regex, ref rowString, ref headerRow);
            this.Cost_ColIndex = SetStatColumn(Cost_regex, ref rowString, ref headerRow);
            this.GP_ColIndex = SetStatColumn(GP_regex, ref rowString, ref headerRow);
            this.NetProfit_ColIndex = SetStatColumn(NetProfit_regex, ref rowString, ref headerRow);
            this.ROI_ColIndex = SetStatColumn(ROI_regex, ref rowString, ref headerRow);
            this.NPPerConv_ColIndex = SetStatColumn(NPPerConv_regex, ref rowString, ref headerRow);
            this.GPPerConv_ColIndex = SetStatColumn(GPPerConv_regex, ref rowString, ref headerRow);
            this.Conversions_ColIndex = SetStatColumn(Conversions_regex, ref rowString, ref headerRow);
            this.Clicks_ColIndex = SetStatColumn(Clicks_regex, ref rowString, ref headerRow);
            this.Impressions_ColIndex = SetStatColumn(Impressions_regex, ref rowString, ref headerRow);
            this.ConvValPerCost_ColIndex = SetStatColumn(ConvValPerCost_regex, ref rowString, ref headerRow);
            this.CTR_ColIndex = SetStatColumn(CTR_regex, ref rowString, ref headerRow);
            this.AvgCPC_ColIndex = SetStatColumn(AvgCPC_regex, ref rowString, ref headerRow);
            this.AvgPosition_ColIndex = SetStatColumn(AvgPosition_regex, ref rowString, ref headerRow);
            this.CostPerConv_ColIndex = SetStatColumn(CostPerConv_regex, ref rowString, ref headerRow);
            this.ConvRate_ColIndex = SetStatColumn(ConvRate_regex, ref rowString, ref headerRow);
            this.ViewThroughConv_ColIndex = SetStatColumn(ViewThroughConv_regex, ref rowString, ref headerRow);

        }

        private int SetStatColumn(string regex, ref string rowString, ref List<string> headerRow)
        {
            try
            {
                int index = -1;
                if (Regex.IsMatch(rowString, regex, options))
                {
                    string wordMatch = Regex.Match(rowString, regex, options).ToString();
                    index = headerRow.IndexOf(wordMatch);
                    this.Columns.Add(wordMatch);
                }
                return index;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting stat info: {ex.Message}");
                return -1;
            }
        }

        public DataTable ToDataTable()
        {
            try
            {
                DataTable table = new DataTable();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    var column = this.Columns[i];
                    if (column != "" && column != null)
                    {
                        table.Columns.Add(column);
                    }
                }

                foreach (var item in this.Values)
                {
                    table.Rows.Add(item.FullRowItems());
                }


                return table;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Filling table: {ex.Message}");
            }
        }

        /// <summary>
        /// Fill the underlying Dictionary from a list of rows
        /// </summary>
        /// <param name="statsList"></param>
        public void Fill(ref List<string> statsList)
        {
            statsList.RemoveAt(0);
            for (int i = 0; i < statsList.Count; i++)
            {
                try
                {
                    string fullRow = statsList[i];

                    List<string> rowStats = fullRow.Split(',').ToList();
                    List<decimal> newList = new List<decimal>();
                    string word = rowStats[this.Word_ColIndex],
                        query = rowStats[this.Query_ColIndex];

                    if (!this.ContainsKey(word))
                    {
                        this[word] = new QueryWord();
                    }

                    for (int j = 0; j < rowStats.Count; j++)
                    {
                        try
                        {
                            string stat = rowStats[j];

                            StatType statType = getStatType(j);
                            this[word].SetStat(stat, statType);

                            if (Regex.IsMatch(stat, numberRegex))
                            {
                                string str;
                                decimal num;
                                str = Regex.Match(stat, numberRegex).Value;
                                if (decimal.TryParse(str, out num))
                                {
                                    newList.Add(num);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error filling Row {i}: {ex.Message}");
                        }
                    }

                    this[word].Fill(word, query, newList);
                    this[word].Rows.Add(newList);

                    Console.WriteLine($"Key: {word}. Rows: {this[word].Rows.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding row{i} to StatsTable: {ex.Message}");
                }
            }
            Console.WriteLine("Sort Finished.");
        }


        private StatType getStatType(int columnIndex)
        {
            if (columnIndex >= 0)
            {
                if (columnIndex == AvgCPC_ColIndex)
                {
                    return StatType.AvgCPC;
                }
                else if (columnIndex == AvgPosition_ColIndex)
                {
                    return StatType.AvgPosition;
                }
                else if (columnIndex == Clicks_ColIndex)
                {
                    return StatType.Clicks;
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
                else if (columnIndex == Cost_ColIndex)
                {
                    return StatType.Cost;
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
                else if (columnIndex == Query_ColIndex)
                {
                    return StatType.Query;
                }
                else if (columnIndex == ROI_ColIndex)
                {
                    return StatType.ROI;
                }
                else if (columnIndex == ViewThroughConv_ColIndex)
                {
                    return StatType.ViewThroughConv;
                }
                else if (columnIndex == Word_ColIndex)
                {
                    return StatType.Word;
                }
                else
                {
                    throw new Exception($"The index {columnIndex} does not correspond to any existing stat column.");
                }
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
