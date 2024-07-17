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
    public class ColumnConfiguration : IEntityTypeConfiguration<ColumnModel>
    {
        public void Configure(EntityTypeBuilder<ColumnModel> builder) 
        {
            builder.HasKey(key => key.Id);
            builder.Property(key => key.Id).ValueGeneratedOnAdd();

            builder.Property(field => field.Name).HasMaxLength(1000);
            builder.Property(field => field.DataType).HasMaxLength(100);

            //Relation: One table many columns
            builder.HasOne(table => table.Table).WithMany(columns => columns.TableColumns).HasForeignKey(column => column.Table_ID);

        }
    }
}
