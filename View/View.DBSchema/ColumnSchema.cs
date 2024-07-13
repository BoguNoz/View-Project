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
        public string? ColumnName { get; set; } = string.Empty; 
        
        public string? ColumnDataType { get; set;} = string.Empty;

        public bool IsItPrimaryKey {  get; set; }

        public bool IsItForeignKey { get; set; }

        public ICollection<string?> ColumnData { get; set; }
    }
}
