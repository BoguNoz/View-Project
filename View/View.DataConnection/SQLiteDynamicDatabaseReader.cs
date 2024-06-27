using CoreFeatures.ResposeModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DataConnection
{
    public class SQLiteDynamicDatabaseReader : DynamicDatabaseReader
    {

        public SQLiteDynamicDatabaseReader(string connectionString) : base(connectionString) 
        { 
        }

        public override async Task<List<string>> GetTableNamesAsync()
        {
            using (connection)
            {
                //Opening conection to data base
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception)
                {
                    throw new Exception("Connection to database error!");
                }

                //Getting schema of tables in database
                var schema = connection.GetSchema("Tables");

                var tables = new List<string>();

                foreach (DataRow tableRow in schema.Rows)
                {
                    tables.Add(tableRow["TABLE_NAME"].ToString().ToUpper()); //Adding each table to list
                }

                connection.Close();

                return tables;
            } 
        }

        public override async Task<List<string>> GetColumsNamesAsync(string tableName)
        {
            if(tableName == null) return new List<string>();

            var columns = new List<string>();

            using (connection)
            {
                //Opening conection to data base
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception)
                {
                    throw new Exception("Connection to database error!");
                }

                //Getting schema of colums in database
                var schema = connection.GetSchema("Columns", new[] { null, null, tableName });

                foreach (DataRow row in schema.Rows)
                {
                    columns.Add(row["COLUMN_NAME"].ToString().ToUpper());
                }

                connection.Close();
            }

            return columns;

        }

        public override async Task<bool> GetRelationsAsync(string tableName)
        {
            return true;
        }

        public override Task<ResponseModel> GetTableContetAsync(string tableName)
        {
            throw new NotImplementedException();
        }

    }
}
