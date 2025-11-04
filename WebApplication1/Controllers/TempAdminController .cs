using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class HomeController : Controller
{
    // ... votre code existant ...

    [HttpGet]
    public async Task<IActionResult> ResetAdminPassword()
    {
        try
        {
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByNameAsync("admin@admin");

            if (user == null)
            {
                return Content("❌ Utilisateur admin@admin non trouvé");
            }

            // Générer un token de réinitialisation
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Réinitialiser le mot de passe
            var result = await userManager.ResetPasswordAsync(user, token, "Admin123!");

            if (result.Succeeded)
            {
                return Content(
                    "<div style='font-family: Arial; padding: 20px; background: #f0f8f0; border: 2px solid #4CAF50; border-radius: 10px;'>" +
                    "<h2 style='color: #4CAF50;'>✅ RÉINITIALISATION RÉUSSIE !</h2>" +
                    "<p><strong>Email :</strong> admin@admin</p>" +
                    "<p><strong>Nouveau mot de passe :</strong> <span style='color: #ff6600; font-weight: bold;'>Admin123!</span></p>" +
                    "<p><a href='/Account/Login' style='background: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>SE CONNECTER MAINTENANT</a></p>" +
                    "</div>",
                    "text/html"
                );
            }
            else
            {
                return Content("❌ Erreur: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        catch (Exception ex)
        {
            return Content($"❌ Erreur: {ex.Message}<br><br>Détails: {ex.StackTrace}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreateNewAdmin()
    {
        try
        {
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();

            // Créer le rôle Admin s'il n'existe pas
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Créer un nouvel admin
            var newAdmin = new IdentityUser
            {
                UserName = "superadmin@trendshop.com",
                Email = "superadmin@trendshop.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newAdmin, "SuperAdmin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
                return Content(
                    "<div style='font-family: Arial; padding: 20px; background: #f0f8f0; border: 2px solid #4CAF50; border-radius: 10px;'>" +
                    "<h2 style='color: #4CAF50;'>✅ NOUVEL ADMIN CRÉÉ !</h2>" +
                    "<p><strong>Email :</strong> superadmin@trendshop.com</p>" +
                    "<p><strong>Mot de passe :</strong> <span style='color: #ff6600; font-weight: bold;'>SuperAdmin123!</span></p>" +
                    "<p><a href='/Account/Login' style='background: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>SE CONNECTER MAINTENANT</a></p>" +
                    "</div>",
                    "text/html"
                );
            }
            else
            {
                return Content("❌ Erreur: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        catch (Exception ex)
        {
            return Content($"❌ Erreur: {ex.Message}");
        }
    }
}