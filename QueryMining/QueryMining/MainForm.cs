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
            List<string> firstRow = inFile.ReadLine().Split(',').ToList<string>();
            int queryColumn = firstRow.IndexOf("Search term");
            string headers = string.Join(",", firstRow);
            string headerRow = $"Word,{headers}";
            outFile.Write(headerRow);
            //while (!inFile.EndOfStream)
            //{
            //    var fullRow = (inFile.ReadLine().Split(',')).ToArray<string>();
            //    string query = fullRow[0].ToString();
            //    List<string[]> words = SplitQuery(ref query);
            //    foreach (string[] key in words)
            //    { 
            //        try
            //        {
            //            if (!data[key])
            //            {
            //                data[key] = true;
            //                WriteToNewFile(ref outFile, ref fullRow, ref words, ref query);
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            //data[key]
            //            //   data[key].Add(row);
            //        }
            //    }
            //}
        }

        private void WriteToNewFile
            (ref StreamWriter outFile, ref string[] fullRow, ref List<string[]> queryWords, ref string query, int queryColumn)
        {
            foreach (string[] words in queryWords)
            {
                List<string> row = new List<string>();
                row.Add(words[queryColumn]);
                foreach (var stat in fullRow)
                {
                    row.Add(stat);
                }
                Console.WriteLine(row.ToString());
                //outFile.WriteLine(row.ToString());
            }
        }

        private static List<string[]> SplitQuery(ref string query)
        {
            List<string[]> result = new List<string[]>();
            foreach (string word in query.Split(' '))
            {
                string[] duo = { query, word };
                result.Add(duo);
            }
            return result;

        }

        private void inFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _inFileName = inFileDialog.FileName;
            txtBoxInFIle.Text = _inFileName;


        }

        private void outFileFolderDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _outFileName = outFileDialog.FileName;
            txtBoxOutFile.Text = _outFileName;


        }
    }





}
