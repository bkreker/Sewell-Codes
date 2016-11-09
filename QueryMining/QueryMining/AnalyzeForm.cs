using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryMining
{
    public partial class AnalyzeForm : Form
    {
        StringWriter _outPutStringStream;
        int _wordColumn = -1,
            _queryColumn = -1;
        bool _processing = false,
            _operationCancelled = false,
            _outFileSavedCorrectly = false;
        string _outFileName = "default.csv";
        private StatDataTable _dataTable;

        public AnalyzeForm()
        {
            InitializeComponent();
        }

        /*public AnalyzeForm(StringWriter outPutStringStream, int wordColumn, int queryColumn) : this()
        {
            _outPutStringStream = outPutStringStream;
            _wordColumn = wordColumn;
            _queryColumn = queryColumn;
            try
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;

                _processing = true;
                backgroundWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something Went Wrong.");
            }
        }
        */

        public AnalyzeForm(StatDataTable dataTable, int wordColumn, int queryColumn, string outFileName) : this()
        {
            this._outFileName = outFileName;
            this._dataTable = dataTable;
            //   this._wordColumn = dataTable.WordCol;
            this._queryColumn = dataTable.QueryCol;
            try
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;
                dgvResults.DataSource = this._dataTable;
                _processing = true;
                backgroundWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something Went Wrong.");
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Processing Data...");
            _processing = true;
            try
            {
                Analyze();
            }
            catch (Exception ex)
            {
                _processing = false;
                MessageBox.Show(ex.Message, "Something went wrong while running the application");
            }

        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Worker completed");

            _processing = false;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.MarqueeAnimationSpeed = 0;

            if (!_operationCancelled)
            {
                // AddToDataGridView(ref _dataDictionary);
                AddToDataGridView(ref _dataTable);
                if (dgvResults.Rows.Count > 0)
                {
                    MessageBox.Show("Finished!");

                }
                else
                {
                    MessageBox.Show("Something went wrong populating the table.");
                }

            }
            else if (_operationCancelled)
            {
                MessageBox.Show("The new file was not saved");
            }
        }

        private void SaveData()
        {
            try
            {

                StreamWriter outFile = new StreamWriter(_outFileName);
                outFile.WriteLine(string.Join(",", _dataTable.Headers));
                foreach (DataRow row in _dataTable.Rows)
                {
                    outFile.WriteLine(string.Join(",", row.ItemArray));
                }
                outFile.Close();
                _outFileSavedCorrectly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving results.");
            }
        }

        private void Analyze()
        {
            Console.WriteLine("Sort Started.");

            try
            {
                var tableRows = (from DataRow row in _dataTable.Rows
                                 select row.ItemArray).ToList();

                var checkedPairs = new Dictionary<string, List<object>>();
                // instead, do a search where you take each query, check each combination of words in that query
                // against every other query in the list, then add those results.

                // for each row
                foreach (object[] fullRow in tableRows)
                {
                    string query = fullRow[_queryColumn].ToString();
                    var queryWords = query.Split(' ').ToList();
                    // for each pairing in the query in row i
                    for (int queryNum = 0; queryNum < queryWords.Count; queryNum++)
                    {
                        string word1 = queryWords[queryNum];

                        for (int wordNum = 0; wordNum < queryWords.Count; wordNum++)
                        {
                            string word2 = queryWords[wordNum];

                            if (word1 != word2)
                            {
                                MineWords(ref checkedPairs, query, new string[] { word1, word2 });
                                MineWords(ref checkedPairs, query, new string[] { word1, "" });
                                MineWords(ref checkedPairs, query, new string[] { word2, "" });

                            } // end if word1 != word2

                        } // end querywords loop 2


                    } // end querywords loop 1 
                } // end rows loop
                foreach (List<object> item in checkedPairs.Values)
                {
                    _dataTable.Rows.Add(item.ToArray());
                }
                Console.WriteLine("Sort Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Calculate the results for individual words and word pairs
        /// </summary>
        /// <param name="checkedPairs"></param>
        /// <param name="pairingList"></param>
        /// <param name="query"></param>
        /// <param name="words"></param>
        private void MineWords(ref Dictionary<string, List<object>> checkedPairs, string query, string[] words)
        {
            if (words[1] == "")
            {
                var pairingList = new object[_dataTable.Columns.Count];
                string word1 = words[0];
                if (!checkedPairs.ContainsKey(word1))
                {
                    try
                    {
                        pairingList[_queryColumn] = word1;
                        AggregateWordColumns(_dataTable.Columns, query, ref words, ref pairingList);
                        checkedPairs[word1] = pairingList.ToList();
                        _dataTable.Rows.Add(pairingList.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error Querying for word pair: {ex.Message}");
                    } // end try/catch
                }
            }
            else
            {
                var pairingList = new object[_dataTable.Columns.Count];
                string wordString = words[0] + " " + words[1];
                string reverseWords = words[1] + " " + words[0];

                if (!checkedPairs.ContainsKey(wordString) && !checkedPairs.ContainsKey(reverseWords))
                {
                    try
                    {
                        pairingList[_queryColumn] = wordString;
                        AggregateWordColumns(_dataTable.Columns, query, ref words, ref pairingList);
                        checkedPairs[wordString] = pairingList.ToList();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error Querying for word pair: {ex.Message}");
                    } // end try/catch
                }

            }
        }

        /// <summary>
        /// Aggregates all rows of the given criteria
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="query"></param>
        /// <param name="pair"></param>
        /// <param name="pairingList"></param>
        private void AggregateWordColumns(DataColumnCollection columns, string query, ref string[] pair, ref object[] pairingList)
        {
            string word1 = pair[0], word2 = pair[1];
            List<List<object>> results = new List<List<object>>();
            if (word2 == "")
            {
                results = (from DataRow row in _dataTable.Rows
                           where row.ItemArray[_queryColumn].ToString().Contains(word1)
                           select row.ItemArray.ToList()).ToList();
            }
            else
            {
                results = (from DataRow row in _dataTable.Rows
                           where row.ItemArray[_queryColumn].ToString().Contains(word1)
                           && row.ItemArray[_queryColumn].ToString().Contains(word2)
                           select row.ItemArray.ToList()).ToList();
            }

            for (int col_index = 0; col_index < columns.Count; col_index++)
            {
                try
                {
                    object total = "N/A";
                    if (col_index == _queryColumn)
                    {
                        total = string.Join(" ", pair);
                    }
                    //else if (col_index == _wordColumn)
                    //{
                    //    total = word1 + " " + word2;
                    //}

                    else
                    {
                        List<object> columnValues = (from resRow in results
                                                     select resRow[col_index]).ToList();
                        if (col_index == _dataTable.QueryCountCol)
                        {
                            total = columnValues.Count();
                        }
                        else
                        {
                            Type colDataType = columns[col_index].DataType;
                            string colName = columns[col_index].Caption;
                            if (Regexes.MatchesAnyStat(colName))
                            {
                                bool isAvg = (Regexes.IsMatch(colName, Regexes.Average));
                                total = AggregateColumnValues(columnValues, isAvg, columns[col_index]);
                            }
                            else
                            {
                                if (columnValues.Any(a => Regexes.IsMatch(a.ToString(), Regexes.Number)) && colDataType == typeof(string))
                                {
                                    columnValues.ForEach(val => val = decimal.Parse(Regexes.Match(val.ToString(), Regexes.Number)));
                                    bool isAvg = columnValues.Any(a => Regexes.IsMatch(a.ToString(), @".*\%.*") || Regexes.IsMatch(colName, Regexes.Average));
                                    total = AggregateColumnValues(columnValues, isAvg, columns[col_index]).ToString();
                                }
                                else
                                {
                                    total = columnValues.Aggregate((sum, next) =>
                                    {
                                        string s = sum.ToString() + " " + next.ToString();
                                        return s;
                                    });
                                }
                            }
                        }
                    }
                    pairingList[col_index] = total;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Aggregating Column Values: {ex.Message}");
                }
            } // end for

        }

        /// <summary>
        /// Aggregates all the values in a single column
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="isAvg"></param>
        /// <returns></returns>
        private object AggregateColumnValues(List<object> columnValues, bool isAvg, DataColumn column)
        {
            try
            {
                if (columnValues.All(item => item.ToString() == "0"))
                {
                    return 0;
                }
                bool colValsAreNumbers = columnValues.All(val => Regexes.IsMatch(val.ToString(), Regexes.Number));
                if (Regexes.MatchesAnyStat(column.Caption) && colValsAreNumbers)
                {
                    if (isAvg)
                    {
                        return columnValues.Aggregate((sum, next) =>
                        {
                            string sumString = sum.ToString();
                            string nextString = next.ToString();
                            string sumMatch = Regexes.Match(sumString, Regexes.Number);
                            string nextMatch = Regexes.Match(nextString, Regexes.Number);
                            if (sum == next)
                            {
                                return sum;
                            }
                            if (column.DataType == typeof(decimal))
                            {
                                decimal sumNum, nextNum;
                                if (decimal.TryParse(nextMatch, out nextNum) && decimal.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            else if (column.DataType == typeof(double))
                            {
                                double sumNum, nextNum;
                                if (double.TryParse(nextMatch, out nextNum) && double.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            else if (column.DataType == typeof(long))
                            {
                                long sumNum, nextNum;
                                if (long.TryParse(nextMatch, out nextNum) && long.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            else
                            {
                                float sumNum, nextNum;
                                if (float.TryParse(nextMatch, out nextNum) && float.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            return sum;
                        });
                    }
                    else
                    {
                        var nonZeroRows = columnValues.Where(val => val.ToString() != "0" && val.ToString() != "0.00");
                        return nonZeroRows.Aggregate((sum, next) =>
                        {
                            string sumString = sum.ToString();
                            string nextString = next.ToString();
                            string sumMatch = Regexes.Match(sumString, Regexes.Number);
                            string nextMatch = Regexes.Match(nextString, Regexes.Number);
                            if (sum == next)
                            {
                                return sum;
                            }
                            if (column.DataType == typeof(decimal))
                            {
                                decimal sumNum, nextNum;
                                if (decimal.TryParse(nextMatch, out nextNum) && decimal.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }
                            }
                            else if (column.DataType == typeof(double))
                            {
                                double sumNum, nextNum;
                                if (double.TryParse(nextMatch, out nextNum) && double.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }
                            }
                            else if (column.DataType == typeof(long))
                            {
                                long sumNum, nextNum;
                                if (long.TryParse(nextMatch, out nextNum) && long.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }
                            }
                            else
                            {
                                float sumNum, nextNum;
                                if (float.TryParse(nextMatch, out nextNum) && float.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }

                            }
                            return sum;
                        });
                    }
                }
                else
                {
                    return columnValues.FirstOrDefault();
                } // end if/else for if all elements are numbers
                //   return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Aggregating values: {ex.Message}");
                return "Error";
            }
        }

        /*private void Calculate()
        {
            Console.WriteLine("Sort Started.");
            _dataDictionary = new StatsTable(ref _outPutStringStream);
            //  AddToDataGridView(ref data);
            try
            {
                foreach (QueryWord word1 in _dataDictionary.Values)
                {
                    var word1Row = word1.Rows;

                    foreach (QueryWord word2 in _dataDictionary.Values)
                    {
                        if (word2 != word1)
                        {
                            foreach (QueryWord word3 in _dataDictionary.Values)
                            {
                                if (word3 != word1)
                                {
                                    if (word3.Query.Contains(word1.Word) && word3.Query.Contains(word2.Word) && word3 != word2)
                                    {
                                        QueryWord newWord = word2 + word1;
                                        Console.WriteLine(newWord.Word);
                                        if (!_minedQueries.ContainsKey(word1.Word + " " + word2.Word) && !_minedQueries.ContainsKey(newWord.Word))
                                        {
                                            _minedQueries[newWord.Word] = newWord;
                                            //   AddToDataGridView(newWord);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in _minedQueries)
                {
                    _dataDictionary.Add(item.Key, item.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went wrong.");
            }

            //   Analyze();
        }
        */

        private void AddToDataGridView(ref StatDataTable data)
        {
            try
            {
                dgvResults.DataSource = data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Filling table: {ex.Message}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
                if (_outFileSavedCorrectly == true)
                {
                    MessageBox.Show($"File saved at {_outFileName}", "Success!");
                }
                else
                {
                    MessageBox.Show($"Couldn't save the file.", "Something happened");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving Results.");
            }

        }
    }

}
