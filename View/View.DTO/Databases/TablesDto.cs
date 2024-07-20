using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.DTO.Databases
{
    public class TablesDto
    {
        [Required][MaxLength(1000)] public string Name { get; set; } 
    }
}
