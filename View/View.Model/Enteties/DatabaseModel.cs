using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Model.Enteties
{
    /// <summary>
    /// DataBaseSchema class is model that represents structure of database schema table in database
    /// </summary>
    public class DatabaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }

        //Relation: One User many schemats
        public string User_ID { get; set; }
        public virtual ApplicationUserModel User { get; set; }

        //Relation: One Schema many tables
        public virtual ICollection<TableModel> DatabaseTables { get; set; }
    }
}
