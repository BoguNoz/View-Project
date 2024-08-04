using CoreFeatures.ResposeModel;
using CoreFeatures.ServerConnection;
using CoreFeatures.ServerConnection.Connections;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using View.DBSchema;
using View.DBSchema.Schemats;
using View.DTO.Databases;
using View.DTO.Models;
using Windows.Media.Core;

namespace View.AppServices
{
    public class User : AppOptions
    {

        //API connection
        private HttpClient client;

        private IServerOperation connection;

        public bool Authorized = false;

        public List<DatabaseDto> SavedDatabases;


        //User data
        public string Name = string.Empty;

        private string password = string.Empty;


        public Dictionary<DatabaseSchema, DatabaseSchemaFileConstructor?> Databases = new Dictionary<DatabaseSchema, DatabaseSchemaFileConstructor?>();

        //Event Handling:
        private delegate Task MyEventHandler(object sender, EventArgs e);
        private event MyEventHandler _event;
        private async Task EventHendlerTask(object sender, EventArgs e) => await API_GetAllSavedDatabasesAsync();
        private void RiseEvent() => _event?.Invoke(this, EventArgs.Empty);


        /// <param name="httpClient">New HttpClient object</param>
        /// <param name="name">Userlogin : mail</param>
        /// <param name="pass">User password</param>
        public User(HttpClient httpClient, string name, string pass)
        {
            client = httpClient;

            Name = name;
            password = pass;

            connection = new ServiceOperation(httpClient, APIServerWebLink);

            _event += EventHendlerTask;
        }


        //-> : File database operations

        /// <summary>
        /// Adds new database schema to user databases from database file asynchronously 
        /// </summary>
        /// <param name="filepath">Path to file</param>
        /// <param name="version">Protocol version</param>
        /// <param name="name">Name of database</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> File_AddDatabaseAsync(string filepath, int version, string name)
        {
            //Forating connection string from parametr data
            var connectionString = "Data Source=" + filepath + ";Version=" + version.ToString() + ";";

            DatabaseSchemaFileConstructor constructor = new DatabaseSchemaFileConstructor(connectionString, name);

            var result = await constructor.CreateDatabaseSchemaAsync();
            if (!result.Status)
                return new ResponseModel<string> { Status = false, Message = result.Message, Result = name };

            Databases.Add(result.Result, constructor);

            return new ResponseModel<string> { Status = true, Message = "Database schema added", Result = name };
        }


        /// <summary>
        /// Delets existing database schema from user databases asynchronously 
        /// </summary>
        /// <param name="name">Database name</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> File_DeleteDatabaseAsync(string name)
        {
            var database = Databases.FirstOrDefault(d => d.Key.Name == name);

            try
            {
                Databases.Remove(database.Key);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = name };
            }

            return new ResponseModel<string> { Status = true, Message = "Database deleted", Result = name };
        }


        /// <summary>
        /// Updates existing database schema from user databases asynchronously 
        /// </summary>
        /// <param name="filepath">Path to file</param>
        /// <param name="version">Protocol version</param>
        /// <param name="name">Name of database</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> File_UpdateDatabaseAsync(string filepath, int version, string name)
        {
            var database = Databases.FirstOrDefault(d => d.Key.Name == name);

            try
            {
                Databases.Remove(database.Key);

                var connectionString = "Data Source=" + filepath + ";Version=" + version.ToString() + ";";

                DatabaseSchemaFileConstructor constructor = new DatabaseSchemaFileConstructor(connectionString, name);

                var result = await constructor.CreateDatabaseSchemaAsync();
                if (!result.Status)
                    return new ResponseModel<string> { Status = false, Message = result.Message, Result = name };

                Databases.Add(result.Result, constructor);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = name };
            }

            return new ResponseModel<string> { Status = true, Message = "Database deleted", Result = name };
        }


        /// <summary>
        /// Gets content of database and adds it to apriopret database schemat asynchronously 
        /// </summary>
        /// <param name="name">Database name</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> File_GetDatabseContentAsync(string name)
        {

            var database = Databases.FirstOrDefault(d => d.Key.Name == name);
            if (database.Value == null)
                return new ResponseModel<string> { Status = false, Message = "Database file does not exist", Result = name };

            var result = await database.Value.GetDatabaseContentAsync();
            if (!result.Status)
                return new ResponseModel<string> { Status = false, Message = result.Message, Result = name };

            return new ResponseModel<string> { Status = true, Message = "Database content gather", Result = name };
        }


        /// <summary>
        /// Gets content of table and adds it to apriopret database schemat asynchronously
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="tableName">Table name</param>
        /// <param name="sortingColumnName">Name of column used for sorting table data</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> File_GetTableContentAsync(string databaseName, string tableName, string sortingColumnName)
        {
            var database = Databases.FirstOrDefault(d => d.Key.Name == databaseName);
            if (database.Value == null)
                return new ResponseModel<string> { Status = false, Message = "Database file does not exist", Result = databaseName };

            var result = await database.Value.GetTableContentAsync(tableName, sortingColumnName);
            if (!result.Status)
                return new ResponseModel<string> { Status = false, Message = result.Message, Result = databaseName };

            return new ResponseModel<string> { Status = true, Message = "Table content gather", Result = databaseName };
        }



        //-> : Server database operations

        /// <summary>
        /// Adds database schema to user database from server asynchronously 
        /// </summary>
        /// <param name="name">Database name</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_GetDatabaseSchemaAsync(string name)
        {

            if (!Authorized)
                return new ResponseModel<string> { Status = false, Message = "User unauthorized!", Result = name };

            var response = await connection.GetDatabaseAsync(name);
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message, Result = name };

            Databases.Add(response.Result, null);

            RiseEvent();

            return new ResponseModel<string> { Status = true, Message = "Database schema added :Server", Result = name };
        }


        /// <summary>
        /// Saves database schema to user database to server asynchronously 
        /// </summary>
        /// <param name="name">Database name</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_AddDatabaseSchemaAsync(string name)
        {
            if (!Authorized)
                return new ResponseModel<string> { Status = false, Message = "User unauthorized!", Result = name };

            var database = Databases.FirstOrDefault(d => d.Key.Name == name);

            database.Key.Name += " (schema)";

            var response = await connection.PostDatabaseAsync(database.Key);
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message, Result = name };

            RiseEvent();

            return new ResponseModel<string> { Status = true, Message = "Database schema added :Server", Result = name };
        }


        /// <summary>
        /// Delets existing database schema from user databases asynchronously 
        /// </summary>
        /// <param name="name">Database name</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_DeleteDatabaseAsync(string name)
        {
            if (!Authorized)
                return new ResponseModel<string> { Status = false, Message = "User unauthorized!", Result = name };

            var database = Databases.FirstOrDefault(d => d.Key.Name == name);

            try
            {
                if(database.Key != null)
                    Databases.Remove(database.Key);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = name };
            }

            var response = await connection.DeleteDatabaseAsync(name);
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message, Result = name };

            return new ResponseModel<string> { Status = true, Message = "Database deleted :Server", Result = name };
        }


        /// <summary>
        /// Update database etity from user service databse asynchronously 
        /// </summary>
        /// <param name="name">Database name</param>
        /// <param name="newDescription">New database description</param>
        /// <param name="newName">New database name</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_UpdateDatabaseAsync(string name, string? newName, string? newDescription)
        {
            if (!Authorized)
                return new ResponseModel<string> { Status = false, Message = "User unauthorized!", Result = name };

            var database = SavedDatabases.FirstOrDefault(d => d.Name == name);

            database.Name = newName;
            database.Description = newDescription;

            var response = await connection.UpdateDatabaseAsync(name, database);
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message, Result = name };


            return new ResponseModel<string> { Status = true, Message = "Database updated :Server", Result = name };
        }


        /// <summary>
        /// Post new database schema to server asynchronously
        /// </summary>
        /// <param name="name">Database name</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_PostDatabaseAsync(string name)
        {
            if (!Authorized)
                return new ResponseModel<string> { Status = false, Message = "User unauthorized!", Result = name };

            var database = Databases.FirstOrDefault(d => d.Key.Name == name);

            var response = await connection.PostDatabaseAsync(database.Key);
            if (!response.Status)
                return new ResponseModel<string> { Status = true, Message = response.Message, Result = name };

            RiseEvent();

            return new ResponseModel<string> { Status = true, Message = "Database post :Server", Result = name };
        }


        /// <summary>
        /// Gets all seved by user databases asynchronously
        /// </summary>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_GetAllSavedDatabasesAsync()
        {
            if (!Authorized)
                return new ResponseModel<string> { Status = false, Message = "User unauthorized!" };

            var response = await connection.GetAllUserDatabasesAsync();
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message };

            SavedDatabases = response.Result;

            return new ResponseModel<string> { Status = true, Message = "Databases aquired :Server" };
        }



        //-> Server user operations


        /// <summary>
        /// Register new user asynchronously
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_RegisterUserAsync(string email, string password)
        {
            var user = new RegisterRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await connection.RegisterUserAsync(user);
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message, Result = email };

            return new ResponseModel<string> { Status = true, Message = "User registered!", Result = email };
        }


        /// <summary>
        /// Authoraze user in service asynchrously
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_AuthorizeUserAsync(string email, string password)
        {
            var user = new LoginRequestDto
            {
                Email = email,
                Password = password,
                TwoFactorCode = "string",
                TwoFactorRecoveryCode = "string"
            };


            var response = await connection.AuthorizeUserAsync(user);
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message, Result = email };

            Authorized = true;

            return new ResponseModel<string> { Status = true, Message = "User loged!", Result = email };
        }


        /// <summary>
        /// refresh user autorization in service asynchrously
        /// </summary>
        /// <returns>Operation confirmation</returns>
        public async Task<ResponseModel<string>> API_RefreshAuthorizationAsync()
        {
            var response = await connection.RefreshAuthorizationAsync();
            if (!response.Status)
                return new ResponseModel<string> { Status = false, Message = response.Message };

            return new ResponseModel<string> { Status = true, Result = "Authorization refreshed" };
        }
    }
}
