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
  
                        var type = await GetDataType(reader.GetDataTypeName(column));

                        switch (type)
                        {
                            case "INTEGER": while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0)) data.Add(reader.GetInt32(0).ToString());
                                    else data.Add(null);
                                }
                                break;
                            case "TEXT": while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0)) data.Add(reader.GetString(0));
                                    else data.Add(null);
                                }
                                break;
                            case "BLOB": while (reader.Read())
                                {
                                    data.Add("BLOB data unable to read");
                                }
                                break;
                            case "DOUBLE": while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0)) data.Add(reader.GetDouble(0).ToString());
                                    else data.Add(null);
                                }
                                break;
                            case "FLOAT": while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0)) data.Add(reader.GetFloat(0).ToString());
                                    else data.Add(null);
                                }
                                break;
                            case "BOOL": while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0)) data.Add(reader.GetBoolean(0).ToString());
                                    else data.Add(null);
                                }
                                break;
                            case "DATE": while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0)) data.Add(reader.GetDateTime(0).ToString());
                                    else data.Add(null);
                                }
                                break;
                        }
                    }
                }
            }

            return data;
        }


        private async Task<string> GetDataType(string type)
        {
            if (type.Contains("CHARACTER")) type = "CHARACTER";
            else if (type.Contains("VARCHAR")) type = "VARCHAR";
            else if (type.Contains("VARYING CHARACTER")) type = "VARYING CHARACTER";
            else if (type.Contains("NCHAR")) type = "NCHAR";
            else if (type.Contains("NATIVE CHARACTER")) type = "NATIVE CHARACTER";
            else if (type.Contains("DECIMAL")) type = "DECIMAL";
            else if (type.Contains("NUMERIC")) type = "NUMERIC";

            switch (type)
            {
                case "INT":
                case "INTEGER":
                case "TINYINT":
                case "SMALLINT":
                case "MEDIUMINT":
                case "BIGINT":
                case "UNSIGNED BIG INT":
                case "INT2":
                case "INT8":
                    return "INTEGER";

                case "CHARACTER":
                case "VARCHAR":
                case "VARYING CHARACTER":
                case "NCHAR":
                case "NATIVE CHARACTER":
                case "NVARCHAR":
                case "TEXT":
                case "CLOB":
                    return "TEXT";

                case "BLOB":
                    return "BLOB";

                case "DOUBLE":
                case "DOUBLE PRECISION":
                case "DECIMAL":
                case "NUMERIC":
                    return "DOUBLE";

                case "REAL":
                case "FLOAT":
                    return "FLOAT";

                case "BOOLEAN":
                    return "BOOL";

                case "DATE":
                case "DATETIME":
                    return "DATE";
                default:
                    return "TEXT";
            }

            return "";
        }
    }
}
