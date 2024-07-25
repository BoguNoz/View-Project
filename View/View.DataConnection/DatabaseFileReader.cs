using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CoreFeatures.ResposeModel;

namespace View.DataConnection
{
    /// <summary>
    /// Abstract class used as a framework for child classes to collect necessary data from a database.
    /// Implementation may differ for each type of SQL database.
    /// </summary>
    public abstract class DatabaseFileReader
    {

        protected string connectionString;


        /// <summary>
        /// Initializes a new instance of DynamicDBReader.
        /// </summary>
        /// <param name="connectionString">Database connection string (must be in the correct format).</param>
        public DatabaseFileReader(string connectionStr)
        {
            connectionString = connectionStr;   
        }


        /// <summary>
        /// Gets table names from the database asynchronously.
        /// </summary>
        /// <returns>List of strings containing table names.</returns>
        public abstract Task<ResponseModel<List<string?>>> GetTableNamesAsync();


        /// <summary>
        /// Gets column names from the specified table asynchronously.
        /// </summary>
        /// <param name="table">Table in use.</param>
        /// <returns>List of strings containing column names.</returns>
        public abstract Task<ResponseModel<List<string?>>> GetColumsNamesAsync(string table);


        /// <summary>
        /// Gets relations between tables asynchronously.
        /// </summary>
        /// <returns>Dictionary (table : List of related tables). All relations are unique and do not repeat.</returns>
        public abstract Task<ResponseModel<Dictionary<string,List<string>>>> GetRelationsAsync();


        /// <summary>
        /// Gets content from specified columns asynchronously.
        /// </summary>
        /// <param name="table">Table in use.</param>
        /// <param name="column">Column in use.</param>
        /// <param name="orderBy">Column used to order data.</param>
        /// <returns>List of strings containing column content.</returns>
        public abstract Task<ResponseModel<List<string?>>> GetColumsContetAsync(string table, string column, string? orderBy);


        /// <summary>
        /// Gets the data type of a column asynchronously.
        /// </summary>
        /// <param name="table">Table in use.</param>
        /// <param name="column">Column in use.</param>
        /// <returns>Single string containing the correct data type.</returns>
        public abstract Task<ResponseModel<string>> GetColumnDataTypeAsync(string table, string column);


        /// <summary>
        /// Gets primary keys for specified table asynchronously.
        /// Note: Not all columns in the table will be included in the dictionary.
        /// </summary>
        /// <param name="table">Table in use.</param>
        /// <returns>Dictionary (column names : is it a primary key).</returns>
        public abstract Task<ResponseModel<Dictionary<string, bool>>> GetPrimaryKeysAsync(string table);


        /// <summary>
        /// Gets foreign keys for specified table asynchronously.
        /// Note: Not all columns in the table will be included in the dictionary.
        /// </summary>
        /// <param name="table">Table in use.</param>
        /// <returns>Dictionary (column names : is it a foreign key).</returns>
        public abstract Task<ResponseModel<Dictionary<string, bool>>> GetForeignKeysAsync(string table);

    }
}
