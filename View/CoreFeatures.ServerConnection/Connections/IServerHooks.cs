using CoreFeatures.ResposeModel;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.DBSchema.Schemats;
using View.DTO.Databases;
using View.DTO.Models;

namespace CoreFeatures.ServerConnection.Connections
{
    public interface IServerHooks
    {

        /// <summary>
        /// Authenticates a user asynchronously.
        /// </summary>
        /// <param name="loginRequest">DTO object containing user login details.</param>
        /// <returns>Authorization confirmation.</returns>
        Task<ResponseModel<string>> AuthorizeUserAsync(LoginRequestDto loginRequest);

        /// <summary>
        /// Refreshes user authentication asynchronously.
        /// </summary>
        /// <returns>Authorization confirmation.</returns>
        Task<ResponseModel<string>> RefreshAuthorizationAsync();


        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="registerRequest">DTO object containing user registration details.</param>
        /// <returns>Authorization confirmation.</returns>
        Task<ResponseModel<string>> RegisterUserAsync(RegisterRequestDto registerRequest);


        /// <summary>
        /// Retrieves all user databases that were added to the system asynchronously.
        /// </summary>
        /// <returns>List of user databases as DTO models.</returns>
        Task<ResponseModel<List<DatabaseDto?>>> GetAllUserDatabasesAsync();


        /// <summary>
        /// Adds a database schema to the system asynchronously.
        /// </summary>
        /// <param name="database">Database schema.</param>
        /// <returns>DatabaseDto object.</returns>
        Task<ResponseModel<DatabaseDto>> AddDatabaseAsync(DatabaseSchema database);


        /// <summary>
        /// Deletes a database entity from the system asynchronously.
        /// </summary>
        /// <param name="name">Databases name.</param>
        /// <returns>Delete operation confirmation.</returns>
        Task<ResponseModel<string>> DeleteDatabaseAsync(string name);

        /// <summary>
        /// Udate a database entity in the system asynchronously.
        /// </summary>
        /// <param name="name">Databases name.</param>
        /// <param name="database">Database schema.</param>
        /// <returns>Udate operation confirmation.</returns>
        Task<ResponseModel<string>> UpdateDatabaseAsync(string name, DatabaseDto database);


        /// <summary>
        /// Converts a database entity to database schemat asynchronously.
        /// </summary>
        /// <param name="name">Databases name.</param>
        /// <returns>Database schema model.</returns>
        Task<ResponseModel<DatabaseSchema>> GetDatabaseAsync(string name);

    }
}
