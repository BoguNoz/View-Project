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
    public abstract class DynamicDBReader
    {
        protected SQLiteConnection connection;

        //Setting up the connection with SQLite data base 
        public DynamicDBReader(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);
        }

        //Getting tables names from data base
        public abstract Task<List<string>> GetTableNamesAsync();

        /// <summary>
        /// Getting colums names from specify table
        /// </summary>
        /// <param name="table">Table in use</param>
        public abstract Task<List<string>> GetColumsNamesAsync(string table);

        //Getting relations between tables
        public abstract Task<Dictionary<string, string>> GetRelationsAsync();

        /// <summary>
        /// Getting content from specify colums
        /// </summary>
        /// <param name="table">Table in use</param>
        /// <param name="column">Column in use</param>
        /// <param name="orderBy">Column used to order by data</param>
        public abstract Task<List<string?>> GetColumsContetAsync(string table, string column, string? orderBy);


    }
}
