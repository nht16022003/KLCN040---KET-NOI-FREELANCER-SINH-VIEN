
using FreelancerStudent.Web.Services;
using FreelancerStudent.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Cấu hình cho HttpClient gọi sang FreelancerStudent.API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5152/"); //Cổng port mà API đang chạy
});

// 2. CẤU HÌNH SESSION (Lưu biến tạm trong bộ nhớ)
builder.Services.AddDistributedMemoryCache(); // Lưu Session trong RAM
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Hết hạn sau 30 phút không hoạt động
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//2. Cấu hình Cookie Authentication để quản lý đăng nhập 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.Name = "FreelancerStudent.Cookie";
    options.LoginPath = "/Account/Login"; //Chưa đăng nhập thì chuyển về đây
    options.AccessDeniedPath = "/Account/Denied"; //Sai quyền thì chuyển về đây
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});


//Đăng ký web service
builder.Services.AddScoped<IAuthWebService, AuthWebService>();
builder.Services.AddScoped<INhaTuyenDungWebService, NhaTuyenDungWebService>();
builder.Services.AddScoped<IJobPostWebService, JobPostWebService>();
builder.Services.AddScoped<IFreelancerStudentWebService, FreelancerStudentWebService>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
