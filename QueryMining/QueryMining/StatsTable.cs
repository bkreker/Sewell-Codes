using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QueryMining
{
    public class StatDataTable : DataTable
    {
        // public int WordCol { get; set; }
        public int QueryCol { get; set; }
        public int QueryCountCol { get; set; }
        public List<string> Headers
        {
            get
            {
                return (from DataColumn col in this.Columns
                        select col.Caption).ToList();
            }
        }
        public void Save(string _outFileName, ref bool _outFileSavedCorrectly)
        {
            try
            {
                StreamWriter outFile = new StreamWriter(_outFileName);
                var tableList = (from DataRow row in this.Rows
                                 select (from i in row.ItemArray
                                         select i.ToString()).ToList<string>()).ToList();

                List<string> headerList = new List<string>();
                foreach (DataColumn column in this.Columns)
                {
                    headerList.Add(column.Caption);
                }

                outFile.WriteLine(string.Join(",", headerList));
                tableList.ForEach(a => outFile.WriteLine(string.Join(",", a)));

                outFile.Close();
                _outFileSavedCorrectly = true;
            }
            catch (Exception ex)
            {
                _outFileSavedCorrectly = false;
                MessageBox.Show(ex.Message, "Error saving file");
            }

        }

        public List<string> SetTableSchema(ref StreamReader inFile, ref char delimChar)
        {
            try
            {
                string firstRowString = inFile.ReadLine();
                string secondRowString = inFile.ReadLine();
                // if it detects it's actually .tsv, switch delimiters
                if (firstRowString.IndexOf('\t') >= 0)
                {
                    delimChar = '\t';
                }
                List<string> firstRow = firstRowString.Split(delimChar).ToList<string>();
                List<string> secondRow = secondRowString.Split(delimChar).ToList<string>();

                for (int i = 0; i < firstRow.Count; i++)
                {
                    string colName = firstRow[i].Trim();
                    string colVal = secondRow[i].Trim();

                    object defaultVal = "";
                    Type colType = typeof(string);
                    bool isUnique = false;

                    if (Regexes.IsMatch(colName, Regexes.Query))
                    {
                        isUnique = true;
                    }
                    else if (Regexes.IsMatch(colVal, Regexes.Number)/* && Regexes.MatchesAnyStat(colName)*/)
                    {
                        if (colVal.Contains('%'))
                        {
                            colVal = colVal.Remove(colVal.IndexOf('%'), 1);
                        }
                        if (colVal.Contains(','))
                        {
                            colVal = colVal.Remove(colVal.IndexOf(','), 1);
                        }
                        if (Regexes.IsDouble(colName))
                        {
                            double dub;
                            if (double.TryParse(colVal, out dub))
                            {
                                colType = typeof(double);
                                defaultVal = 0.00;

                            }
                        }
                        else if (Regexes.IsDecimal(colName))
                        {
                            decimal dec;
                            if (decimal.TryParse(colVal, out dec))
                            {
                                colType = typeof(decimal);
                                defaultVal = 0m;

                            }
                        }
                        else if (Regexes.IsLong(colName))
                        {
                            long lon;
                            if (long.TryParse(colVal, out lon))
                            {
                                colType = typeof(long);
                                defaultVal = 0;
                            }

                        }
                        else if (Regexes.IsInt(colName))
                        {
                            int ig;
                            if (int.TryParse(colVal, out ig))
                            {
                                colType = typeof(int);
                                defaultVal = 0;
                            }

                        }
                        else
                        {
                            float fl;
                            if (float.TryParse(colVal, out fl))
                            {
                                colType = typeof(float);
                                defaultVal = 0.0;
                            }
                        }
                    }

                    DataColumn newColumn = new DataColumn(colName, colType);
                    newColumn.DefaultValue = defaultVal;
                    newColumn.Unique = isUnique;

                    this.Columns.Add(newColumn);

                }

                this.Columns.Add("QueryCount", typeof(int));
                this.Columns["QueryCount"].DefaultValue = 1;
                this.QueryCountCol = this.Columns.IndexOf("QueryCount");
                return secondRow;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting Table Schema: {ex.Message}");
            }
        }

        public static void FormatRow(ref List<string> row, ref DataColumnCollection columns)
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

        public void AddRowToTable(List<string> inputRow)
        {
            object[] outputRow;
            List<DataRow> existingRows = (from DataRow r in this.Rows
                                          where r.ItemArray[this.QueryCol].ToString() == inputRow[this.QueryCol]
                                          select r).ToList();
            if (existingRows.Count > 0)
            {
                outputRow = this.AggregateRows(existingRows, inputRow, inputRow.Count);
                int rowIx = this.Rows.IndexOf(existingRows[0]);
                this.Rows[rowIx].ItemArray = outputRow;
            }
            else
            {
                outputRow = inputRow.ToArray();
                try
                {
                    this.Rows.Add(outputRow);

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

        public void AddRowToTable(object[] newRow)
        {

            try
            {
                this.Rows.Add(newRow);
            }
            catch (ConstraintException ex)
            {
                Console.WriteLine($"Duplicate Query attempted: {ex.Message}, {ex.Data}");
                var existingRows = (from DataRow row in this.Rows
                                    where row.ItemArray[this.QueryCol].ToString() == newRow[this.QueryCol].ToString()
                                    select row.ItemArray[this.QueryCol].ToString()).ToList();
                AddRowToTable(newRow, existingRows);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong adding new row to table: {ex.Message}");
            }

        }

        public void AddRowToTable(object[] aggregatedRow, ref List<DataRow> existingRows,ref  List<string> existingKeys)
        {            

            if (existingKeys.Count > 0)
            {
                object[] outputRow = AggregateRows(existingRows, aggregatedRow, aggregatedRow.Length);
                int rowIx = this.Rows.IndexOf(existingRows[0]);
                this.Rows[rowIx].ItemArray = outputRow;
            }
            else
            {
                try
                {
                    this.Rows.Add(aggregatedRow);
                }
                catch (ConstraintException ex)
                {
                    Console.WriteLine($"Duplicate Query attempted: {ex.Message}, {ex.Data}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong adding new row to table: {ex.Message}");
                }
            }
        }

        public void AddRowToTable(object[] aggregatedRow, List<string> existingKeys)
        {

            var existingRows = (from DataRow row in this.Rows
                                where existingKeys.Any(key => row.ItemArray[QueryCol].ToString() == key)
                                select row).ToList();

            if (existingRows.Count > 0)
            {
                object[] outputRow = AggregateRows(existingRows, aggregatedRow, aggregatedRow.Length);
                int rowIx = this.Rows.IndexOf(existingRows[0]);
                this.Rows[rowIx].ItemArray = outputRow;

            }
            else
            {
                try
                {
                    this.Rows.Add(aggregatedRow);
                }
                catch (ConstraintException ex)
                {
                    Console.WriteLine($"Duplicate Query attempted: {ex.Message}, {ex.Data}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong adding new row to table: {ex.Message}");
                }
            }
        }

        public object[] AggregateRows(List<DataRow> existingRows, object[] inputRow, int arrSize)
        {
            object[] outputArr = new object[arrSize];

            for (int i = 0; i < arrSize; i++)
            {

                List<object> columnValues = (from row in existingRows
                                             select row.ItemArray[i]).ToList();

                columnValues.Add(inputRow[i]);

                bool isAvg = Regexes.IsMatch(this.Columns[i].Caption, Regexes.Average);

                outputArr[i] = AggregateColumnValues(columnValues, this.Columns[i], isAvg);
            }

            return outputArr;
        }

        public object[] AggregateRows(List<DataRow> existingRows, List<string> inputRow, int arrSize)
        {
            object[] outputArr = new object[arrSize];

            for (int i = 0; i < arrSize; i++)
            {
                List<object> columnValues = (from row in existingRows
                                             select row.ItemArray[i]).ToList();

                columnValues.Add(inputRow[i]);

                bool isAvg = Regexes.IsMatch(this.Columns[i].Caption, Regexes.Average);

                outputArr[i] = AggregateColumnValues(columnValues, this.Columns[i], isAvg);
            }

            return outputArr;
        }

        /// <summary>
        /// Aggregates all the values in a single column
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="isAvg"></param>
        /// <returns></returns>
        public object AggregateColumnValues(List<object> columnValues, DataColumn column, bool isAvg = false)
        {
            try
            {
                if (columnValues.All(item => item.ToString() == "0"))
                {
                    return 0;
                }
                bool colValsAreNumbers = columnValues.All(val => Regexes.IsMatch(val.ToString(), Regexes.Number));
                if (Regexes.IsMatch(column.Caption, Regexes.Query) && columnValues.Count > 0 && !colValsAreNumbers)
                {
                    return columnValues[0];
                }
                if (Regexes.MatchesAnyStat(column.Caption) && colValsAreNumbers)
                {
                    if (isAvg)
                    {
                        return columnValues.Aggregate((sum, next) =>
                        {
                            string sumString = sum.ToString();
                            string nextString = next.ToString();
                            string sumMatch = Regexes.Match(sumString, Regexes.Number);
                            string nextMatch = Regexes.Match(nextString, Regexes.Number);
                            if (sum == next)
                            {
                                return sum;
                            }
                            if (column.DataType == typeof(decimal))
                            {
                                decimal sumNum, nextNum;
                                if (decimal.TryParse(nextMatch, out nextNum) && decimal.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            else if (column.DataType == typeof(double))
                            {
                                double sumNum, nextNum;
                                if (double.TryParse(nextMatch, out nextNum) && double.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            else if (column.DataType == typeof(long))
                            {
                                long sumNum, nextNum;
                                if (long.TryParse(nextMatch, out nextNum) && long.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            else
                            {
                                float sumNum, nextNum;
                                if (float.TryParse(nextMatch, out nextNum) && float.TryParse(sumMatch, out sumNum))
                                {
                                    sum = (sumNum + nextNum) / 2;
                                }
                            }
                            return sum;
                        });
                    }
                    else
                    {
                        var nonZeroRows = columnValues.Where(val => val.ToString() != "0" && val.ToString() != "0.00");
                        return nonZeroRows.Aggregate((sum, next) =>
                        {
                            string sumString = sum.ToString();
                            string nextString = next.ToString();
                            string sumMatch = Regexes.Match(sumString, Regexes.Number);
                            string nextMatch = Regexes.Match(nextString, Regexes.Number);
                            if (sum == next)
                            {
                                return sum;
                            }
                            if (column.DataType == typeof(decimal))
                            {
                                decimal sumNum, nextNum;
                                if (decimal.TryParse(nextMatch, out nextNum) && decimal.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }
                            }
                            else if (column.DataType == typeof(double))
                            {
                                double sumNum, nextNum;
                                if (double.TryParse(nextMatch, out nextNum) && double.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }
                            }
                            else if (column.DataType == typeof(long))
                            {
                                long sumNum, nextNum;
                                if (long.TryParse(nextMatch, out nextNum) && long.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }
                            }
                            else
                            {
                                float sumNum, nextNum;
                                if (float.TryParse(nextMatch, out nextNum) && float.TryParse(sumMatch, out sumNum))
                                {
                                    sum = sumNum + nextNum;
                                }

                            }
                            return sum;
                        });
                    }
                }
                else
                {
                    return columnValues.FirstOrDefault();
                } // end if/else for if all elements are numbers
                //   return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Aggregating values: {ex.Message}");
                return "Error";
            }
        }

    }

}
