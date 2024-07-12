using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DBShema
{
    /// <summary>
    /// TableSchema is class that represent table in database and holds any necessary data to recreate its structure.
    /// </summary>

    public class TableSchema
    {
        public string? TableName { get; set; } = string.Empty;

        public ICollection<ColumnSchema> TableColumns { get; set; }
    }
}
