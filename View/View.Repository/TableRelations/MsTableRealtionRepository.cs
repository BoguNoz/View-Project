using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model;
using View.Model.Enteties;

namespace View.Repository.Nowy_folder
{
    public class MsTableRealtionRepository : BaseRepository, ITableReltionRepository
    {
        public MsTableRealtionRepository(AppDbContext dbContext) : base(dbContext)
        {
        }


        public async Task<ResponseModel<TableRelationModel?>> SaveTableRelationAsync(TableRelationModel? model)
        {
            if (model == null)
                return new ResponseModel<TableRelationModel?> { Status = false, Message = "Null task parameter", Result = model };

            DbContext.Add(model);

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ResponseModel<TableRelationModel?> { Status = false, Message = $"Error: {ex.Message}", Result = model };
            }

            return new ResponseModel<TableRelationModel?> { Status = true, Message = "Table relation saved successfully", Result = model };
        }


        public async Task<ResponseModel<TableRelationModel?>> DeleteTableRelationAsync(int tableId, int relationId)
        {

            var relation = await DbContext.TableRelations.SingleOrDefaultAsync(t => t.Table_ID == tableId && t.Relation_ID == relationId);

            if (relation == null)
                return new ResponseModel<TableRelationModel?> { Status = true, Message = "Table relation deleted successfully", Result = relation };

            DbContext.TableRelations.Remove(relation);


            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResponseModel<TableRelationModel?> { Status = false, Message = $"Error: {ex.Message}", Result = relation };
            }

            return new ResponseModel<TableRelationModel?> { Status = true, Message = "Table relation deleted successfully", Result = relation };
        }
    }
}
