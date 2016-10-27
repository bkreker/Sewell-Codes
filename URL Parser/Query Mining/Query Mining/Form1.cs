using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace Query_Mining
{
    public partial class Form1 : Form
    {
        public DataTable data = new DataTable();

        public Form1()
        {
            this.Visible = false;
            InitializeComponent();
            this.Visible = false;
            DoThings();
        }

        private void DoThings()
        {
            DialogResult s = inFileDialog.ShowDialog();
            if (s == DialogResult.OK)// && o == DialogResult.OK)
            {
                string inFileName = inFileDialog.FileName;
                Console.WriteLine($"Chosen file:\n{inFileName}");
                //string outFileName = inFileDialog;
                Console.ReadLine();
                try
                {
                    ReadData(inFileDialog.FileName, outFileFolderDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            else
            {
                MessageBox.Show("Operation Cancelled.");
            }
            Console.ReadLine();
        }

        private void ReadData(string fileName, string outFileFolderName)
        {
            string outFileName = outFileFolderName + fileName + "Query_Mining.csv";
            Console.WriteLine("Processing Data...");
            try
            {
                StreamReader inFile = File.OpenText(fileName);
                StreamWriter outFile = new StreamWriter(outFileName);
                ProcessData(ref inFile, ref outFile);
                inFile.Close();
                outFile.Close();
                Console.WriteLine($"Done Processing. File saved at:\n{outFileName}");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        private void ProcessData(ref StreamReader inFile, ref StreamWriter outFile)
        {
            string[] headers = inFile.ReadLine().Split(',');
            data.Headers = headers;
            while (!inFile.EndOfStream)
            {
                var fullRow = (inFile.ReadLine().Split(',')).ToArray<string>();
                string query = fullRow[0].ToString();
                List<string[]> words = SplitQuery(ref query);
                foreach (string[] key in words)
                {
                    try
                    {
                        if (!data[key])
                        {
                            data[key] = true;
                            WriteToNewFile(ref outFile, ref fullRow, ref words, ref query);
                        }
                        else
                        {

                        }
                    }
                    catch (Exception)
                    {
                        //data[key]
                        //   data[key].Add(row);
                    }
                }
            }
        }

        private void WriteToNewFile
            (ref StreamWriter outFile, ref string[] fullRow, ref List<string[]> queryWords, ref string query)
        {
            foreach (string[] words in queryWords)
            {
                List<string> row = new List<string>();
                row.Add(words[1]);
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
    }
}
