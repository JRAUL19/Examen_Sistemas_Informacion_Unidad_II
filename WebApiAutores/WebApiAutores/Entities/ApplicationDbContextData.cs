using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entities
{
    public static class ApplicationDbContextData
    {
        public static async Task LoadDataAsync(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            try
            {
                if (!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                if (!userManager.Users.Any())
                {
                    var userAdmin = new IdentityUser
                    {
                        Email = "jperez@me.com",
                        UserName = "jperez@me.com"
                    };
                    await userManager.CreateAsync(userAdmin, "Cur0c@2020");
                    await userManager.AddToRoleAsync(userAdmin, "Admin");

                    var normalUser = new IdentityUser
                    {
                        Email = "pepito@me.com",
                        UserName = "pepito@me.com"
                    };
                    await userManager.CreateAsync(normalUser, "Cur0c@2020");
                    await userManager.AddToRoleAsync(normalUser, "User");
                }
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
                logger.LogError(e.Message);
            }
        }
    }
}
