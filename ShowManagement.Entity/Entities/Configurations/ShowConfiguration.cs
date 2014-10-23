using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity.Configurations
{
    public class ShowConfiguration : EntityTypeConfiguration<Show>
    {
        public ShowConfiguration()
        {
            // Primary Key
            this.HasKey(x => x.ShowId);

            // Properties
            this.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(x => x.Directory)
                .HasMaxLength(255);

            // Table and Column Mappings
            this.ToTable("Show");
        }
    }
}
