using CoreFeatures.ResposeModel;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using View.DBSchema.Schemats;
using View.DTO.Databases;
using View.DTO.Models;

namespace CoreFeatures.ServerConnection.Connections
{
    public class ServiceOperation : BaseConnection, IServerHooks
    {

        public ServiceOperation(HttpClient httpClient, string serverOptions) : base(httpClient, serverOptions)
        {
        }


        public async Task<ResponseModel<string>> AuthorizeUserAsync(LoginRequestDto loginRequest)
        {
            try
            {
                string json = JsonConvert.SerializeObject(loginRequest, Formatting.Indented);

                var loginResult = await Client.PostAsync(connectionSting + "/login", new StringContent(json, Encoding.UTF8, "application/json"));

                if (loginResult.IsSuccessStatusCode)
                {
                    // Fetching login data
                    var tokenResponse = await loginResult.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<JObject>(tokenResponse);
                    accessToken = tokenData["accessToken"].ToString();
                    refreshToken = tokenData["refreshToken"].ToString();

                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else
                {
                    return new ResponseModel<string> { Status = false, Message = loginResult.StatusCode.ToString(), Result = loginRequest.Email };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = loginRequest.Email };
            }

            return new ResponseModel<string> { Status = true, Message = $"User logged" };
        }


        public async Task<ResponseModel<string>> RefreshAutorizationAsync()
        {
            if (refreshToken == string.Empty)
                return new ResponseModel<string> { Status = false, Message = "Refresh token error", Result = string.Empty };

            try
            {
                var request = new RefreshRequestDto
                {
                    RefreshToken = refreshToken 

                };

                string json = JsonConvert.SerializeObject(request, Formatting.Indented);
                var result = await Client.PostAsync(connectionSting + "/refresh", new StringContent(json, Encoding.UTF8, "application/json"));

                if (result.IsSuccessStatusCode)
                {
                    var tokenResponse = await result.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<JObject>(tokenResponse);
                    accessToken = tokenData["accessToken"].ToString();
                    refreshToken = tokenData["refreshToken"].ToString();

                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                }
                else
                {
                    return new ResponseModel<string> { Status = false, Message = result.StatusCode.ToString() };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message };
            }

            return new ResponseModel<string> { Status = true, Message = "User autorization refreshed" };
        }


        public async Task<ResponseModel<string>> RegisterUserAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                string user = JsonConvert.SerializeObject(registerRequest, Formatting.Indented);
                var result = await Client.PostAsync(connectionSting + "/register", new StringContent(user, Encoding.UTF8, "application/json"));

                if(!result.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = result.StatusCode.ToString(), Result = registerRequest.Email };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = registerRequest.Email };
            }

            return new ResponseModel<string> { Status = true, Message = $"User registered" };
        }


        public async Task<ResponseModel<List<DatabaseDto?>>> GetAllUserDatabasesAsync()
        {
            var databases = new List<DatabaseDto>();

            try
            { 

                var response = await Client.GetAsync(connectionSting + "/databases");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    databases = JsonConvert.DeserializeObject<List<DatabaseDto>>(content);
                }
                else
                {
                    return new ResponseModel<List<DatabaseDto?>> { Status = false, Message = response.StatusCode.ToString(), Result = new List<DatabaseDto?>() };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<DatabaseDto?>> { Status = false, Message = ex.Message, Result = new List<DatabaseDto?>() };
            }

            return new ResponseModel<List<DatabaseDto?>> { Status = false, Message = "Database models acquired successfully", Result = databases };
        }


        public async Task<ResponseModel<DatabaseDto>> AddDatabaseAsync(DatabaseSchema schema)
        {
            if (schema == null)
                return new ResponseModel<DatabaseDto> { Status = false, Message = "Database schema not found" };

            var database = new DatabaseDto
            {
                Name = schema.Name,
                Description = schema.Descryption,
                CreationDate = DateTime.Now,
            };

            //Adding database schema to database
            var result_db = await AddDatabaseSchematAsync(database);
            if (!result_db.Status)
                return new ResponseModel<DatabaseDto> { Status = false, Message = result_db.Message };


            //Ading tables schemats to database
            var result_tab = await AddTablesSchematsAsync(database, schema);
            if (!result_tab.Status)
                return new ResponseModel<DatabaseDto> { Status = false, Message = result_tab.Message };


            //Ading relations to database
            var result_rel = await AddRelationshipAsync(result_tab.Result, schema);
            if (!result_rel.Status)
                return new ResponseModel<DatabaseDto> { Status = false, Message = result_rel.Message };


            return new ResponseModel<DatabaseDto> { Status = true, Message = "Database added" };
        }


        public async Task<ResponseModel<string>> DeleteDatabaseAsync(string name)
        {
            try
            {

                var result = await Client.GetAsync(connectionSting + $"/databases/name={name}");
                if(!result.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = "Database not found", Result = name };

                var content = await result.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(content);
                var id = json["id"].ToString();

                var delete = await Client.DeleteAsync(connectionSting + $"/databases/{id}");
                if (!delete.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = "Database can not be deleted", Result = name };

            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = name };
            }

            return new ResponseModel<string> { Status = true, Message = "Database deleted", Result = name };
        }



        //Tasks use in process of adding database schemt 
        private async Task<ResponseModel<string>> AddDatabaseSchematAsync(DatabaseDto databaseDto)
        {
            try
            {

                var databaseJson = JsonConvert.SerializeObject(databaseDto, Formatting.Indented);

                var addDatabase = await Client.PostAsync(connectionSting + "/databases/items", new StringContent(databaseJson, Encoding.UTF8, "application/json"));
                if (!addDatabase.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = addDatabase.StatusCode.ToString() + ": Databse can not be added" };

            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message };
            }


            return new ResponseModel<string> { Status = true, Message = "Database schema added" };
        }


        private async Task<ResponseModel<Dictionary<string, string>>> AddTablesSchematsAsync(DatabaseDto databaseDto, DatabaseSchema schema)
        {
            if (schema == null)
                return new ResponseModel<Dictionary<string, string>> { Status = false, Message = "Database schema not found", Result = new Dictionary<string, string>() };

            var tables = new Dictionary<string, string>();

            try
            {
                //Fetching database
                var database = await Client.GetAsync(connectionSting + $"/databases/name={databaseDto.Name}");
                if (!database.IsSuccessStatusCode)
                    return new ResponseModel<Dictionary<string, string>> { Status = false, Message = database.StatusCode.ToString() + ": Databse not found", Result = new Dictionary<string, string>() };

                var databaseContent = await database.Content.ReadAsStringAsync();
                var databaseData = JsonConvert.DeserializeObject<JObject>(databaseContent);
                var databaseId = databaseData["id"].ToString();

                //Ading each table
                foreach (var table in schema.Tables)
                {
                    var tempTable = new TablesDto
                    {
                        Name = table.TableName
                    };

                    var tableJson = JsonConvert.SerializeObject(tempTable, Formatting.Indented);
                    var addTable = await Client.PostAsync(connectionSting + $"/tables/items/databases/{databaseId}", new StringContent(tableJson, Encoding.UTF8, "application/json"));

                    //Ading columns schemats to database
                    var result = await AddColumnsSchematsAsync(tempTable, databaseId, table);
                    if (!result.Status)
                        return new ResponseModel<Dictionary<string, string>> { Status = false, Message = result.Message, Result = new Dictionary<string, string>() };

                    tables.Add(table.TableName, result.Result);

                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<Dictionary<string, string>> { Status = false, Message = ex.Message, Result = new Dictionary<string, string>() };
            }

            return new ResponseModel<Dictionary<string, string>> { Status = true, Message = "Table schema added", Result = tables };
        }


        private async Task<ResponseModel<string>> AddColumnsSchematsAsync(TablesDto tableDto, string databaseId, TableSchema schema)
        {
            if (schema == null)
                return new ResponseModel<string> { Status = false, Message = "Database schema not found", Result = string.Empty };

            var tableId = string.Empty;

            try
            {
                //Fetching table
                var table = await Client.GetAsync(connectionSting + $"/tables/{tableDto.Name}/databases/{databaseId}");
                if (!table.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = table.StatusCode.ToString() + ": Table not found", Result = string.Empty };

                var content = await table.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<JObject>(content);
                tableId = data["id"].ToString();

                //Ading each table
                foreach (var column in schema.TableColumns)
                {
                    var newColumn = new ColumnDto
                    {

                        Name = column.ColumnName,
                        DataType = column.ColumnDataType,
                        PrimaryKeyStatus = column.IsItPrimaryKey,
                        ForeignKeyStatus = column.IsItForeignKey
                    };

                    var columnJson = JsonConvert.SerializeObject(newColumn, Formatting.Indented);
                    var addColumn = await Client.PostAsync(connectionSting + $"/columns/items/tables/{tableId}", new StringContent(columnJson, Encoding.UTF8, "application/json"));

                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = string.Empty };
            }

            return new ResponseModel<string> { Status = true, Message = "Column schema added", Result = tableId };
        }


        private async Task<ResponseModel<string>> AddRelationshipAsync(Dictionary<string, string> tables, DatabaseSchema schema)
        {
            if (schema == null)
                return new ResponseModel<string> { Status = false, Message = "Database schema not found" };

            try
            {
                foreach (var table in schema.Tables)
                {

                    var t = tables[table.TableName];
                    if (t == null)
                        return new ResponseModel<string> { Status = false, Message = "Table schema not found" };

                    foreach (var relation in table.Relationships)
                    {

                        var r = tables[relation];
                        if (r == null)
                            return new ResponseModel<string> { Status = false, Message = "Table schema not found" };

                        var result = await Client.PostAsync(connectionSting + $"/relations/tables/{t}/tables{r}", new StringContent("", Encoding.UTF8, "application/json"));
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message };

            }

            return new ResponseModel<string> { Status = true, Message = "Relationship schema added" };
        }

       
    }



}

