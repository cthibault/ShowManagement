namespace ShowManagement.Web.MultiMigrations.ShowManagementContext
{
    using ShowManagement.Web.Data.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ShowManagement.Web.Models.ShowManagementDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"MultiMigrations\ShowManagementContext";
        }

        protected override void Seed(ShowManagement.Web.Models.ShowManagementDbContext context)
        {
            context.Shows.AddOrUpdate(
                new Show 
                { 
                    ShowId = 1,
                    Name = "Arrow Test Data", 
                    Directory = @"c:\td\a", 
                    TvdbId = 257655 
                });

            context.ShowParsers.AddOrUpdate(
                new ShowParser
                {
                    ShowParserId = 1,
                    Type = 1,
                    Pattern = @"(\.[sS]\d\d?)",
                    ExcludedCharacters = ".sS",
                    ShowId = 1
                },
                new ShowParser
                {
                    ShowParserId = 2,
                    Type = 2,
                    Pattern = @"([eE]\d\d?\.)",
                    ExcludedCharacters = ".eE",
                    ShowId = 1
                });

            context.Shows.AddOrUpdate(
                s => s.Name,
                new Show { Name = "Show 2", Directory = @"C:\Testing\Shows\Show 2", },
                new Show { Name = "Show 3", }
                );
        }
    }
}
