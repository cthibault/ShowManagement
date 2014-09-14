namespace ShowManagement.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ShowManagement.Web.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using ShowManagement.Web.Data.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<ShowManagement.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            this.AddUserAndRole(context);

            context.Shows.AddOrUpdate(
                s => s.Name,
                new Show { Name = "Show 1", Directory = @"C:\Testing\Shows\Show 1", },
                new Show { Name = "Show 2", Directory = @"C:\Testing\Shows\Show 2", },
                new Show { Name = "Show 3", }
                );
        }

        private bool AddUserAndRole(ApplicationDbContext context)
        {
            IdentityResult identityResult;

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            const string CAN_EDIT_ROLE = "canEdit";

            identityResult = roleManager.Create(new IdentityRole(CAN_EDIT_ROLE));
            if (identityResult.Succeeded)
            {
                var user = new ApplicationUser { UserName = "test@sm.com" };

                identityResult = userManager.Create(user, "p@ssw0rd");
                if (identityResult.Succeeded)
                {
                    identityResult = userManager.AddToRole(user.Id, CAN_EDIT_ROLE);
                }
            }

            return identityResult.Succeeded;
        }
    }
}
