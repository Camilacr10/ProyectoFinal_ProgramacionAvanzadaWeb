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

// DbContext
builder.Services.AddDbContext<SgcDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()
    )
);

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SgcDbContext>()
    .AddDefaultTokenProviders();

// Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Login/AccessDenied";
});

// AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MapeoClases>(); });

// BLL & DAL
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<ITrackingServicio, TrackingServicio>();
builder.Services.AddScoped<ITrackingRepositorio, TrackingRepositorio>();
builder.Services.AddScoped<IFlujoSolicitudesServicio, FlujoSolicitudesServicio>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Migraciones y seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<SgcDbContext>();
    db.Database.Migrate();
    await SeedData.InitializeAsync(services);
}

// Pipeline
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

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.Run();
