using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.DTO.Models;

namespace View.DTO.Databases
{
    public class TablesDto
    {

        [JsonProperty("name")]
        [Required][MaxLength(1000)] public string Name { get; set; }

    }
}
