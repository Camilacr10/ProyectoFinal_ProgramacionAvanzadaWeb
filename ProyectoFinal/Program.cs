using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalBLL.Mapeos;
using ProyectoFinalBLL.Services;
using ProyectoFinalDAL.Data;
using ProyectoFinalDAL.Data.Seed;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 🔹 CONFIGURAR DbContext con SQL Server y resiliencia
// ======================================================
builder.Services.AddDbContext<SgcDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()
    )
);

// ======================================================
// 🔹 CONFIGURAR IDENTITY
// ======================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SgcDbContext>()
    .AddDefaultTokenProviders();

// ======================================================
// 🔹 CONFIGURAR COOKIES DE AUTENTICACIÓN
// ======================================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Login/AccessDenied";
});


// AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MapeoClases>(); });


// ======================================================
// 🔹 REGISTRAR SERVICIOS BLL y REPOSITORIOS DAL
// ======================================================
// Cliente
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

// Solicitud
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();

// Tracking
builder.Services.AddScoped<ITrackingServicio, TrackingServicio>();
builder.Services.AddScoped<ITrackingRepositorio, TrackingRepositorio>();


// ======================================================
// 🔹 MVC
// ======================================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ======================================================
// 🔹 APLICAR MIGRACIONES Y EJECUTAR SEED DATA
// ======================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<SgcDbContext>();
        db.Database.Migrate();
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al aplicar migraciones o seed: {ex.Message}");
    }
}

// ======================================================
// 🔹 PIPELINE HTTP
// ======================================================
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

// ======================================================
// 🔹 RUTA POR DEFECTO
// ======================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "Solicitudes",
    pattern: "{controller=Solicitudes}/{action=Index}/{id?}");

app.Run();