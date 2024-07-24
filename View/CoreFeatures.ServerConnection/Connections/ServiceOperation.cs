using CoreFeatures.ResposeModel;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using View.DBShema;
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

                var loginResult = await Client.PostAsync(connectionSting + "login", new StringContent(json, Encoding.UTF8, "application/json"));

                if (loginResult.IsSuccessStatusCode)
                {
                    // Fetching login data
                    var tokenResponse = await loginResult.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<JObject>(tokenResponse);
                    var accessToken = tokenData["accessToken"].ToString();
                    var refreshToken = tokenData["refreshToken"].ToString();

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

            return new ResponseModel<string> { Status = true, Message = $"User: {loginRequest.Email} has logged in successfully" };
        }


        public async Task<ResponseModel<string>> RegisterUserAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                string user = JsonConvert.SerializeObject(registerRequest, Formatting.Indented);
                var result = await Client.PostAsync(connectionSting + "register", new StringContent(user, Encoding.UTF8, "application/json"));

                if(!result.IsSuccessStatusCode)
                    return new ResponseModel<string> { Status = false, Message = result.StatusCode.ToString(), Result = registerRequest.Email };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { Status = false, Message = ex.Message, Result = registerRequest.Email };
            }

            return new ResponseModel<string> { Status = true, Message = $"User: {registerRequest.Email} has registered in successfully" };
        }


        public Task<ResponseModel<DatabaseDto>> DeleteDatabaseSchemaAsync(DatabaseDto database)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<List<DatabaseDto?>>> GetAllUserDatabasesAsync()
        {
            var databases = new List<DatabaseDto>();

            try
            { 

                var response = await Client.GetAsync(connectionSting + "databases");

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


        public async Task<ResponseModel<DatabaseDto>> AddDatabaseSchemaAsync(DBSchema schema)
        {
            if (schema == null)
                return new ResponseModel<DatabaseDto> { Status = false, Message = "Database schema not found" };

            var database = new DatabaseDto {
                Name = schema.Name,
                Description = schema.Descryption,
                CreationDate = DateTime.Now,
            };

            try
            {
                //var databaseDto = 
            }
            catch
            {
                return new ResponseModel<DatabaseDto>();// { }
            }

        }

       
    }

}
