using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Model.Enteties
{

    public class ApplicationUserModel : IdentityUser
    {
        //Relation: One User many schemats
        public virtual ICollection<DatabaseModel> UsersShemats { get; set; }
    }
}
