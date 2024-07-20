using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View.Model.Enteties;

namespace View.Model.Configurations
{
    public class TableConfiguration : IEntityTypeConfiguration<TableModel>
    {
        public void Configure(EntityTypeBuilder<TableModel> builder)
        {
            builder.HasKey(key => key.Id);
            builder.Property(key => key.Id).ValueGeneratedOnAdd();

            builder.Property(field => field.Name).HasMaxLength(1000);

            //Relation: One Schema many tables --
            builder.HasOne(schema => schema.Database).WithMany(tables => tables.DatabaseTables).HasForeignKey(table => table.Database_ID).IsRequired();

            //Relation: One table many columns --
            builder.HasMany(colums => colums.TableColumns).WithOne(table => table.Table).HasForeignKey(table => table.Table_ID);

            //Relaton: Many tables many tables
            builder.HasMany(tables => tables.TableRelations).WithMany(tables => tables.InRelationWith);
        }
    }
}
