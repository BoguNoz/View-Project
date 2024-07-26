using CoreFeatures.ResposeModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using View.DataConnection;


namespace View.DBSchema.Schemats
{

    public class DatabaseSchema 
    {

        public string Name = string.Empty;

        public string Descryption = string.Empty;

        public ICollection<TableSchema> Tables = new List<TableSchema>();
     
    }
}
