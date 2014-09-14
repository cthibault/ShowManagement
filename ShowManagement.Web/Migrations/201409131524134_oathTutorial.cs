namespace ShowManagement.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oathTutorial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "HomeTown", c => c.String());
            AddColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DateOfBirth");
            DropColumn("dbo.AspNetUsers", "HomeTown");
        }
    }
}
