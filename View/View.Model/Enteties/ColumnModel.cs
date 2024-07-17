using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Model.Enteties
{
    public class ColumnModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string DataType { get; set; } = string.Empty;

        public bool PrimaryKeyStatus { get; set; }

        public bool ForeignKeyStatus { get; set; }

        //Relation: One table many columns
        public int Table_ID { get; set; }
        public virtual TableModel Table { get; set; }

    }
}
