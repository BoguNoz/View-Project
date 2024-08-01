using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace View.Model.Enteties
{
    public class TableRelationModel
    {

        //Relaton: Many tables many tables
        [JsonIgnore]
        public int? Table_ID { get; set; }
        public TableModel Table { get; set; }

        public int? Relation_ID { get; set; }
        public TableModel Relation { get; set; }


    }
}
