using CoreFeatures.ResposeModel;
using CoreFeatures.ServerConnection.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Data.Entity;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using View.DataConnection;
using View.DBSchema.Schemats;
using View.DTO.Databases;
using View.DTO.Models;


namespace Test_NT
{
    internal class Program
    {

        public static async Task<bool> Login()
        {
            var user = new RegisterRequestDto
            {
                Email = "testuser_1@gmail.com",
                Password = "Tu123!",

            };

            var userL = new LoginRequestDto
            {
                Email = "testuser_1@gmail.com",
                Password = "Tu123!",
                TwoFactorCode = "string",
                TwoFactorRecoveryCode = "string"
            };


            var schema = new DatabaseSchema(@"Data Source=C:\\Users\\bnozd\\Desktop\\chinook.db;Version=3;");
            var db = await schema.CreateDatabaseSchemaAsync();
            var tables = schema.Tables;
            schema.Name = "chinook.db";

            HttpClient client = new HttpClient();

            var connection = new ServiceOperation(client, @"https://localhost:7166");
            var reg = await connection.RegisterUserAsync(user);
            var login = await connection.AuthorizeUserAsync(userL);

            var result = await connection.DeleteDatabaseAsync("chinook.db");

            //var refresh = await connection.RefreshAutorizationAsync();

            
            //var create = await connection.AddDatabaseAsync(schema);

            //var table = await client.DeleteAsync(@"https://localhost:7166" + $"/relations/tables/{12}/tables/{1}");
            //var table = await client.DeleteAsync(@"https://localhost:7166" + $"/tables/{12}");

            //var content = await table.Content.ReadAsStringAsync();
            //var data = JsonConvert.DeserializeObject<List<TablesDto>>(content);


            //tableId = data["id"].ToString();

            

            return true;

        }

        static void Main(string[] args)
        {

            Login().Wait();

            Console.ReadLine();
        }
    }
}