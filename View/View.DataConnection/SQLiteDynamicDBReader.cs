using CoreFeatures.ResposeModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace View.DataConnection
{
    /// <summary>
    /// SQLiteDynamicDBReader class is iplementation of DynamicDBReader abstract class and is responsible for accessing database and retrieving esenciale data from it
    /// </summary>
    public class SQLiteDynamicDBReader : DynamicDBReader
    {
        /// <summary>
        /// Constructor of SQLiteDynamicDBReader class is responsible for initialization of new instance of SQLiteDynamicDatabaseReade
        /// </summary>
        /// <param name="connectionString">Data base connection string (SQLite only).</param>
        public SQLiteDynamicDBReader(string connectionString) : base(connectionString) 
        { 
        }


        public override async Task<ResponseModel> GetTableNamesAsync()
        {
            //Creating List to hold data about tables names
            var tables = new List<string?>();

            using (connection)
            {
                
                //Opening conection to data base
                await connection.OpenAsync();

                //Getting list of tables in database
                var schema = await connection.GetSchemaAsync("Tables");

                //Fetching tables names from the list of tables
                foreach (DataRow tableRow in schema.Rows)
                {
                    tables.Add(tableRow["TABLE_NAME"].ToString()); //Adding each table to list
                }
        
            }

            return new ResponseModel { Status = true, Message = "Collecting tables names ended successful", Result = tables }; 
        }


        public override async Task<ResponseModel> GetColumsNamesAsync(string table)
        {
            //Checking if table name is not null 
            if(table == null) return new ResponseModel { Status = false, Message = "Collecting columns names failed due to null parameter", Result = new List<string?>() };

            //Creating List to hold data about colums names
            var columns = new List<string?>();

            using (connection)
            { 
                //Opening conection to data base
                await connection.OpenAsync();

                //Getting list of colums in data base
                var schema = await connection.GetSchemaAsync("Columns", new[] { null, null, table });

                //Fetching column names from the list of colums
                foreach (DataRow row in schema.Rows)
                {
                    columns.Add(row["COLUMN_NAME"].ToString());
                }
            }

            return new ResponseModel { Status = true, Message = "Collecting columns names ended successful", Result = columns }; 

        }


        public override async Task<ResponseModel> GetRelationsAsync()
        {
            //Creating Dictionary to hold data about connections 
            var relations = new Dictionary<string,string>();

            using (connection)
            {       
                await connection.OpenAsync(); 

                //Getting list of foregins keys from tables in data base
                var foreignKeys = await connection.GetSchemaAsync("ForeignKeys");

                //Fetching all relations between tables from list of foregins keys
                foreach (DataRow row in foreignKeys.Rows)
                {
                    string tableMain = row["TABLE_NAME"].ToString();
                    string tableInRelation = row["FKEY_TO_TABLE"].ToString();
                    relations.Add(tableMain, tableInRelation);
                }
            }

            return new ResponseModel { Status = true, Message = "Collecting relation between tables ended successfully", Result = relations }; ;
        }


        public override async Task<ResponseModel> GetColumsContetAsync(string table, string column, string? orderBy)
        {
            //Checking if table name is not null 
            if (table == null || column == null) new ResponseModel { Status = false, Message = "Collecting data from column failed due to null parameter", Result = new List<string?>() };

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
                            //If row != null add value else add null
                            data.Add(!reader.IsDBNull(0) ? reader.GetValue(0).ToString() : null);
                        }
                  
                    }
                }
            }

            return new ResponseModel { Status = true, Message = "Collecting data from column ended successful", Result = data };
        }


        public override async Task<ResponseModel> GetColumnDataTypeAsync(string table, string column)
        {
            //Checking if column name is not null 
            if (table == null || column == null) return new ResponseModel { Status = false, Message = "Collecting column data type failed due to null parameter", Result = string.Empty };

            //Creating empty string to hold data about data type
            var type = string.Empty;

            using(connection)
            {
                //Opening conection to data base
                await connection.OpenAsync();

                //Getting schema of columns in database
                var schema = await connection.GetSchemaAsync("Columns", new[] { null, null, table });

                //Iterating over schema content in order to fetch data type of correct column
                foreach (DataRow row in schema.Rows)
                {
                    if (row["COLUMN_NAME"].ToString().ToUpper() == column.ToUpper())
                        type = row["DATA_TYPE"].ToString().ToUpper();
                }
            }

            //if the correct column is not found type equals string.Empty, in this case the operation failed
            if (type == string.Empty) 
                return new ResponseModel { Status = false, Message = $"Collecting column data type failed due column named {column} not existing in data base", Result = string.Empty };
            else 
                return new ResponseModel { Status = true, Message = "Collecting column data type ended successful", Result = type };
        }


        public override async Task<ResponseModel> GetPrimaryKeysAsync(string table)
        {
            if (table == null) return new ResponseModel { Status = false, Message = "Collecting primary keys failed due to null parameter", Result = new Dictionary<string, bool>() };

            //Creating List to hold data about primary keys
            var keys = new Dictionary<string,bool>();

            using (connection)
            {
                //Opening conection to data base
                await connection.OpenAsync();

                //Getting schema of columns in database
                var schema = await connection.GetSchemaAsync("Columns", new[] { null, null, table });

                //Fetching all primary keys from table 
                foreach (DataRow row in schema.Rows)
                {
                    string columnName = row["COLUMN_NAME"].ToString();
                    bool status = bool.Parse(row["PRIMARY_KEY"].ToString());
                    keys.Add(columnName,status);
                }
            }

            return new ResponseModel { Status = true, Message = "Collecting primary keys ended successful", Result = keys };
        }


        public override async Task<ResponseModel> GetForeignKeysAsync(string table)
        {
            if (table == null) return new ResponseModel { Status = false, Message = "Collecting foregin keys failed due to null parameter", Result = new Dictionary<string, bool>() };

            //Creating List to hold data about primary keys
            var keys = new Dictionary<string,bool>();

            using (connection)
            {
                //Opening conection to data base
                await connection.OpenAsync();

                //Getting list of foregins keys from tables in data base
                var foreignKeys = await connection.GetSchemaAsync("ForeignKeys");

                //Fetching all primary keys from table 
                foreach (DataRow row in foreignKeys.Rows)
                {
                    if(row["TABLE_NAME"].ToString().ToUpper() == table.ToUpper())
                    {
                        string columnName = row["FKEY_FROM_COLUMN"].ToString();
                        keys.Add(columnName, true);
                    }
                }
            }

            return new ResponseModel { Status = true, Message = "Collecting foregin keys ended successful", Result = keys };
        }
    }
}
