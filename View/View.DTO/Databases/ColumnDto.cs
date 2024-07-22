using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DTO.Databases
{
    public class ColumnDto
    { 
        [Required] [MaxLength(1000)] public string Name { get; set; }

        [MaxLength(1000)] public string DataType { get; set; }

        [Required] public bool PrimaryKeyStatus { get; set; }

        [Required] public bool ForeignKeyStatus { get; set; }
    }
}
