namespace TFridBlog.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TFridBlog.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TFridBlog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TFridBlog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore< IdentityRole > (context));

            if (!context.Roles.Any(r=> r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }
            #region Add Users and Assign Roles

            var userStore = new UserStore<Models.ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if  ( !context.Users.Any(u => u.Email == "JasonTwichell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "JasonTwichell@coderfoundry.com",
                    Email = "JasonTwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Prof"
                };
                userManager.Create(user, "Abc&123!");
                userManager.AddToRoles(user.Id, "Admin");

            }

            if (!context.Users.Any(u => u.Email == "terrifrid@hotmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "terrifrid@hotmail.com",
                    Email = "terrifrid@hotmail.com",
                    FirstName = "Terri",
                    LastName = "Frid",
                    DisplayName = "Queen"
                };
                userManager.Create(user, "6444Midmar");
                userManager.AddToRoles(user.Id, "Admin");

            }

            if (!context.Users.Any(u => u.Email == "aRussell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "aRussell@coderfoundry.com",
                    Email = "aRussell@coderfoundry.com",
                    FirstName = "Drew",
                    LastName = "Russell",
                    DisplayName = "Stache"
                };
                userManager.Create(user, "Abc&123!");
                userManager.AddToRoles(user.Id, "Moderator");

            }

            #endregion
        }
    }
}
