using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DTO.Models
{
    public class RefreshRequestDto
    {
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
