using CareerLink.Models;
using Microsoft.AspNetCore.Identity;

namespace CareerLink.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var configuration =
                serviceProvider.GetRequiredService<IConfiguration>();

            // ============================================
            // 1. Create application roles
            // ============================================

            string[] roles =
            {
                "Admin",
                "Recruiter",
                "JobSeeker"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult =
                        await roleManager.CreateAsync(
                            new IdentityRole(role));

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            roleResult.Errors.Select(e => e.Description));

                        throw new InvalidOperationException(
                            $"Failed to create role '{role}': {errors}");
                    }
                }
            }

            // ============================================
            // 2. Get administrator credentials
            //    from User Secrets
            // ============================================

            var adminEmail =
                configuration["AdminCredentials:Email"];

            var adminPassword =
                configuration["AdminCredentials:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "Admin credentials are not configured. " +
                    "Please check Manage User Secrets.");
            }

            // ============================================
            // 3. Find the administrator account
            // ============================================

            var adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            // ============================================
            // 4. Create the administrator if it
            //    does not already exist
            // ============================================

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "CareerLink Administrator",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult =
                    await userManager.CreateAsync(
                        adminUser,
                        adminPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        createResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to create default admin: {errors}");
                }
            }
            else
            {
                // ============================================
                // 5. Reset the existing admin password
                //    to the password from User Secrets
                //
                //    This fixes the situation where the
                //    account was previously created with
                //    a different password.
                // ============================================

                var passwordResetToken =
                    await userManager.GeneratePasswordResetTokenAsync(
                        adminUser);

                var passwordResetResult =
                    await userManager.ResetPasswordAsync(
                        adminUser,
                        passwordResetToken,
                        adminPassword);

                if (!passwordResetResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        passwordResetResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to reset admin password: {errors}");
                }

                // Make sure the account is active
                adminUser.IsActive = true;

                // Make sure the email is confirmed
                adminUser.EmailConfirmed = true;

                // Make sure username and email are correct
                adminUser.UserName = adminEmail;
                adminUser.Email = adminEmail;

                await userManager.UpdateAsync(adminUser);
            }

            // ============================================
            // 6. Make sure the administrator has the
            //    Admin role
            // ============================================

            if (!await userManager.IsInRoleAsync(
                    adminUser,
                    "Admin"))
            {
                var roleResult =
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "Admin");

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to assign Admin role: {errors}");
                }
            }
        }
    }
}