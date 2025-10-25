using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalBLL.Services;
using ProyectoFinalDAL.Data;
using ProyectoFinalDAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 🧩 CONFIGURACIÓN DEL DbContext
builder.Services.AddDbContext<SgcDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("ProyectoFinalDAL")
    )
);

// 🔴 REGISTRA REPOS Y SERVICIOS
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddControllersWithViews();

// 🔑 AUTENTICACIÓN Y AUTORIZACIÓN
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 🌐 MANEJO DE ERRORES Y PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 🔑 IMPORTANTE: primero autenticación, luego autorización
app.UseAuthentication();
app.UseAuthorization();

// 🌐 RUTAS
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.Run();
