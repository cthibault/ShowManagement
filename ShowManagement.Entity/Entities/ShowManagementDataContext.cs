using Repository.Pattern.Ef6.DataContext;
using ShowManagement.Entity.Configurations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity
{
    public partial class ShowManagementDataContext : DataContext
    {
        static ShowManagementDataContext()
        {
            Database.SetInitializer<ShowManagementDataContext>(null);
        }

        public ShowManagementDataContext()
            : base("ShowManagementConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new ShowConfiguration());
            modelBuilder.Configurations.Add(new ShowParserConfiguration());
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowParser> ShowParsers { get; set; }
    }
}
