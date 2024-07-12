using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<TableSchema> tables = new List<TableSchema>();


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
                var result = await dBReader.GetColumsNamesAsync(tableName);

                if (!result.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = result.Message };

                var columnList = result.Result as List<string?>;

                foreach (var column in columnList)
                {
                   
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
            var tebles = new List<TableSchema>();

            try
            {
                var result = await dBReader.GetTableNamesAsync();

                if (!result.Status)
                    return new ResponseModel<List<TableSchema>> { Status = false, Message = result.Message };

                var tableList = result.Result;

                //Creating objects representing tables 
                foreach (var name in tableList)
                {
                    var colums = await AcquireColumnData(name);

                    if (!colums.Status)
                        return new ResponseModel<List<TableSchema>> { Status = false, Message = colums.Message };

                    var table = new TableSchema
                    {
                        TableName = name,
                        TableColumns = colums.Result,
                    };
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
            return new ResponseModel<bool> { Status = true, Message = "Creating database schema ended successful" };
        }
    }
}
