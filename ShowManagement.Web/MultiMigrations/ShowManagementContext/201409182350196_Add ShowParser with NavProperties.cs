namespace ShowManagement.Web.MultiMigrations.ShowManagementContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShowParserwithNavProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShowParsers",
                c => new
                    {
                        ShowParserId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Pattern = c.String(),
                        ExcludedCharacters = c.String(),
                        ShowId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShowParserId)
                .ForeignKey("dbo.Shows", t => t.ShowId, cascadeDelete: true)
                .Index(t => t.ShowId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShowParsers", "ShowId", "dbo.Shows");
            DropIndex("dbo.ShowParsers", new[] { "ShowId" });
            DropTable("dbo.ShowParsers");
        }
    }
}
