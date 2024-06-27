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
    public abstract class DynamicDatabaseReader
    {
        protected SQLiteConnection connection;

        //Setting up the connection with SQLite data base 
        public DynamicDatabaseReader(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);
        }

        //Getting tables names from data base
        public abstract Task<List<string>> GetTableNamesAsync();

        //Getting colums names from specify table
        public abstract Task<List<string>> GetColumsNamesAsync(string tableName);

        //Getting relations between tables
        public abstract Task<bool> GetRelationsAsync(string tableName);

        //Getting content from specify table
        public abstract Task<ResponseModel> GetTableContetAsync(string tableName);

    }
}
