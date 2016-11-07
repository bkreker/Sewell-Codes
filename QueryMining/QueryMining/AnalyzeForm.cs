using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        StatsTable minedQueries = new StatsTable();
        private DataTable _dataTable;

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

        public AnalyzeForm(DataTable _dataTable, int _wordColumn, int _queryColumn)
        {
            this._dataTable = _dataTable;
            this._wordColumn = _wordColumn;
            this._queryColumn = _queryColumn;
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
                Sort();
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

        private void SortRecursive()
        {
            foreach (QueryWord word1 in _dataDictionary.Values)
            {
                recursiveSortHelper(_dataDictionary, word1);
            }
        }

        private void recursiveSortHelper(StatsTable s, QueryWord word1)
        {

        }
        private void SortDataTable()
        {
            Console.WriteLine("Sort Started.");
            try
            {
                foreach (DataRow row1 in _dataTable.Rows)
                {
                    string query1 = row1[_queryColumn].ToString();
                    string word1 = row1[_wordColumn].ToString();
                    string[] queryKey = {word1, query1};

                    foreach (DataRow row2 in _dataTable.Rows)
                    {

                    }                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void Sort()
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
                                        if (!minedQueries.ContainsKey(word1.Word + " " + word2.Word) && !minedQueries.ContainsKey(newWord.Word))
                                        {
                                            minedQueries[newWord.Word] = newWord;
                                            //   AddToDataGridView(newWord);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in minedQueries)
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
