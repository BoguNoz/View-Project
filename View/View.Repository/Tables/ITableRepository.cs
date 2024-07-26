using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model.Enteties;

namespace View.Repository.Tables
{
    public interface ITableRepository
    {
        /// <summary>
        /// Retrieves a table from the database based on its ID.
        /// </summary>
        /// <param name="id">The ID of the table.</param>
        Task<TableModel?> GetTableByIdAsync(int id);

        /// <summary>
        /// Retrieves a table from the database based on its name within a specific database.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="id">The ID of the database.</param>
        Task<TableModel?> GetTableByNameAsync(string name, int id);

        /// <summary>
        /// Retrieves all tables owned by a user from the database.
        /// </summary>
        /// <param name="id">The ID of the database.</param>
        Task<List<TableModel?>> GetAllTableAsync(int id);

        /// <summary>
        /// Saves a new table entity in the database.
        /// </summary>
        /// <param name="model">The complete model of the table entity.</param>
        Task<ResponseModel<TableModel?>> SaveTableAsync(TableModel? model);

        /// <summary>
        /// Deletes an existing table entity from the database.
        /// </summary>
        /// <param name="name">The name of the table to delete.</param>
        /// <param name="id">The ID of the database.</param>
        Task<ResponseModel<TableModel?>> DeleteTableAsync(string name, int id);
    }
}
