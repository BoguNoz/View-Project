using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace View.DBSchema.Schemats
{
    public class ColumnSchema
    {
        /// <summary>
        /// ColumnName represents column name fetched from database
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// ColumDataType represents column data type fetched from database
        /// </summary>
        public string ColumnDataType { get; set; } = string.Empty;


        /// <summary>
        /// IsItPrimaryKey is statement is this column primary key or not
        /// </summary>
        public bool IsItPrimaryKey { get; set; }

        /// <summary>
        /// IsItForeignKey is statement is this column foregin key or not
        /// </summary>
        public bool IsItForeignKey { get; set; }


        /// <summary>
        /// ColumnData is collection that holds all data owned by that column
        /// </summary>
        public ICollection<string?> ColumnData { get; set; }
    }
}
