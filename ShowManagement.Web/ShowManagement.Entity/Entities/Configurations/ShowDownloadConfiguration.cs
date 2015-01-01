using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity.Configurations
{
    public class ShowDownloadConfiguration : EntityTypeConfiguration<ShowDownload>
    {
        public ShowDownloadConfiguration()
        {
            // Primary Key
            this.HasKey(x => x.ShowDownloadId);

            // Properties
            this.Property(x => x.OriginalPath)
                .IsRequired()
                .HasMaxLength(255);
            this.Property(x => x.CurrentPath)
                .IsRequired()
                .HasMaxLength(255);
            this.Property(x => x.ModifiedDate)
                .IsRequired();
            this.Property(x => x.CreatedDate)
                .IsRequired();

            // Table and Column Mappings
            this.ToTable("ShowDownloads");
        }
    }
}
