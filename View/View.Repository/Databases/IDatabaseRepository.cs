using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model.Enteties;

namespace View.Repository.Databases
{
    public interface IDatabaseRepository
    {
        /// <summary>
        /// Retrieves a database schema from the database based on its ID.
        /// </summary>
        /// <param name="id">The ID of the database.</param>
        Task<DatabaseModel?> GetDatabaseByIdAsync(int id);


        /// <summary>
        /// Retrieves a database schema from the database based on its name and user ID.
        /// </summary>
        /// <param name="name">The name of the database.</param>
        /// <param name="id">The ID of the user.</param>
        Task<DatabaseModel?> GetDatabaseByNameAsync(string name, string id);


        /// <summary>
        /// Retrieves all databases owned by a user from the database.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        Task<List<DatabaseModel?>> GetAllDatabasesAsync(string id);


        /// <summary>
        /// Saves a new database entity in the database.
        /// </summary>
        /// <param name="model">The complete model of the database entity.</param>
        Task<ResponseModel<DatabaseModel?>> SaveDatabaseAsync(DatabaseModel? model);


        /// <summary>
        /// Deletes an existing database entity from the database.
        /// </summary>
        /// <param name="id">The ID of the database to delete.</param>
        Task<ResponseModel<DatabaseModel?>> DeleteDatabaseAsync(int id);
    }
}
