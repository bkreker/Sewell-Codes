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
        Dictionary<string, QueryWord> _dataDictionary = new Dictionary<string, QueryWord>();

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

                backgroundWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something Went Wrong.");
            }
        }

        private void Analyze()
        {
            Console.WriteLine("Analyze Started.");
            dgvResults.DataSource = (from a in _dataDictionary
                                     select a).ToList();
            Console.WriteLine("Analyze Started.");
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // INSERT TIME CONSUMING OPERATIONS HERE
            // THAT DON'T REPORT PROGRESS

            Console.WriteLine("Processing Data...");
            try
            {
                _processing = true;
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
                MessageBox.Show("Finished!");

            }
            else if (_operationCancelled)
            {
                MessageBox.Show("The new file was not saved");
            }
        }

        private void Sort()
        {
            Console.WriteLine("Sort Started.");
            StatsTable data = new StatsTable(ref _outPutStringStream);
            foreach (var word1 in data.Values)
            {
                var word1Row = word1.Stats;

                foreach (var word2 in data.Values)
                {
                    if (word2.Query.Contains(word1.Word) && word2.Query != word1.Query)
                    {
                        QueryWord newWord = word1 + word2;
                    }
                }
            }
            //string fileString = _outPutStringStream.ToString();
            //List<string> statsList = fileString.Split('\n').ToList();
            //string headers = statsList[0];
            //statsList.RemoveAt(0);
            //foreach (string fullRow in statsList)
            //{
            //    var rowStats = fullRow.Split(',').ToList();
            //    string word = rowStats[_wordColumn];
            //    string query = rowStats[_queryColumn];
            //    string key = word; //, rowStats[_queryColumn] };
            //    rowStats.RemoveAt(_queryColumn);
            //    rowStats.RemoveAt(_wordColumn);
            //    List<double> newList = new List<double>();
            //    foreach (string item in rowStats)
            //    {
            //        double stat;
            //        if (double.TryParse(item, out stat))
            //        {
            //            newList.Add(stat);
            //        }
            //        else
            //        {
            //            newList.Add(0);
            //        }
            //    }
            //    try
            //    {
            //        _dataDictionary[key].Stats.Add(newList);
            //    }
            //    catch (KeyNotFoundException)
            //    {
            //        _dataDictionary[key] = new QueryWord(word, query, newList);

            //        _dataDictionary[key].Stats.Add(newList);
            //    }
            //    Console.WriteLine($"Key: {word}. Rows: {_dataDictionary[key].Stats.Count}");
            //}
            //Console.WriteLine("Sort Finished.");

            Analyze();
        }
    }

}
