namespace ShowManagement.Web.MultiMigrations.ShowManagementContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialShowMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shows",
                c => new
                    {
                        ShowId = c.Int(nullable: false, identity: true),
                        TvdbId = c.Int(nullable: false),
                        ImdbId = c.String(),
                        Name = c.String(),
                        Directory = c.String(),
                    })
                .PrimaryKey(t => t.ShowId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shows");
        }
    }
}
