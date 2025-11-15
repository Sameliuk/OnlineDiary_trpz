using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Data;
using OnlineDiaryApp.Repositories;
using OnlineDiaryApp.Repositories.Implementation;
using OnlineDiaryApp.Repositories.Implementations;
using OnlineDiaryApp.Repositories.Interfaces;
using System.Text;
using OnlineDiaryApp.Patterns.Observers;
using OnlineDiaryApp.Patterns.Facade;
using OnlineDiaryApp.Services.Interfaces;
using OnlineDiaryApp.Services.Implementations;
using OnlineDiaryApp.Patterns.Observers.Interfaces;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<INoteFileRepository, NoteFileRepository>();
builder.Services.AddScoped<INotebookRepository, NotebookRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<INotebookService, NotebookService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IGoogleDriveService, GoogleDriveService>();
builder.Services.AddScoped<IReminderSubject, ReminderSubject>();

builder.Services.AddScoped<NotificationFacade>();
builder.Services.AddScoped<EmailObserver>();
builder.Services.AddScoped<LogObserver>();


builder.Services.AddHostedService<ReminderBackgroundService>();

builder.Services.AddHttpContextAccessor();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
