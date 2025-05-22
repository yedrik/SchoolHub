var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de que la sesión expire
    options.Cookie.HttpOnly = true; // La cookie de sesión no es accesible mediante script del lado del cliente
    options.Cookie.IsEssential = true; // Marca la cookie de sesión como esencial para el RGPD
});
// DB Context
builder.Services.AddScoped<SchoolHub.Utilities.DBContextUtility>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SchoolHub.repositories.Models.EstudianteRepository>();

// Dependencias internas
// Repositories
builder.Services.AddScoped<SchoolHub.repositories.Models.ProfesorRepository>();
builder.Services.AddScoped<SchoolHub.repositories.Models.EstudianteRepository>();
builder.Services.AddScoped<SchoolHub.repositories.Models.MensajeRepository>();
builder.Services.AddScoped<SchoolHub.repositories.Models.CursoRepository>();
builder.Services.AddScoped<SchoolHub.repositories.Models.TareaRepository>();
builder.Services.AddScoped<SchoolHub.repositories.Models.CalificacionesRepository>();
builder.Services.AddScoped<SchoolHub.repositories.Models.UsuarioRepository>();
builder.Services.AddScoped<SchoolHub.repositories.Models.TareaRepository>();

// Services
builder.Services.AddScoped<SchoolHub.Services.EstudianteService>();
builder.Services.AddScoped<SchoolHub.Services.ProfesorService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
