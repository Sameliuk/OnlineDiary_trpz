using OnlineDiaryApp.Data;
using OnlineDiaryApp.Services;
using OnlineDiaryApp.Interfaces;
using OnlineDiaryApp.Repositories.Implementations;
using OnlineDiaryApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Repositories.Implementation;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<TagService>();

builder.Services.AddScoped<EmailService>(); 
builder.Services.AddScoped<IEmailSender, EmailServiceAdapter>(); 
builder.Services.AddScoped<ReminderService>(); 


builder.Services.AddHostedService<ReminderBackgroundService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
