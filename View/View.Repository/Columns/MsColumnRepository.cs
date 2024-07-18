using CoreFeatures.ResposeModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model;
using View.Model.Enteties;

namespace View.Repository.Columns
{
    public class MsColumnRepository : BaseRepository, IColumnRepository
    {
        public MsColumnRepository(AppDbContext dbContext) : base(dbContext)
        {
        }


        public async Task<ColumnModel?> GetColumnByIdAsync(int id)
        {
            var column = await DbContext.Columns.SingleOrDefaultAsync(c => c.Id == id);

            return column;
        }


        public async Task<ColumnModel?> GetColumnByNameAsync(string name, int id)
        {
            var column = await DbContext.Columns.Where(c => c.Table_ID == id).SingleOrDefaultAsync(c => c.Name == name);

            return column;
        }


        public async Task<List<ColumnModel?>> GetAllColumnsAsync(int id)
        {
            var columns = await DbContext.Columns.Where(c => c.Table_ID == id).ToListAsync();

            return columns;
        }


        public async Task<ResponseModel<ColumnModel?>> SaveEmoteAsync(ColumnModel? model)
        {
            if(model == null)
                return new ResponseModel<ColumnModel?> { Status = false, Message = "Null task parameter", Result = model };

            DbContext.Entry(model).State = model.Id == default(int) ? EntityState.Added : EntityState.Modified;

            try
            {
                DbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                return new ResponseModel<ColumnModel?> { Status = false, Message = $"Error: {ex.Message}", Result = model };
            }

            return new ResponseModel<ColumnModel?> { Status = true, Message = "Column saved successfully", Result = model };
        }


        public async Task<ResponseModel<ColumnModel?>> DeleteColumnAsync(int id)
        {
            var column = await GetColumnByIdAsync(id);

            if(column == null) 
                return new ResponseModel<ColumnModel?> { Status = true, Message = "Column deleted successfully", Result = column };

            DbContext.Columns.Remove(column);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResponseModel<ColumnModel?> { Status = false, Message = $"Error: {ex.Message}", Result = column };
            }

            return new ResponseModel<ColumnModel?> { Status = true, Message = "Column deleted successfully", Result = column };
        }
    }
}
