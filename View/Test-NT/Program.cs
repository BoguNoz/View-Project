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
using View.DBSchema;
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


            var schema = new DatabaseSchemaFileConstructor(@"Data Source=C:\\Users\\bnozd\\Desktop\\chinook.db;Version=3;","chinook.db");
            var db = await schema.CreateDatabaseSchemaAsync();
            var addContent = schema.GetDatabaseContentAsync();
            var addContent2 = schema.GetDatabaseContentAsync();

            HttpClient client = new HttpClient();

            var connection = new ServiceOperation(client, @"https://localhost:7166");

            var authorize = await connection.AuthorizeUserAsync(userL);

            //var add = await connection.AddDatabaseAsync(db.Result);

            var result = await connection.GetDatabaseAsync("chinook.db");

            return true;

        }

        static void Main(string[] args)
        {

            Login().Wait();

            Console.ReadLine();
        }
    }
}