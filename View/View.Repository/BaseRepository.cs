using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model;

namespace View.Repository
{
    public class BaseRepository
    {
        protected AppDbContext DbContext; 

        public BaseRepository(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
