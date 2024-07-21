using CoreFeatures.ResposeModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model;
using View.Model.Enteties;

namespace View.Repository.Databases
{
    public class MsDatabaseRepository : BaseRepository, IDatabaseRepository
    {
        public MsDatabaseRepository(AppDbContext dbContext) : base(dbContext)
        {
        }


        public async Task<DatabaseModel?> GetDatabaseByIdAsync(int id)
        {

            var database = await DbContext.Databases.Include(t => t.DatabaseTables).SingleOrDefaultAsync(d => d.Id == id);

            return database;
        }


        public async Task<DatabaseModel?> GetDatabaseByNameAsync(string name, string id)
        {
            var database = await DbContext.Databases.Where(u => u.User_ID == id).SingleOrDefaultAsync(d => d.Name == name);

            return database;
        }

        public async Task<List<DatabaseModel?>> GetAllDatabasesAsync(string id)
        {
            var databases = await DbContext.Databases.Where(u => u.User_ID == id).ToListAsync();

            return databases;
        }


        public async Task<ResponseModel<DatabaseModel?>> SaveDatabaseAsync(DatabaseModel? model)
        {
            if (model == null)
                return new ResponseModel<DatabaseModel?> { Status = false, Message = "Null task parameter", Result = model };

            DbContext.Entry(model).State = model.Id == default(int) ? EntityState.Added : EntityState.Modified;

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ResponseModel<DatabaseModel?> { Status = false, Message = $"Error: {ex.Message}", Result = model };
            }

            return new ResponseModel<DatabaseModel?> { Status = true, Message = "Database schem saved successfully", Result = model };
        }


        public async Task<ResponseModel<DatabaseModel?>> DeleteDatabaseAsync(int id)
        {
            var database = await GetDatabaseByIdAsync(id);

            if (database == null)
                return new ResponseModel<DatabaseModel?> { Status = true, Message = "Database schema deleted successfully", Result = database };

            DbContext.Databases.Remove(database);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResponseModel<DatabaseModel?> { Status = false, Message = $"Error: {ex.Message}", Result = database };
            }

            return new ResponseModel<DatabaseModel?> { Status = true, Message = "Database schema deleted successfully", Result = database };
        }

    }
}
