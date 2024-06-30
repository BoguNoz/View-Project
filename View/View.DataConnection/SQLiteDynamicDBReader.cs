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
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

namespace View.DataConnection
{
    /// <summary>
    /// This class is responsible for accessing database and retrieving esenciale data from it
    /// </summary>

    public class SQLiteDynamicDBReader : DynamicDBReader
    {
        /// <summary>
        /// Initialization of new instance of SQLiteDynamicDatabaseReade
        /// </summary>
        /// <param name="connectionString">Data base connection string (SQLite only).</param>

        public SQLiteDynamicDBReader(string connectionString) : base(connectionString) 
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
                    tables.Add(tableRow["TABLE_NAME"].ToString()); //Adding each table to list
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
                    columns.Add(row["COLUMN_NAME"].ToString());
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


        public override async Task<List<string?>> GetColumsContetAsync(string table, string column, string? orderBy)
        {
            //Checking if table name is not null 
            if (table == null || column == null) return new List<string?>();

            //If order by column is not specify then data will be order by themself
            if (orderBy == null) orderBy = column;

            //Creating Dictionary to hold data from column 
            var data = new List<string?>();

            using (connection)
            {
                ///SQL query used to fetch data from specific column in specific table
                string query = $"SELECT {column} FROM {table}  ORDER BY {orderBy}";

                //Opening conection to data base
                await connection.OpenAsync();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        //Fetching all rows in column
                        while (reader.Read())
                        {
                            //If row contains != null add value else add null
                            data.Add(!reader.IsDBNull(0) ? reader.GetValue(0).ToString() : null);
                        }
                  
                    }
                }
            }

            return data;
        }

    }
}
