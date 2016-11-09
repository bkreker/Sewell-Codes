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
            txtBoxInFile.Text = @"C:\Users\jdegr_000\OneDrive\Work Files\Analysis\Google AdWords\Shopping\Siamese Cable Search Terms.csv";
            txtBoxOutFile.Text = "test save.csv";
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
                        // _dataTable.Save(_outFileName, ref _outFileSavedCorrectly);
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

            if (_inFileReadCorrectly)
            {
                DialogResult result = MessageBox.Show($"File saved at:\n{_outFileName}. Analyze Now?", "Done Processing", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    var analysis = new AnalyzeForm(_dataTable, _wordColumn, _queryColumn, _outFileName);
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

        private void FormatRow(ref List<string> row, ref DataColumnCollection columns)
        {
            if (row.All(r => r == ""))
            {
                throw new Exception($"The row was empty");
            }
            if (row.Any(val => val.IndexOf('%') > 0))
            {
                List<string> percentCells = row.Where(val => val.IndexOf('%') > 0).ToList();
                foreach (string val in percentCells)
                {
                    int i = row.IndexOf(val);
                    decimal num;
                    row[i] = row[i].Remove(row[i].IndexOf('%'), 1);
                    if (decimal.TryParse(row[i], out num))
                    {
                        row[i] = (num / 100).ToString();
                    }
                }
            }
            if (row.Any(val => val.IndexOf(',') > 0))
            {
                List<string> commaCells = row.Where(val => val.IndexOf(',') > 0).ToList();
                foreach (string val in commaCells)
                {
                    int i = row.IndexOf(val);
                    row[i] = row[i].Remove(row[i].IndexOf(','), 1);
                    try
                    {
                        row[i] = Regexes.Match(row[i], Regexes.Number);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error formatting row cell: {ex.Message}");
                    }
                }
            }
        }

        private void ReadDataToDataTable()
        {
            Console.WriteLine("Processing Data...");
            _processing = true;
            _dataTable = new StatDataTable();
            List<string> inputRow = new List<string>();
            string query = "";
            try
            {
                StreamReader inFile = File.OpenText(_inFileName);
                char delimChar = ',';
                string writeDelim = ",";

                List<string> SecondRow = _dataTable.SetTableSchema(ref inFile, ref delimChar);
                ColumnHeaderSelect c = new ColumnHeaderSelect(_dataTable.Columns);
                c.ShowDialog();
                DataColumnCollection Columns = _dataTable.Columns;

                if (c.DialogResult == DialogResult.OK)
                {
                    _queryColumn = c.SelectedIndex;
                    _dataTable.QueryCol = c.SelectedIndex;

                    //string newFirstRow = $"Word,{string.Join(writeDelim, firstRow)}";
                    List<string> headerRow = (from DataColumn h in _dataTable.Columns
                                              select h.Caption).ToList();

                    List<object> outputRow = new List<object>();
                    headerRow.ForEach(header => outputRow.Add(header));
                    // Write the new lines to the output stream
                    while (!inFile.EndOfStream)
                    {
                        try
                        {
                            inputRow = (inFile.ReadLine().Split(delimChar)).ToList();
                            FormatRow(ref inputRow, ref Columns);
                            //  query = inputRow[_dataTable.QueryCol];

                            _dataTable.Rows.Add(inputRow.ToArray<object>());
                            //  Console.WriteLine($"Query: {query} read from file.");
                        }
                        catch (Exception ex)
                        {
                            //  Console.Error.WriteLine($"Error importing row: {ex.Message}");
                            Console.WriteLine($"Error importing row: {ex.Message}");
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
                throw new Exception($"Something went wrong reading the file: {ex.Message}\nInputRow: {string.Join(",", inputRow)}\nQuery{query}");
            }

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
