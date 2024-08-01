using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.AppServices
{
    public static class UserContext
    {
        public static User User = new User(new HttpClient(), "Default", "Default");

        public static ObservableCollection<string> Databases = new ObservableCollection<string>();

        public static ObservableCollection<string> Shemats = new ObservableCollection<string>();
    }
}
