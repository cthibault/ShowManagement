using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity.Configurations
{
    public class ShowParserConfiguration : EntityTypeConfiguration<ShowParser>
    {
        public ShowParserConfiguration()
        {
            // Primary Key
            this.HasKey(x => x.ShowParserId);

            // Properties
            this.Property(x => x.Pattern)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(x => x.ExcludedCharacters)
                .HasMaxLength(20);

            // Table and Column Mappings
            this.ToTable("ShowParsers");

            // Relationships
            //this.HasRequired(x => x.Show)
            //    .w
        }
    }
}
