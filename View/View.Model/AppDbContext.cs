using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using View.Model.Configurations;
using View.Model.Enteties;

namespace View.Model
{
    public class AppDbContext : IdentityDbContext<ApplicationUserModel>
    {
        DbSet<ApplicationUserModel> ApplicationUsers { get; set; }
        DbSet<DatabaseModel> Databases { get; set; }
        DbSet<TableModel> Tables { get; set; }  
        DbSet<ColumnModel> Columns { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new DatabaseConfiguration());
            builder.ApplyConfiguration(new TableConfiguration());
            builder.ApplyConfiguration(new ColumnConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
