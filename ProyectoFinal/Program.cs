using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Data;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// 🔹 DbContext con SQL Server
builder.Services.AddDbContext<SgcDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 🔹 Identity con ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SgcDbContext>()
    .AddDefaultTokenProviders();

// 🔹 Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Login/AccessDenied";
});

// 🔹 MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Ejecutar Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

// 🔹 Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 🔹 Ruta por defecto al login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.Run();
