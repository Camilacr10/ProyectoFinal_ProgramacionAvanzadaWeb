using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Data;

// 👇 USINGS PARA DI
using ProyectoFinalDAL.Repositories;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalBLL.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext (usa tu cadena de conexión)
builder.Services.AddDbContext<SgcDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔴 REGISTRA REPO Y SERVICIO (¡CLAVE!)
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
