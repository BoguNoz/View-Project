using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using View.DBSchema.Schemats;

namespace View.DBSchema
{
    public interface IDataReader
    {
        /// <summary>
        /// Retrieves a list of objects representing columns in a specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>List of ColumnSchema objects representing columns in the table.</returns>
        public Task<ResponseModel<List<ColumnSchema>>> CreateColumnSchemaAsync(string tableName);


        /// <summary>
        /// Retrieves a list of objects representing tables in a specified database.
        /// </summary>
        /// <returns>List of TableSchema objects representing tables in the database.</returns>
        protected Task<ResponseModel<List<TableSchema>>> CreateTableSchemaAsync();


        /// <summary>
        /// Collects data for tables contained in a specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="sortingColumn">Name of the column used to order data.</param>
        /// <returns>Response indicating success or failure.</returns>
        protected Task<ResponseModel<bool>> GetTableContentAsync(string tableName, string sortingColumn);


        /// <summary>
        /// Collects data for database
        /// </summary>
        /// <returns>Response indicating success or failure.</returns>
        Task<ResponseModel<bool>> GetDatabaseContentAsync();


        /// <summary>
        /// Creates the schema for the database.
        /// </summary>
        /// <returns>Response indicating full database schema.</returns>
        Task<ResponseModel<DatabaseSchema>> CreateDatabaseSchemaAsync();
        
    }
}
