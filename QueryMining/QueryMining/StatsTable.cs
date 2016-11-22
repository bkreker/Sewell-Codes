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
using System.Runtime.Serialization;

namespace QueryMining
{
    public class StatDataTable : DataTable
    {
        // public int WordCol { get; set; }
        public static int QueryCol { get; set; }
        public static int QueryCountCol { get; set; }
        public static bool AvgAll { get; set; }

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
                StatDataTable.QueryCountCol = this.Columns.IndexOf("QueryCount");
                ColumnCollection = this.Columns;
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

                this.Columns.Add("QueryCount", typeof(int));
                this.Columns["QueryCount"].DefaultValue = 1;
                StatDataTable.QueryCountCol = this.Columns.IndexOf("QueryCount");
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

                    if (StatDataTable.ColumnCollection == null)
                    {
                        SetStaticColumns();
                    }

                }

                this.Columns.Add("QueryCount", typeof(int));
                this.Columns["QueryCount"].DefaultValue = 1;
                StatDataTable.QueryCountCol = this.Columns.IndexOf("QueryCount");
                ColumnCollection = this.Columns;
                return secondRow;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Setting Table Schema: {ex.Message}");
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
                string colName = "", cellVal = item.ToString();

                Type type = StatDataTable.ColumnCollection[i].DataType;
                colName = StatDataTable.ColumnCollection[i].Caption;

                if (type == typeof(string))
                    return item.ToString();

                if (Regexes.IsMatch(cellVal, Regexes.Number) && i != QueryCol)
                {
                    if (cellVal.Contains('%'))
                        cellVal = cellVal.Remove(cellVal.IndexOf('%'), 1);

                    if (cellVal.Contains(','))
                        cellVal = cellVal.Remove(cellVal.IndexOf(','), 1);

                    if (type == typeof(double))
                    {
                        double dub;
                        if (double.TryParse(cellVal, out dub))
                        {
                            return dub;
                        }
                    }
                    else if (type == typeof(decimal))
                    {
                        decimal dec;
                        if (decimal.TryParse(cellVal, out dec))
                        {
                            return dec;
                        }
                    }
                    else if (type == typeof(long))
                    {
                        long lon;
                        if (long.TryParse(cellVal, out lon))
                        {
                            return lon;
                        }

                    }
                    else if (type == typeof(int))
                    {
                        int ig;
                        if (int.TryParse(cellVal, out ig))
                        {
                            return ig;
                        }
                    }

                    float fl;
                    if (float.TryParse(cellVal, out fl))
                    {
                        return fl;
                    }

                }
                return cellVal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Formatting cell from value {item}: {ex.Message}");
                return "Error";
            }
        }

        public void AddRowToTable(List<string> newRow)
        {
            AddRowToTable(newRow.ToArray<object>());
        }

        //public void AddRowToTable(object[] newRow)
        //{
        //    try
        //    {
        //        for (int i = 0; i < newRow.Count(); i++)
        //        {
        //            newRow[i] = FormatCell(newRow[i], i);
        //        }
        //        this.Rows.Add(newRow);
        //    }
        //    catch (ConstraintException ex)
        //    {
        //        Console.WriteLine($"Duplicate Query attempted: {ex.Message}, {ex.Data}");
        //        var existingQueries = (from DataRow row in this.Rows
        //                            where row.ItemArray[StatDataTable.QueryCol].ToString() == newRow[StatDataTable.QueryCol].ToString()
        //                            select row.ItemArray[StatDataTable.QueryCol].ToString()).ToList();

        //        AddRowToTable(newRow, existingQueries);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Something went wrong adding new row to table: {ex.Message}");
        //    }
        //}

        public void AddRowToTable(object[] newRow, List<string> existingQueries = null, List<DataRow> existingRows = null)
        {
            try
            {
                for (int i = 0; i < newRow.Count(); i++)
                {
                    newRow[i] = FormatCell(newRow[i], i);
                }
                this.Rows.Add(newRow);
            }
            catch (ConstraintException ex)
            {
                Console.WriteLine($"Attempted to add duplicate key. Attempting Aggregation.");
                try
                {
                    if (existingRows == null)
                        existingRows = (from DataRow row in this.Rows
                                        where row.ItemArray[StatDataTable.QueryCol] == newRow[StatDataTable.QueryCol]
                                        select row).ToList();

                    if (existingQueries == null)
                        existingQueries = (from DataRow row in this.Rows
                                           where row.ItemArray[StatDataTable.QueryCol] == newRow[StatDataTable.QueryCol]
                                           select row.ItemArray[QueryCol].ToString()).ToList();

                    if (existingQueries.Count > 0)
                    {
                        object[] outputRow = AggregateRows(existingRows, newRow, newRow.Length);
                        int rowIx = this.Rows.IndexOf(existingRows[0]);
                        this.Rows[rowIx].ItemArray = outputRow;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Something went wrong adding new row to table: {e.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong adding new row to table: {ex.Message}");
            }

        }

        /*public void AddRowToTable(object[] newRow, List<string> existingQueries)
        {
            try
            {
                var existingRows = (from DataRow row in this.Rows
                                    where existingQueries.Any(key => row.ItemArray[QueryCol].ToString() == key)
                                    select row).ToList();

                if (existingRows.Count > 0)
                {
                    object[] outputRow = AggregateRows(existingRows, newRow, newRow.Length);
                    int rowIx = this.Rows.IndexOf(existingRows[0]);
                    this.Rows[rowIx].ItemArray = outputRow;

                }
                else
                {
                    try
                    {
                        this.Rows.Add(newRow);
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
            catch (Exception ex)
            {
                throw new ImportError(ex.Message);
            }
        }
        */

        public static object[] AggregateRows(List<DataRow> existingRows, object[] inputRow, int arrSize)
        {
            object[] outputArr = inputRow;

            for (int i = 0; i < inputRow.Count(); i++)
            {
                inputRow[i] = FormatCell(inputRow[i], i);

                List<object> columnValues = (from row in existingRows
                                             select row.ItemArray[i]).ToList();

                columnValues.Add(inputRow[i]);

                bool isAvg = Regexes.IsMatch(StatDataTable.ColumnCollection[i].Caption, Regexes.Average) || AvgAll;

                outputArr[i] = AggregateColumnValues(columnValues, StatDataTable.ColumnCollection[i], isAvg);
            }

            return outputArr;
        }

        public static object[] AggregateRows(List<object[]> existingRows, object[] inputRow, int arrSize)
        {
            object[] outputArr = inputRow;

            for (int i = 0; i < inputRow.Count(); i++)
            {
                inputRow[i] = FormatCell(inputRow[i], i);

                List<object> columnValues = (from row in existingRows
                                             select row[i]).ToList();

                columnValues.Add(inputRow[i]);

                bool isAvg = Regexes.IsMatch(StatDataTable.ColumnCollection[i].Caption, Regexes.Average) || AvgAll;

                outputArr[i] = AggregateColumnValues(columnValues, StatDataTable.ColumnCollection[i], isAvg);
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
        public static object[] AggregateRows(string query, string wordString, List<DataRow> existingRows, ref bool _operationCancelled)
        {
            object[] aggregatedRow = new object[StatDataTable.ColumnCollection.Count];
            for (int columnIndex = 0; columnIndex < StatDataTable.ColumnCollection.Count; columnIndex++)
            {
                if (_operationCancelled)
                    throw new OperationCanceledException();

                try
                {
                    object columnTotal = "N/A";
                    if (columnIndex == StatDataTable.QueryCol)
                    {
                        columnTotal = query == wordString ? query : wordString;
                    }
                    else if (columnIndex == StatDataTable.QueryCol)
                    {
                        columnTotal = existingRows.Count;
                    }
                    else
                    {
                        List<object> columnValues = (from resRow in existingRows
                                                     select resRow.ItemArray[columnIndex]).ToList();

                        columnTotal = StatDataTable.AggregateRowColumnValues(columnValues, columnIndex);
                    }
                    aggregatedRow[columnIndex] = columnTotal;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Aggregating Column Values: {ex.Message}");
                }
            } // end for

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
                if (columnValues.All(item => item.ToString() == "0"))
                    return 0;

                bool colValsAreNumbers = columnValues.All(val => Regexes.IsMatch(val.ToString(), Regexes.Number));

                if (Regexes.IsMatch(column.Caption, Regexes.Query) && columnValues.Count > 0 && !colValsAreNumbers)
                    return columnValues[0];

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


        public static object AggregateRowColumnValues(List<object> columnValues, int col_index)
        {
            if (col_index == StatDataTable.QueryCol)
                return columnValues.Count();


            Type colDataType = StatDataTable.ColumnCollection[col_index].DataType;
            string colName = StatDataTable.ColumnCollection[col_index].Caption;
            if (Regexes.MatchesAnyStat(colName))
            {
                bool isAvg = Regexes.IsMatch(colName, Regexes.Average);
                isAvg = isAvg || AvgAll;
                return StatDataTable.AggregateColumnValues(columnValues, StatDataTable.ColumnCollection[col_index], isAvg);
            }
            else
            {
                if (columnValues.Any(a => Regexes.IsMatch(a.ToString(), Regexes.Number)) && colDataType == typeof(string))
                {
                    columnValues.ForEach(val => val = decimal.Parse(Regexes.Match(val.ToString(), Regexes.Number)));
                    bool isAvg = columnValues.Any(a => Regexes.IsMatch(a.ToString(), Regexes.Percent) || Regexes.IsMatch(colName, Regexes.Average));

                    isAvg = isAvg || AvgAll;

                    return StatDataTable.AggregateColumnValues(columnValues, StatDataTable.ColumnCollection[col_index], isAvg).ToString();
                }
                else
                {
                    return columnValues.Aggregate((sum, next) =>
                    {
                        string s = sum.ToString() + " " + next.ToString();
                        return s;
                    });
                }
            }
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
