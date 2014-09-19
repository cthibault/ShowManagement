namespace ShowManagement.Web.MultiMigrations.ApplicationContext
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using ShowManagement.Web.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ShowManagement.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"MultiMigrations\ApplicationContext";
        }

        protected override void Seed(ShowManagement.Web.Models.ApplicationDbContext context)
        {
            this.AddUserAndRole(context);
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
