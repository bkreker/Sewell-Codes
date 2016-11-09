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
    public class StatDataTable : DataTable
    {
        // public int WordCol { get; set; }
        public int QueryCol { get; set; }
        public int QueryCountCol { get; set; }
        public List<string> Headers
        {
            get
            {
                return (from DataColumn col in this.Columns
                        select col.Caption).ToList();


            }
        }
        public void Save(string _outFileName, ref bool _outFileSavedCorrectly)
        {
            try
            {
                StreamWriter outFile = new StreamWriter(_outFileName);
                var tableList = (from DataRow row in this.Rows
                                 select (from i in row.ItemArray
                                         select i.ToString()).ToList<string>()).ToList();

                List<string> headerList = new List<string>();
                foreach (DataColumn column in this.Columns)
                {
                    headerList.Add(column.Caption);
                }

                outFile.WriteLine(string.Join(",", headerList));
                tableList.ForEach(a => outFile.WriteLine(string.Join(",", a)));

                outFile.Close();
                _outFileSavedCorrectly = true;
            }
            catch (Exception ex)
            {
                _outFileSavedCorrectly = false;
                MessageBox.Show(ex.Message, "Error saving file");
            }

        }

        public List<string> SetTableSchema(ref StreamReader inFile, ref char delimChar)
        {
            try
            {
                string firstRowString = inFile.ReadLine();
                string secondRowString = inFile.ReadLine();
                // if it detects it's actually .tsv, switch delimiters
                if (firstRowString.IndexOf('\t') >= 0)
                {
                    delimChar = '\t';
                }
                List<string> firstRow = firstRowString.Split(delimChar).ToList<string>();
                List<string> secondRow = secondRowString.Split(delimChar).ToList<string>();

                for (int i = 0; i < firstRow.Count; i++)
                {
                    string colName = firstRow[i].Trim(),
                        colVal = secondRow[i].Trim();
                    object defaultVal = "";
                    Type columnType = typeof(string);
                    if (Regexes.IsMatch(colVal, Regexes.Number) && Regexes.MatchesAnyStat(colName))
                    {
                        if (colVal.Contains('%'))
                        {
                            colVal = colVal.Remove(colVal.IndexOf('%'), 1);
                        }
                        if (colVal.Contains(','))
                        {
                            colVal = colVal.Remove(colVal.IndexOf(','), 1);
                        }
                        decimal dec;
                        double dub;
                        float fl;
                        long lon;
                        int ig;
                        if (Regexes.IsDouble(colName))
                        {
                            if (double.TryParse(colVal, out dub))
                            {
                                columnType = typeof(double);
                                defaultVal = 0.00;

                            }
                        }
                        else if (Regexes.IsDecimal(colName))
                        {
                            if (decimal.TryParse(colVal, out dec))
                            {
                                columnType = typeof(decimal);
                                defaultVal = 0m;

                            }
                        }
                        else if (Regexes.IsInt(colName))
                        {
                            if (int.TryParse(colVal, out ig))
                            {
                                columnType = typeof(int);
                                defaultVal = 0;
                            }

                        }
                        else if (Regexes.IsLong(colName))
                        {
                            if (long.TryParse(colVal, out lon))
                            {
                                columnType = typeof(long);
                                defaultVal = 0;
                            }

                        }
                        else if (float.TryParse(colVal, out fl))
                        {
                            columnType = typeof(float);
                            defaultVal = 0.0;
                        }
                    }

                    DataColumn newColumn = new DataColumn(colName, columnType);
                    newColumn.DefaultValue = defaultVal;
                    this.Columns.Add(newColumn);

                }

                this.Columns.Add("QueryCount", typeof(int));
                this.Columns["QueryCount"].DefaultValue = 1;
                this.QueryCountCol = this.Columns.IndexOf("QueryCount");
                return secondRow;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting Table Schema: {ex.Message}");
            }
        }


    }

}
