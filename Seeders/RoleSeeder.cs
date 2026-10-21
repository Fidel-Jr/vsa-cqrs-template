using Microsoft.AspNetCore.Identity;

namespace VsaCqrsTemplate.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<
                    RoleManager<IdentityRole>>();

            string[] roles =
            {
            "Admin",
            "Staff"
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }
        }
    }
}
