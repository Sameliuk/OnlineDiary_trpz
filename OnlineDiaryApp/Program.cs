using OnlineDiaryApp.Data;
using OnlineDiaryApp.Services;
using OnlineDiaryApp.Repositories.Implementations;
using OnlineDiaryApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Repositories.Implementation;

var builder = WebApplication.CreateBuilder(args);

// 1. Додаємо MVC
builder.Services.AddControllersWithViews();

// 2. Додаємо DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Додаємо Session і кешування
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Реєстрація репозиторіїв
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();

// 5. Реєстрація сервісів
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ReminderService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHostedService<ReminderBackgroundService>();


// 6. HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 7. Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// 8. Маршрути
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
