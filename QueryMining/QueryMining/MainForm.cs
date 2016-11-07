using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.Data;


/**
*    What search queries have high impressions but no clicks? 
*    What search queries have resulted in a conversion? 
*    What search queries have a below-average CTR for the ad group? 
*    What search queries have an above-average cost/conversion?
*    Do I have a problem with ad poaching and duplication?
**/

namespace QueryMining
{
    public partial class MainForm : Form
    {
        //   public Dictionary<string[], List<>> dataTable = new Dictionary<string[], bool>();
        string _inFileName { get { return txtBoxInFile.Text; } }
        string _outFileName { get { return txtBoxOutFile.Text; } }

        StringWriter _outPutStringStream = new StringWriter();
        StatDataTable _dataTable = new StatDataTable();
        bool _outFileSavedCorrectly = false,
            _inFileReadCorrectly = false,
            _operationCancelled = false,
            _processing = false;

        int _queryColumn = -1,
             _wordColumn = -1;

        public MainForm()
        {
            InitializeComponent();
            txtBoxInFile.Text = "New Test in File.csv";
            txtBoxOutFile.Text = "test.csv";
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                inFileDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something Went Wrong");
            }
        }

        private void btnSelectOutFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (outFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtBoxOutFile.Text = outFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went wrong");
            }

        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (_inFileName != "" && _outFileName != "")
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;

                btnGo.Enabled = false;
                btnImport.Enabled = false;
                btnSelectFolder.Enabled = false;
                btnClose.Text = "Cancel and Close";
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += bw_DoWork;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                bw.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Select Valid Names for file and Folder.");
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (txtBoxOutFile.Text != "" && txtBoxInFile.Text != "" && inFileDialog.CheckFileExists)
            {
                Console.WriteLine("Processing Data...");
                _processing = true;
                try
                {
                    //ReadData();
                    ReadDataToDataTable();
                    if (_inFileReadCorrectly)
                    {
                        //SaveData();
                        _dataTable.Save(_outFileName, ref _outFileSavedCorrectly);
                    }
                    else if (!_operationCancelled)
                    {
                        MessageBox.Show("The file was not read in correctly. Nothing was saved.");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something went wrong while running the application");
                }
            }
            else
            {
                MessageBox.Show("Couldn't open the selected file.");
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _processing = false;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.MarqueeAnimationSpeed = 0;

            btnGo.Enabled = true;
            btnImport.Enabled = true;
            btnSelectFolder.Enabled = true;
            btnClose.Text = "Close";

            if (_outFileSavedCorrectly)
            {

                if (MessageBox.Show($"File saved at:\n{_outFileName}. Analyze Now?", "Done Processing", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    //var analysis = new AnalyzeForm(_outPutStringStream, _wordColumn, _queryColumn);
                    var analysis = new AnalyzeForm(_dataTable, _wordColumn, _queryColumn);
                    analysis.ShowDialog();
                    //this.Hide();
                }

            }
            else if (!_operationCancelled)
            {
                MessageBox.Show("The new file was not saved");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_processing)
            {
                MessageBox.Show("The operation was cancelled.");
            }
            this.Close();
        }

        private void ReadData()
        {
            Console.WriteLine("Processing Data...");
            _processing = true;
            try
            {
                StreamReader inFile = File.OpenText(_inFileName);
                string firstRowString = inFile.ReadLine();
                char delimChar = ',';
                string writeDelim = ",";

                // if it detects it's actually .tsv, switch delimiters
                if (firstRowString.IndexOf('\t') >= 0)
                {
                    delimChar = '\t';
                }
                List<string> firstRow = firstRowString.Split(delimChar).ToList<string>();

                ColumnHeaderSelect c = new ColumnHeaderSelect(firstRow);
                c.ShowDialog();

                if (c.DialogResult == DialogResult.OK)
                {
                    int queryColumn = c.SelectedIndex;

                    string newFirstRow = $"Word,{string.Join(writeDelim, firstRow)}";
                    Console.WriteLine(newFirstRow);
                    _outPutStringStream.WriteLine(newFirstRow);

                    // Write the new lines to the output stream
                    while (!inFile.EndOfStream)
                    {
                        List<string> fullRow = (inFile.ReadLine().Split(delimChar)).ToList<string>();
                        string query = fullRow[queryColumn].ToString();
                        List<string> queryWords = SplitQuery(query);

                        foreach (string word in queryWords)
                        {
                            string newRow = $"{word},{string.Join(writeDelim, fullRow)}";
                            Console.WriteLine($"Query: {query}, Word: {word}");
                            _outPutStringStream.WriteLine(newRow);
                        }
                    }
                    _wordColumn = 0;
                    _queryColumn = 1;
                    _inFileReadCorrectly = true;
                }
                else
                {
                    _inFileReadCorrectly = false;
                }

                inFile.Close();
            }
            catch (OperationCanceledException)
            {
                _operationCancelled = true;
                MessageBox.Show("The operation was cancelled.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Something went wrong reading the file: {ex.Message}");
            }
        }

        private void ReadDataToDataTable()
        {
            Console.WriteLine("Processing Data...");
            _processing = true;
            try
            {
                StreamReader inFile = File.OpenText(_inFileName);
                char delimChar = ',';
                string writeDelim = ",";

              List<string> SecondRow =  _dataTable.SetTableSchema(ref inFile, ref delimChar);

                ColumnHeaderSelect c = new ColumnHeaderSelect(_dataTable.Columns);
                c.ShowDialog();

                if (c.DialogResult == DialogResult.OK)
                {
                    _queryColumn = c.SelectedIndex;
                    _dataTable.Query_ColIndex = c.SelectedIndex;

                    //string newFirstRow = $"Word,{string.Join(writeDelim, firstRow)}";
                    List<string> headerRow = (from DataColumn h in _dataTable.Columns
                                              select h.Caption).ToList();

                    List<object> outputRow = new List<object>();
                    headerRow.ForEach(a => outputRow.Add(a));
                    // Write the new lines to the output stream
                    while (!inFile.EndOfStream)
                    {
                        var inputRow = (inFile.ReadLine().Split(delimChar)).ToList<string>();
                        string query = inputRow[_dataTable.Query_ColIndex].ToString();
                        List<string> queryWords = SplitQuery(query);

                        foreach (string word in queryWords)
                        {
                            outputRow = new List<object>();
                            inputRow.ForEach(a => outputRow.Add(a));
                            outputRow.Add(word);

                            Console.WriteLine($"Query: {query}, Word: {word}");
                            _dataTable.Rows.Add(outputRow.ToArray());
                        }
                    }
                    _inFileReadCorrectly = true;
                }

                inFile.Close();
            }
            catch (OperationCanceledException)
            {
                _operationCancelled = true;
                _processing = false;
                MessageBox.Show("The operation was cancelled.");
            }
            catch (Exception ex)
            {
                _processing = false;
                throw new Exception($"Something went wrong reading the file: {ex.Message}");
            }

        }

        //private void SetTableSchema(ref StreamReader inFile, ref char delimChar)
        //{
        //    string firstRowString = inFile.ReadLine();
        //    string secondRowString = inFile.ReadLine();
        //    // if it detects it's actually .tsv, switch delimiters
        //    if (firstRowString.IndexOf('\t') >= 0)
        //    {
        //        delimChar = '\t';
        //    }

        //    List<string> firstRow = firstRowString.Split(delimChar).ToList<string>();
        //    List<string> secondRow = secondRowString.Split(delimChar).ToList<string>();

        //    for (int i = 0; i < firstRow.Count; i++)
        //    {
        //        string a = firstRow[i], b = secondRow[i];
        //        decimal dec;
        //        double dub;
        //        int integ;

        //        if (decimal.TryParse(b, out dec))
        //        {
        //            _dataTable.Columns.Add(a.Trim(), typeof(decimal));

        //        }
        //        else if (double.TryParse(b, out dub))
        //        {
        //            _dataTable.Columns.Add(a.Trim(), typeof(double));
        //        }
        //        else if (int.TryParse(b, out integ))
        //        {
        //            _dataTable.Columns.Add(a.Trim(), typeof(int));
        //        }
        //        else
        //        {
        //            _dataTable.Columns.Add(a.Trim(), typeof(string));
        //        }
        //    }
        //    _dataTable.Columns.Add("Word", typeof(string));
        //    _wordColumn = _dataTable.Columns.IndexOf("Word");
        //    _dataTable.Rows.Add(secondRow.ToArray());

        //}

        //private void SaveDataTable()
        //{
        //    try
        //    {
        //        StreamWriter outFile = new StreamWriter(_outFileName);
        //        var tableList = (from DataRow row in _dataTable.Rows
        //                         select (from i in row.ItemArray
        //                                 select i.ToString()).ToList<string>()).ToList();

        //        List<string> headerList = new List<string>();
        //        foreach (DataColumn column in _dataTable.Columns)
        //        {
        //            headerList.Add(column.Caption);
        //        }

        //        outFile.WriteLine(string.Join(",", headerList));
        //        tableList.ForEach(a => outFile.WriteLine(string.Join(",", a)));

        //        //foreach (var row in tableList)
        //        //{
        //        //    outFile.WriteLine(row);
        //        //    Console.WriteLine(row);
        //        //}
        //        //outFile.Write(_dataTable);
        //        outFile.Close();
        //        _outFileSavedCorrectly = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _outFileSavedCorrectly = false;
        //        MessageBox.Show(ex.Message, "Error saving file");
        //    }

        //}

        private void SaveData()
        {
            try
            {
                StreamWriter outFile = new StreamWriter(_outFileName);
                outFile.Write(_outPutStringStream);
                outFile.Close();
                _outFileSavedCorrectly = true;
            }
            catch (Exception ex)
            {
                _outFileSavedCorrectly = false;
                MessageBox.Show(ex.Message, "Error saving file");
            }
        }

        private static List<string> SplitQuery(string query)
        {
            List<string> result = new List<string>();

            foreach (string word in query.Split(' '))
            {
                result.Add(word);
            }
            return result;

        }

        private void inFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            txtBoxInFile.Text = inFileDialog.FileName; ;
        }

        private void outFileFolderDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            txtBoxOutFile.Text = outFileDialog.FileName;
        }
    }





}
