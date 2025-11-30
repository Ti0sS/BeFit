using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=mydatabase.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    await SeedRoles(roleManager);
    await SeedExerciseTypes(context);
    await SeedFirstUserAsAdmin(userManager);
}
async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    const string adminRoleName = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));
    }
}

async Task SeedExerciseTypes(ApplicationDbContext context)
{
    if (!context.ExerciseTypes.Any())
    {
        context.ExerciseTypes.AddRange(
            new ExerciseType { Name = "Przysiad" },
            new ExerciseType { Name = "Martwy ciąg" },
            new ExerciseType { Name = "Wyciskanie na ławce" },
            new ExerciseType { Name = "Podciąganie" },
            new ExerciseType { Name = "Wiosłowanie" },
            new ExerciseType { Name = "OHP (Overhead Press)" }
        );

        await context.SaveChangesAsync();
    }
}

async Task SeedFirstUserAsAdmin(UserManager<AppUser> userManager)
{
    var users = userManager.Users.ToList();

    if (users.Count == 0)
        return;

    var firstUser = users.First();

    if (!await userManager.IsInRoleAsync(firstUser, "Admin"))
    {
        await userManager.AddToRoleAsync(firstUser, "Admin");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
