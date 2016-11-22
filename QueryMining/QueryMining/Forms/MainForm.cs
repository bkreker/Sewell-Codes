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
    public partial class MainForm : Form
    {
        enum MineType { One, Two, Three }
        private MineType _mineType = MineType.Two;

        private static bool
              _outFileSavedCorrectly = false;

        public static Dictionary<string, bool> CheckedKeys { get; set; }

        private static bool AvgAllValues
        {
            get { return StatDataTable.AvgAll; }
            set { StatDataTable.AvgAll = value; }
        }

        string _outFileName = "default.csv";

        private StatDataTable
            _inFileTable,
            _outFileTable;

        private DataColumnCollection Columns { get { return StatDataTable.ColumnCollection; } }
        private int QueryColIndex { get { return StatDataTable.QueryCol; } }
        private int QueryCountIndex { get { return StatDataTable.QueryCountCol; } }
        private List<string> Headers { get { return StatDataTable.Headers; } }

        public MainForm()
        {
            InitializeComponent();
            CheckedKeys = new Dictionary<string, bool>();
            tsmiMine1Word.Tag = MineType.One;
            tsmiMine2Words.Tag = MineType.Two;
            tsmiMine3Words.Tag = MineType.Three;
            AvgAllValues = true;
            tsmiMine2Words.Checked = true;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportForm form = new ImportForm();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                this._inFileTable = form.DataTable;
                this._outFileTable = this._inFileTable;
                try
                {
                    dgvInFile.DataSource = this._inFileTable;
                    lblRowCount.Text = dgvInFile.RowCount.ToString();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something Went Wrong.");
                }
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

        private void PerformBackgroundWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Processing Data...");
            try
            {
                StatDataTable.Processing = true;
                Analyze();
            }
            catch (Exception ex)
            {
                StatDataTable.Processing = false;
                MessageBox.Show(ex.Message, "Something went wrong while running the application");
            }

        }

        private void BackgroundWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Worker completed");
            StatDataTable.Processing = false;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.MarqueeAnimationSpeed = 0;
            btnCancel.Enabled = false;
            if (!StatDataTable.OperationCancelled)
            {
                if (dgvInFile.Rows.Count > 0)
                {
                    MessageBox.Show("Finished!");

                }
                else
                {
                    MessageBox.Show("Something went wrong populating the table.");
                }

            }
            else if (StatDataTable.OperationCancelled)
            {
                MessageBox.Show("The new file was not saved");
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;
                btnCancel.Enabled = true;
                backgroundWorker.RunWorkerAsync();

            }

            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        private void Analyze()
        {
            Console.WriteLine("Sort Started.");

            try
            {
                MainForm.CheckedKeys = new Dictionary<string, bool>();
                var existingKeys = (from DataRow r in _inFileTable.Rows
                                    select r.ItemArray).ToList();

                existingKeys.ForEach(row => MainForm.CheckedKeys.Add(row[QueryColIndex].ToString(), true));

                // instead, do a search where you take each query, check each combination of words in that query
                // against every other query in the list, then add those results.
                List<object[]> tableRows = (from DataRow row in _inFileTable.Rows
                                            select row.ItemArray).ToList();
                foreach (object[] fullRow in tableRows)
                {
                    if (StatDataTable.OperationCancelled)
                        throw new OperationCanceledException();

                    string query = fullRow[QueryColIndex].ToString();
                    var queryWords = query.Split(' ').ToList();
                    // for each pairing in the query in row i
                    for (int word1Num = 0; word1Num < queryWords.Count; word1Num++)
                    {
                        string word1 = queryWords[word1Num];
                        MineWords(query, word1);

                        if (_mineType == MineType.One)
                            continue;

                        else
                        {
                            for (int word2Num = word1Num + 1; word2Num < queryWords.Count; word2Num++)
                            {
                                if (StatDataTable.OperationCancelled)
                                    throw new OperationCanceledException();

                                string word2 = queryWords[word2Num];
                                if (word1 != word2)
                                {
                                    MineWords(query, word2);

                                    if (_mineType == MineType.Two)
                                    {
                                        MineWords(query, word1, word2);
                                    }
                                    else if (_mineType == MineType.Three)
                                    {
                                        for (int word3Num = word2Num + 1; word2Num < queryWords.Count; word2Num++)
                                        {
                                            string word3 = queryWords[word3Num];
                                            if (word1 != word3 && word2 != word3)
                                            {
                                                MineWords(query, word1, word3);
                                                MineWords(query, word2, word3);
                                                MineWords(query, word1, word2, word3);

                                            } // end if word1 != word2
                                        }
                                    }
                                }
                                //resetLblRowCount();
                            } // end querywords loop 2     
                            resetLblRowCount();

                        } // end if/else for minetype == One
                        resetLblRowCount();
                    } // end rows loop
                    resetLblRowCount();
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

        /// <summary>
        /// Calculate the Results for 1, 2, or 3 words
        /// </summary>
        /// <param name="query"></param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        private void MineWords(string query, string word1, string word2 = "", string word3 = "")
        {
            if (StatDataTable.OperationCancelled)
                throw new OperationCanceledException();

            string wordString = string.Join(" ", new string[] { word1, word2, word3 }).Trim();
            string reverseWords = string.Join(" ", new string[] { word3, word2, word1 }).Trim();

            if (wordString != "")
            {
                try
                {
                    var existingKeys = (from pair in MainForm.CheckedKeys
                                        where pair.Key == wordString || pair.Key == reverseWords
                                        select pair.Key).ToList();

                    if (existingKeys.Count == 0)
                    {
                        var existingRows = (from DataRow row in _inFileTable.Rows
                                            where row.ItemArray[QueryColIndex].ToString().Contains(word1)
                                                && row.ItemArray[QueryColIndex].ToString().Contains(word2)
                                                && row.ItemArray[QueryColIndex].ToString().Contains(word3)
                                            select row).ToList();

                        object[] newRow;
                        if (existingKeys.Count > 1)
                        {
                            newRow = StatDataTable.Mine(query, wordString, existingRows);
                        }
                        else
                        {
                            newRow = existingRows[0].ItemArray;
                        }
                        _outFileTable.AddRowToTable(newRow, existingKeys, existingRows);
                        MainForm.CheckedKeys[wordString] = true;
                        MainForm.CheckedKeys[reverseWords] = true;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Querying for word(s):{wordString} {ex.Message}");
                }// end try/catch
            }
        }

        private void resetLblRowCount()
        {
            try
            {
                if (dgvOutFile.InvokeRequired)
                {
                    dgvOutFile.Invoke(new MethodInvoker(delegate
                    {
                        dgvOutFile.DataSource = _outFileTable;
                        dgvOutFile.Sort(dgvOutFile.Columns[this.QueryCountIndex], ListSortDirection.Descending);
                        dgvOutFile.Refresh();
                    }));
                }
                else
                {
                    dgvOutFile.Refresh();
                }
                if (lblRowCount.InvokeRequired)
                {
                    lblRowCount.Invoke(new MethodInvoker(delegate { lblRowCount.Text = _outFileTable.Rows.Count.ToString(); }));
                }
                else
                {
                    lblRowCount.Text = _outFileTable.Rows.Count.ToString();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong Updating the Row Count:{ex.Message}");
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
            StatDataTable.OperationCancelled = true;
            StatDataTable.Processing = false;
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
            if (StatDataTable.Processing)
            {
                DialogResult result = MessageBox.Show("Cancel Analysis?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    StatDataTable.OperationCancelled = true;
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
                if (StatDataTable.Processing)
                {
                    DialogResult result = MessageBox.Show("End Analysis and Export Existing Data?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        StatDataTable.OperationCancelled = true;
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
                foreach (DataRow row in _outFileTable.Rows)
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

    }
}
