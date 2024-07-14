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

        /// <summary>
        /// dbReader is object that is link between database in use and rest of application
        /// <summary>
        private DynamicDBReader dbReader;


        /// <summary>
        /// Tables is list that holds all tables that are part of current database  
        /// <summary>
        public IList<TableSchema> Tables = new List<TableSchema>();


        /// <param name="connectionString">Data base connection string.</param>
        public DBSchema(string connectionString) 
        { 
            dbReader = new SQLiteDynamicDBReader(connectionString); //Future note: change this to include other database types
        }


        /// <summary>
        /// AcquireColumnData is task that creates list of objects that represents columns in specified table
        /// <summary>
        /// <param name="tableName">Name of table that will be use.</param>
        /// <returns>Returns List of ColumnSchea objects representing columns in table</returns>
        private async Task<ResponseModel<List<ColumnSchema>>> AcquireColumnDataAsync(string tableName)
        {
            var colums = new List<ColumnSchema>();

            try
            {
                //Fetching primary keys from current table
                var pk = await dbReader.GetPrimaryKeysAsync(tableName);
                if (!pk.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = pk.Message };

                //Fetching foregin keys from current table
                var fk = await dbReader.GetForeignKeysAsync(tableName);
                if (!fk.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = fk.Message };

                //Fetching column names from current table
                var result = await dbReader.GetColumsNamesAsync(tableName);
                if (!result.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = result.Message };

                var columnList = result.Result;

                ////Creating objects representing colums foreach column name
                foreach (var name in columnList)
                {
                    var type = await dbReader.GetColumnDataTypeAsync(tableName, name);
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


        /// <summary>
        /// AcquireTableData is task that creates list of objects that represents tables in specified database
        /// <summary>
        /// <returns>Returns List of ColumnSchea objects representing tables in database</returns>
        private async Task<ResponseModel<List<TableSchema>>> AcquireTableDataAsync()
        {

            try
            {
                //Fetching tables names from current database
                var result = await dbReader.GetTableNamesAsync();
                if (!result.Status)
                    return new ResponseModel<List<TableSchema>> { Status = false, Message = result.Message };

                //Fetching relationships data from current database
                var relations = await dbReader.GetRelationsAsync();
                if (!relations.Status)
                    return new ResponseModel<List<TableSchema>> { Status = false, Message = relations.Message };

                var tableList = result.Result;

                //Creating objects representing tables 
                foreach (var name in tableList)
                {
                    //Fetching column names from current table
                    var colums = await AcquireColumnDataAsync(name);
                    if (!colums.Status)
                        return new ResponseModel<List<TableSchema>> { Status = false, Message = colums.Message };

                    var table = new TableSchema
                    {
                        TableName = name,
                        TableColumns = colums.Result,
                        Relationships = relations.Result.ContainsKey(name) ? relations.Result[name] : new List<string>()
                    };

                    Tables.Add(table);
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel<List<TableSchema>> { Status = false, Message = $"Acquire table data ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel<List<TableSchema>> { Status = true, Message = "Acquire table data ended successful" };
        }


        /// <summary>
        /// CompileTableData is task that is collecting data for tables contained in specified table
        /// <param name="tableName">Name of table that will be use.</param>
        /// <param name="sortingColumn">Name of column that represents column that will be use to order data.</param>
        /// <summary>
        public async Task<ResponseModel<bool>> CompileTableDataAsync(string tableName, string sortingColumn)
        {
            if (!Tables.Any(t => t.TableName == tableName))
                return new ResponseModel<bool> { Status = false, Message = $"Compiling data for table ended unsuccessful due to uncorect table parameter" };

            var table = Tables.FirstOrDefault(t => t.TableName == tableName);        
            if(!table.TableColumns.Any(c => c.ColumnName == sortingColumn))
                return new ResponseModel<bool> { Status = false, Message = $"Compiling data for table ended unsuccessful due to uncorect column parameter" };

            try
            {
                //Fetching column data from current table
                foreach (var column in table.TableColumns)
                {
                    var result = await dbReader.GetColumsContetAsync(tableName, column.ColumnName, sortingColumn);
                    if(!result.Status)
                        return new ResponseModel<bool> { Status = false, Message = result.Message };

                    column.ColumnData = result.Result;
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool> { Status = false, Message = $"Compiling data for table ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel<bool> { Status = true, Message = $"Compiling data for table ended successful" };
        }


        public async Task<ResponseModel<bool>> CreateDBSchemaAsync()
        {
            try
            {
                var result = await AcquireTableDataAsync();
                if (!result.Status)
                    return new ResponseModel<bool> { Status = false, Message = result.Message };

            }
            catch (Exception ex)
            {
                return new ResponseModel<bool> { Status = false, Message = $"Creating database schema ended unsuccessful due to error: {ex}" };
            }

            return new ResponseModel<bool> { Status = true, Message = "Creating database schema ended successful" };
        }
    }
}
