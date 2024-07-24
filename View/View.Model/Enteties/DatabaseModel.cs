using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace View.Model.Enteties
{
    public class DatabaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }

        //Relation: One User many schemats
        [JsonIgnore]
        public string User_ID { get; set; }
        [JsonIgnore]
        public virtual ApplicationUserModel User { get; set; }

        //Relation: One Schema many tables
        public virtual ICollection<TableModel> DatabaseTables { get; set; } 
    }
}
