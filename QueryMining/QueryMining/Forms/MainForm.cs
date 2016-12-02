using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QueryMining.Properties;

namespace QueryMining
{
    public partial class MainForm : Form
    {
        enum MineType { One, Two, Three }
        private MineType _mineType = MineType.Two;

        private static bool
              _outFileSavedCorrectly = false;

        public static Dictionary<string, string> CheckedKeys { get; set; }

        private static bool _avgAll
        {
            get { return Program.AvgAll; }
            set { Program.AvgAll = value; }
        }
        private static bool _processing
        {
            get { return Program.Processing; }
            set { Program.Processing = value; }
        }
        private static bool _operationCancelled
        {
            get { return Program.OperationCancelled; }
            set { Program.OperationCancelled = value; }
        }
        Queue<String> FullQueries = new Queue<string>();
        private static bool _tableChanged = false;
        string _outFileName = "default.csv";

        private StatDataTable _dataTable;

        private DataColumnCollection Columns { get { return StatDataTable.ColumnCollection; } }
        private int QueryColIndex { get { return StatDataTable.QueryCol; } }
        private int QueryCountIndex { get { return StatDataTable.QueryCountCol; } }
        private List<string> Headers { get { return StatDataTable.Headers; } }

        public MainForm()
        {
            InitializeComponent();

            CheckedKeys = new Dictionary<string, string>();
            tsmiMine1Word.Tag = MineType.One;
            tsmiMine2Words.Tag = MineType.Two;
            tsmiMine3Words.Tag = MineType.Three;
            tsmiMine2Words.Checked = true;

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportForm form = new ImportForm();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                this._dataTable = form.DataTable;
                try
                {
                    dgvMineResults.DataSource = this._dataTable;
                    lblRowCount.Text = dgvMineResults.RowCount.ToString();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something Went Wrong.");
                }
            }
        }


        private void PerformBackgroundWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Processing Data...");
            try
            {
                Program.Processing = true;
                Analyze();
            }
            catch (Exception ex)
            {
                Program.Processing = false;
                MessageBox.Show(ex.Message, "Something went wrong while running the application");
            }

        }

        private void BackgroundWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Worker completed");
            Program.Processing = false;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.MarqueeAnimationSpeed = 0;
            btnCancel.Enabled = false;
            if (!Program.OperationCancelled)
            {
                if (dgvMineResults.Rows.Count > 0)
                    MessageBox.Show("Finished!");
                else
                    MessageBox.Show("Something went wrong populating the table.");
            }
            else if (Program.OperationCancelled)
            {
                MessageBox.Show("The new file was not saved");
            }
            Console.WriteLine("Checked Keys:");
            foreach (var key in CheckedKeys)
            {
                Console.WriteLine(key.Key + " " + key.Value);
            }

            Console.WriteLine("Rows in data table:");
            int i = 0;
            foreach (DataRow row in _dataTable.Rows)
            {
                Console.WriteLine($"{i++} {row.ItemArray[QueryColIndex]}");

            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            try
            {
                //_mineType = MineType.Three;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;
                btnCancel.Enabled = true;
                Program.Processing = true;
                Thread cancellationChecker = new Thread(new ThreadStart(ThrowIfCancelled));
                Thread displayUpdater = new Thread(new ThreadStart(resetLblRowCountAsync));
                cancellationChecker.Start();
                displayUpdater.Start();
                backgroundWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                backgroundWorker.CancelAsync();
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        private async void Analyze()
        {
            Console.WriteLine("Sort Started.");
            try
            {
                CheckedKeys = new Dictionary<string, string>();

                foreach (var query in (from DataRow r in _dataTable.Rows select r.ItemArray[QueryColIndex].ToString()))
                {
                    FullQueries.Enqueue(query);
                    CheckedKeys.Add(query, query.Split(' ').Reverse().ToString());
                }
                // Take each query, check each combination of words in that query
                // against every other query in the list, then add those results.
                for (int i = 0; i < FullQueries.Count; i++)
                {
                    Thread thread1, thread2, threadd3, thread4, thread5, thread6;
                    string query = FullQueries.Dequeue();
                    var queryWords = query.Split(' ').ToList();
                    // for each pairing in the query in row i
                    for (int word1Num = 0; word1Num < queryWords.Count; word1Num++)
                    {
                        string word1 = queryWords[word1Num];
                        MineWords(word1);

                        if (_mineType == MineType.One)
                            continue;

                        else
                        {
                            for (int word2Num = word1Num + 1; word2Num < queryWords.Count; word2Num++)
                            {
                                string word2 = queryWords[word2Num];
                                if (word1 != word2)
                                {
                                    MineWords(word2);
                                    MineWords(word1, word2);

                                    if (_mineType == MineType.Three)
                                    {
                                        for (int word3Num = word2Num + 1; word3Num < queryWords.Count; word3Num++)
                                        {
                                            string word3 = queryWords[word3Num];
                                            if (word1 != word3 && word2 != word3)
                                            {
                                                try
                                                {
                                                    thread4 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word2, word3); }));
                                                    thread5 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word1, word3); }));
                                                    thread6 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word1, word2, word3); }));
                                                    thread4.Name = "thread4"; thread5.Name = "thread5"; thread6.Name = "thread6";
                                                    thread4.Start();
                                                    thread6.Start();
                                                    thread5.Start();

                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.Message);
                                                }
                                                //await Task.Run(() => MineWords(word2, word3));
                                                //await Task.Run(() => MineWords(word1, word2, word3));

                                            } // end if word1 != word2
                                        }
                                    }
                                }
                            } // end querywords loop 2     

                        } // end if/else for minetype == One
                    } // end rows loop
                }
                Console.WriteLine("Finished Mining");
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

        private void ThrowIfCancelled()
        {
            try
            {
                while (Program.Processing)
                {
                    if (Program.OperationCancelled)
                        throw new OperationCanceledException();
                }
            }
            catch (OperationCanceledException)
            {
                Program.Processing = false;
                Program.OperationCancelled = false;
                MessageBox.Show("Operation Cancelled.");
            }
        }

        /// <summary>
        /// Calculate the Results for 1, 2, or 3 words
        /// </summary>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        private void MineWords(string word1, string word2 = "", string word3 = "")
        {
            // ThrowIfCancelled();

            string wordString = string.Join(" ", new string[] { word1.Trim(), word2.Trim(), word3.Trim() }).Trim();
            string reverseWords = wordString.Split(' ').Reverse().ToString();
            if (!KeyExists(wordString, reverseWords))
            {
                if (wordString != "" && !Regexes.IsExcluded(wordString))
                {
                    try
                    {
                        int existingKeys = (from pair in MainForm.CheckedKeys
                                            where pair.Key == wordString || pair.Key == reverseWords
                                            select pair.Key).Count();

                        if (existingKeys == 0)
                        {
                            var mineableRows = (from DataRow row in _dataTable.Rows
                                                where row.ItemArray[QueryColIndex].ToString().Contains(word1)
                                                    && row.ItemArray[QueryColIndex].ToString().Contains(word2)
                                                    && row.ItemArray[QueryColIndex].ToString().Contains(word3)
                                                select row).ToList();

                            object[] newRow;
                            if (mineableRows.Count > 1)
                            {
                                newRow = StatDataTable.Mine(wordString, mineableRows);
                                newRow[QueryColIndex] = wordString;
                            }
                            else
                            {
                                newRow = mineableRows[0].ItemArray;
                            }
                            _dataTable.AddRowToTable(newRow);
                            _tableChanged = true;
                        }
                        MainForm.CheckedKeys[wordString] = MainForm.CheckedKeys[reverseWords];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error Querying for word(s):{wordString} {ex.Message}");
                    }// end try/catch
                }
                else
                {
                    MainForm.CheckedKeys[wordString] = MainForm.CheckedKeys[reverseWords];

                }
            }
            else
            {
                //  Console.WriteLine("Key already checked.");
            }
        }

        private bool KeyExists(string wordString, string reverseWordString)
        {
            bool result = false;
            result = CheckedKeys.ContainsKey(wordString) || CheckedKeys.ContainsKey(reverseWordString) ||
                CheckedKeys.ContainsValue(wordString) || CheckedKeys.ContainsValue(reverseWordString);
            return result;

        }

        private void resetLblRowCountAsync()
        {
            while (Program.Processing)
            {
                if (_tableChanged)
                {
                    _tableChanged = false;
                    try
                    {

                        if (dgvMineResults.InvokeRequired)
                        {
                            dgvMineResults.Invoke(new MethodInvoker(delegate
                            {
                                dgvMineResults.DataSource = _dataTable;
                                //dgvMineResults.Sort(dgvMineResults.Columns[this.QueryCountIndex], ListSortDirection.Descending);
                                dgvMineResults.Refresh();
                            }));
                        }
                        else
                        {
                            dgvMineResults.DataSource = _dataTable;
                            dgvMineResults.Refresh();
                        }

                        if (lblRowCount.InvokeRequired)
                        {
                            lblRowCount.Invoke(new MethodInvoker(delegate
                            {
                                lblRowCount.Text = _dataTable.Rows.Count.ToString();
                            }));
                        }
                        else lblRowCount.Text = _dataTable.Rows.Count.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong Updating the Row Count:{ex.Message}");
                    }
                }
            }
        }

        private void outFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            _outFileName = outFileDialog.FileName;
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Program.OperationCancelled = true;
            Program.Processing = false;
        }

        private void dgvResults_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox s = new AboutBox();
            s.ShowDialog();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (Program.Processing)
            {
                DialogResult result = MessageBox.Show("Cancel Analysis?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Program.OperationCancelled = true;
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.Processing)
                {
                    DialogResult result = MessageBox.Show("End Analysis and Export Existing Data?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        Program.OperationCancelled = true;
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

        private void SaveData()
        {
            try
            {
                StreamWriter outFile = new StreamWriter(_outFileName);
                outFile.WriteLine(string.Join(",", this.Headers));
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

        private void tsmiAvgAll_Click(object sender, EventArgs e)
        {
            try
            {
                tsmiAvgAll.Checked = !tsmiAvgAll.Checked;
                _avgAll = tsmiAvgAll.Checked;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning avgAll: {ex.Message}");
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
                        item.Checked = false;

                    else
                        item.Checked = true;

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


    }
}
