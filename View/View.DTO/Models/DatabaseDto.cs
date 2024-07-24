using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DTO.Databases
{
    public class DatabaseDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        [Required] [MaxLength(50)] public string Name { get; set; }

        [JsonProperty("description")]
        [MaxLength(2000)] public string Description { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("databaseTables")]
        public ICollection<TablesDto> DatabaseTables { get; set; }

    }
}
