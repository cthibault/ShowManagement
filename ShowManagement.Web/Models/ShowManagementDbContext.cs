using ShowManagement.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShowManagement.Web.Models
{
    public class ShowManagementDbContext : DbContext
    {
        public ShowManagementDbContext()
            : base("DefaultConnection")
        {
        }

        public static ShowManagementDbContext Create()
        {
            return new ShowManagementDbContext();
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowParser> ShowParsers { get; set; }
    }
}