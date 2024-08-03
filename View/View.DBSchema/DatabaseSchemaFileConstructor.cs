using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.DataConnection;
using View.DBSchema.Schemats;

namespace View.DBSchema
{
    public class DatabaseSchemaFileConstructor : IDataReader
    {
        private DatabaseSchema schema = new DatabaseSchema();
        private DatabaseFileReader dbReader;

        public DatabaseSchemaFileConstructor(string connectionString, string name)
        {
            dbReader = new SQLiteDatabaseFileReader(connectionString);
            schema.Name = name;
        }

        public async Task<ResponseModel<List<ColumnSchema>>> CreateColumnSchemaAsync(string tableName)
        {
            var columns = new List<ColumnSchema>();

            try
            {
                // Fetching primary keys from the current table
                var pk = await dbReader.GetPrimaryKeysAsync(tableName);
                if (!pk.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = $"Failed to fetch primary keys for table '{tableName}': {pk.Message}" };

                // Fetching foreign keys from the current table
                var fk = await dbReader.GetForeignKeysAsync(tableName);
                if (!fk.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = $"Failed to fetch foreign keys for table '{tableName}': {fk.Message}" };

                // Fetching column names from the current table
                var result = await dbReader.GetColumsNamesAsync(tableName);
                if (!result.Status)
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = $"Failed to fetch column names for table '{tableName}': {result.Message}" };

                var columnList = result.Result;

                // Creating objects representing columns for each column name
                foreach (var name in columnList)
                {
                    var type = await dbReader.GetColumnDataTypeAsync(tableName, name);
                    if (!type.Status)
                        return new ResponseModel<List<ColumnSchema>> { Status = false, Message = $"Failed to fetch data type for column '{name}' in table '{tableName}': {type.Message}" };

                    var isPk = pk.Result.ContainsKey(name) ? pk.Result[name] : false;
                    var isFk = fk.Result.ContainsKey(name) ? fk.Result[name] : false;

                    var column = new ColumnSchema
                    {
                        ColumnName = name,
                        ColumnDataType = type.Result,
                        IsItPrimaryKey = isPk,
                        IsItForeignKey = isFk,
                    };

                    columns.Add(column);
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<ColumnSchema>> { Status = false, Message = $"An error occurred while creating column schema for table '{tableName}': {ex.Message}" };
            }

            return new ResponseModel<List<ColumnSchema>> { Status = true, Message = "Column schema created successfully.", Result = columns };
        }

        public async Task<ResponseModel<List<TableSchema>>> CreateTableSchemaAsync()
        {
            try
            {
                // Fetching table names from the current database
                var result = await dbReader.GetTableNamesAsync();
                if (!result.Status)
                    return new ResponseModel<List<TableSchema>> { Status = false, Message = $"Failed to fetch table names: {result.Message}" };

                // Fetching relationship data from the current database
                var relations = await dbReader.GetRelationsAsync();
                if (!relations.Status)
                    return new ResponseModel<List<TableSchema>> { Status = false, Message = $"Failed to fetch relationships: {relations.Message}" };

                var tableList = result.Result;

                // Creating objects representing tables
                foreach (var name in tableList)
                {
                    // Fetching column names from the current table
                    var columns = await CreateColumnSchemaAsync(name);
                    if (!columns.Status)
                        return new ResponseModel<List<TableSchema>> { Status = false, Message = $"Failed to create column schema for table '{name}': {columns.Message}" };

                    var table = new TableSchema
                    {
                        TableName = name,
                        TableColumns = columns.Result,
                        Relationships = relations.Result.ContainsKey(name) ? relations.Result[name] : new List<string>()
                    };

                    schema.Tables.Add(table);
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<TableSchema>> { Status = false, Message = $"An error occurred while creating table schema: {ex.Message}" };
            }

            return new ResponseModel<List<TableSchema>> { Status = true, Message = "Table schema created successfully." };
        }

        public async Task<ResponseModel<bool>> GetTableContentAsync(string tableName, string sortingColumn)
        {
            if (!schema.Tables.Any(t => t.TableName == tableName))
                return new ResponseModel<bool> { Status = false, Message = $"Table '{tableName}' does not exist in the schema." };

            var table = schema.Tables.FirstOrDefault(t => t.TableName == tableName);
            if (!table.TableColumns.Any(c => c.ColumnName == sortingColumn))
                return new ResponseModel<bool> { Status = false, Message = $"Column '{sortingColumn}' does not exist in table '{tableName}'." };

            try
            {
                // Fetching column data from the current table
                foreach (var column in table.TableColumns)
                {
                    var result = await dbReader.GetColumsContetAsync(tableName, column.ColumnName, sortingColumn);
                    if (!result.Status)
                        return new ResponseModel<bool> { Status = false, Message = $"Failed to fetch content for column '{column.ColumnName}' in table '{tableName}': {result.Message}" };

                    column.ColumnData = result.Result;

                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool> { Status = false, Message = $"An error occurred while fetching table content for '{tableName}': {ex.Message}" };
            }

            return new ResponseModel<bool> { Status = true, Message = "Table content fetched successfully." };
        }

        public async Task<ResponseModel<bool>> GetDatabaseContentAsync()
        {
            foreach (var table in schema.Tables)
            {
                var result = await GetTableContentAsync(table.TableName, table.TableColumns.First().ColumnName);

                if (!result.Status)
                    return new ResponseModel<bool> { Status = false, Message = $"Failed to fetch content for table '{table.TableName}': {result.Message}" };
            }

            return new ResponseModel<bool> { Status = true, Message = "Database content fetched successfully." };
        }

        public async Task<ResponseModel<DatabaseSchema>> CreateDatabaseSchemaAsync()
        {
            try
            {
                var result = await CreateTableSchemaAsync();
                if (!result.Status)
                    return new ResponseModel<DatabaseSchema> { Status = false, Message = $"Failed to create table schema: {result.Message}" };
            }
            catch (Exception ex)
            {
                return new ResponseModel<DatabaseSchema> { Status = false, Message = $"An error occurred while creating the database schema: {ex.Message}" };
            }

            return new ResponseModel<DatabaseSchema> { Status = true, Message = "Database schema created successfully.", Result = schema };
        }
    }
}
