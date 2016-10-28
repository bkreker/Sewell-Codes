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
        public Dictionary<string[], bool> data = new Dictionary<string[], bool>();
        string _inFileName = "";
        string _outFileName = "";
        public MainForm()
        {
            InitializeComponent();
            _inFileName = "Shopping Search Terms csv.csv";
            txtBoxInFile.Text = _inFileName;
            _outFileName = "test.csv";
            txtBoxOutFile.Text = _outFileName;
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

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (outFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _outFileName = outFileDialog.FileName;
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

        private async void Analyze()
        {
            try
            {
                await Task.Run(() => ReadData(_inFileName, _outFileName));


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something Went Wrong");
            }
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // INSERT TIME CONSUMING OPERATIONS HERE
            // THAT DON'T REPORT PROGRESS
            //Thread.Sleep(10000);
            Console.WriteLine("Processing Data...");
            try
            {
                StreamReader inFile = File.OpenText(_inFileName);
                StreamWriter outFile = new StreamWriter(_outFileName);
                ProcessData(ref inFile, ref outFile);

                inFile.Close();
                outFile.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}");
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
            MessageBox.Show($"File saved at:\n{_outFileName}", "Done Processing");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReadData(string fileName, string outFileName)
        {
            Console.WriteLine("Processing Data...");
            try
            {
                StreamReader inFile = File.OpenText(fileName);
                StreamWriter outFile = new StreamWriter(outFileName);
                ProcessData(ref inFile, ref outFile);
                inFile.Close();
                outFile.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}");
            }
        }

        private void ProcessData(ref StreamReader inFile, ref StreamWriter outFile)
        {
            try
            {
                List<string> firstRow = inFile.ReadLine().Split(',').ToList<string>();
                int queryColumn = firstRow.IndexOf("Search term");
                Console.WriteLine($"Word,{string.Join(",", firstRow)}");
                outFile.WriteLine($"Word,{string.Join(",", firstRow)}");

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
                            outFile.WriteLine(newRow);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Something Went Wrong");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something Went Wrong");
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
            _inFileName = inFileDialog.FileName;
            txtBoxInFile.Text = _inFileName;


        }

        private void outFileFolderDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _outFileName = outFileDialog.FileName;
            txtBoxOutFile.Text = _outFileName;


        }
    }





}
