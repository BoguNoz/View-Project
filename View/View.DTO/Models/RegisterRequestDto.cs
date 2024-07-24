using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DTO.Models
{
    public class RegisterRequestDto
    {
        [JsonProperty("email")]
        [Required] public string Email { get; set; }

        [JsonProperty("password")]
        [Required] public string Password { get; set; }
    }
}
