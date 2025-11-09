using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Data;
using OnlineDiaryApp.Interfaces;
using OnlineDiaryApp.Repositories;
using OnlineDiaryApp.Repositories.Implementation;
using OnlineDiaryApp.Repositories.Implementations;
using OnlineDiaryApp.Repositories.Interfaces;
using OnlineDiaryApp.Services;
using System.Text;
using OnlineDiaryApp.Patterns.Observers;
using OnlineDiaryApp.Patterns.Facade;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// ✅ Додаємо підтримку MVC
builder.Services.AddControllersWithViews();

// ✅ Налаштування БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Кеш та сесії
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Репозиторії
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<INoteFileRepository, NoteFileRepository>();
builder.Services.AddScoped<INotebookRepository, NotebookRepository>();

// ✅ Сервіси
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<ReminderService>();
builder.Services.AddScoped<NotebookService>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<NotificationFacade>();


builder.Services.AddScoped<IEmailSender, EmailServiceAdapter>();

// ✅ Спостерігачі (Observer pattern)
builder.Services.AddScoped<IReminderSubject, ReminderSubject>();
builder.Services.AddScoped<EmailObserver>();
builder.Services.AddScoped<LogObserver>();

// ✅ Сервіс Google Drive
builder.Services.AddSingleton<GoogleDriveService>();

// ✅ Фоновий сервіс для нагадувань
builder.Services.AddHostedService<ReminderBackgroundService>();

// ✅ Доступ до HttpContext (для сесій і користувача)
builder.Services.AddHttpContextAccessor();

// ✅ CORS — якщо використовуєш API-запити або інтеграцію з Google Drive
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Налаштування конвеєра
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll"); // 🔹 ВАЖЛИВО: CORS має бути ДО авторизації

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
