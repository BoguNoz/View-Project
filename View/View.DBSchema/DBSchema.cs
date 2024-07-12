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

        
        public async Task<ResponseModel> AcquireColumnData(string tableName)
        {
            var colums = new List<ColumnSchema>();

            try
            {
                var result = await dBReader.GetColumsNamesAsync(tableName);

                if (result.Status == false)
                    return new ResponseModel { Status = false, Message = result.Message };

                var temp = result.Result;

                //foreach(var column in result.Result)
                //{

                //}

            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = false, Message = $"Acquire column data ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel { Status = true, Message = "Acquire column data ended successful", Result = colums };
        }


        public async Task<ResponseModel> AcquireTableData()
        {
            var tebles = new List<TableSchema>();

            try
            {
                var result = await dBReader.GetTableNamesAsync();

                if (result.Status == false)
                    return new ResponseModel { Status = false, Message = result.Message };

            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = false, Message = $"Acquire table data ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel { Status = true, Message = "Acquire table data ended successful", Result = tables };
        }


        public async Task<ResponseModel> CreateDBSchema()
        {
            return new ResponseModel { Status = true, Message = "Creating database schema ended successful" };
        }
    }
}
