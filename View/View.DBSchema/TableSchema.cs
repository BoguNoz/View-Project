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
        /// <summary>
        /// TableName represents table name fetched from database
        /// </summary>
        public string? TableName { get; set; } = string.Empty;

        /// <summary>
        /// TableColumns is collection that holds all colums (as objects) owned by that table
        /// </summary>
        public ICollection<ColumnSchema> TableColumns { get; set; }

        /// <summary>
        /// Relationships is List that holds data about relationships between tables current database  
        /// <summary>
        public ICollection<string> Relationships { get; set; }
    }
}
