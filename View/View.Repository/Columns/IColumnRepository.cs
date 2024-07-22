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
        /// GetColumnByIdAsync is async task that is fetching column from database with corresponding id.
        /// </summary>
        /// <param name="id">column id.</param>
        Task<ColumnModel?> GetColumnByIdAsync(int id);

        /// <summary>
        /// GetColumnByNameAsync is async task that is fetching column from database with corresponding name.
        /// </summary>
        /// <param name="name">column name.</param>
        /// <param name="id">table id.</param>
        Task<ColumnModel?> GetColumnByNameAsync(string name, int id);

        /// <summary>
        /// GetAllColumnsAsync is async task that is fetching all table columns from database.
        /// </summary>
        /// <param name="id">table id.</param>
        Task<List<ColumnModel?>> GetAllColumnsAsync(int id);

        /// <summary>
        /// SaveEmoteAsync is async task that is saving new column entity in database
        /// </summary>
        /// <param name="model">complete model of column entity</param>
        Task<ResponseModel<ColumnModel?>> SaveColumnAsync(ColumnModel model);


        /// <summary>
        /// DeleteColumnAsync is async task that is deleting existing column entity from database
        /// </summary>
        /// <param name="id">column id</param>
        Task<ResponseModel<ColumnModel?>> DeleteColumnAsync(int id);
    }
}
