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
            InitializeComponent();
            DoThings();
        }

        private void DoThings()
        {
            var inDialog = openFileDialog1;//.ShowDialog();
            var outDialog = openFileDialog1;
            DialogResult s = this.openFileDialog1.ShowDialog();

            if (s == DialogResult.OK)
            {
                Console.WriteLine($"Chosen file:\n{openFileDialog1.FileName}");
                Console.ReadLine();
                try
                {
                    ReadData(openFileDialog1.FileName);
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

        private void ReadData(string fileName)
        {
            Console.WriteLine("Processing Data...");
            try
            {
                StreamReader inFile = File.OpenText(fileName);
                ProcessData(inFile);
                Console.WriteLine("Done Processing");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");

            }


        }

        private static void WriteNewFile()
        {
            throw new NotImplementedException();
        }

        private void ProcessData(StreamReader inFile)
        {
            string[] headers = inFile.ReadLine().Split(',');
            data.Headers = headers;
            while (!inFile.EndOfStream)
            {
                var a = (inFile.ReadLine().Split(',')).ToArray<string>();
                string query = a[0].ToString();
                List<string[]> words = SplitQuery(ref query);
                foreach (string[] key in words)
                {
                    try
                    {
                        data[key].Add(a);
                    }
                    catch (Exception)
                    {
                        data[key] = new List<string[]>();
                        data[key].Add(a);
                    }
                }
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
