using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model.Enteties;

namespace View.Model.Configurations
{
    public class DatabaseConfiguration : IEntityTypeConfiguration<DatabaseModel>
    {
        public void Configure(EntityTypeBuilder<DatabaseModel> builder)
        {
            builder.HasKey(key => key.Id);
            builder.Property(key => key.Id).ValueGeneratedOnAdd();

            builder.Property(field => field.Name).HasMaxLength(50);
            builder.Property(field => field.Description).HasMaxLength(2000);

            //Relation: One User many schemats
            builder.HasOne(user => user.User).WithMany(schemats => schemats.UsersShemats).HasForeignKey(user => user.User_ID);
        }
    }
}
