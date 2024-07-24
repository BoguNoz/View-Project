using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoreFeatures.ServerConnection
{
    public abstract class BaseConnection
    {

        protected readonly HttpClient Client;

        protected string accessToken = string.Empty;

        protected string refreshToken = string.Empty;

        protected string connectionSting;

        protected BaseConnection(HttpClient httpClient, string serverOption) 
        {
            Client = httpClient; 

            connectionSting = serverOption;
        }
    }
}
