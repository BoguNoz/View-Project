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
    /// DynamicDBReader is abstract class used as framework for thier children classes, and contains all necessary methods used for collecting necesary dta from data base.
    /// It was created because the implementation of this class may differ for each type of SQL database
    /// </summary>
    public abstract class DynamicDBReader
    {
        protected SQLiteConnection connection;

        /// <summary>
        /// Base constructor responsible for initialization of new instance of DynamicDatabaseReade
        /// </summary>
        /// <param name="connectionString">Data base connection string must be in correct format in order to work.</param>
        public DynamicDBReader(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// GetTableNamesAsync gets tables names from database asynchronous 
        /// </summary>
        /// <returns>Returns List of strings containing table names</returns>
        public abstract Task<ResponseModel> GetTableNamesAsync();

        /// <summary>
        /// GetColumsNamesAsync gets colums names from specify table asynchronous 
        /// </summary>
        /// <param name="table">Table in use</param>
        /// <returns>Returns List of strings containing columns names</returns>
        public abstract Task<ResponseModel> GetColumsNamesAsync(string table);

        /// <summary>
        /// GetRelationsAsync get relations between tables asynchronous 
        /// </summary>
        /// <returns>Returns Dictionary(string,string) containing relations (table : table). All relations are unique and do not repeat</returns>
        public abstract Task<ResponseModel> GetRelationsAsync();

        /// <summary>
        /// GetColumsContetAsync gets content from specify colums asynchronous 
        /// </summary>
        /// <param name="table">Table in use</param>
        /// <param name="column">Column in use</param>
        /// <param name="orderBy">Column used to order by data</param>
        /// <returns>Returns List of strings containing content of column</returns>
        public abstract Task<ResponseModel> GetColumsContetAsync(string table, string column, string? orderBy);

        /// <summary>
        /// GetColumnDataTypeAsync gets type of data in column asynchronous
        /// </summary>
        /// <param name="table">Table in use</param>
        /// <param name="column">Column in use</param>
        /// <returns>Returns single string containing corect data type</returns>
        public abstract Task<ResponseModel> GetColumnDataTypeAsync(string table, string column);

        /// <summary>
        /// GetPrimaryKeysAsync gets dictionary of columns as keys with value being thier status as primary keys for specify table
        /// </summary>
        /// <param name="table">Table in use</param>
        /// <returns>Returns Dictionary(string,bool) containing (names of column : is it PK) in specify table. False is not PK, true is PK</returns>
        public abstract Task<ResponseModel> GetPrimaryKeysAsync(string table);

        /// <summary>
        /// GetForeignKeysAsync gets dictionary of columns as keys with value being thier status as foregin keys for specify table
        /// </summary>
        /// <param name="table">Table in use</param>
        /// <returns>Returns Dictionary(string,bool) containing (names of column : is it PK) in specify table. False is not FK, true is FK</returns>
        public abstract Task<ResponseModel> GetForeignKeysAsync(string table);

    }
}
