using CoreFeatures.ResposeModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DataConnection
{
    public class SQLiteDynamicDatabaseReader : DynamicDatabaseReader
    {
        /// <summary>
        /// Initialization of new instance of SQLiteDynamicDatabaseReade
        /// </summary>
        /// <param name="connectionString">Data base connection string (SQLite only).</param>

        public SQLiteDynamicDatabaseReader(string connectionString) : base(connectionString) 
        { 
        }

        public override async Task<List<string>> GetTableNamesAsync()
        {
            //Creating List to hold data about tables names
            var tables = new List<string>();

            using (connection)
            {
                
                //Opening conection to data base
                await connection.OpenAsync();

                //Getting list of tables in database
                var schema = connection.GetSchema("Tables");

                //Fetching tables names from the list of tables
                foreach (DataRow tableRow in schema.Rows)
                {
                    tables.Add(tableRow["TABLE_NAME"].ToString().ToUpper()); //Adding each table to list
                }
        
            }

            return tables;
        }

        public override async Task<List<string>> GetColumsNamesAsync(string table)
        {
            //Checking if table name is not null 
            if(table == null) return new List<string>();

            //Creating List to hold data about colums names
            var columns = new List<string>();

            using (connection)
            { 
                //Opening conection to data base
                await connection.OpenAsync();

                //Getting list of colums in data base
                var schema = connection.GetSchema("Columns", new[] { null, null, table });

                //Fetching column names from the list of colums
                foreach (DataRow row in schema.Rows)
                {
                    columns.Add(row["COLUMN_NAME"].ToString().ToUpper());
                }

            }

            return columns;

        }

        public override async Task<Dictionary<string, string>> GetRelationsAsync()
        {
            //Creating Dictionary to hold data about connections 
            var relations = new Dictionary<string,string>();

            using (connection)
            {       
                await connection.OpenAsync(); 

                //Getting list of foregins keys from tables in data base
                var foreignKeys = connection.GetSchema("ForeignKeys");

                //Fetching all relations between tables from list of foregins keys
                foreach (DataRow row in foreignKeys.Rows)
                {
                    string tableMain = row["TABLE_NAME"].ToString();
                    string tableInRelation = row["FKEY_TO_TABLE"].ToString();
                    relations.Add(tableMain, tableInRelation);
                }
            }

            return relations;
        }

        public override Task<ResponseModel> GetTableContetAsync(string table)
        {
            throw new NotImplementedException();
        }

    }
}
