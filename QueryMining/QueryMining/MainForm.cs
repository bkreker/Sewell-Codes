using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace QueryMining
{
    public partial class MainForm : Form
    {
        //   public Dictionary<string[], bool> data = new Dictionary<string[], bool>();
        string _inFileName { get { return txtBoxInFile.Text; } }
        string _outFileName { get { return txtBoxOutFile.Text; } }
        StringWriter _outPutStringStream = new StringWriter();
        bool _outFileSavedCorrectly = false;
        bool _inFileReadCorrectly = false;

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
            // INSERT TIME CONSUMING OPERATIONS HERE
            // THAT DON'T REPORT PROGRESS
            //Thread.Sleep(10000);
            if (txtBoxOutFile.Text != "" && txtBoxInFile.Text != "" && inFileDialog.CheckFileExists)
            {
                Console.WriteLine("Processing Data...");
                try
                {
                    ReadData();
                    if (_inFileReadCorrectly)
                    {
                        SaveData();
                    }
                    else
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

            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.MarqueeAnimationSpeed = 0;

            btnGo.Enabled = true;
            btnImport.Enabled = true;
            btnSelectFolder.Enabled = true;
            btnClose.Text = "Close";
            if (_outFileSavedCorrectly)
            {
                MessageBox.Show($"File saved at:\n{_outFileName}", "Done Processing");

            }
            else
            {
                MessageBox.Show("The new file was not saved");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReadData()
        {
            Console.WriteLine("Processing Data...");
            try
            {
                try
                {
                    StreamReader inFile = File.OpenText(_inFileName);
                    string firstRowString = inFile.ReadLine();
                    List<string> firstRow = firstRowString.Split('\t').ToList<string>();
                    ColumnHeaderSelect c = new ColumnHeaderSelect(firstRow);
                    c.ShowDialog();
                    int queryColumn = c.SelectedIndex;

                    string newFirstRow = $"Word,{string.Join(",", firstRow)}";
                    Console.WriteLine(newFirstRow);
                    _outPutStringStream.WriteLine(newFirstRow);
                    // outFile.WriteLine(newFirstRow);

                    while (!inFile.EndOfStream)
                    {
                        List<string> fullRow = (inFile.ReadLine().Split(',')).ToList<string>();
                        string query = fullRow[queryColumn].ToString();
                        List<string> queryWords = SplitQuery(query);

                        foreach (string word in queryWords)
                        {
                            try
                            {
                                //  data[key] = true;
                                string newRow = $"{word},{string.Join(",", fullRow)}";
                                Console.WriteLine($"Query: {query}, Word: {word}");
                                _outPutStringStream.WriteLine(newRow);

                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Something Went Wrong Processing the file: {ex.Message}");
                            }
                        }
                    }
                    inFile.Close();
                    _inFileReadCorrectly = true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Something Went Wrong Processing the file: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Something went wrong: {ex.Message}");
            }
        }



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
