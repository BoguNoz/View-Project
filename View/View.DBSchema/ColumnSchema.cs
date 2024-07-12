using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace View.DBShema
{
    /// <summary>
    /// ColumnSchema is class that represent column in database and holds any necessary data to recreate its structure.
    /// </summary>
    
    public class ColumnSchema
    {
        public string ColumnName { get; set; } = string.Empty; 
        
        public string ColumnType { get; set;} = string.Empty;

        public ICollection<string?> ColumnData { get; set; }
    }
}
