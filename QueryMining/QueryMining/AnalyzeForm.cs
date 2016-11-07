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

        public AnalyzeForm(StatDataTable dataTable, int wordColumn, int queryColumn)
        {
            this._dataTable = dataTable;
            this._wordColumn = wordColumn;
            this._queryColumn = queryColumn;
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
                Calculate();
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
                return (Left.Word != Right.Word || Left.Query != Right.Query);
            }

            public QWord Add( QWord Right, DataColumnCollection colHeaders)
            {
                QWord result = new QWord();
                List<object> resultList = new List<object>();
                result.Word = this.Word + " " + Right.Word;
                result.Query = this.Query + " / " + Right.Query;
                result.FullRow = new List<object>();
                for (int i = 0; i < this.FullRow.Count && i < Right.FullRow.Count; i++)
                {
                    if (Regex.IsMatch(this.FullRow[i].ToString(), Regexes.Number) && Regex.IsMatch(Right.FullRow[i].ToString(), Regexes.Number))
                    {
                        decimal leftNum = (decimal)this.FullRow[i];
                        decimal rightNum = (decimal)Right.FullRow[i];
                        if (isAvg)
                        {


                        }                 
                    }
                }
                return result;
            }
        }

        private void CalculateFromDataTable()
        {
            Console.WriteLine("Sort Started.");
            try
            {
                foreach (DataRow row1 in _dataTable.Rows)
                {
                    QWord word1 = new QWord(row1[_wordColumn].ToString(), row1[_queryColumn].ToString(), row1);

                    foreach (DataRow row2 in _dataTable.Rows)
                    {
                        QWord word2 = new QWord(row2[_wordColumn].ToString(), row2[_queryColumn].ToString(), row2);
                        // string[] queryKey2 = { row2[_wordColumn].ToString(), row2[_queryColumn].ToString() };
                        if (word1 != word2)
                        {
                            foreach (DataRow row3 in _dataTable.Rows)
                            {
                                QWord word3 = new QWord(row3[_wordColumn].ToString(), row3[_queryColumn].ToString(), row3);
                                // string[] queryKey3 = { row3[_wordColumn].ToString(), row3[_queryColumn].ToString() };
                                if (word3 != word1)
                                {
                                    QWord newWord;
                                    // Console.WriteLine(newWord.Word);
                                    if (!_minedQueries.ContainsKey(word1.Word + " " + word2.Word) && !_minedQueries.ContainsKey(newWord.Word))
                                    {
                                        //  _minedQueries[newWord.Word] = newWord;
                                        //   AddToDataGridView(newWord);
                                    }

                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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
