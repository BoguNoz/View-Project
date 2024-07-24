using CoreFeatures.ServerConnection.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using View.DataConnection;
using View.DBShema;
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
                Email = "test_user1@gmail.com",
                Password = "Tu123!",
                TwoFactorCode = "string",
                TwoFactorRecoveryCode = "string"
            };


            var schema = new DBSchema(@"Data Source=C:\\Users\\bnozd\\Desktop\\chinook.db;Version=3;");
            var db = await schema.CreateDBSchemaAsync();
            var tables = schema.Tables;
            schema.dbName = "chinook.db";

            var connection = new ServiceOperation(new HttpClient(), @"https://localhost:7166/");
            //var reg = await connection.RegisterUserAsync(user);
            var result = await connection.AuthorizeUserAsync(userL);
            var databases = await connection.GetAllUserDatabasesAsync();

            return true;

        }
        static void Main(string[] args)
        {

            Login().Wait();

            Console.ReadLine();
        }
    }
}