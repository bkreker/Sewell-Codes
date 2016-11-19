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
    public partial class ImportForm : Form
    {
        //   public Dictionary<string[], List<>> dataTable = new Dictionary<string[], bool>();
        string _inFileName { get { return txtBoxInFile.Text; } }


        //   StringWriter _outPutStringStream = new StringWriter();
        private StatDataTable _dataTable { get; set; }
        public StatDataTable DataTable { get { return this._dataTable; } }
        bool
            _avgAllValues = true,
            // _outFileSavedCorrectly = false,
            _inFileReadCorrectly = false,
            _operationCancelled = false,
            _processing = false;

        public bool AvgAllValues { get { return _avgAllValues; } }

        //int _queryColumn = -1,
        //     _wordColumn = -1;

        //public int QueryColumnIndex { get { return _queryColumn; } }
        //public int WordColumnIndex { get { return _wordColumn; } }

        public ImportForm()
        {
            InitializeComponent();

            this.DialogResult = DialogResult.None;
            _dataTable = new StatDataTable();
            txtBoxInFile.Text = @"C:\Users\joshd\OneDrive\Work Files\Analysis\Google AdWords\Shopping\Siamese Cable Search Terms.csv";

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



        private void btnGo_Click(object sender, EventArgs e)
        {
            if (_inFileName != "")
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;

                btnGo.Enabled = false;
                btnImport.Enabled = false;

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
            if (txtBoxInFile.Text != "" && inFileDialog.CheckFileExists)
            {
                Console.WriteLine("Processing Data...");
                _processing = true;
                try
                {
                    ReadDataToDataTable();
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
            btnClose.Text = "Close";

            if (_inFileReadCorrectly)
            {               
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (!_operationCancelled)
            {
                MessageBox.Show("Something went wrong, the file was not imported correctly.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_processing)
            {
                DialogResult result = MessageBox.Show("Cancel File Import and Close the Program?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

        private void rBtnAvgAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnAvgSome.Checked)
            {
                _avgAllValues = false;
            }
            else
            {
                _avgAllValues = true;
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
                    //_queryColumn = c.SelectedIndex;
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
                            AddRowToTable(inputRow);
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

        private void AddRowToTable(List<string> inputRow)
        {
            object[] outputRow;
            List<DataRow> existingRows = (from DataRow r in _dataTable.Rows
                                          where r.ItemArray[_dataTable.QueryCol].ToString() == inputRow[_dataTable.QueryCol]
                                          select r).ToList();
            if (existingRows.Count > 0)
            {
                outputRow = _dataTable.AggregateRows(existingRows, inputRow, inputRow.Count);
                int rowIx = _dataTable.Rows.IndexOf(existingRows[0]);
                _dataTable.Rows[rowIx].ItemArray = outputRow;
            }
            else
            {
                outputRow = inputRow.ToArray();
                try
                {
                    _dataTable.Rows.Add(outputRow);

                }
                catch (ConstraintException ex)
                {
                    Console.WriteLine($"Duplicate Query attempted: {ex.Message}, {ex.Data}");
                }
                catch (DuplicateNameException ex)
                {
                    Console.WriteLine($"Duplicate Query attempted: {ex.Message}, {ex.Data}");
                }
            }

        }

        private void inFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            txtBoxInFile.Text = inFileDialog.FileName; ;
        }

    }





}
