using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinqToExcel;
using LinqToExcel.Query;

namespace Common.Configuration
{
    /// <summary>
    /// Get configuration data from \TestConfig\TestConfig.xlsx
    /// </summary>
    internal class DataGetter : IDisposable
    {
        private static ExcelQueryFactory _excelFile;

        public DataGetter()
        {
            var path = Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                @"..\..\..\TestConfig\TestConfig.xlsx")
            );

            _excelFile = new ExcelQueryFactory(path)
            { 
                UsePersistentConnection = false,
                ReadOnly = true,
                TrimSpaces = TrimSpacesType.Both
            };
        }

        /// <summary>
        /// Gets spreadsheet row
        /// </summary>
        /// <param name="sheetName">Worksheet name</param>
        /// <param name="columnName">Column name</param>
        /// <param name="columnValue">First column value</param>
        /// <returns>(<see cref="Row"/>) Returns Row object</returns>
        private Row GetRow(string sheetName, string columnName, string columnValue)
        {
            try
            {
                var result = !string.IsNullOrEmpty(columnValue)
                    ? GetSheetData(sheetName).FirstOrDefault(x => x[columnName] == columnValue)
                    : GetSheetData(sheetName).FirstOrDefault();
                return result;
            }      
        
            catch
            {
                throw new Exception("Microsoft ACE OLEDB x86 driver " +
                    "not installed (see https://www.microsoft.com/en-us/download/details.aspx?id=54920) " +
                    "or error in config file TestConfig.xlsx. " +
                    "Check whether or not the following items exist and correct: " +
                    $"sheet '{sheetName}', column name '{columnName}', column value '{columnValue}'.");
            }
        }

        /// <summary>
        /// Returns dictionary with specific sheet row data by given parameters. Can process sheet data
        /// that presented as 'many columns and 1 row' or 'many columns and many rows' 
        /// </summary>
        /// <param name="sheetName">Sheet name</param>
        /// <param name="columnName">Column name</param>
        /// <param name="columnValue">First column value</param>
        /// <param name="keys">Additional column names to process (optional)</param>
        /// <returns>(<see cref="IDictionary{String, String}"/>) Dictionary with row data. Parameter name as Key
        /// and parameter value as Value.</returns>
        internal IDictionary<string, string> GetRowData(string sheetName, string columnName, 
            string columnValue, params string[] keys)
        {
            var result = new Dictionary<string, string>();
            var row = GetRow(sheetName, columnName, columnValue);
            if (string.IsNullOrEmpty(columnValue.Trim()))
            {
                result.Add(columnName, row[columnName]);
            }
            foreach (var key in keys)
            {
                result.Add(key, row[key]);
            }
            return result;
        }

        /// <summary>
        /// Extracts specific sheet data (non-empty columns and rows)
        /// </summary>
        /// <param name="sheetName">Sheet name</param>
        /// <returns>(<see cref="IQueryable{Row}"/>) Returns Linq query</returns>
        private IQueryable<Row> GetSheetData(string sheetName)
        {
            return _excelFile.Worksheet(sheetName);
        }

        /// <summary>
        /// Closes all connections and disposes data.
        /// </summary>
        public void Dispose()
        {
            _excelFile.Dispose();
        }
    }
}

