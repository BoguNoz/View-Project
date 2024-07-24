using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DTO.Databases
{
    public class ColumnDto
    {

        [JsonProperty("name")]
        [Required] [MaxLength(1000)] public string Name { get; set; }

        [JsonProperty("dataType")]
        [MaxLength(1000)] public string DataType { get; set; }

        [JsonProperty("primaryKeyStatus")]
        [Required] public bool PrimaryKeyStatus { get; set; }

        [JsonProperty("foreignKeyStatus")]
        [Required] public bool ForeignKeyStatus { get; set; }
    }
}
