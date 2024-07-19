using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace View.Model.Enteties
{

    public class ApplicationUserModel : IdentityUser
    {
        //Relation: One User many schemats
        [JsonIgnore] public virtual ICollection<DatabaseModel> UsersShemats { get; set; }
    }
}
