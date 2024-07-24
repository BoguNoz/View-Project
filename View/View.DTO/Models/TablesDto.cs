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
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        [Required][MaxLength(1000)] public string Name { get; set; }

        [JsonProperty("tableColumns")]
        public ICollection<ColumnDto> TableColumns { get; set; }

        [JsonProperty("tableRelations")]
        public ICollection<TableRelationDto> TableRelations { get; set; }
    }
}
