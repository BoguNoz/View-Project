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
        /// GetDatabaseByIdAsync is async task that is fetching database schema from database with corresponding id.
        /// </summary>
        /// <param name="id">database id.</param>
        Task<DatabaseModel?> GetDatabaseByIdAsync(int id);

        /// <summary>
        /// GetDatabaseByNameAsync is async task that is fetching database schema from database with corresponding name.
        /// </summary>
        /// <param name="name">database name.</param>
        /// <param name="id">user id.</param>
        Task<DatabaseModel?> GetDatabaseByNameAsync(string name, string id);

        /// <summary>
        /// GetAllDatabasesAsync is async task that is fetching all databases owned by user from database.
        /// </summary>
        /// <param name="id">user id.</param>
        Task<List<DatabaseModel?>> GetAllDatabasesAsync(string id);

        /// <summary>
        /// SaveDatabaseAsync is async task that is saving new database entity in database
        /// </summary>
        /// <param name="model">complete model of database entity</param>
        Task<ResponseModel<DatabaseModel?>> SaveDatabaseAsync(DatabaseModel? model);


        /// <summary>
        /// DeleteDatabaseAsync is async task that is deleting existing database entity from database
        /// </summary>
        /// <param name="id">dtabase id</param>
        Task<ResponseModel<DatabaseModel?>> DeleteDatabaseAsync(int id);
    }
}
