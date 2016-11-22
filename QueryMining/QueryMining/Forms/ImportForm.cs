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

        public static bool _inFileReadCorrectly = false;

        public bool AvgAllValues
        {
            get { return StatDataTable.AvgAll; }
            set { StatDataTable.AvgAll = value; }
        }

        public ImportForm()
        {
            InitializeComponent();

            this.DialogResult = DialogResult.None;
            _dataTable = new StatDataTable();
            AvgAllValues = true;
            txtBoxInFile.Text = @"C:\Users\joshd\Documents\Codes\QueryMining\QueryMining\bin\Debug\Other Bulk Cable Shopping Terms.csv";

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
                StatDataTable.Processing = true;
                try
                {
                    ReadDataToDataTable();
                }
                catch (OperationCanceledException)
                {

                    Console.WriteLine("The operation was cancelled by the user");
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
            StatDataTable.Processing = false;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.MarqueeAnimationSpeed = 0;

            btnGo.Enabled = true;
            btnImport.Enabled = true;

            if (_inFileReadCorrectly)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (!StatDataTable.Processing)
            {
                MessageBox.Show("Something went wrong, the file was not imported correctly.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (StatDataTable.Processing)
            {
                DialogResult result = MessageBox.Show("Cancel File Import and Close the Program?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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


        private void rBtnAvgAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cboxAvgAll.Checked)
            {
                AvgAllValues = true;
            }
            else
            {
                AvgAllValues = false;
            }
        }

        private void txtBoxInFile_DoubleClick(object sender, EventArgs e)
        {
            txtBoxInFile.SelectAll();
        }

        private void ReadDataToDataTable()
        {
            Console.WriteLine("Processing Data...");
            StatDataTable.Processing = true;
            List<string> inputRow = new List<string>();
            string query = "";
            try
            {
                StreamReader inFile = File.OpenText(_inFileName);
                char delimChar = ',';

                var firstRowString = inFile.ReadLine();
                if (firstRowString.IndexOf('\t') > 0)
                {
                    delimChar = '\t';
                }
                var firstRow = firstRowString.Split(delimChar).ToList();
                var secondRow = inFile.ReadLine().Split(delimChar).ToList();
                _dataTable = new StatDataTable(firstRow, secondRow);

                ColumnHeaderSelect c = new ColumnHeaderSelect(StatDataTable.ColumnCollection);
                c.ShowDialog();
                DataColumnCollection Columns = _dataTable.Columns;

                if (c.DialogResult == DialogResult.OK)
                {
                    StatDataTable.QueryCol = c.SelectedIndex;

                    List<string> headerRow = (from DataColumn h in StatDataTable.ColumnCollection
                                              select h.Caption).ToList();

                    List<object> outputRow = new List<object>();
                    headerRow.ForEach(header => outputRow.Add(header));
                    // Write the new lines to the output stream
                    while (!inFile.EndOfStream)
                    {
                        if (_operationCancelled)
                            throw new OperationCanceledException();

                        try
                        {
                            inputRow = (inFile.ReadLine().Split(delimChar)).ToList();
                            StatDataTable.FormatRow(ref inputRow);
                            _dataTable.AddRowToTable(inputRow);
                        }
                        catch (Exception ex)
                        {
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
                StatDataTable.Processing = false;
                MessageBox.Show("The operation was cancelled.");
            }
            catch (Exception ex)
            {
                StatDataTable.Processing = false;
                throw new Exception($"Something went wrong reading the file: {ex.Message}\nInputRow: {string.Join(",", inputRow)}\nQuery{query}");
            }

        }

        private void inFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            txtBoxInFile.Text = inFileDialog.FileName; ;
        }

    }





}
