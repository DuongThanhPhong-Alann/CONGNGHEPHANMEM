using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Repositories.Interfaces;
using QLCCCC.Repositories;
using QLCCCC.Models;
using QLCCCC.Services;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình kết nối đến cơ sở dữ liệu
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký các repository
builder.Services.AddScoped<IHoaDonDichVuRepository, HoaDonDichVuRepository>();
builder.Services.AddScoped<ITinTucRepository, TinTucRepository>();
builder.Services.AddScoped<IPhanAnhRepository, PhanAnhRepository>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IDichVuRepository, DichVuRepository>();
builder.Services.AddScoped<ICuDanRepository, CuDanRepository>();
builder.Services.AddScoped<IChungCuRepository, ChungCuRepository>();
builder.Services.AddScoped<ICanHoRepository, CanHoRepository>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();


// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

// Cấu hình logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Cấu hình xác thực và phân quyền
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Cấu hình pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Thêm middleware xác thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Định nghĩa route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();