using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using View.DataConnection;


namespace View.DBSchema.Schemats
{

    public class DatabaseSchema : IDataReader
    {


        private DynamicDBReader dbReader;

        public string Name = string.Empty;

        public string Descryption = string.Empty;

        public IList<TableSchema> Tables = new List<TableSchema>();


        public DatabaseSchema(string connectionString)
        {
            dbReader = new SQLiteDynamicDBReader(connectionString); 
        }


       public async Task<ResponseModel<List<ColumnSchema>>> CreateColumnSchemaAsync(string tableName)
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
                return new ResponseModel<List<ColumnSchema>> { Status = false, Message = ex.Message };
            }

            return new ResponseModel<List<ColumnSchema>> { Status = true, Message = "Column schema creation successful", Result = colums };
        }


        public async Task<ResponseModel<List<TableSchema>>> CreateTableSchemaAsync()
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
                    var colums = await CreateColumnSchemaAsync(name);
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
                return new ResponseModel<List<TableSchema>> { Status = false, Message = ex.Message };
            }

            return new ResponseModel<List<TableSchema>> { Status = true, Message = "Table schema creation successful" };
        }


        public async Task<ResponseModel<bool>> GetTableContentAsync(string tableName, string sortingColumn)
        {
            if (!Tables.Any(t => t.TableName == tableName))
                return new ResponseModel<bool> { Status = false, Message = "Invalid table parameter" };

            var table = Tables.FirstOrDefault(t => t.TableName == tableName);
            if (!table.TableColumns.Any(c => c.ColumnName == sortingColumn))
                return new ResponseModel<bool> { Status = false, Message = "Invalid column parameter" };

            try
            {
                //Fetching column data from current table
                foreach (var column in table.TableColumns)
                {
                    var result = await dbReader.GetColumsContetAsync(tableName, column.ColumnName, sortingColumn);
                    if (!result.Status)
                        return new ResponseModel<bool> { Status = false, Message = result.Message };

                    column.ColumnData = result.Result;
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool> { Status = false, Message = $"Error: {ex}" };
            }

            return new ResponseModel<bool> { Status = true, Message = "Data compilation successful" };
        }


        public async Task<ResponseModel<bool>> CreateDatabaseSchemaAsync()
        {

            try
            {
                var result = await CreateTableSchemaAsync();
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
