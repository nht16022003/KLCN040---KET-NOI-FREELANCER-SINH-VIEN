using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Repositories;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using FreelancerStudent.API.Services;
using FreelancerStudent.API.Hubs;

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
builder.Services.AddScoped<IBaiDangTimViecRepository, BaiDangTimViecRepository>();
builder.Services.AddScoped<IBaiDangTimViecService, BaiDangTimViecService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IGiaoDichNapTienRepository, GiaoDichNapTienRepository>();
builder.Services.AddScoped<IGiaoDichNapTienService, GiaoDichNapTienService>();




//  ĐĂNG KÝ SIGNALR VÀ CẤU HÌNH CORS CHO WEB GỌI SANG
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Cho phép Web MVC gọi sang
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Bắt buộc bật dòng này cho kết nối WebSocket SignalR
    });
});


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

// 1. BẬT CORS CHO PHÉP WEB MVC GỌI SANG VÀ KẾT NỐI SIGNALR
app.UseCors("AllowWebClient");

// app.UseHttpsRedirection(); // Bỏ qua HTTPS redirect trong local dev để tránh lỗi 307 cho kết nối SignalR

app.UseAuthorization();
app.MapControllers();


// Endpoint WebSocket để Client kết nối vào
app.MapHub<ChatHub>("/chatHub"); //  Đường dẫn: https://localhost:PORT/chatHub

app.Run();
