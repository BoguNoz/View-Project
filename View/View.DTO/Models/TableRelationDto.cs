using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DTO.Models
{
    public class TableRelationDto
    {
        [JsonProperty("relation_ID")]
        public int Relation_ID { get; set; }
    }
}
