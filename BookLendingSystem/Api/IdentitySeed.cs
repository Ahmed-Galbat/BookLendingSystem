using BookLendingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BookLendingSystem.Api
{
    public static class IdentitySeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Member"))
            {
                await roleManager.CreateAsync(new IdentityRole("Member"));
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@booklending.com",
                Email = "admin@booklending.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
