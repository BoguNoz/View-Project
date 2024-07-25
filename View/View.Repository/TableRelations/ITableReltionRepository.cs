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
        /// SaveTableRelationAsync is async task that is saving new table relation entity in database
        /// </summary>
        /// <param name="model">complete model of table relation entity</param>
        Task<ResponseModel<TableRelationModel?>> SaveTableRelationAsync(TableRelationModel? model);

        /// <summary>
        /// DeleteTableRelationAsync is async task that is deleting existing table relation entity from database
        /// </summary>
        /// <param name="tableId">table id</param>
        /// <param name="relationId">second table id</param>
        Task<ResponseModel<TableRelationModel?>> DeleteTableRelationAsync(int tableId, int relationId);
    }
}
