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
        /// GetTableByIdAsync is async task that is fetching table from database with corresponding id.
        /// </summary>
        /// <param name="id">table id.</param>
        Task<TableModel?> GetTableByIdAsync(int id);

        /// <summary>
        /// GetTableByNameAsync is async task that is fetching table from database with corresponding name.
        /// </summary>
        /// <param name="name">table name.</param>
        /// <param name="id">database id.</param>
        Task<TableModel?> GetTableByNameAsync(string name, int id);

        /// <summary>
        /// GetAllTableAsync is async task that is fetching all tables owned by user from database.
        /// </summary>
        /// <param name="id">database id.</param>
        Task<List<TableModel?>> GetAllTableAsync(int id);

        /// <summary>
        /// SaveTableAsync is async task that is saving new table entity in database
        /// </summary>
        /// <param name="model">complete model of table entity</param>
        Task<ResponseModel<TableModel?>> SaveTableAsync(TableModel? model);

        /// <summary>
        /// DeleteTableAsync is async task that is deleting existing table entity from database
        /// </summary>
        /// <param name="id">table id</param>
        Task<ResponseModel<TableModel?>> DeleteTableAsync(int id);
    }
}
