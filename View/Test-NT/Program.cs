using CoreFeatures.ServerConnection.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Data.Entity;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
                Email = "admin@gmail.com",
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

            var connection = new ServiceOperation(client, @"https://localhost:7166/");
            //var reg = await connection.RegisterUserAsync(user);
            var result = await connection.AuthorizeUserAsync(userL);


            
            var create = await connection.AddDatabaseAsync(schema);

            return true;

        }
        static void Main(string[] args)
        {

            Login().Wait();

            Console.ReadLine();
        }
    }
}