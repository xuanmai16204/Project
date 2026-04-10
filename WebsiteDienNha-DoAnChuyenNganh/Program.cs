using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Repositories;
using WebsiteDienNha_DoAnChuyenNganh.Services;
using WebsiteDienNha_DoAnChuyenNganh.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext (SQL Server) and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        // Cho phép số điện thoại làm unique identifier
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// Register repositories and services
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<LoyaltyService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ComparisonService>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<ChatbotService>();
builder.Services.AddScoped<VietQRService>();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

// Bind VietQR settings
builder.Services.Configure<VietQRSetting>(builder.Configuration.GetSection("VietQR"));

// Seed initializer
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run database seed on startup
using (var scope = app.Services.CreateScope())
{
	var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
	await initializer.InitializeAsync();
}

app.Run();
