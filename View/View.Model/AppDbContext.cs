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
        public DbSet<ApplicationUserModel> ApplicationUsers { get; set; }
        public DbSet<DatabaseModel> Databases { get; set; }
        public DbSet<TableModel> Tables { get; set; }  
        public DbSet<ColumnModel> Columns { get; set; }
        public DbSet<TableRelationModel> TableRelations { get; set; }

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
