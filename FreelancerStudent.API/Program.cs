using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Repositories;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using FreelancerStudent.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext kết nối SQL Server
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ĐĂNG KÝ DEPENDENCY INJECTION CHO REPOSITORY TẠI ĐÂY:
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INhaTuyenDungRepository, NhaTuyenDungRepository>();
builder.Services.AddScoped<INhaTuyenDungService, NhaTuyenDungService>();
builder.Services.AddScoped<IFreelancerStudentRepository, FreelancerStudentRepository>();
builder.Services.AddScoped<IFreelancerStudentService, FreelancerStudentService>();
builder.Services.AddScoped<IJobPostRepository, JobPostRepository>();
builder.Services.AddScoped<IJobPostService, JobPostService>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletService, WalletService>();

// 2. Đăng ký Controllers
builder.Services.AddControllers();

// 3. ĐĂNG KÝ SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. BẬT GIAO DIỆN SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreelancerStudent API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
