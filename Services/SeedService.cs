using Microsoft.AspNetCore.Identity;
using OnlineExam.Data;
using OnlineExam.Models;

namespace OnlineExam.Services;

public class SeedService
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

        try
        {
            // Ensure the database is ready
            logger.LogInformation("Ensuring the database is created.");
            await context.Database.EnsureCreatedAsync();

            // Add roles
            logger.LogInformation($"Seed roles");
            await AddRoleAsync(roleManager, "Admin");
            await AddRoleAsync(roleManager, "User");

            // Add admin user
            logger.LogInformation($"Seeding admin user.");
            var adminEmail = "admin@OnlineExam.com";
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admimUser = new Users
                {
                    FullName = "Mahmoud",
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpper(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };
                var result = await userManager.CreateAsync(admimUser, "Admin@123");
                if (result.Succeeded)
                {
                    logger.LogInformation($"Assigning Admin role to the admin user.");
                    await userManager.AddToRoleAsync(admimUser, "Admin");
                }
                else
                {
                    logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

            }
            // Add sample user
            logger.LogInformation("Seeding sample user.");
            var userEmail = "user@codehub.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new Users
                {
                    FullName = "Sample User",
                    UserName = userEmail,
                    NormalizedUserName = userEmail.ToUpper(),
                    Email = userEmail,
                    NormalizedEmail = userEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(user, "User@123");
                if (result.Succeeded)
                {
                    logger.LogInformation("Assigning User role to the sample user.");
                    await userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    logger.LogError("Failed to create sample user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Add sample exam data
            if (!context.Exams.Any())
            {
                logger.LogInformation("Seeding sample exam data.");

                var exam1 = new Exam
                {
                    Title = "C# Programming Basics",
                    Description = "Test your knowledge of C# programming fundamentals",
                    DurationMinutes = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.Exams.Add(exam1);
                await context.SaveChangesAsync();

                var questions1 = new List<Question>
                {
                    new() {
                        Title = "What is the correct way to declare a variable in C#?",
                        ChoiceA = "var x = 10;",
                        ChoiceB = "variable x = 10;",
                        ChoiceC = "int x = 10;",
                        ChoiceD = "string x = 10;",
                        CorrectAnswer = "C",
                        ExamId = exam1.Id
                    },
                    new() {
                        Title = "Which keyword is used to create a class in C#?",
                        ChoiceA = "class",
                        ChoiceB = "new",
                        ChoiceC = "create",
                        ChoiceD = "object",
                        CorrectAnswer = "A",
                        ExamId = exam1.Id
                    },
                    new() {
                        Title = "What is the purpose of the 'using' statement in C#?",
                        ChoiceA = "To import namespaces",
                        ChoiceB = "To declare variables",
                        ChoiceC = "To create loops",
                        ChoiceD = "To handle exceptions",
                        CorrectAnswer = "A",
                        ExamId = exam1.Id
                    }
                };

                context.Questions.AddRange(questions1);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while seeding the database.");
        }
    }

    private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                throw new Exception($"Faild to create role `{roleName}`: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}