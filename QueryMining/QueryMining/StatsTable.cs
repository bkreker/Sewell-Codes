﻿using System;
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
using QueryMining.Forms;
using System.Runtime.Serialization;

namespace QueryMining
{
    public class StatDataTable : DataTable
    {
        public static int QueryCol { get; set; }
        public static int QueryCountCol { get; set; }
        public static int MatchedKeywordCol { get; set; }
        public static bool AvgAll
        {
            get { return Program.AvgAll; }
            set { Program.AvgAll = value; }
        }
        public static bool TableChanged { get; set; }
        public static int RowCount { get; set; }
        public static DataColumnCollection ColumnCollection { get; set; }
        public static List<string> Headers
        {
            get
            {
                return (from DataColumn col in StatDataTable.ColumnCollection
                        select col.Caption).ToList();
            }
        }
        public StatDataTable()
        {
            StatDataTable.RowCount = 0;
            StatDataTable.ColumnCollection = null;

            StatDataTable.QueryCol = 0;
            StatDataTable.QueryCountCol = 0;
            StatDataTable.MatchedKeywordCol = 1;

            Program.Processing = false;
            Program.OperationCancelled = false;
        }

        public StatDataTable(string firstRowString, string secondRowString, char delimChar = ',')
        {
            try
            {
                if (firstRowString.IndexOf('\t') >= 0)
                {
                    delimChar = '\t';
                }
                List<string> firstRow = firstRowString.Split(delimChar).ToList<string>();
                List<string> secondRow = secondRowString.Split(delimChar).ToList<string>();

                SetTableSchema(firstRow, secondRow);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting Table Schema: {ex.Message}");
            }
        }

        /// <summary>
        /// Read in the data from the file, after selecting the query column
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="delimChar"></param>
        /// <param name="_processing"></param>
        /// <param name="_operationCancelled"></param>
        /// <returns></returns>
        public bool FillTable(ref StreamReader inFile, char delimChar, ref bool _processing, ref bool _operationCancelled)
        {
            DataColumnCollection columns = StatDataTable.ColumnCollection == null ? this.Columns : StatDataTable.ColumnCollection;

            ColumnHeaderSelect c = new ColumnHeaderSelect(columns);
            if (c.DialogResult == DialogResult.OK)
            {
                StatDataTable.QueryCol = c.SelectedIndex;

                List<string> headerRow = (from DataColumn h in columns
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
                        List<string> inputRow = (inFile.ReadLine().Split(delimChar)).ToList();
                        StatDataTable.FormatRow(ref inputRow);
                        this.AddRowToTable(inputRow);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error importing row: {ex.Message}");
                        return false;
                    }

                }
            }
            return true;
        }

        /// <summary>
        /// Initializes the StatDataTable and sets the schema, and adds the header row and first data row.
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="secondRow"></param>
        public StatDataTable(List<string> firstRow, List<string> secondRow)
        {
            try
            {
                SetTableSchema(firstRow, secondRow);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting Table Schema: {ex.Message}");
            }
        }

        public void SetTableSchema(List<string> firstRow, List<string> secondRow)
        {
            try
            {
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
                            // Column is int, but just check that it's a number, so I'm using float to account for extra decimals in the file
                            float fl;
                            if (float.TryParse(colVal, out fl))
                            {
                                colType = typeof(int);
                                defaultVal = 0;

                            }

                        }
                        if (defaultVal.ToString() == "")
                        {
                            float fl;
                            if (float.TryParse(colVal, out fl))
                            {
                                colType = typeof(float);
                                defaultVal = 0.0;
                            }
                            else
                            {
                                colType = typeof(string);
                                defaultVal = "";
                            }
                        }
                    }

                    DataColumn newColumn = new DataColumn(colName, colType);
                    newColumn.DefaultValue = defaultVal;
                    newColumn.Unique = isUnique;

                    this.Columns.Add(newColumn);

                }

                DataColumn queryCountColumn = new DataColumn("QueryCount", typeof(int));
                queryCountColumn.DefaultValue = 1;
                this.Columns.Add(queryCountColumn);
                StatDataTable.QueryCountCol = this.Columns.IndexOf("QueryCount");

                ColumnCollection = this.Columns;
                if (StatDataTable.ColumnCollection == null)
                {
                    SetStaticColumns();
                }

                this.AddRowToTable(secondRow);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting Table Schema: {ex.Message}");
            }
        }

        public void SetStaticColumns()
        {
            StatDataTable.ColumnCollection = this.Columns;
        }
        public void ClearStaticColumns()
        {
            StatDataTable.ColumnCollection = null;
        }

        public List<string> SetTableSchema(ref StreamReader inFile, char delimChar)
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

            SetTableSchema(firstRow, secondRow);
            return secondRow;

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

        public static void FormatRow(ref List<string> row)
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
                        Console.WriteLine($"Error formatting row cell {i}: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Returns a single cell's value, formatted into the correct type
        /// </summary>
        /// <param name="item"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static object FormatCell(object item, int i)
        {
            try
            {

                string colName = "",
                    cellVal = item.ToString();

                Type type = StatDataTable.ColumnCollection[i].DataType;
                colName = StatDataTable.ColumnCollection[i].Caption;
                //String
                if (type == typeof(string))
                    return item.ToString();
                if (i != QueryCol && Regexes.IsMatch(cellVal, Regexes.Number))
                {
                    if (cellVal.Contains('%'))
                        cellVal = cellVal.Remove(cellVal.IndexOf('%'), 1);

                    if (cellVal.Contains(','))
                        cellVal = cellVal.Remove(cellVal.IndexOf(','), 1);

                    float fl;
                    switch (type.Name)
                    {
                        case "Double":
                            double dub;
                            if (double.TryParse(cellVal, out dub)) return dub;
                            break;
                        case "Decimal":
                            decimal dec;
                            if (decimal.TryParse(cellVal, out dec)) return dec;
                            break;
                        case "Int32":
                            int ig;
                            if (cellVal.Contains("."))
                                if (float.TryParse(cellVal, out fl)) return (int)fl;
                            if (int.TryParse(cellVal, out ig)) return ig;
                            break;
                        case "Int64":
                            long lon;
                            if (long.TryParse(cellVal, out lon)) return lon;
                            break;
                        case "Single":
                            if (float.TryParse(cellVal, out fl)) return fl;
                            break;
                        default:
                            if (float.TryParse(cellVal, out fl)) return fl;
                            break;
                    }

                    if (float.TryParse(cellVal, out fl)) return fl;
                }
                return cellVal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Formatting cell from value {item}: {ex.Message}");
                return "Error";
            }
        }

        public bool AddRowToTable(IEnumerable<string> newRow)
        {

            return AddRowToTable(newRow.ToArray<object>());
        }

        public bool AddRowToTable(object[] newRow, List<DataRow> existingRows = null)
        {
            var unchangedRowCount = StatDataTable.RowCount;
            StatDataTable.TableChanged = false;
            for (int i = 0; i < newRow.Count(); i++)
            {
                newRow[i] = FormatCell(newRow[i], i);
            }
            if (existingRows == null)
            {
                existingRows = (from DataRow row in this.Rows
                                where row.ItemArray[StatDataTable.QueryCol].ToString() == newRow[StatDataTable.QueryCol].ToString()
                                select row).ToList();
            }
            try
            {
                if (existingRows.Count == 0)
                {
                    StatDataTable.RowCount++;
                    this.Rows.Add(newRow);
                    return true;
                }
                else
                {
                    try
                    {

                        if (existingRows.Count > 0)
                        {
                            object[] outputRow = AggregateRows(existingRows, newRow);
                            int rowIx = this.Rows.IndexOf(existingRows[0]);
                            this.Rows[rowIx].ItemArray = outputRow;
                            return true;

                        }
                        else
                        {
                            RowCount = unchangedRowCount;
                            Console.WriteLine($"Row Not Added");
                        }
                    }
                    catch (NullReferenceException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        throw new ImportError($"Something went wrong adding new row to table: {e.Message}");
                    }
                }
            }
            catch (ConstraintException ex)
            {
                existingRows = (from DataRow row in this.Rows
                                where row.ItemArray[StatDataTable.QueryCol].ToString() == newRow[StatDataTable.QueryCol].ToString()
                                select row).ToList();
                if (existingRows.Count > 0)
                {
                    object[] outputRow = AggregateRows(existingRows, newRow);
                    int rowIx = this.Rows.IndexOf(existingRows[0]);
                    this.Rows[rowIx].ItemArray = outputRow;
                    return true;
                }
                else
                {
                    RowCount = unchangedRowCount;
                    Console.WriteLine($"Row Not Added, {ex.Message}");
                }

            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Null Reference error when adding new row to table: {ex.Message}");
            }
            catch (Exception ex)
            {
                RowCount = unchangedRowCount;
                Console.WriteLine($"Something went wrong adding new row to table: {ex.Message}");
            }
            return false;
        }

        public static object[] AggregateRows(IEnumerable<DataRow> existingRows, object[] inputRow)
        {
            return AggregateRows((from DataRow row in existingRows
                                  select row.ItemArray), inputRow);
        }

        public static object[] AggregateRows(IEnumerable<object[]> existingRows, object[] inputRow)
        {
            object[] outputArr = new object[ColumnCollection.Count];
            inputRow.CopyTo(outputArr, 0);
            for (int col_ix = 0; col_ix < StatDataTable.ColumnCollection.Count && col_ix < outputArr.Length; col_ix++)
            {
                if (col_ix == StatDataTable.QueryCountCol)
                {
                    outputArr[col_ix] = existingRows.Count();
                }
                else if (col_ix == StatDataTable.MatchedKeywordCol)
                {
                    outputArr[StatDataTable.MatchedKeywordCol] = inputRow[StatDataTable.MatchedKeywordCol];
                }
                else
                {
                    if (outputArr[col_ix] != null)
                    {
                        outputArr[col_ix] = FormatCell(outputArr[col_ix], col_ix);

                    }
                    List<object> allColumnValues = (from row in existingRows
                                                    select row[col_ix]).ToList();

                    allColumnValues.Add(outputArr[col_ix]);

                    bool isAvg = Regexes.IsMatch(StatDataTable.ColumnCollection[col_ix].Caption, Regexes.Average) || AvgAll;

                    outputArr[col_ix] = AggregateColumnValues(allColumnValues, StatDataTable.ColumnCollection[col_ix], isAvg);
                }
            }

            return outputArr;
        }

        /// <summary>
        /// Aggregates all rows of the given criteria
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="query"></param>
        /// <param name="wordPair"></param>
        /// <param name="newRow"></param>
        public static object[] Mine(string wordString, List<DataRow> existingRows)
        {
            object[] aggregatedRow = new object[StatDataTable.ColumnCollection.Count];
            for (int columnIndex = 0; columnIndex < StatDataTable.ColumnCollection.Count; columnIndex++)
            {
                try
                {
                    if (columnIndex == StatDataTable.QueryCol)
                    {
                        aggregatedRow[StatDataTable.QueryCol] = wordString;
                    }
                    else if (columnIndex == StatDataTable.QueryCountCol)
                    {
                        aggregatedRow[StatDataTable.QueryCountCol] = existingRows.Count;
                    }
                    else
                    {
                        object columnTotal = "N/A";
                        List<object> columnValues = (from resRow in existingRows
                                                     select resRow.ItemArray[columnIndex]).ToList();

                        columnTotal = StatDataTable.AggregateColumnValues(columnValues, columnIndex);

                        aggregatedRow[columnIndex] = columnTotal;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Invalid Operation in Mine({wordString}, {existingRows.Count}:\n{ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Aggregating Column Values: {ex.Message}");
                }
            } // end for
            aggregatedRow[QueryCol] = wordString;
            return aggregatedRow;
        }

        /// <summary>
        /// Aggregates all the values in a single column
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="isAvg"></param>
        /// <returns></returns>
        public static object AggregateColumnValues(List<object> columnValues, DataColumn column, bool isAvg = true)
        {
            try
            {
                if (column == ColumnCollection[QueryCol])
                    return columnValues[0];

                if (columnValues.All(item => item.ToString() == "0"))
                    return 0;

                bool colValsAreNumbers = columnValues.All(val => Regexes.IsMatch(val.ToString(), Regexes.Number));

                if ((Regexes.IsMatch(column.Caption, Regexes.Query) && columnValues.Count > 0 && !colValsAreNumbers) || column.DataType == typeof(string))
                    return columnValues[0];

                if (column == ColumnCollection[QueryCountCol])
                    return columnValues.Aggregate((a, b) => (int)a + (int)b);

                if (Regexes.MatchesAnyStat(column.Caption) && colValsAreNumbers)
                {
                    if (isAvg)
                    {
                        switch (column.DataType.Name)
                        {
                            case "Double":
                                return columnValues.Cast<double>().Average();
                            case "Decimal":
                                return columnValues.Cast<decimal>().Average();
                            case "Int32":
                                return columnValues.Cast<int>().Average();
                            case "Int64":
                                return columnValues.Cast<long>().Average();
                            case "Single":
                                return columnValues.Cast<float>().Average();
                            default:
                                return columnValues[0];
                        }
                    }
                    else
                    {
                        switch (column.DataType.Name)
                        {
                            case "Double":
                                return columnValues.Cast<double>().Sum();
                            case "Decimal":
                                return columnValues.Cast<decimal>().Sum();
                            case "Int32":
                                return columnValues.Cast<int>().Sum();
                            case "Int64":
                                return columnValues.Cast<long>().Sum();
                            case "Single":
                                return columnValues.Cast<float>().Sum();
                            default:
                                return columnValues[0];
                        }
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
        /*  return columnValues.Aggregate((sum, next) =>
                     {
                         string sumString = sum.ToString();
                         string nextString = next.ToString();
                         if (sumString == nextString && sumString == "0" || sumString == "0.00")
                         {
                             return sumString;
                         }
                         string sumMatch = Regexes.Match(sumString, Regexes.Number);
                         string nextMatch = Regexes.Match(nextString, Regexes.Number);
                         double divisor = 2.0;
                         switch (type.Name)
                         {
                             case "Double":
                                 double sumDub, nextDub;
                                 if (double.TryParse(nextMatch, out nextDub) && double.TryParse(sumMatch, out sumDub))
                                     return (sumDub + nextDub) / divisor;
                                 break;
                             case "Decimal":
                                 decimal sumDec, nextDec;
                                 if (decimal.TryParse(nextMatch, out nextDec) && decimal.TryParse(sumMatch, out sumDec))
                                     return (decimal)(sumDec + nextDec) / (decimal)divisor;
                                 break;
                             case "Int32":
                                 int sumInt, nextInt;
                                 if (int.TryParse(nextMatch, out sumInt) && int.TryParse(sumMatch, out nextInt))
                                     return (int)(sumInt + nextInt) / divisor;
                                 break;
                             case "Int64":
                                 long sumLong, nextLong;
                                 if (long.TryParse(nextMatch, out nextLong) && long.TryParse(sumMatch, out sumLong))
                                     return (long)(sumLong + (double)nextLong) / divisor;
                                 break;
                             case "Single":
                                 float sumFloat, nextFloat;
                                 if (float.TryParse(nextMatch, out nextFloat) && float.TryParse(sumMatch, out sumFloat))
                                     return (float)(sumFloat + nextFloat) / divisor;
                                 break;
                             default:
                                 float sumNum, nextNum;
                                 if (float.TryParse(nextMatch, out nextNum) && float.TryParse(sumMatch, out sumNum))
                                     return (float)(sumNum + nextNum) / divisor;
                                 break;
                         }

                         return sum;
                     });
                        var nonZeroRows = columnValues.Where(val => val.ToString() != "0" && val.ToString() != "0.00");
                        return nonZeroRows.Aggregate((sum, next) =>
                        {
                            string sumString = sum.ToString();
                            string nextString = next.ToString();
                            string sumMatch = Regexes.Match(sumString, Regexes.Number);
                            string nextMatch = Regexes.Match(nextString, Regexes.Number);

                            var type = column.DataType;
                            switch (type.Name)
                            {
                                case "Double":
                                    double sumDub, nextDub;
                                    if (double.TryParse(nextMatch, out nextDub) && double.TryParse(sumMatch, out sumDub))
                                        return (sumDub + nextDub);
                                    break;
                                case "Decimal":
                                    decimal sumDec, nextDec;
                                    if (decimal.TryParse(nextMatch, out nextDec) && decimal.TryParse(sumMatch, out sumDec))
                                        return (decimal)(sumDec + nextDec);
                                    break;
                                case "Int32":
                                    int sumInt, nextInt;
                                    if (int.TryParse(nextMatch, out sumInt) && int.TryParse(sumMatch, out nextInt))
                                        return (int)(sumInt + nextInt);
                                    break;
                                case "Int64":
                                    long sumLong, nextLong;
                                    if (long.TryParse(nextMatch, out nextLong) && long.TryParse(sumMatch, out sumLong))
                                        return (long)(sumLong + (double)nextLong);
                                    break;
                                case "Single":
                                    float sumFloat, nextFloat;
                                    if (float.TryParse(nextMatch, out nextFloat) && float.TryParse(sumMatch, out sumFloat))
                                        return (float)(sumFloat + nextFloat);
                                    break;
                                default:
                                    float sumNum, nextNum;
                                    if (float.TryParse(nextMatch, out nextNum) && float.TryParse(sumMatch, out sumNum))
                                        return (float)(sumNum + nextNum);
                                    break;
                            }

                            return sum;
                        });
                     */

        public static object AggregateColumnValues(List<object> columnValues, int col_index)
        {
            var column = StatDataTable.ColumnCollection[col_index];
            string colName = column.Caption;
            bool isAvg = Regexes.IsMatch(colName, Regexes.Average);

            return AggregateColumnValues(columnValues, column, isAvg || AvgAll);

        }

        [Serializable]
        private class ImportError : Exception
        {
            public ImportError()
            {
            }

            public ImportError(string message) : base(message)
            {
            }

            public ImportError(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected ImportError(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

}
