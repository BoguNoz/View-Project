using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model.Enteties;

namespace View.Repository.Columns
{
    public interface IColumnRepository
    {

        /// <summary>
        /// Retrieves a column from the database based on its ID.
        /// </summary>
        /// <param name="id">The ID of the column.</param>
        Task<ColumnModel?> GetColumnByIdAsync(int id);


        /// <summary>
        /// Retrieves a column from the database based on its name within a specific table.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="id">The ID of the table.</param>
        Task<ColumnModel?> GetColumnByNameAsync(string name, int id);


        /// <summary>
        /// Retrieves all columns for a specific table from the database.
        /// </summary>
        /// <param name="id">The ID of the table.</param>
        Task<List<ColumnModel?>> GetAllColumnsAsync(int id);


        /// <summary>
        /// Saves a new column entity in the database.
        /// </summary>
        /// <param name="model">The complete model of the column entity.</param>
        Task<ResponseModel<ColumnModel?>> SaveColumnAsync(ColumnModel model);


        /// <summary>
        /// Deletes an existing column entity from the database.
        /// </summary>
        /// <param name="id">The ID of the column to delete.</param>
        Task<ResponseModel<ColumnModel?>> DeleteColumnAsync(int id);
    }
}
