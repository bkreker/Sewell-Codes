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
using QueryMining.Properties;

namespace QueryMining
{
    public partial class AnalyzeForm : Form
    {
        int _queryCountColIndex = -1,
            _queryColIndex = -1;

        enum MineType { One, Two, Three }
        private MineType _mineType = MineType.Two;

        bool _avgAllValues = true,
            _processing = false,
            _operationCancelled = false,
            _outFileSavedCorrectly = false;

        string _outFileName = "default.csv";
        private StatDataTable _dataTable;
        private DataGridViewColumn QueryColumn { get { return dgvResults.Columns[_queryColIndex]; } }
        private DataGridViewColumn QueryCountColumn
        {
            get
            {
                return dgvResults.Columns[_queryCountColIndex];
            }
        }

        public AnalyzeForm()
        {
            InitializeComponent();
            tsmiMine1Word.Tag = MineType.One;
            tsmiMine2Words.Tag = MineType.Two;
            tsmiMine3Words.Tag = MineType.Three;
        }

        private void resetLblRowCount()
        {
            try
            {
                if (lblRowCount.InvokeRequired)
                {
                    lblRowCount.Invoke(new MethodInvoker(delegate { lblRowCount.Text = dgvResults.RowCount.ToString(); }));
                }
                else
                {
                    lblRowCount.Text = dgvResults.RowCount.ToString();
                }

                if (dgvResults.InvokeRequired)
                {
                    dgvResults.Invoke(new MethodInvoker(delegate { dgvResults.Refresh(); }));
                }
                else
                {
                    dgvResults.Refresh();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong Updating the Row Count:{ex.Message}");
            }
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Processing Data...");
            try
            {
                _processing = true;
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
                var existingKeys = (from DataRow r in _dataTable.Rows
                                    select r.ItemArray).ToList();
                existingKeys.ForEach(row => checkedPairs.Add(row[_queryColIndex].ToString(), row.ToList()));

                // instead, do a search where you take each query, check each combination of words in that query
                // against every other query in the list, then add those results.

                foreach (object[] fullRow in tableRows)
                {
                    if (_operationCancelled)
                        throw new OperationCanceledException();

                    string query = fullRow[_queryColIndex].ToString();
                    var queryWords = query.Split(' ').ToList();
                    // for each pairing in the query in row i
                    for (int queryNum = 0; queryNum < queryWords.Count; queryNum++)
                    {
                        string word1 = queryWords[queryNum];
                        MineWords(ref checkedPairs, query, word1);
                        if (_mineType != MineType.One)
                        {
                            for (int wordNum = 1; wordNum < queryWords.Count; wordNum++)
                            {
                                if (_operationCancelled)
                                    throw new OperationCanceledException();

                                string word2 = queryWords[wordNum];
                                if (word1 != word2)
                                {
                                    MineWords(ref checkedPairs, query, word2);
                                    if (_mineType != MineType.Three)
                                    {
                                        MineWords(ref checkedPairs, query, word1, word2);
                                    }
                                    else
                                    {
                                        for (int word3Num = 3; wordNum < queryWords.Count; wordNum++)
                                        {
                                            string word3 = queryWords[word3Num];
                                            if (word1 != word3 && word3 != word2)
                                            {
                                                MineWords(ref checkedPairs, query, word1, word2, word3);
                                                MineWords(ref checkedPairs, query, word1, word2);
                                                MineWords(ref checkedPairs, query, word1, word3);

                                            } // end if word1 != word2
                                        }

                                    }
                                }
                                resetLblRowCount();
                            } // end querywords loop 2     
                            resetLblRowCount();

                        } // end querywords loop 1 
                        resetLblRowCount();
                    } // end rows loop
                    resetLblRowCount();
                }
                Console.WriteLine("Sort Finished");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Operation Cancelled by user.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            try
            {
                var checkedPairs = new Dictionary<string, List<object>>();
                var existingKeys = (from DataRow r in _dataTable.Rows
                                    select r.ItemArray).ToList();
                existingKeys.ForEach(row => checkedPairs.Add(row[_queryColIndex].ToString(), row.ToList()));

                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;
                backgroundWorker.RunWorkerAsync();

            }

            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculate the Results for 1, 2, or 3 words
        /// </summary>
        /// <param name="checkedPairs"></param>
        /// <param name="query"></param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        private void MineWords(ref Dictionary<string, List<object>> checkedPairs, string query, string word1, string word2 = "", string word3 = "")
        {
            if (_operationCancelled)
                throw new OperationCanceledException();

            string wordString = string.Join(" ", new string[] { word1, word2, word3 }).Trim();
            string reverseWords = string.Join(" ", new string[] { word3, word2, word1 }).Trim();
            object[] newRow = new object[_dataTable.Columns.Count];

            if (word1 != "")
            {
                try
                {
                    var existingKeys = (from pair in checkedPairs
                                        where pair.Key == wordString
                                        select pair.Key).ToList();
                    var existingRows = (from DataRow row in _dataTable.Rows
                                        where row.ItemArray[_queryColIndex].ToString().Contains(word1)
                                            && row.ItemArray[_queryColIndex].ToString().Contains(word2)
                                            && row.ItemArray[_queryColIndex].ToString().Contains(word3)
                                        select row.ItemArray).ToList();

                    AggregateRows(query, wordString, ref newRow, ref existingRows);
                    checkedPairs[wordString] = newRow.ToList();

                    if (existingKeys.Count > 0)
                    {
                        _dataTable.AddRowToTable(newRow, existingKeys);
                    }
                    else
                    {
                        _dataTable.AddRowToTable(newRow);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Querying for word pair: {ex.Message}");
                }// end try/catch
            }
        }

        /// <summary>
        /// Aggregates all rows of the given criteria
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="query"></param>
        /// <param name="wordPair"></param>
        /// <param name="newRow"></param>
        private void AggregateRows(string query, string wordString, ref object[] newRow, ref List<object[]> existingRows/*, string word1, string word2 = "", string word3 = ""*/)
        {
            if (_operationCancelled)
                throw new OperationCanceledException();

            //   List<List<object>> results = new List<List<object>>();

            for (int columnIndex = 0; columnIndex < dgvResults.ColumnCount; columnIndex++)
            {
                if (_operationCancelled)
                    throw new OperationCanceledException();

                try
                {
                    object total = "N/A";
                    if (columnIndex == _queryColIndex)
                    {
                        total = query == wordString ? query : wordString;
                    }
                    else
                    {
                        List<object> columnValues = (from resRow in existingRows
                                                     select resRow[columnIndex]).ToList();
                        total = AggregateRowColumnValues(columnValues, columnIndex);
                    }
                    newRow[columnIndex] = total;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Aggregating Column Values: {ex.Message}");
                }
            } // end for

        }

        private object AggregateRowColumnValues(List<object> columnValues, int col_index)
        {
            if (col_index == this._queryCountColIndex)
            {
                return columnValues.Count();
            }
            else
            {
                Type colDataType = _dataTable.Columns[col_index].DataType;
                string colName = _dataTable.Columns[col_index].Caption;
                if (Regexes.MatchesAnyStat(colName))
                {
                    bool isAvg = Regexes.IsMatch(colName, Regexes.Average) || _avgAllValues;
                    return AggregateSingleColumnValues(columnValues, isAvg, _dataTable.Columns[col_index]);
                }
                else
                {
                    if (columnValues.Any(a => Regexes.IsMatch(a.ToString(), Regexes.Number)) && colDataType == typeof(string))
                    {
                        columnValues.ForEach(val => val = decimal.Parse(Regexes.Match(val.ToString(), Regexes.Number)));
                        bool isAvg = columnValues.Any(a => Regexes.IsMatch(a.ToString(), Regexes.Percent) || Regexes.IsMatch(colName, Regexes.Average)) || _avgAllValues;

                        return AggregateSingleColumnValues(columnValues, (isAvg || _avgAllValues), _dataTable.Columns[col_index]).ToString();
                    }
                    else
                    {
                        return columnValues.Aggregate((sum, next) =>
                         {
                             string s = sum.ToString() + " " + next.ToString();
                             return s;
                         });
                    }
                }

            }
        }

        /// <summary>
        /// Aggregates all the values in a single column
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="isAvg"></param>
        /// <returns></returns>
        private object AggregateSingleColumnValues(List<object> columnValues, bool isAvg, DataColumn column)
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

        private void outFileDialog_FileOk(object sender, CancelEventArgs e)
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

        private void SetMineType(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem btn = sender as ToolStripMenuItem;
                foreach (ToolStripMenuItem item in mineQueriesToolStripMenuItem.DropDownItems)
                {
                    if (item != btn)
                    {
                        item.Checked = false;
                    }
                    else
                    {
                        item.Checked = true;
                    }
                }
                _mineType = (MineType)btn.Tag;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went Wrong setting the Mine Type, setting it to Mine 2 Words");
                _mineType = MineType.Two;
            }
            //MineType type = btn.Tag as MineType;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox s = new AboutBox();
            s.ShowDialog();

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_processing)
            {
                DialogResult result = MessageBox.Show("Cancel Analysis?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    _operationCancelled = true;
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportForm form = new ImportForm();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                this._avgAllValues = form.AvgAllValues;
                this._dataTable = form.DataTable;
                this._queryColIndex = _dataTable.QueryCol;
                this._queryCountColIndex = _dataTable.QueryCountCol;
                try
                {
                    dgvResults.DataSource = this._dataTable;
                    dgvResults.Sort(dgvResults.Columns[_dataTable.QueryCountCol], ListSortDirection.Descending);
                    // dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    // dgvResults.AllowUserToResizeColumns = true;

                    lblRowCount.Text = dgvResults.RowCount.ToString();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something Went Wrong.");
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (_processing)
                {
                    DialogResult result = MessageBox.Show("End Analysis and Export Existing Data?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        _operationCancelled = true;
                        outFileDialog.ShowDialog();
                    }
                }
                else
                {
                    outFileDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Exporting Results.");
            }
        }
    }
}
