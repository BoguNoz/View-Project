using CoreFeatures.ResposeModel;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.DBShema;
using View.DTO.Databases;
using View.DTO.Models;

namespace CoreFeatures.ServerConnection.Connections
{
    public interface IServerHooks
    {

        /// <summary>
        /// AuthorizeUserAsync is async task that enable user authentication 
        /// </summary>
        /// <param name="loginRequest">DTO object containing all necessary user login details</param>
        /// <returns>Authorization confirmation</returns>
        Task<ResponseModel<string>> AuthorizeUserAsync(LoginRequestDto loginRequest);


        /// <summary>
        /// RegisterUserAsync is async task that enable user registration 
        /// </summary>
        /// <param name="registerRequest">DTO object containing all necessary user register details</param>
        /// <returns>Authorization confirmation</returns>
        Task<ResponseModel<string>> RegisterUserAsync(RegisterRequestDto registerRequest);


        /// <summary>
        /// GetAllUserDatabasesAsync is async task that retrieves all user databases that were added to database
        /// </summary>
        /// <returns>List of user databases as list of DTO models</returns>
        Task<ResponseModel<List<DatabaseDto?>>> GetAllUserDatabasesAsync();


        /// <summary>
        /// DatabaseDataToDtoAsync is async task that adds database schema to database
        /// </summary>
        /// <param name="database">Database schema</param>
        /// <returns>DatabaseDto object</returns>
        Task<ResponseModel<DatabaseDto>> AddDatabaseSchemaAsync(DBSchema database);


        /// <summary>
        /// DeleteDatabaseSchemaAsync is async task that deletes database entity from database
        /// </summary>
        /// <param name="database">Database DTO model</param>
        /// <returns>Delete operation conformation</returns>
        Task<ResponseModel<DatabaseDto>> DeleteDatabaseSchemaAsync(DatabaseDto database);

    }
}
