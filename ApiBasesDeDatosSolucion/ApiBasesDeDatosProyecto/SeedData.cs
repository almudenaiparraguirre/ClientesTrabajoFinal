using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        // Obtener los administradores de roles y usuarios
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Definir los roles
        string[] roleNames = { "SuperAdmin", "Admin", "Client" };

        // Crear los roles si no existen
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Crear SuperAdmin por defecto
        var superAdminEmail = "superadmin@admin.com";
        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser { UserName = superAdminEmail, Email = superAdminEmail };
            await userManager.CreateAsync(superAdmin, "SuperPassword123!");
        }

        await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
        await AssignClaimsToUser(userManager, superAdmin, new List<Claim>
        {
            new Claim(ClaimTypes.Role, "SuperAdmin"),
            new Claim("Permissions", "ManageAdmins"),
            new Claim("Permissions", "ManageClients"),
            new Claim("Permissions", "ManageAll")
        });

        // Crear Admin por defecto
        var adminEmail = "admin@admin.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
            await userManager.CreateAsync(adminUser, "AdminPassword123!");
        }

        await userManager.AddToRoleAsync(adminUser, "Admin");
        await AssignClaimsToUser(userManager, adminUser, new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("Permissions", "ViewAdmins"),
            new Claim("Permissions", "ManageClients")
        });

        // Crear usuario Client por defecto (sin claims adicionales)
        var clientEmail = "client@client.com";
        var clientUser = await userManager.FindByEmailAsync(clientEmail);
        if (clientUser == null)
        {
            clientUser = new ApplicationUser { UserName = clientEmail, Email = clientEmail };
            await userManager.CreateAsync(clientUser, "ClientPassword123!");
        }

        await userManager.AddToRoleAsync(clientUser, "Client");
    }

    // Método para asignar claims al usuario
    private static async Task AssignClaimsToUser(UserManager<ApplicationUser> userManager, ApplicationUser user, List<Claim> claims)
    {
        // Obtener los claims actuales del usuario
        var currentClaims = await userManager.GetClaimsAsync(user);

        foreach (var claim in claims)
        {
            // Verificar si el claim ya existe
            if (!currentClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
            {
                // Si no existe, agregar el claim
                await userManager.AddClaimAsync(user, claim);
            }
        }
    }
}
