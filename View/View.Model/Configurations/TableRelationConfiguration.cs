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
    public class TableRelationConfiguration : IEntityTypeConfiguration<TableRelationModel>
    {
        public void Configure(EntityTypeBuilder<TableRelationModel> builder)
        {
            builder.HasKey(key => new {key.Table_ID, key.Relation_ID});

            //Relaton: Many tables many tables
            builder.HasOne(table => table.Table).WithMany(relation => relation.TableRelations).HasForeignKey(id => id.Table_ID)
                .OnDelete(DeleteBehavior.Cascade).IsRequired();

            builder.HasOne(relation => relation.Relation).WithMany(tables => tables.InRelationWithTable).HasForeignKey(id => id.Relation_ID)
                .OnDelete(DeleteBehavior.ClientCascade).IsRequired();
        }
    }
}
