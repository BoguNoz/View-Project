using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace View.Model.Enteties
{
    public class TableModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        //Relation: One Schema many tables
        public int Database_ID { get; set; }
        public virtual DatabaseModel Database { get; set; }

        //Relation: One table many columns
        public virtual ICollection<ColumnModel> TableColumns { get; set; } 

        //Relaton: Many tables many tables
        public virtual ICollection<TableModel> TableRelations { get; set; } 
        public virtual ICollection<TableModel> InRelationWith { get; set; } 
    }
}
