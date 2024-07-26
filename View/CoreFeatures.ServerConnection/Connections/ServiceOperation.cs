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
using System.Reflection.Metadata;
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
                // Serialize login request to JSON
                string json = JsonConvert.SerializeObject(loginRequest, Formatting.Indented);

                // Send login request
                var loginResult = await Client.PostAsync(connectionSting + "/login", new StringContent(json, Encoding.UTF8, "application/json"));

                if (loginResult.IsSuccessStatusCode)
                {
                    // Fetch access token and refresh token
                    var tokenResponse = await loginResult.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<JObject>(tokenResponse);
                    accessToken = tokenData["accessToken"].ToString();
                    refreshToken = tokenData["refreshToken"].ToString();

                    // Set authorization header
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

            // User logged successfully
            return new ResponseModel<string> { Status = true, Message = $"User logged" };
        }


        public async Task<ResponseModel<string>> RefreshAuthorizationAsync()
        {
            if (string.IsNullOrEmpty(refreshToken))
                return new ResponseModel<string> { Status = false, Message = "Refresh token error", Result = string.Empty };

            try
            {
                // Create a refresh request
                var request = new RefreshRequestDto
                {
                    RefreshToken = refreshToken
                };

                // Serialize request to JSON
                string json = JsonConvert.SerializeObject(request, Formatting.Indented);

                // Send refresh request
                var result = await Client.PostAsync(connectionSting + "/refresh", new StringContent(json, Encoding.UTF8, "application/json"));

                if (result.IsSuccessStatusCode)
                {
                    // Parse token response
                    var tokenResponse = await result.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<JObject>(tokenResponse);
                    accessToken = tokenData["accessToken"].ToString();
                    refreshToken = tokenData["refreshToken"].ToString();

                    // Set authorization header
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

            // User authorization refreshed successfully
            return new ResponseModel<string> { Status = true, Message = "User authorization refreshed" };
        }


        public async Task<ResponseModel<string>> RegisterUserAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                // Serialize register request to JSON
                string user = JsonConvert.SerializeObject(registerRequest, Formatting.Indented);

                // Send registration request
                var result = await Client.PostAsync(connectionSting + "/register", new StringContent(user, Encoding.UTF8, "application/json"));

                if (!result.IsSuccessStatusCode)
                {
                    // Handle unsuccessful registration
                    return new ResponseModel<string> { Status = false, Message = result.StatusCode.ToString(), Result = registerRequest.Email };
                }
            }
            catch (Exception ex)
            {

                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = registerRequest.Email };
            }

            // User registered successfully
            return new ResponseModel<string> { Status = true, Message = $"User registered" };
        }


        public async Task<ResponseModel<List<DatabaseDto?>>> GetAllUserDatabasesAsync()
        {
            var databases = new List<DatabaseDto>();

            try
            {
                // Send request to retrieve databases
                var response = await Client.GetAsync(connectionSting + "/databases");

                if (response.IsSuccessStatusCode)
                {
                    // Read content from response
                    var content = await response.Content.ReadAsStringAsync();

                    // Deserialize database models
                    databases = JsonConvert.DeserializeObject<List<DatabaseDto>>(content);
                }
                else
                {
                    // Handle unsuccessful response
                    return new ResponseModel<List<DatabaseDto>> { Status = false, Message = response.StatusCode.ToString(), Result = new List<DatabaseDto>() };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<DatabaseDto>> { Status = false, Message = ex.Message, Result = new List<DatabaseDto>() };
            }

            return new ResponseModel<List<DatabaseDto>> { Status = true, Message = "Database models acquired successfully", Result = databases };
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
                // Send a GET request to check if the database exists
                var result = await Client.GetAsync(connectionSting + $"/databases/name={name}");
                if (!result.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = "Database not found", Result = name };

                // Read the response content
                var content = await result.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(content);
                var id = json["id"].ToString();

                // Send a DELETE request to delete the database
                var delete = await Client.DeleteAsync(connectionSting + $"/databases/{id}");

                if (!delete.IsSuccessStatusCode)
                {
                    return new ResponseModel<string> { Status = false, Message = "Database cannot be deleted", Result = name };
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = name };
            }

            return new ResponseModel<string> { Status = true, Message = "Database deleted", Result = name };
        }


        public async Task<ResponseModel<string>> UpdateDatabaseAsync(string name, DatabaseDto databaseDto)
        {
            try
            {
                //Fetching database
                var database = await Client.GetAsync(connectionSting + $"/databases/name={name}");
                if (!database.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = "Orginal database can not be found", Result = name };

                var content = await database.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<JObject>(content);
                var id = data["id"].ToString();

                // Serialize dto model to JSON
                string json = JsonConvert.SerializeObject(databaseDto, Formatting.Indented);

                //Udatate database
                var update = await Client.PutAsync(connectionSting + $"/databases/{id}", new StringContent(json, Encoding.UTF8, "application/json"));

                if (!update.IsSuccessStatusCode)
                {
                    return new ResponseModel<string> { Status = false, Message = "Orginal database can not be updated", Result = name };
                }

               
            }
            catch(Exception ex)
            {
                new ResponseModel<string> { Status = false, Message = ex.Message, Result = name };
            }

            return new ResponseModel<string> { Status = true, Message = "Database updated succesfully", Result = name };
        }


        public async Task<ResponseModel<DatabaseSchema>> GetDatabaseAsync(string name)
        {
            var database = new DatabaseSchema();

            //Convert database schema
            var result_db = await GetDatabaseSchema(name, database);
            if(!result_db.Status)
                return new ResponseModel<DatabaseSchema> { Status = false, Message = $"Error converting to database schema: {result_db.Message}", Result = database };

            //Convert table schemats
            var result_tab = await GetTableSchemats(result_db.Result, database);
            if (!result_tab.Status)
                return new ResponseModel<DatabaseSchema> { Status = false, Message = $"Error converting to table schemats: {result_tab.Message}", Result = database };


            return new ResponseModel<DatabaseSchema> { Status = true, Message = "Database aquired", Result = database };
        }




        //Tasks used in process of adding database schemt 
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

                        var result = await Client.PostAsync(connectionSting + $"/relations/tables/{t}/tables/{r}", new StringContent("", Encoding.UTF8, "application/json"));
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message };

            }

            return new ResponseModel<string> { Status = true, Message = "Relationship schema added" };
        }

       
        //Task used in process od convertin database entity to database schema
        private async Task<ResponseModel<string>> GetDatabaseSchema(string name, DatabaseSchema schema)
        {
            string id = string.Empty;

            try
            {
                //Fetching database
                var response = await Client.GetAsync(connectionSting + $"/databases/name={name}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseModel<string> { Status = false, Message = $"Database {name} can not be found", Result = string.Empty };
                }

                //Converting respose do dto model
                var content = await response.Content.ReadAsStringAsync();
                var database = JsonConvert.DeserializeObject<DatabaseDto>(content);

                schema.Name = database.Name;
                schema.Descryption = database.Description;
                
                //Fetching database id
                var json = JsonConvert.DeserializeObject<JObject>(content);
                id = json["id"].ToString();

            }
            catch(Exception ex) 
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = string.Empty };
            }

            return new ResponseModel<string> {Status = true, Message = "Database data aquired succesfully", Result = id };
        }


        private async Task<ResponseModel<string>> GetTableSchemats(string id, DatabaseSchema schema)
        {
            try
            {
                var response = await Client.GetAsync(connectionSting + $"/tables/databases/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseModel<string> { Status = false, Message = $"Tables can not be found", Result = string.Empty };
                }

                var content = await response.Content.ReadAsStringAsync();
                var tables = JsonConvert.DeserializeObject<List<TablesDto>>(content);

                //Adding tables schemats with columns
                foreach(var table in tables)
                {

                    var columns = await GetColumnSchemats(table.Name, id);
                    if (!columns.Status)
                    {
                        return new ResponseModel<string> { Status = false, Message = $"Table columns not found", Result = string.Empty };
                    }

                    var relations = await GetRelationsSchemats(table.Name, id);
                    if (!relations.Status)
                    {
                        return new ResponseModel<string> { Status = false, Message = $"Table relations not found", Result = string.Empty };
                    }

                    var newTable = new TableSchema
                    {
                        TableName = table.Name,
                        TableColumns = columns.Result,
                        Relationships = relations.Result
                    };

                    schema.Tables.Add(newTable);
                }



            }
            catch( Exception ex ) 
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = string.Empty };
            }

            return new ResponseModel<string> { Status = true, Message = "Tables data aquired succesfully" };
        }


        private async Task<ResponseModel<List<ColumnSchema>>> GetColumnSchemats(string name, string id)
        {
            var columns = new List<ColumnSchema>();

            try
            {
                var response = await Client.GetAsync(connectionSting + $"/tables/{name}/databases/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseModel<List<ColumnSchema>> { Status = false, Message = $"Tables can not be found", Result = columns };
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(content);
                var tableColumns = json["tableColumns"].ToString();

                var columnsDto = JsonConvert.DeserializeObject<List<ColumnDto>>(tableColumns);

                foreach(var dto in columnsDto)
                {
                    var column = new ColumnSchema
                    {
                        ColumnName = dto.Name,
                        ColumnDataType = dto.DataType,
                        IsItForeignKey = dto.ForeignKeyStatus,
                        IsItPrimaryKey = dto.PrimaryKeyStatus
                    };

                    columns.Add(column);
                }

            }
            catch(Exception ex) 
            {
                return new ResponseModel<List<ColumnSchema>> { Status = false, Message = ex.Message, Result = columns };
            }

            return new ResponseModel<List<ColumnSchema>> { Status = true, Message = "Coumns data aquired succesfully", Result = columns };
        }


        private async Task<ResponseModel<List<string>>> GetRelationsSchemats(string name, string id)
        {
            var tables = new List<string>();

            try
            {
                var response = await Client.GetAsync(connectionSting + $"/tables/{name}/databases/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseModel<List<string>> { Status = false, Message = $"Tables can not be found", Result = tables };
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(content);
                var tableRelations= json["tableRelations"].ToString();

                var tablesDto = JsonConvert.DeserializeObject<List<TablesDto>>(tableRelations);

                foreach (var dto in tablesDto)
                {

                    tables.Add(dto.Name);
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel<List<string>> { Status = false, Message = ex.Message, Result = tables };
            }

            return new ResponseModel<List<string>> { Status = true, Message = "Coumns data aquired succesfully", Result = tables };
        }
    }



}

