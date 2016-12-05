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

namespace QueryMining.Forms
{
    public partial class MainForm : Form
    {
        private MineType _mineType
        {
            get { return Program.MineType; }
            set { Program.MineType = value; }
        }

        private static bool
              _outFileSavedCorrectly = false;

        public static Dictionary<string, bool> CheckedKeys { get; set; }

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
        private static bool _tableChanged
        {
            get { return StatDataTable.TableChanged; }
            set { StatDataTable.TableChanged = value; }
        }
        string _outFileName = "default.csv";

        private StatDataTable _dataTable;

        Thread cancellationChecker, displayUpdater;

        private DataColumnCollection Columns { get { return StatDataTable.ColumnCollection; } }
        private int QueryColIndex { get { return StatDataTable.QueryCol; } }
        private int QueryCountIndex { get { return StatDataTable.QueryCountCol; } }
        private List<string> Headers { get { return StatDataTable.Headers; } }

        public MainForm()
        {
            InitializeComponent();
            CheckedKeys = new Dictionary<string, bool>();
            dgvMineResults.DataError += DataError;

            cancellationChecker = new Thread(new ParameterizedThreadStart(delegate
            {
                ThrowIfCancelled(ref backgroundWorker);
            }));

            cancellationChecker.Name = "cancellationChecker";
            displayUpdater = new Thread(new ThreadStart(updateDisplayAsync)); displayUpdater.Name = "displayUpdater";

            cancellationChecker.Start();
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
                   // dgvMineResults.Sort(dgvMineResults.Columns[this.QueryCountIndex], ListSortDirection.Descending);
                    lblRowCount.Text = dgvMineResults.RowCount.ToString();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something Went Wrong.");
                }
            }
        }

        private void DataError(object dgv, DataGridViewDataErrorEventArgs e)
        {
            dgv = dgv as DataGridView;

            Console.WriteLine($"In column {StatDataTable.ColumnCollection[e.ColumnIndex].Caption}, row{e.RowIndex}");
            Console.WriteLine($"Context: {e.Context}\nMessage: {e.Exception.Message}\nTarget Site: {e.Exception.TargetSite}");
            Console.WriteLine($"StackTrace:\n{e.Exception.StackTrace}");
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
            lblRemRows.Visible = false;
            lblTitleRemRows.Visible = false;

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
            Console.WriteLine($"Keys Checked: {CheckedKeys.Count}");


            Console.WriteLine($"Rows in data table: {_dataTable.Rows.Count}");
        }

        private void tsmiMine_Click(object sender, EventArgs e)
        {
            try
            {
                MineTypeSelectorForm options = new Forms.MineTypeSelectorForm();
                if (options.ShowDialog() == DialogResult.OK)
                {
                    //_mineType = MineType.Three;
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar1.MarqueeAnimationSpeed = 10;

                    lblRemRows.Text = FullQueries.Count.ToString();
                    lblRemRows.Visible = true;
                    lblTitleRemRows.Visible = true;
                    btnCancel.Enabled = true;

                    Program.Processing = true;
                    Program.OperationCancelled = false;

                    //   displayUpdater.Start();
                    backgroundWorker.RunWorkerAsync();
                }
                else
                {
                    Console.WriteLine("Mining Cancelled");
                }

            }
            catch (Exception ex)
            {
                backgroundWorker.CancelAsync();
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        private void EmptyThread()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.Name} started before actually using anything.");
        }

        private string Pop(ref Queue<string> fullQueries)
        {
            string res = FullQueries.Dequeue();
            if (lblRemRows.InvokeRequired)
            {
                lblRemRows.Invoke(new MethodInvoker(delegate
              {
                  lblRemRows.Text = FullQueries.Count.ToString();
              }));

            }
            else
            {
                lblRemRows.Text = FullQueries.Count.ToString();
            }
            return res;
        }
        private void Analyze()
        {
            Console.WriteLine("Sort Started.");
            if (dgvMineResults.InvokeRequired)
            {
                dgvMineResults.Invoke(new MethodInvoker(delegate {
                    dgvMineResults.Sort(dgvMineResults.Columns[this.QueryCountIndex], ListSortDirection.Descending);
                }));
            }
            try
            {
                CheckedKeys = new Dictionary<string, bool>();

                foreach (var query in (from DataRow r in _dataTable.Rows select r.ItemArray[QueryColIndex].ToString()))
                {
                    FullQueries.Enqueue(query);
                    CheckedKeys.Add(query, true);
                }
                // Take each query, check each combination of words in that query
                // against every other query in the list, then add those results.
                for (int i = 0; i < FullQueries.Count; i++)
                {
                    Thread
                       thread1 = new Thread(EmptyThread), thread2 = new Thread(EmptyThread),
                       thread3 = new Thread(EmptyThread), thread4 = new Thread(EmptyThread),
                       thread5 = new Thread(EmptyThread), thread6 = new Thread(EmptyThread);

                    string query = Pop(ref FullQueries);

                    var queryWords = query.Split(' ').ToList();
                    // for each pairing in the query in row i
                    for (int word1Num = 0; word1Num < queryWords.Count; word1Num++)
                    {
                        string word1 = queryWords[word1Num];
                        thread1 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word1); }));
                        thread1.Name = "thread1";
                        thread1.Start();

                        if (_mineType == MineType.One)
                            continue;

                        else
                        {
                            for (int word2Num = word1Num + 1; word2Num < queryWords.Count; word2Num++)
                            {
                                string word2 = queryWords[word2Num];
                                if (word1 != word2)
                                {
                                    // MineWords(word2);
                                    thread2 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word2); })); thread2.Name = "thread2";
                                    thread3 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word1, word2); })); thread3.Name = "thread3";
                                    thread2.Start();
                                    thread3.Start();
                                    // MineWords(word1, word2);

                                    if (_mineType == MineType.Three)
                                    {
                                        for (int word3Num = word2Num + 1; word3Num < queryWords.Count; word3Num++)
                                        {
                                            string word3 = queryWords[word3Num];
                                            if (word1 != word3 && word2 != word3)
                                            {
                                                try
                                                {
                                                    thread4 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word2, word3); })); thread4.Name = "thread4";
                                                    thread5 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word1, word3); })); thread5.Name = "thread5";
                                                    thread6 = new Thread(new ParameterizedThreadStart(delegate { MineWords(word1, word2, word3); })); thread6.Name = "thread6";

                                                    thread4.Start();
                                                    thread6.Start();
                                                    thread5.Start();
                                                    while (thread4.IsAlive || thread5.IsAlive || thread6.IsAlive)
                                                    {
                                                        Thread.Sleep(1);
                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.Message);
                                                }
                                            } // end if word1 != word2
                                        }
                                    }
                                    while (thread2.IsAlive || thread3.IsAlive)
                                    {
                                        Thread.Sleep(1);
                                    }
                                    updateDisplay();
                                }
                            } // end querywords loop 2     

                        } // end if/else for minetype == One
                        while (thread1.IsAlive)
                        {
                            Thread.Sleep(1);
                        }
                        updateDisplay();
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
                Console.WriteLine(ex.Message, "Error");
            }
        }

        private static void ThrowIfCancelled(ref BackgroundWorker backgroundWorker)
        {
            try
            {
                while (Program.Processing)
                {
                    if (Program.OperationCancelled)
                    {
                        if (backgroundWorker.IsBusy)
                            backgroundWorker.CancelAsync();

                        throw new OperationCanceledException();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Program.Processing = false;
                //  Program.OperationCancelled = false;
                MessageBox.Show("Operation Cancelled.");
            }
        }

        private bool KeyExists(string word1, string word2, string word3 = "")
        {
            string wordString = string.Join(" ", new string[] { word1.Trim(), word2.Trim(), word3.Trim() }).Trim();
            return KeyExists(wordString);
        }

        private bool KeyExists(string wordString)
        {
            bool result = false;
            result = CheckedKeys.ContainsKey(wordString) || CheckedKeys.ContainsKey(string.Join(" ", wordString.Split(' ').Reverse()));
            return result;
        }


        /// <summary>
        /// Calculate the Results for 1, 2, or 3 words
        /// </summary>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        private void MineWords(string word1, string word2 = "", string word3 = "")
        {
            string wordString = string.Join(" ", new string[] { word1.Trim(), word2.Trim(), word3.Trim() }).Trim();
            string reverseWords = string.Join(" ", wordString.Split(' ').Reverse());
            if (KeyExists(wordString))
            {
                //    Console.WriteLine($"\t{Thread.CurrentThread.Name}: Key already present: {wordString} ");
                _tableChanged = false;
            }
            else if (wordString == "" || Regexes.IsExcluded(wordString))
            {
                //   Console.WriteLine($"\t{Thread.CurrentThread.Name}: Key excluded: {wordString} ");
                MainForm.CheckedKeys[wordString] = false;
                _tableChanged = false;
            }
            else
            {
                try
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
                    MainForm.CheckedKeys[wordString] = true;
                    Console.WriteLine($"\t{Thread.CurrentThread.Name} Finished, wordstring: {wordString}, tablechanged = {_tableChanged}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\t{Thread.CurrentThread.Name}: Error Querying for word(s): {wordString} {ex.Message}");
                }// end try/catch
            }

        }

        private void updateDisplayAsync()
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
                                lblRowCount.Text = StatDataTable.RowCount.ToString();
                            }));
                        }
                        else lblRowCount.Text = StatDataTable.RowCount.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong Updating the Row Count:{ex.Message}");
                    }
                }
            }
        }

        private void updateDisplay()
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
                           // dgvMineResults.DataSource = _dataTable;
                            dgvMineResults.Refresh();
                        }));
                    }
                    else
                    {
                       // dgvMineResults.DataSource = _dataTable;
                        dgvMineResults.Refresh();
                    }

                    if (lblRowCount.InvokeRequired)
                    {
                        lblRowCount.Invoke(new MethodInvoker(delegate
                        {
                            lblRowCount.Text = StatDataTable.RowCount.ToString();
                        }));
                    }
                    else lblRowCount.Text = StatDataTable.RowCount.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong Updating the Row Count:{ex.Message}");
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
            if (Program.Processing)
            {
                DialogResult result = MessageBox.Show("Cancel Analysis?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Program.OperationCancelled = true;
                }
            }
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
                DialogResult result = MessageBox.Show("Cancel Analysis and close Application?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

        private void tsmiReset_Click(object sender, EventArgs e)
        {
            this._dataTable = new StatDataTable();
            this.dgvMineResults.DataSource = _dataTable;
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


    }
}
