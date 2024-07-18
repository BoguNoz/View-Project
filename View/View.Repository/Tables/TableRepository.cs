using CoreFeatures.ResposeModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model;
using View.Model.Enteties;

namespace View.Repository.Tables
{
    public class TableRepository : BaseRepository, ITableRepository
    {
        public TableRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

      
        public async Task<TableModel?> GetTableByIdAsync(int id)
        {
            var table = await DbContext.Tables.SingleOrDefaultAsync(t => t.Id == id);

            return table;
        }


        public async Task<TableModel?> GetTableByNameAsync(string name, int id)
        {
            var table = await DbContext.Tables.Where(d => d.Database_ID == id).SingleOrDefaultAsync(t => t.Name == name);

            return table;
        }


        public async Task<List<TableModel?>> GetAllTableAsync(int id)
        {
            var tables = await DbContext.Tables.Where(d => d.Database_ID == id).ToListAsync();

            return tables;
        }


        public async Task<ResponseModel<TableModel?>> AsigRelationshipBetweenTablesAsync(int baseId, int relationId)
        {

            var baseTable = await GetTableByIdAsync(baseId);
            if (baseTable == null)
                return new ResponseModel<TableModel?> { Status = false, Message = "Base table is not exist", Result = baseTable };

            var relationTable = await GetTableByIdAsync(relationId);
            if (relationTable == null)
                return new ResponseModel<TableModel?> { Status = false, Message = "Relation table is not exist", Result = relationTable };

            try
            {
                baseTable.InRelationWith.Add(relationTable);

                DbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                return new ResponseModel<TableModel?> { Status = false, Message = $"Error: {ex.Message}" };
            }

            return new ResponseModel<TableModel?> { Status = true, Message = "Table relationship saved successfully" };
        }


        public async Task<ResponseModel<TableModel?>> SaveTableAsync(TableModel? model)
        {
            if (model == null)
                return new ResponseModel<TableModel?> { Status = false, Message = "Null task parameter", Result = model };

            DbContext.Entry(model).State = model.Id == default(int) ? EntityState.Added : EntityState.Modified;

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResponseModel<TableModel?> { Status = false, Message = $"Error: {ex.Message}", Result = model };
            }

            return new ResponseModel<TableModel?> { Status = true, Message = "Table saved successfully", Result = model };
        }


        public async Task<ResponseModel<TableModel?>> DeleteTableAsync(int id)
        {
            var table = await GetTableByIdAsync(id);

            if (table == null)
                return new ResponseModel<TableModel?> { Status = true, Message = "Table deleted successfully", Result = table };

            DbContext.Tables.Remove(table);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResponseModel<TableModel?> { Status = false, Message = $"Error: {ex.Message}", Result = table };
            }

            return new ResponseModel<TableModel?> { Status = true, Message = "Table deleted successfully", Result = table };
        }
    }
}
