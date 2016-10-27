using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
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

        private async void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                if (_inFileName != "" && _outFileName != "")
                {
                    await Task.Run(() => ReadData(_inFileName, _outFileName));
                }
                else
                {
                    MessageBox.Show("Select Valid Names for file and Folder.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something Went Wrong");
            }
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
                MessageBox.Show($"File saved at:\n{outFileName}", "Done Processing");

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
                            Console.WriteLine(newRow);
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
