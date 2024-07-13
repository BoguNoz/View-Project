using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using View.DataConnection;


namespace View.DBShema
{
    /// <summary>
    /// DBSchema is class that represent database and holds any necessary data to recreate database structure.
    /// It also is medium on witch any operation on database will be dane
    /// </summary>

    public class DBSchema
    {
        //dbReader is object that is link between database in use and rest of application
        private DynamicDBReader dBReader;

        //tables is list that holds all tables that are part of current database  
        public List<TableSchema> Tables = new List<TableSchema>();

        /// <param name="connectionString">Data base connection string.</param>
        public DBSchema(string connectionString) 
        { 
            dBReader = new SQLiteDynamicDBReader(connectionString); //Future note: change this to include other database types
        }

        
        public async Task<ResponseModel<List<ColumnSchema>>> AcquireColumnData(string tableName)
        {
            var colums = new List<ColumnSchema>();

            try
            {
                //Fetching primary keys from current table
                var pk = await dBReader.GetPrimaryKeysAsync(tableName);
                if (!pk.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = pk.Message };

                //Fetching foregin keys from current table
                var fk = await dBReader.GetForeignKeysAsync(tableName);
                if (!fk.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = fk.Message };

                //Fetching column names from current table
                var result = await dBReader.GetColumsNamesAsync(tableName);
                if (!result.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = result.Message };

                var columnList = result.Result;

                ////Creating objects representing colums foreach column name
                foreach (var name in columnList)
                {
                    var type = await dBReader.GetColumnDataTypeAsync(tableName, name);
                    if (!type.Status)
                        return new ResponseModel<List<ColumnSchema>> { Status = false, Message = type.Message };

                    var isPk = pk.Result.ContainsKey(name) ? pk.Result[name] : false;

                    var isFk = fk.Result.ContainsKey(name) ? fk.Result[name] : false;

                    var column = new ColumnSchema
                    {
                        ColumnName = name,
                        ColumnDataType = type.Result,
                        IsItPrimaryKey = isPk,
                        IsItForeignKey = isFk,
                    };

                    colums.Add(column);
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel<List<ColumnSchema>> { Status = false, Message = $"Acquire column data ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel<List<ColumnSchema>> { Status = true, Message = "Acquire column data ended successful", Result = colums };
        }


        public async Task<ResponseModel<List<TableSchema>>> AcquireTableData()
        {
            var tables = new List<TableSchema>();

            try
            {
                //Fetching tables names from current database
                var result = await dBReader.GetTableNamesAsync();
                if (!result.Status)
                    return new ResponseModel<List<TableSchema>> { Status = false, Message = result.Message };

                //Fetching relationships data from current database
                var relations = await dBReader.GetRelationsAsync();
                if (!relations.Status)
                    return new ResponseModel<List<TableSchema>> { Status = false, Message = relations.Message };

                var tableList = result.Result;

                //Creating objects representing tables 
                foreach (var name in tableList)
                {
                    //Fetching column names from current table
                    var colums = await AcquireColumnData(name);
                    if (!colums.Status)
                        return new ResponseModel<List<TableSchema>> { Status = false, Message = colums.Message };

                    var table = new TableSchema
                    {
                        TableName = name,
                        TableColumns = colums.Result,
                        Relationships = relations.Result.ContainsKey(name) ? relations.Result[name] : new List<string>()
                    };

                    tables.Add(table);
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel<List<TableSchema>> { Status = false, Message = $"Acquire table data ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel<List<TableSchema>> { Status = true, Message = "Acquire table data ended successful", Result = tables };
        }


        public async Task<ResponseModel<bool>> CreateDBSchema()
        {
            try
            {
                var tables = await AcquireTableData();
                if (!tables.Status)
                    return new ResponseModel<bool> { Status = false, Message = tables.Message };

                Tables = tables.Result;


            }
            catch (Exception ex)
            {
                return new ResponseModel<bool> { Status = false, Message = $"Creating database schema ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel<bool> { Status = true, Message = "Creating database schema ended successful" };
        }
    }
}
