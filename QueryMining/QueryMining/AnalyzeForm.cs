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
            _operationCancelled = false;
        //  Dictionary<string, List<List<double>>> _dataDictionary = new Dictionary<string, List<List<double>>>();

        StatsTable _dataDictionary = new StatsTable();
        StatsTable _minedQueries = new StatsTable();
        private StatDataTable _dataTable;

        public AnalyzeForm()
        {
            InitializeComponent();
        }

        public AnalyzeForm(StringWriter outPutStringStream, int wordColumn, int queryColumn) : this()
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

        public AnalyzeForm(StatDataTable dataTable, int wordColumn, int queryColumn) : this()
        {
            this._dataTable = dataTable;
            this._wordColumn = dataTable.WordCol;
            this._queryColumn = dataTable.QueryCol;
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

        private void Analyze()
        {
            //Console.WriteLine("Analyze Started.");
            //dgvResults.DataSource = (from a in _dataDictionary
            //                         select a).ToList();
            //Console.WriteLine("Analyze Started.");
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Processing Data...");
            try
            {
                CalculateFromDataTable();
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
                AddToDataGridView(ref _dataDictionary);
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

        private struct QWord
        {
            public string Word { get; set; }
            public string Query { get; set; }
            public QWord(string w, string q, DataRow row)
            {
                this.Word = w;
                this.Query = q;
                List<object> objList = (from i in row.ItemArray
                                        select i).ToList();
                this.FullRow = objList;
            }
            public QWord(string w, string q, List<object> row)
            {
                this.Word = w;
                this.Query = q;
                this.FullRow = row;
            }

            public List<object> FullRow { get; set; }

            static public bool operator ==(QWord Left, QWord Right)
            {
                return (Left.Word == Right.Word && Left.Query == Right.Query);
            }

            static public bool operator !=(QWord Left, QWord Right)
            {
                return (Left.Query != Right.Query && Left.Word != Right.Word);
            }

            public QWord Add(QWord Right, DataColumnCollection columns)
            {
                QWord result = new QWord();
                object[] resultList = new object[this.FullRow.Count];
                result.Word = this.Word + " " + Right.Word;
                result.Query = this.Query + " / " + Right.Query;
                if (Right != this)
                {
                    for (int i = 0; i < this.FullRow.Count && i < Right.FullRow.Count; i++)
                    {
                        Type colType = columns[i].DataType;
                        string colName = columns[i].Caption;
                        try
                        {
                            bool isAvg = Regex.IsMatch(columns[i].Caption, Regexes.Average);
                            object celval = "N/A";
                            if (colType == typeof(decimal))
                            {
                                decimal leftNum = (decimal)this.FullRow[i];
                                decimal rightNum = (decimal)Right.FullRow[i];

                                if (isAvg)
                                {
                                    celval = (leftNum + rightNum) / 2;
                                }
                                else
                                {
                                    celval = leftNum + rightNum;
                                }
                            }
                            else if (colType == typeof(double))
                            {
                                double leftNum = (double)this.FullRow[i];
                                double rightNum = (double)Right.FullRow[i];

                                if (isAvg)
                                {
                                    celval = (leftNum + rightNum) / 2;
                                }
                                else
                                {
                                    celval = leftNum + rightNum;
                                }

                            }
                            else if (colType == typeof(string))
                            {
                                string leftVal = this.FullRow[i].ToString();
                                string rightval = Right.FullRow[i].ToString();

                                if (Regex.IsMatch(colName, Regexes.Query))
                                {
                                    celval = leftVal + " / " + rightval;
                                }
                                else if (Regex.IsMatch(leftVal, Regexes.Number) && Regex.IsMatch(rightval, Regexes.Number))
                                {
                                    celval = (decimal)this.FullRow[i] + (decimal)Right.FullRow[i];
                                }
                                else
                                {
                                    celval = leftVal + " " + rightval;
                                }
                            }

                            resultList[i] = celval;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error adding QWords: {ex.Message}");
                        }
                    }
                    result.FullRow = resultList.ToList();

                    return result;
                }
                else
                {
                    return this;
                }
            }
        }

        private void CalculateFromDataTable()
        {
            Console.WriteLine("Sort Started.");

            try
            {
                var checkedPairs = new Dictionary<string, List<object>>();
                var set1 = (from DataRow row in _dataTable.Rows
                            select row.ItemArray).ToList();

                // instead, do a search where you take each query, check each combination of words in that query
                // against every other query in the list, then add those results.

                // for each row
                foreach (object[] fullRow in set1)
                {
                    string query = fullRow[_queryColumn].ToString();
                    var queryWords = query.Split(' ').ToList();
                    // for each pairing in the query in row i
                    for (int i = 0; i < queryWords.Count; i++)
                    {
                        string word1 = queryWords[i];
                        for (int j = 0; j < queryWords.Count; j++)
                        {
                            string word2 = queryWords[j];

                            if (word1 != word2)
                            {
                                var pairingList = new object[_dataTable.Columns.Count];
                                string[] pair = { word1, word2 };
                                string words = word1 + " " + word2;
                                string reverseWords = word2 + " " + word1;
                                if (!checkedPairs.ContainsKey(reverseWords) && !checkedPairs.ContainsKey(words))
                                {
                                    try
                                    {
                                        var results = (from DataRow row in _dataTable.Rows
                                                       where row.ItemArray[_queryColumn].ToString().Contains(word1)
                                                       && row.ItemArray[_queryColumn].ToString().Contains(word2)
                                                       select row.ItemArray.ToList()).ToList();
                                        SumColumns(results, _dataTable.Columns, query, ref pair, ref pairingList);

                                        checkedPairs[words] = pairingList.ToList();

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error Querying for word pair: {ex.Message}");
                                    } // end try/catch
                                } // end if word pair already has been analyzed
                            } // end if word1 != word2
                        } // end querywords loop 2

                    } // end querywords loop 1 
                } // end rows loop
                foreach (var item in checkedPairs.Values)
                {
                    _dataTable.Rows.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void SumColumns(List<List<object>> results, DataColumnCollection columns, string query, ref string[] pair,
            ref object[] pairingList)// ref Dictionary<string, List<object>> checkedPairs)
        {
            string word1 = pair[0], word2 = pair[1];
            for (int col_index = 0; col_index < columns.Count; col_index++)
            {
                try
                {
                    object total = "N/A";
                    if (col_index == _queryColumn)
                    {
                        total = query;
                    }
                    else if (col_index == _wordColumn)
                    {
                        total = word1 + " " + word2;
                    }
                    else
                    {
                        var columnValues = (from resRow in results
                                            select resRow[col_index]).ToList();
                        Type dataType = columns[col_index].DataType;
                        string colName = columns[col_index].Caption;
                        if (dataType == typeof(decimal) || dataType == typeof(double))
                        {
                            bool isAvg = (Regex.IsMatch(colName, Regexes.Average));
                            total = (decimal)AggregateValues(columnValues, isAvg);
                        }
                        else
                        {
                            if (Regex.IsMatch(columnValues[col_index].ToString(), Regexes.Number))
                            {
                                columnValues.ForEach(val => val = decimal.Parse(Regex.Match(val.ToString(), Regexes.Number).ToString()));
                                bool isAvg = columnValues.Any(a => Regex.IsMatch(a.ToString(), @".\%."));
                                total = (decimal)AggregateValues(columnValues, isAvg);
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
                    pairingList[col_index] = total;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Aggregating Column Values: {ex.Message}");
                }
            } // end for
        }

        private object AggregateValues(List<object> columnValues, bool isAvg)
        {
            try
            {
                var result = columnValues.Aggregate((sum, next) =>
                    {
                        decimal num;
                        if (Regex.IsMatch(next.ToString(), Regexes.Number))
                        {
                            next = Regex.Match(next.ToString(), Regexes.Number);
                        }
                        if (decimal.TryParse(next.ToString(), out num))
                        {
                            if (isAvg)
                            {
                                sum = ((decimal)sum + num) / 2;
                            }
                            else
                            {
                                sum = (decimal)sum + num;
                            }
                        }
                        return sum;
                    });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Aggregating values: {ex.Message}");
                return "Error";
            }
        }

        private void Calculate()
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

        }

        private void AddToDataGridView(ref StatsTable data)
        {
            try
            {
                dgvResults.DataSource = data.ToDataTable();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Filling table: {ex.Message}");
            }
        }
        private void AddToDataGridView(QueryWord newWord)
        {
            throw new NotImplementedException();
        }
    }

}
