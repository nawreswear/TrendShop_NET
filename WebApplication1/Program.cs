using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebApplication1.Data;
using WebApplication1.Models.Repositories;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);


// Lire la clé secrète depuis la configuration
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Ajouter les services au conteneur
builder.Services.AddControllersWithViews();

// Enregistrer DbContext avec pool pour SQL Server
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDBConnection"))
);

// Ajouter Identity avec EF stores
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
// Dans Program.cs
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";
});
// Configurer les options du mot de passe
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});

// Enregistrer les repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuration du pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Gestion des pages d'erreur
app.UseStatusCodePagesWithReExecute("/Error/{0}");

//app.MapControllerRoute(
    //name: "default",
    //pattern: "{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Middleware pour les erreurs 404
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Request.Path = "/Error/404";
        await next();
    }
});

app.Run();