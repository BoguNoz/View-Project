using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model.Enteties;

namespace View.Repository.Nowy_folder
{
    public interface ITableReltionRepository
    {

        /// <summary>
        /// Saves a new table relation entity in the database.
        /// </summary>
        /// <param name="model">The complete model of the table relation entity.</param>
        Task<ResponseModel<TableRelationModel?>> SaveTableRelationAsync(TableRelationModel? model);

        /// <summary>
        /// Deletes an existing table relation entity from the database.
        /// </summary>
        /// <param name="tableId">The ID of the first table.</param>
        /// <param name="relationId">The ID of the second table.</param>
        Task<ResponseModel<TableRelationModel?>> DeleteTableRelationAsync(int tableId, int relationId);
    }
}
