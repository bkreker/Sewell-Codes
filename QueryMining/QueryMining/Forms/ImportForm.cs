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

namespace QueryMining.Forms
{
    public partial class ImportForm : Form
    {
        private List<string> triedFiles = new List<string>();
        string _inFileName
        {
            get
            {
                if (comboBoxInFile.InvokeRequired)
                {
                    string retVal = "";
                    comboBoxInFile.Invoke(new MethodInvoker(delegate
                    {
                        retVal = comboBoxInFile.Text;
                    }));
                    return retVal;
                }

                return comboBoxInFile.Text;
            }
            set
            {
                if (comboBoxInFile.InvokeRequired)
                {
                    comboBoxInFile.Invoke(new MethodInvoker(delegate
                    {
                        comboBoxInFile.Text = value;
                    }));
                }
                else
                    comboBoxInFile.Text = value;
            }
        }

        private const string PAST_FILE_NAMES_FILE = "pastFiles.txt";

        //   StringWriter _outPutStringStream = new StringWriter();
        private StatDataTable _dataTable { get; set; }

        public StatDataTable DataTable { get { return this._dataTable; } }

        public static bool _inFileReadCorrectly = false;

        public ImportForm()
        {
            InitializeComponent();

            this.DialogResult = DialogResult.None;
            _dataTable = new StatDataTable();

            FillComboBox();
            // comboBoxInFile.Text = @"C:\Users\joshd\Documents\Codes\QueryMining\QueryMining\bin\Debug\Other Bulk Cable Shopping Terms.csv";

        }

        private void FillComboBox()
        {
            StreamReader pastFileNamesFile = new StreamReader(PAST_FILE_NAMES_FILE);

            while (!pastFileNamesFile.EndOfStream)
            {
                string fileName = pastFileNamesFile.ReadLine();
                triedFiles.Add(fileName);
                comboBoxInFile.Items.Add(fileName);
            }
            comboBoxInFile.SelectedIndex = comboBoxInFile.Items.Count - 1;
            pastFileNamesFile.Close();
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
                if (!triedFiles.Contains(_inFileName))
                {
                    try
                    {
                        StreamWriter outFile = File.AppendText(PAST_FILE_NAMES_FILE);
                        outFile.WriteLine(_inFileName);
                        outFile.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error Saving file name.\n{ex.Message}");
                    }
                }
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 10;

                btnGo.Enabled = false;
                btnImport.Enabled = false;
                Program.AvgAll = cboxAvgAll.Checked;
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
            if (_inFileName != "" && inFileDialog.CheckFileExists)
            {
                Console.WriteLine("Processing Data...");
                Program.Processing = true;
                Program.OperationCancelled = false;
                try
                {
                    ImportData();
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
            Program.Processing = false;
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
            else if (!Program.Processing)
            {
                MessageBox.Show("Something went wrong, the file was not imported correctly.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (Program.Processing)
            {
                DialogResult result = MessageBox.Show("Cancel File Import and Close the Program?", "Still Processing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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



        private void txtBoxInFile_DoubleClick(object sender, EventArgs e)
        {
            comboBoxInFile.SelectAll();
        }

        private void ImportData()
        {
            Console.WriteLine("Processing Data...");
            Program.Processing = true;
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
                Program.OperationCancelled = true;
                Program.Processing = false;
                MessageBox.Show("The operation was cancelled.");
            }
            catch (Exception ex)
            {
                Program.Processing = false;
                throw new Exception($"Something went wrong reading the file: {ex.Message}\nInputRow: {string.Join(",", inputRow)}\nQuery{query}");
            }

        }

        private void inFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _inFileName = inFileDialog.FileName;
            if (comboBoxInFile.Items.IndexOf(_inFileName) < 0)
            {
                comboBoxInFile.Items.Add(_inFileName);
            }
            comboBoxInFile.SelectedIndex = comboBoxInFile.Items.IndexOf(_inFileName);
        }

    }





}
