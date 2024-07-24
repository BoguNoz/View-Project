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
        /// <param name="database">Database DTO model.</param>
        /// <returns>Delete operation confirmation.</returns>
        Task<ResponseModel<DatabaseDto>> DeleteDatabaseAsync(DatabaseDto database);

    }
}
