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
        /// <param name="connectionStr">Data base connection string (SQLite only).</param>
        public SQLiteDynamicDBReader(string connectionStr) : base(connectionStr) 
        {
        }


        public override async Task<ResponseModel<List<string?>>> GetTableNamesAsync()
        {
            var tables = new List<string?>();

            using (var connection = new SQLiteConnection(connectionString))
            {

                try
                {
                    await connection.OpenAsync();

                    //Getting list of tables in database
                    var schema = await connection.GetSchemaAsync("Tables");

                    //Fetching tables names from the list of tables
                    foreach (DataRow tableRow in schema.Rows)
                    {
                        tables.Add(tableRow["TABLE_NAME"].ToString()); //Adding each table to list
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel<List<string?>> { Status = false, Message = $"Collecting tables names ended unsuccessful due to error: {ex}", Result = new List<string?>() };
                }
        
            }

            return new ResponseModel<List<string?>> { Status = true, Message = "Collecting tables names ended successful", Result = tables }; 
        }


        public override async Task<ResponseModel<List<string?>>> GetColumsNamesAsync(string table)
        {
            if(table == null) return new ResponseModel<List<string?>> { Status = false, Message = "Collecting columns names failed due to null parameter", Result = new List<string?>() };

            var columns = new List<string?>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    //Getting list of colums in data base
                    var schema = await connection.GetSchemaAsync("Columns", new[] { null, null, table });

                    //Fetching column names from the list of colums
                    foreach (DataRow row in schema.Rows)
                    {
                        columns.Add(row["COLUMN_NAME"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel<List<string?>> { Status = false, Message = $"Collecting columns names ended unsuccessful due to error: {ex}", Result = new List<string?>() };
                }
            }

            return new ResponseModel<List<string?>> { Status = true, Message = "Collecting columns names ended successful", Result = columns }; 

        }


        public override async Task<ResponseModel<Dictionary<string, List<string>>>> GetRelationsAsync()
        {
            var relations = new Dictionary<string,List<string>>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    //Getting list of foregins keys from tables in data base
                    var foreignKeys = await connection.GetSchemaAsync("ForeignKeys");

                    //Fetching all relations between tables from list of foregins keys
                    foreach (DataRow row in foreignKeys.Rows)
                    {
                        string tableMain = row["TABLE_NAME"].ToString();
                        string tableInRelation = row["FKEY_TO_TABLE"].ToString();

                        if(relations.ContainsKey(tableMain))
                            relations[tableMain].Add(tableInRelation);
                        else
                            relations.Add(tableMain, new List<string> {  tableInRelation } );
                        
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel<Dictionary<string, List<string>>> { Status = false, Message = $"Collecting relation between tables ended unsuccessful due to error: {ex}", Result = new Dictionary<string, List<string>>() };

                }
            }

            return new ResponseModel<Dictionary<string, List<string>>> { Status = true, Message = "Collecting relation between tables ended successfully", Result = relations }; ;
        }


        public override async Task<ResponseModel<List<string?>>> GetColumsContetAsync(string table, string column, string? orderBy)
        {
            if (table == null || column == null) new ResponseModel<List<string?>> { Status = false, Message = "Collecting data from column failed due to null parameter", Result = new List<string?>() };

            //If order by column is not specify then data will be order by themself
            if (orderBy == null) orderBy = column;

            var data = new List<string?>();

            using (var connection = new SQLiteConnection(connectionString))
            {

                ///SQL query used to fetch data from specific column in specific table
                string query = $"SELECT {column} FROM {table}  ORDER BY {orderBy}";

                try { 
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
                catch (Exception ex)
                {
                    return new ResponseModel<List<string?>> { Status = false, Message = $"Collecting data from column ended unsuccessful due to error: {ex}", Result = new List<string?>() };
                }
            }

            return new ResponseModel<List<string?>> { Status = true, Message = "Collecting data from column ended successful", Result = data };
        }


        public override async Task<ResponseModel<string>> GetColumnDataTypeAsync(string table, string column)
        {
            if (table == null || column == null) return new ResponseModel<string> { Status = false, Message = "Collecting column data type failed due to null parameter", Result = string.Empty };

            var type = "404";

            using(var connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    //Getting schema of columns in database
                    var schema = await connection.GetSchemaAsync("Columns", new[] { null, null, table });

                    //Iterating over schema content in order to fetch data type of correct column
                    foreach (DataRow row in schema.Rows)
                    {
                        if (row["COLUMN_NAME"].ToString() == column)
                            type = row["DATA_TYPE"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel<string> { Status = false, Message = $"Collecting column data type ended unsuccessful due to error: {ex}", Result = string.Empty };
                }
            }

            //if the correct column is not found type equals string.Empty, in this case the operation failed
            if (type == "404") 
                return new ResponseModel<string> { Status = false, Message = $"Collecting column data type failed due column named {column} not existing in data base", Result = string.Empty };
            else 
                return new ResponseModel<string> { Status = true, Message = "Collecting column data type ended successful", Result = type };
        }


        public override async Task<ResponseModel<Dictionary<string, bool>>> GetPrimaryKeysAsync(string table)
        {
            if (table == null) return new ResponseModel<Dictionary<string, bool>> { Status = false, Message = "Collecting primary keys failed due to null parameter", Result = new Dictionary<string, bool>() };

            var keys = new Dictionary<string,bool>();
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    //Getting schema of columns in database
                    var schema = await connection.GetSchemaAsync("Columns", new[] { null, null, table });

                    //Fetching all primary keys from table 
                    foreach (DataRow row in schema.Rows)
                    {
                        string columnName = row["COLUMN_NAME"].ToString();
                        bool status = bool.Parse(row["PRIMARY_KEY"].ToString());
                        keys.Add(columnName, status);
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel<Dictionary<string, bool>> { Status = false, Message = $"Collecting column data type ended unsuccessful due to error: {ex}", Result = new Dictionary<string, bool>() };
                }
            }

            return new ResponseModel<Dictionary<string, bool>> { Status = true, Message = "Collecting primary keys ended successful", Result = keys };
        }


        public override async Task<ResponseModel<Dictionary<string, bool>>> GetForeignKeysAsync(string table)
        {
            if (table == null) return new ResponseModel<Dictionary<string, bool>> { Status = false, Message = "Collecting foregin keys failed due to null parameter", Result = new Dictionary<string, bool>() };

            var keys = new Dictionary<string,bool>();

           using (var connection = new SQLiteConnection(connectionString))
           {
                try
                {
                    await connection.OpenAsync();

                    //Getting list of foregins keys from tables in data base
                    var foreignKeys = await connection.GetSchemaAsync("ForeignKeys");

                    //Fetching all primary keys from table 
                    foreach (DataRow row in foreignKeys.Rows)
                    {
                        if (row["TABLE_NAME"].ToString().ToUpper() == table.ToUpper())
                        {
                            string columnName = row["FKEY_FROM_COLUMN"].ToString();
                            keys.Add(columnName, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel<Dictionary<string, bool>> { Status = false, Message = $"Collecting column data type ended unsuccessful due to error: {ex}", Result = new Dictionary<string, bool>() };
                }
           }

            return new ResponseModel<Dictionary<string, bool>> { Status = true, Message = "Collecting foregin keys ended successful", Result = keys };
        }
    }
}
