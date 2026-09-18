using Microsoft.AspNetCore.Authentication.Cookies;
using FreelancerStudent.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Controllers và Views
builder.Services.AddControllersWithViews();

// 2. Đăng ký HttpClient gọi sang API Backend
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7172/";

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<ICustomerApiService, CustomerApiService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// 3. Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// 4. Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Sử dụng Session và Authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 5. CẤU HÌNH ROUTE MẶC ĐỊNH VÀO TRANG LOGIN ĐẦU TIÊN
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
