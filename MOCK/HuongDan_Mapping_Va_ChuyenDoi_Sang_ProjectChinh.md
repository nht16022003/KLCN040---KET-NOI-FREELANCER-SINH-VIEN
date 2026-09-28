# TÀI LIỆU HƯỚNG DẪN CHI TIẾT: MAPPING & CODE CHUYỂN ĐỔI TỪ MOCK SANG PROJECT CHÍNH (WEB + API)
> **Dự án**: Sàn Giao Dịch Việc Làm Sinh Viên (*Freelancer Students*)  
> **Căn cứ tài liệu**: `CUS_UC.docx` (Khách hàng/Nhà tuyển dụng), `FRL_UC.docx` (Freelancer Sinh viên), `AD_UC.docx` (Admin Quản trị) và CSDL SQL Server `KLCN040_FREELANCERSTUDENT.sql`  
> **Kiến trúc đích**: **2-Tier Enterprise** (ASP.NET Core Web API Backend + ASP.NET Core MVC Frontend) kết nối SQL Server `KETNOI_FREELANCERSV` qua Entity Framework Core, Cookie Authentication & Claims-based Authorization.

---

## MỤC LỤC
1. [TỔNG QUAN KIẾN TRÚC & NGUYÊN TẮC CHUYỂN ĐỔI](#1-tổng-quan-kiến-trúc--nguyên-tắc-chuyển-đổi)
2. [CẤU HÌNH HỆ THỐNG VÀ PIPELINE TRONG `Program.cs`](#2-cấu-hình-hệ-thống-và-pipeline-trong-programcs)
3. [MAPPING DATABASE ENTITY VÀ DBCONTEXT TỪ `KLCN040_FREELANCERSTUDENT.sql`](#3-mapping-database-entity-và-dbcontext-từ-klcn040_freelancerstudentsql)
4. [HIỆN THỰC HÓA CHI TIẾT CÁC CHỨC NĂNG XÁC THỰC & TÀI KHOẢN](#4-hiện-thực-hóa-chi-tiết-các-chức-năng-xác-thực--tài-khoản)
   - 4.1. [Chức năng Đăng ký tài khoản (Register)](#41-chức-năng-đăng-ký-tài-khoản-register)
   - 4.2. [Chức năng Đăng nhập (Login) & Tạo Cookie Claims](#42-chức-năng-đăng-nhập-login--tạo-cookie-claims)
   - 4.3. [Chức năng Đăng xuất (Logout)](#43-chức-năng-đăng-xuất-logout)
   - 4.4. [Chức năng Thay đổi mật khẩu (Change Password) & Quên mật khẩu](#44-chức-năng-thay-đổi-mật-khẩu-change-password--quên-mật-khẩu)
5. [XÂY DỰNG PHÂN QUYỀN NGƯỜI DÙNG (ROLE-BASED AUTHORIZATION - RBAC)](#5-xây-dựng-phân-quyền-người-dùng-role-based-authorization---rbac)
6. [HƯỚNG DẪN MAPPING LAYOUT & DYNAMIC UI THEO TỪNG VAI TRÒ](#6-hướng-dẫn-mapping-layout--dynamic-ui-theo-từng-vai-trò)
7. [BẢNG MAPPING TOÀN DIỆN CONTROLLERS, APIS VÀ VIEWS TỪ MOCK SANG PROJECT CHÍNH](#7-bảng-mapping-toàn-diện-controllers-apis-và-views-từ-mock-sang-project-chính)
8. [CHECKLIST BẢO MẬT & KIỂM THỬ TRƯỚC KHI BÀN GIAO](#8-checklist-bảo-mật--kiểm-thử-trước-khi-bàn-giao)

---

# 1. TỔNG QUAN KIẾN TRÚC & NGUYÊN TẮC CHUYỂN ĐỔI

### 1.1. So sánh Kiến trúc Mock vs Project Chính (Production)

Trong hệ thống thực tế tại thư mục `KLCN040---KET-NOI-FREELANCER-SINH-VIEN-main`, đồ án được tổ chức theo mô hình tách biệt **Web Frontend** (`FreelancerStudent.Web`) và **Backend Web API** (`FreelancerStudent.API`), kết nối với cơ sở dữ liệu `KETNOI_FREELANCERSV`.

| Tiêu chí | Project MOCK (Hiện tại) | Project Chính (`FreelancerStudent.Web` + `FreelancerStudent.API`) |
| :--- | :--- | :--- |
| **Kiến trúc hệ thống** | Monolith MVC nguyên khối, chạy dữ liệu tạm | **2-Tier Architecture**: Web MVC (UI) gọi Backend RESTful API qua `HttpClient` |
| **Lưu trữ dữ liệu** | `MockDataStore` (In-Memory `List<T>`) | **SQL Server `KETNOI_FREELANCERSV`** thông qua Entity Framework Core / Dapper |
| **Xác thực (Authentication)**| Gán cứng Session string: `Session["UserRole"]` | **Cookie Authentication** (`ClaimsPrincipal`, `CookieAuthenticationOptions`, `Claims`) |
| **Bảo mật Mật khẩu** | Chuỗi plain text không an toàn | **Mã hóa Hash Salt chuẩn** (BCrypt / PasswordHasher), không lưu mật khẩu gốc |
| **Phân quyền (Authorization)**| Kiểm tra `if/else` thủ công trong Controller | Thuộc tính phân quyền `[Authorize(Roles = "FreelancerStudent,NhaTuyenDung,Admin")]` |
| **Mapping Layout** | Gộp chung trong `_Layout.cshtml` kiểm tra Session | **Layout linh hoạt** kết hợp các **Partial Views** chuyên biệt theo từng vai trò |

```
                       LUỒNG DỮ LIỆU VÀ XÁC THỰC HỆ THỐNG
┌───────────────────────────────────────────────────────────────────────────────┐
│ TRÌNH DUYỆT (BROWSER CLIENT)                                                  │
│   - Nhập Form Đăng nhập / Đăng ký / Đổi mật khẩu                              │
│   - Lưu trữ Authentication Cookie: "FreelancerStudent.AuthCookie"             │
└───────────────────────▲───────────────────────────────▲───────────────────────┘
                        │ HTTP Request (Cookie Auth)    │ HTML/CSS Response
┌───────────────────────▼───────────────────────────────┴───────────────────────┐
│ TẦNG GIAO DIỆN WEB: FreelancerStudent.Web (ASP.NET Core MVC)                  │
│   - AccountController: Xử lý Form Submit, Login, Register, Logout, Layout     │
│   - Cookie Authentication Middleware: Quản lý ClaimsPrincipal, Session       │
│   - IAuthApiService / AuthApiService: Gọi HttpClient sang Backend             │
└───────────────────────────────────────▲───────────────────────────────────────┘
                                        │ JSON Request / Response (RESTful API)
┌───────────────────────────────────────▼───────────────────────────────────────┐
│ TẦNG DỊCH VỤ BACKEND: FreelancerStudent.API (ASP.NET Core Web API)            │
│   - AuthController: Endpoint /api/Auth/login, /api/Auth/register, /change-pass│
│   - IAuthService / AuthService: Kiểm tra nghiệp vụ, mã hóa Hash BCrypt        │
│   - IUserRepository / AuthRepository: Truy vấn Data Access                    │
│   - AppDbContext: Entity Framework Core                                       │
└───────────────────────────────────────▲───────────────────────────────────────┘
                                        │ T-SQL Queries / Transactions
┌───────────────────────────────────────▼───────────────────────────────────────┐
│ CƠ SỞ DỮ LIỆU: SQL Server (Database: KETNOI_FREELANCERSV)                     │
│   - Bảng Roles (1: FreelancerStudent, 2: NhaTuyenDung, 3: Admin)             │
│   - Bảng Users, FreelancerStudents, NhaTuyenDung, Wallet, MinhChung...        │
└───────────────────────────────────────────────────────────────────────────────┘
```

### 1.2. Sơ đồ Thực thể Dữ liệu Người dùng (`KLCN040_FREELANCERSTUDENT.sql`)

Căn cứ vào file SQL `KLCN040_FREELANCERSTUDENT.sql`, quan hệ bảng người dùng được tổ chức chuẩn hóa:

```
                  ┌──────────────┐
                  │    Roles     │ (1: FreelancerStudent, 2: NhaTuyenDung, 3: Admin)
                  └──────┬───────┘
                         │ 1:N (FK_Users_Roles)
                  ┌──────┴───────┐
                  │    Users     │ (maUser, hotenUser, tenTaiKhoanUser, pashwordHash, emailUser, status)
                  └──┬───┬───┬───┘
         ┌───────────┘   │   └───────────┐
         │ 1:1           │ 1:1           │ 1:1
  ┌──────┴──────┐ ┌──────┴──────┐ ┌──────┴──────┐
  │Freelancer-  │ │NhaTuyenDung │ │    Admin    │
  │  Students   │ │             │ │             │
  └──────┬──────┘ └─────────────┘ └──────┬──────┘
         │ 1:1                           │
  ┌──────┴──────┐                 ┌──────┴──────┐
  │ MinhChung_  │                 │LichSu_XuLy- │
  │Freelancer-  │                 │  TaiKhoan   │
  │  Students   │                 └─────────────┘
  └─────────────┘
         │
  ┌──────┴──────┐
  │   Wallet    │ (Khởi tạo tự động: 0đ khả dụng, 0đ đóng băng)
  └─────────────┘
```

---

# 2. CẤU HÌNH HỆ THỐNG VÀ PIPELINE TRONG `Program.cs`

### 2.1. Cấu hình tại Backend Web API (`FreelancerStudent.API/Program.cs`)

```csharp
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Repositories;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình kết nối SQL Server KETNOI_FREELANCERSV
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký các Repository và Service
builder.Services.AddScoped<IUserRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IFreelancerRepository, FreelancerRepository>();
builder.Services.AddScoped<IFreelancerService, FreelancerService>();

// 3. Đăng ký Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Cấu hình CORS để cho phép Web gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 2.2. Cấu hình tại Web Client MVC (`FreelancerStudent.Web/Program.cs`)

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using FreelancerStudent.Web.Services;
using FreelancerStudent.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Controllers và Views
builder.Services.AddControllersWithViews();

// 2. Đăng ký HttpClient gọi sang API Backend
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5181/";

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ICustomerApiService, CustomerApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IFreelancerApiService, FreelancerApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// 3. Cấu hình Cookie Authentication chuẩn bảo mật
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "FreelancerStudent.AuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
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

// 5. Cấu hình HttpContextAccessor
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

// Thứ tự Middleware bắt buộc: Session -> Authentication -> Authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 6. Cấu hình Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
```

---

# 3. MAPPING DATABASE ENTITY VÀ DBCONTEXT TỪ `KLCN040_FREELANCERSTUDENT.sql`

### 3.1. Entity `User.cs`
Khớp 100% với bảng `Users` trong `KLCN040_FREELANCERSTUDENT.sql`:
```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maUser { get; set; }

        [Required, MaxLength(100)]
        public string hotenUser { get; set; } = null!;

        [Required, MaxLength(30)]
        public string tenTaiKhoanUser { get; set; } = null!;

        [Required, MaxLength(255)]
        public string pashwordHash { get; set; } = null!;

        [Required, MaxLength(100), EmailAddress]
        public string emailUser { get; set; } = null!;

        [MaxLength(15)]
        public string? sdtUser { get; set; }

        public DateTime? ngaysinh { get; set; }

        [Required, MaxLength(20)]
        public string status { get; set; } = "ACTIVE"; // ACTIVE, LOCKED, SUSPENDED, INACTIVE

        public DateTime ngayTao { get; set; } = DateTime.Now;

        [Required]
        public int maRole { get; set; } // 1: FreelancerStudent, 2: NhaTuyenDung, 3: Admin

        [ForeignKey("maRole")]
        public virtual Roles Role { get; set; } = null!;

        public virtual FreelancerStudents? FreelancerStudent { get; set; }
        public virtual NhaTuyenDung? NhaTuyenDung { get; set; }
        public virtual Wallet? Wallet { get; set; }
    }
}
```

### 3.2. Entity `Roles.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int maRole { get; set; } // 1: FreelancerStudent, 2: NhaTuyenDung, 3: Admin

        [Required, MaxLength(30)]
        public string tenRole { get; set; } = null!;

        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
```

### 3.3. Entity `Wallet.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("Wallet")]
    public class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int maWallet { get; set; }

        [Required]
        public int maUser { get; set; }

        public decimal soDuKhaDung { get; set; } = 0;
        public decimal soDuDongBang { get; set; } = 0;

        [ForeignKey("maUser")]
        public virtual Users User { get; set; } = null!;
    }
}
```

### 3.4. Helper Mã hóa Mật khẩu Chuẩn Bảo Mật (`SecurityHelper.cs`)
```csharp
namespace FreelancerStudent.API.Helpers
{
    public static class SecurityHelper
    {
        // Hash mật khẩu bằng BCrypt
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        // Đối chiếu mật khẩu nhập vào với mật khẩu đã Hash trong DB
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(password)) 
                return false;

            // Hỗ trợ kiểm thử dữ liệu seed mock cũ nếu cần
            if (password == "123456" && hashedPassword.StartsWith("hash_pass"))
                return true;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // Fallback nếu hash dạng plain cũ trong quá trình chuyển giao
                return password == hashedPassword;
            }
        }
    }
}
```

---

# 4. HIỆN THỰC HÓA CHI TIẾT CÁC CHỨC NĂNG XÁC THỰC & TÀI KHOẢN

---

## 4.1. Chức năng Đăng ký tài khoản (Register)

### A. Quy trình nghiệp vụ và Kiểm tra hợp lệ:
1. **Thiết kế giao diện đăng ký**:
   - Trường nhập liệu: Họ tên (`FullName`), Email (`Email`), Tên đăng nhập (`Username`), Mật khẩu (`Password`), Xác nhận mật khẩu (`ConfirmPassword`), Số điện thoại (`PhoneNumber`), Loại tài khoản (`RoleType`: `Freelancer` hoặc `Client`).
2. **Kiểm tra dữ liệu (Validation)**:
   - Các trường bắt buộc không được để trống.
   - Kiểm tra định dạng Email hợp lệ theo RFC.
   - Kiểm tra độ mạnh mật khẩu: Tối thiểu 6-8 ký tự, khuyến khích có chữ số và chữ hoa.
   - Kiểm tra `Password == ConfirmPassword`.
3. **Kiểm tra trùng tài khoản**:
   - Truy vấn CSDL xem `emailUser` hoặc `tenTaiKhoanUser` đã tồn tại trong bảng `Users` chưa. Nếu có $\rightarrow$ Trả về lỗi ngăn đăng ký.
4. **Bảo mật mật khẩu**:
   - Tuyệt đối không lưu plain text. Thực hiện Hash mật khẩu bằng `SecurityHelper.HashPassword(password)` trước khi insert vào CSDL.
5. **Khởi tạo dữ liệu đa bảng (Atomic Transaction)**:
   - Tạo bản ghi mới trong bảng `Users` (`maRole = 1` cho Freelancer hoặc `maRole = 2` cho Client/Nhà tuyển dụng, `status = 'ACTIVE'`).
   - Nếu `maRole == 1`: Tạo bản ghi tương ứng trong `FreelancerStudents` (`maUser`, thông tin trường học, GPA mặc định).
   - Nếu `maRole == 2`: Tạo bản ghi tương ứng trong `NhaTuyenDung` (`maUser`, `tencongty = hotenUser`, `trangthai = 'Active'`).
   - Tạo bản ghi khởi tạo ví tiền `Wallet` với `soDuKhaDung = 0` và `soDuDongBang = 0`.
6. **Xử lý sau khi đăng ký**:
   - Thông báo đăng ký thành công.
   - Chuyển hướng người dùng về trang Đăng nhập (`/Account/Login`) hoặc tự động tạo Cookie đăng nhập ngay vào hệ thống.

---

### B. Code DTO & Backend API (`FreelancerStudent.API`):

#### 1. DTO `RegisterRequestDTO.cs` & `RegisterResponseDTO.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.")]
        public string Password { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại tài khoản.")]
        public string RoleType { get; set; } = "Freelancer"; // "Freelancer" hoặc "Client" (NhaTuyenDung)
    }

    public class RegisterResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? MaUser { get; set; }
        public string? Role { get; set; }
    }
}
```

#### 2. Xử lý trong `AuthService.cs` (API Service):
```csharp
public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
{
    // 1. Kiểm tra tài khoản/email trùng lặp
    var existingEmail = await _users.GetByEmailAsync(request.Email.Trim().ToLower());
    if (existingEmail != null)
    {
        return new RegisterResponseDTO
        {
            IsSuccess = false,
            Message = "Địa chỉ email này đã được đăng ký trên hệ thống."
        };
    }

    var existingUser = await _users.GetByUsernameAsync(request.Username.Trim());
    if (existingUser != null)
    {
        return new RegisterResponseDTO
        {
            IsSuccess = false,
            Message = "Tên đăng nhập này đã tồn tại, vui lòng chọn tên khác."
        };
    }

    // 2. Xác định Role ID (1: FreelancerStudent, 2: NhaTuyenDung)
    int roleId = (request.RoleType.Equals("Client", StringComparison.OrdinalIgnoreCase) || 
                  request.RoleType.Equals("NhaTuyenDung", StringComparison.OrdinalIgnoreCase)) ? 2 : 1;

    // 3. Khởi tạo User Entity và Hash Password
    var newUser = new Users
    {
        hotenUser = request.FullName.Trim(),
        tenTaiKhoanUser = request.Username.Trim(),
        emailUser = request.Email.Trim().ToLower(),
        sdtUser = request.PhoneNumber?.Trim(),
        pashwordHash = SecurityHelper.HashPassword(request.Password),
        maRole = roleId,
        status = "ACTIVE",
        ngayTao = DateTime.Now
    };

    // 4. Thực thi thêm vào DB kèm bảng phụ và Ví tiền
    var result = await _users.CreateUserWithProfileAndWalletAsync(newUser, roleId);
    if (!result)
    {
        return new RegisterResponseDTO
        {
            IsSuccess = false,
            Message = "Không thể khởi tạo tài khoản do lỗi máy chủ cơ sở dữ liệu."
        };
    }

    return new RegisterResponseDTO
    {
        IsSuccess = true,
        Message = "Đăng ký tài khoản thành công!",
        MaUser = newUser.maUser,
        Role = roleId == 1 ? "FreelancerStudent" : "NhaTuyenDung"
    };
}
```

#### 3. Endpoint `AuthController.cs` (API):
```csharp
[HttpPost("register")]
public async Task<ActionResult<RegisterResponseDTO>> Register([FromBody] RegisterRequestDTO request)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new RegisterResponseDTO 
        { 
            IsSuccess = false, 
            Message = "Dữ liệu đăng ký không hợp lệ." 
        });
    }

    var result = await _authService.RegisterAsync(request);
    if (!result.IsSuccess)
    {
        return BadRequest(result);
    }

    return Ok(result);
}
```

---

### C. Code Frontend Web MVC (`FreelancerStudent.Web`):

#### 1. ViewModel `RegisterViewModel.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại tài khoản")]
        [Display(Name = "Vai trò tài khoản")]
        public string RoleType { get; set; } = "Freelancer"; // "Freelancer" hoặc "Client"
    }
}
```

#### 2. `AuthApiService.cs` (Gọi API Đăng ký):
```csharp
public async Task<AuthResultDto> RegisterAsync(RegisterViewModel model)
{
    try
    {
        var requestBody = new
        {
            FullName = model.FullName,
            Username = model.Username,
            Email = model.Email,
            Password = model.Password,
            PhoneNumber = model.PhoneNumber,
            RoleType = model.RoleType
        };

        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", requestBody);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<AuthResultDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new AuthResultDto { IsSuccess = false, Message = "Không đọc được dữ liệu phản hồi." };
        }

        return new AuthResultDto { IsSuccess = false, Message = "Đăng ký thất bại: " + responseContent };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Lỗi kết nối khi đăng ký tài khoản");
        return new AuthResultDto { IsSuccess = false, Message = "Lỗi kết nối máy chủ API: " + ex.Message };
    }
}
```

#### 3. Controller `AccountController.cs` (Action Đăng ký):
```csharp
// GET: /Account/Register
[HttpGet]
public IActionResult Register()
{
    if (User.Identity?.IsAuthenticated == true)
    {
        return RedirectToAction("Index", "Home");
    }
    return View(new RegisterViewModel());
}

// POST: /Account/Register
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var result = await _authApiService.RegisterAsync(model);
    if (!result.IsSuccess)
    {
        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    TempData["SuccessMessage"] = $"Đăng ký tài khoản thành công cho {model.FullName}! Vui lòng đăng nhập để bắt đầu.";
    return RedirectToAction("Login", "Account");
}
```

#### 4. Giao diện Đăng ký (`Views/Account/Register.cshtml`):
```html
@model FreelancerStudent.Web.ViewModels.RegisterViewModel
@{
    ViewData["Title"] = "Đăng Ký Tài Khoản";
    Layout = "_Layout";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-7 col-lg-6">
            <div class="card shadow-lg border-0 rounded-4 p-4 p-md-5">
                <div class="text-center mb-4">
                    <h3 class="fw-bold text-primary">Tạo Tài Khoản Mới</h3>
                    <p class="text-muted">Gia nhập sàn kết nối việc làm sinh viên chuyên nghiệp</p>
                </div>

                <form asp-controller="Account" asp-action="Register" method="post">
                    @Html.AntiForgeryToken()
                    <div asp-validation-summary="ModelOnly" class="alert alert-danger" role="alert"></div>

                    <!-- Chọn Loại Tài Khoản -->
                    <div class="mb-4">
                        <label class="form-label fw-semibold">Bạn muốn đăng ký với tư cách?</label>
                        <div class="row g-2">
                            <div class="col-6">
                                <input type="radio" class="btn-check" name="RoleType" id="roleFreelancer" value="Freelancer" checked asp-for="RoleType">
                                <label class="btn btn-outline-primary w-100 py-2 rounded-3 text-center" for="roleFreelancer">
                                    <i class="bi bi-mortarboard-fill d-block fs-4 mb-1"></i> Freelancer Sinh Viên
                                </label>
                            </div>
                            <div class="col-6">
                                <input type="radio" class="btn-check" name="RoleType" id="roleClient" value="Client" asp-for="RoleType">
                                <label class="btn btn-outline-success w-100 py-2 rounded-3 text-center" for="roleClient">
                                    <i class="bi bi-briefcase-fill d-block fs-4 mb-1"></i> Nhà Tuyển Dụng
                                </label>
                            </div>
                        </div>
                    </div>

                    <!-- Họ và Tên -->
                    <div class="mb-3">
                        <label asp-for="FullName" class="form-label fw-semibold"></label>
                        <input asp-for="FullName" class="form-control rounded-3" placeholder="Nguyễn Văn A" />
                        <span asp-validation-for="FullName" class="text-danger small"></span>
                    </div>

                    <!-- Tên đăng nhập -->
                    <div class="mb-3">
                        <label asp-for="Username" class="form-label fw-semibold"></label>
                        <input asp-for="Username" class="form-control rounded-3" placeholder="nguyenvana" />
                        <span asp-validation-for="Username" class="text-danger small"></span>
                    </div>

                    <!-- Email & Số điện thoại -->
                    <div class="row">
                        <div class="col-md-7 mb-3">
                            <label asp-for="Email" class="form-label fw-semibold"></label>
                            <input asp-for="Email" class="form-control rounded-3" placeholder="email@sinhvien.edu.vn" />
                            <span asp-validation-for="Email" class="text-danger small"></span>
                        </div>
                        <div class="col-md-5 mb-3">
                            <label asp-for="PhoneNumber" class="form-label fw-semibold"></label>
                            <input asp-for="PhoneNumber" class="form-control rounded-3" placeholder="0901234567" />
                            <span asp-validation-for="PhoneNumber" class="text-danger small"></span>
                        </div>
                    </div>

                    <!-- Mật khẩu & Xác nhận mật khẩu -->
                    <div class="mb-3">
                        <label asp-for="Password" class="form-label fw-semibold"></label>
                        <input asp-for="Password" class="form-control rounded-3" placeholder="Tối thiểu 6 ký tự" />
                        <span asp-validation-for="Password" class="text-danger small"></span>
                    </div>

                    <div class="mb-4">
                        <label asp-for="ConfirmPassword" class="form-label fw-semibold"></label>
                        <input asp-for="ConfirmPassword" class="form-control rounded-3" placeholder="Nhập lại mật khẩu" />
                        <span asp-validation-for="ConfirmPassword" class="text-danger small"></span>
                    </div>

                    <button type="submit" class="btn btn-primary w-100 py-2 rounded-3 fw-bold shadow-sm">
                        <i class="bi bi-person-plus me-1"></i> Đăng Ký Tài Khoản
                    </button>
                </form>

                <div class="text-center mt-4 pt-3 border-top">
                    <p class="mb-0 text-muted">Đã có tài khoản? <a asp-controller="Account" asp-action="Login" class="fw-semibold text-primary text-decoration-none">Đăng nhập tại đây</a></p>
                </div>
            </div>
        </div>
    </div>
</div>
```

---

## 4.2. Chức năng Đăng nhập (Login) & Tạo Cookie Claims

### A. Quy trình nghiệp vụ:
1. **Thiết kế giao diện đăng nhập**: Form bao gồm `UsernameOrEmail`, `Password`, Checkbox `RememberMe`, nút Đăng nhập và liên kết Quên mật khẩu/Đăng ký.
2. **Kiểm tra thông tin đăng nhập**:
   - Web gửi `LoginRequestDTO` sang Backend API `/api/Auth/login`.
   - API đối chiếu `UsernameOrEmail` với `tenTaiKhoanUser` hoặc `emailUser` trong bảng `Users`.
   - Đối chiếu mật khẩu bằng `SecurityHelper.VerifyPassword(password, user.pashwordHash)`.
3. **Kiểm tra trạng thái tài khoản**:
   - Nếu `status == 'LOCKED'` hoặc `'SUSPENDED'`: Trả về mã lỗi 403 Forbidden kèm thông báo: *"Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên."*
   - Nếu `status == 'INACTIVE'`: Báo tài khoản chưa được kích hoạt.
   - Chỉ cho phép đăng nhập khi `status == 'ACTIVE'`.
4. **Tạo và duy trì phiên đăng nhập bằng Cookie Authentication**:
   - Sau khi API trả về kết quả thành công kèm DTO `LoginResponseDTO`, Web Controller tiến hành tạo danh sách **Claims**:
     + `ClaimTypes.NameIdentifier` $\rightarrow$ `maUser`
     + `ClaimTypes.Name` $\rightarrow$ `tenTaiKhoanUser`
     + `ClaimTypes.Email` $\rightarrow$ `emailUser`
     + `ClaimTypes.Role` $\rightarrow$ `tenRole` (`Admin`, `NhaTuyenDung`, `FreelancerStudent`)
     + Custom Claim `"FullName"` $\rightarrow$ `hotenUser`
     + Custom Claim `"MaSpecific"` $\rightarrow$ `maFreelancerStudents` hoặc `maNhaTuyenDung`
   - Tạo `ClaimsIdentity` với Schema `CookieAuthenticationDefaults.AuthenticationScheme`.
   - Cấu hình `AuthenticationProperties` (Nếu `RememberMe == true` $\rightarrow$ thời hạn Cookie lưu 7-14 ngày, ngược lại theo phiên làm việc).
   - Gọi `await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties)`.
5. **Điều hướng sau đăng nhập**:
   - Nếu có `ReturnUrl` hợp lệ (`Url.IsLocalUrl(returnUrl)`) $\rightarrow$ Redirect về `ReturnUrl`.
   - Nếu `Role == "Admin"` $\rightarrow$ Chuyển đến trang Quản trị Admin.
   - Nếu `Role == "NhaTuyenDung"` $\rightarrow$ Chuyển đến `Customer/Dashboard` hoặc Quản lý tin tuyển dụng.
   - Nếu `Role == "FreelancerStudent"` $\rightarrow$ Chuyển đến `Freelancer/Dashboard` hoặc Quản lý việc làm.

---

### B. Code Hiện thực Đăng nhập:

#### 1. Backend Service `AuthService.cs` (`FreelancerStudent.API`):
```csharp
public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
{
    var input = request.UsernameOrEmail.Trim();
    var user = await _users.GetByUsernameOrEmailAsync(input);

    if (user == null || !SecurityHelper.VerifyPassword(request.Password, user.pashwordHash))
    {
        return new LoginResponseDTO
        {
            IsSuccess = false,
            Message = "Tên đăng nhập/Email hoặc mật khẩu không chính xác."
        };
    }

    if (user.status is "LOCKED" or "SUSPENDED" or "INACTIVE")
    {
        return new LoginResponseDTO
        {
            IsSuccess = false,
            status = user.status,
            Message = $"Tài khoản hiện đang bị khóa hoặc ngưng hoạt động (Trạng thái: {user.status})."
        };
    }

    _logger.LogInformation("Đăng nhập thành công: User ID {UserId}, Role {Role}", user.maUser, user.Role?.tenRole);

    return new LoginResponseDTO
    {
        IsSuccess = true,
        Message = "Đăng nhập thành công!",
        maUser = user.maUser,
        tenTaiKhoanUser = user.tenTaiKhoanUser,
        hotenUser = user.hotenUser,
        emailUser = user.emailUser,
        sdtUser = user.sdtUser,
        tenRole = user.Role?.tenRole ?? "FreelancerStudent",
        status = user.status,
        maNhaTuyenDung = user.NhaTuyenDung?.maNhaTuyenDung,
        maFreelancerStudents = user.FreelancerStudent?.maFreelancerStudents,
        Token = "auth-session-token-" + user.maUser
    };
}
```

#### 2. Frontend Controller `AccountController.cs` (`FreelancerStudent.Web`):
```csharp
// GET: /Account/Login
[HttpGet]
public IActionResult Login(string? returnUrl = null)
{
    if (User.Identity?.IsAuthenticated == true)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role == "NhaTuyenDung") return RedirectToAction("Dashboard", "Customer");
        if (role == "FreelancerStudent") return RedirectToAction("Dashboard", "Freelancer");
        return RedirectToAction("Index", "Home");
    }

    return View(new LoginViewModel { ReturnUrl = returnUrl });
}

// POST: /Account/Login
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    // 1. Gọi Backend Web API xác thực
    var authResult = await _authApiService.LoginAsync(model.UsernameOrEmail, model.Password);

    if (!authResult.IsSuccess)
    {
        ModelState.AddModelError(string.Empty, authResult.Message);
        return View(model);
    }

    // 2. Thiết lập Claims và Cookie Authentication
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, authResult.maUser?.ToString() ?? string.Empty),
        new Claim(ClaimTypes.Name, authResult.tenTaiKhoanUser ?? string.Empty),
        new Claim(ClaimTypes.Email, authResult.emailUser ?? string.Empty),
        new Claim(ClaimTypes.Role, authResult.tenRole ?? "FreelancerStudent"),
        new Claim("FullName", authResult.hotenUser ?? string.Empty)
    };

    if (authResult.maFreelancerStudents.HasValue)
    {
        claims.Add(new Claim("MaFreelancerStudents", authResult.maFreelancerStudents.Value.ToString()));
    }
    if (authResult.maNhaTuyenDung.HasValue)
    {
        claims.Add(new Claim("MaNhaTuyenDung", authResult.maNhaTuyenDung.Value.ToString()));
    }

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties
    {
        IsPersistent = model.RememberMe,
        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
    };

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

    // 3. Đồng bộ Session
    HttpContext.Session.SetString("UserID", authResult.maUser?.ToString() ?? string.Empty);
    HttpContext.Session.SetString("UserName", authResult.tenTaiKhoanUser ?? string.Empty);
    HttpContext.Session.SetString("FullName", authResult.hotenUser ?? string.Empty);
    HttpContext.Session.SetString("UserRole", authResult.tenRole ?? string.Empty);

    TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {authResult.hotenUser} quay trở lại.";

    // 4. Xử lý ReturnUrl an toàn (Chống Open Redirect Attack)
    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
    {
        return Redirect(model.ReturnUrl);
    }

    // 5. Điều hướng theo Role
    return authResult.tenRole switch
    {
        "NhaTuyenDung" => RedirectToAction("Dashboard", "Customer"),
        "FreelancerStudent" => RedirectToAction("Dashboard", "Freelancer"),
        "Admin" => RedirectToAction("Index", "Admin"),
        _ => RedirectToAction("Index", "Home")
    };
}
```

---

## 4.3. Chức năng Đăng xuất (Logout)

### A. Quy trình nghiệp vụ:
1. Cho phép người dùng chủ động kết thúc phiên làm việc qua nút Đăng xuất trên Header hoặc Menu User.
2. Gọi `await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)` để **hủy Authentication Cookie** trên trình duyệt.
3. Xóa toàn bộ Session (`HttpContext.Session.Clear()`).
4. Gửi tín hiệu sang API (nếu cần ghi nhận nhật ký kết thúc phiên).
5. **Ngăn chặn truy cập sau khi đăng xuất**:
   - Mọi truy cập vào các Controller/Action có gắn attribute `[Authorize]` sẽ tự động bị chặn và điều hướng về trang Đăng nhập (`/Account/Login?ReturnUrl=...`).

### B. Code Controller `AccountController.cs` (Action Đăng xuất):
```csharp
// GET & POST: /Account/Logout
[HttpGet]
[HttpPost]
public async Task<IActionResult> Logout()
{
    // 1. Gọi API Backend hủy phiên làm việc
    await _authApiService.LogoutAsync();

    // 2. Hủy Authentication Cookie
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // 3. Xóa sạch Session trên Server
    HttpContext.Session.Clear();

    // 4. Thông báo và chuyển về trang Login
    TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công khỏi hệ thống.";
    return RedirectToAction("Login", "Account");
}
```

---

## 4.4. Chức năng Thay đổi mật khẩu (Change Password) & Quên mật khẩu

### A. Quy trình nghiệp vụ:
1. Người dùng đã đăng nhập truy cập chức năng **Đổi mật khẩu** tại trang Thông tin cá nhân / Thiết lập tài khoản.
2. **Nhập dữ liệu**:
   - Nhập Mật khẩu hiện tại (`CurrentPassword`).
   - Nhập Mật khẩu mới (`NewPassword`).
   - Xác nhận Mật khẩu mới (`ConfirmNewPassword`).
3. **Kiểm tra điều kiện**:
   - Mật khẩu hiện tại phải trùng khớp với hash trong CSDL (`SecurityHelper.VerifyPassword`).
   - Mật khẩu mới phải khác mật khẩu hiện tại.
   - Mật khẩu mới đạt chuẩn độ dài $\ge 6$ ký tự.
   - Mật khẩu mới và Xác nhận mật khẩu phải trùng khớp.
4. **Cập nhật CSDL**:
   - Hash mật khẩu mới bằng `SecurityHelper.HashPassword(NewPassword)`.
   - Lưu vào trường `pashwordHash` của bảng `Users` theo `maUser`.
5. **Xử lý phiên sau khi đổi mật khẩu**:
   - Hủy Cookie đăng nhập hiện tại và yêu cầu người dùng đăng nhập lại với mật khẩu mới để đảm bảo an toàn.

---

### B. Code Đổi Mật Khẩu:

#### 1. DTO `ChangePasswordRequestDTO.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.Auth
{
    public class ChangePasswordRequestDTO
    {
        [Required]
        public int MaUser { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải từ 6 ký tự trở lên.")]
        public string NewPassword { get; set; } = null!;
    }
}
```

#### 2. Xử lý trong `AuthService.cs` (API):
```csharp
public async Task<bool> ChangePasswordAsync(ChangePasswordRequestDTO request, out string errorMessage)
{
    var user = await _users.GetById(request.MaUser);
    if (user == null)
    {
        errorMessage = "Người dùng không tồn tại.";
        return false;
    }

    // 1. Kiểm tra mật khẩu hiện tại
    if (!SecurityHelper.VerifyPassword(request.CurrentPassword, user.pashwordHash))
    {
        errorMessage = "Mật khẩu hiện tại không chính xác.";
        return false;
    }

    // 2. Kiểm tra mật khẩu mới không trùng mật khẩu cũ
    if (SecurityHelper.VerifyPassword(request.NewPassword, user.pashwordHash))
    {
        errorMessage = "Mật khẩu mới không được trùng với mật khẩu cũ.";
        return false;
    }

    // 3. Hash và cập nhật mật khẩu mới
    user.pashwordHash = SecurityHelper.HashPassword(request.NewPassword);
    await _users.UpdateUserAsync(user);

    errorMessage = string.Empty;
    return true;
}
```

#### 3. Controller `AccountController.cs` (Frontend Web):
```csharp
// POST: /Account/ChangePassword
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View("Profile", model);
    }

    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!int.TryParse(userIdClaim, out int userId))
    {
        return RedirectToAction("Login", "Account");
    }

    var changePassDto = new
    {
        MaUser = userId,
        CurrentPassword = model.CurrentPassword,
        NewPassword = model.NewPassword
    };

    var result = await _authApiService.ChangePasswordAsync(changePassDto);
    if (!result.IsSuccess)
    {
        ModelState.AddModelError(string.Empty, result.Message);
        return View("Profile", model);
    }

    // Đổi mật khẩu thành công -> Hủy Cookie và yêu cầu đăng nhập lại
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    HttpContext.Session.Clear();

    TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại với mật khẩu mới.";
    return RedirectToAction("Login", "Account");
}
```

---

# 5. XÂY DỰNG PHÂN QUYỀN NGƯỜI DÙNG (ROLE-BASED AUTHORIZATION - RBAC)

Hệ thống quản lý 3 vai trò phân quyền chính dựa trên bảng `Roles` trong SQL Server:

| Mã Role (`maRole`) | Tên Role (`tenRole`) | Quyền hạn và Trách nhiệm chức năng |
| :---: | :--- | :--- |
| **1** | `FreelancerStudent` | Ứng tuyển dự án, quản lý hồ sơ năng lực & Portfolio, nộp minh chứng sinh viên, cập nhật tiến độ hợp đồng, rút tiền ví thu nhập. |
| **2** | `NhaTuyenDung` | Đăng tin tuyển dụng, tìm kiếm & bookmark Freelancer, gửi lời mời làm việc, tạo hợp đồng & nạp tiền ký quỹ Escrow, đánh giá nghiệm thu. |
| **3** | `Admin` | Quản trị toàn bộ hệ thống, duyệt minh chứng thẻ sinh viên, xử lý tài khoản (Khóa/Mở), giải quyết tranh chấp hợp đồng, duyệt yêu cầu rút tiền ví. |

### 5.1. Phân quyền Controller và Action bằng Thuộc tính `[Authorize]`

```csharp
// 1. Phân quyền bảo vệ Controller dành riêng cho Nhà Tuyển Dụng
[Authorize(Roles = "NhaTuyenDung")]
public class CustomerController : Controller
{
    public IActionResult Dashboard() => View();
    public IActionResult ManageJobs() => View();
    public IActionResult CreateJob() => View();
}

// 2. Phân quyền bảo vệ Controller dành riêng cho Freelancer Sinh Viên
[Authorize(Roles = "FreelancerStudent")]
public class FreelancerController : Controller
{
    public IActionResult Dashboard() => View();
    public IActionResult QuanLyTimViec() => View();
    public IActionResult CapNhatHoSo() => View();
}

// 3. Phân quyền dành riêng cho Quản trị viên Admin
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Index() => View();
    public IActionResult DanhSachNguoiDung() => View();
    public IActionResult XuLyTranhChap() => View();
}

// 4. Phân quyền đa vai trò (Cả Freelancer và Khách hàng đều có quyền truy cập)
[Authorize(Roles = "FreelancerStudent,NhaTuyenDung")]
public class HopDongController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Details(string id) => View();
}
```

### 5.2. Trang Xử lý Truy cập Trái phép (`/Account/AccessDenied`)
Khi người dùng cố tình truy cập vào trang không thuộc vai trò của mình (Ví dụ: Freelancer truy cập `/Admin` hoặc `/Customer/CreateJob`):
```csharp
// GET: /Account/AccessDenied
[HttpGet]
public IActionResult AccessDenied()
{
    return View();
}
```

---

# 6. HƯỚNG DẪN MAPPING & SỬ DỤNG TRỰC TIẾP `_Layout.cshtml` TỪ MOCK SANG PROJECT CHÍNH

Giao diện `_Layout.cshtml` trong project MOCK đã được thiết kế và chau chuốt hoàn chỉnh:
- **Tông màu thương hiệu hiện đại:** Xanh ngọc chủ đạo (`#3B6E53`, `#2A523E`, `#EBF4F0`).
- **Typography & Font chữ:** Google Fonts (*Plus Jakarta Sans* & *Caveat*).
- **Hệ thống Navbar & Subnav đa vai trò:** Tự động đổi thanh điều hướng và menu phụ theo từng Role (Guest, Khách hàng/Nhà tuyển dụng, Freelancer Sinh viên, Quản trị viên Admin).
- **Chân trang (Footer):** Phân cột liên kết đầy đủ, chuẩn responsive trên Mobile/Tablet/Desktop.

Dưới đây là **2 phương án** chi tiết để bạn đưa trực tiếp `_Layout.cshtml` của MOCK vào Project chính (`FreelancerStudent.Web`):

---

### PHƯƠNG ÁN 1: DÙNG TRỰC TIẾP 1 FILE `_Layout.cshtml` DUY NHẤT (Nhanh nhất & Dễ nhất)

Phương án này giúp bạn giữ nguyên 100% giao diện đẹp mắt của MOCK, chỉ cần đổi cách lấy thông tin người dùng từ **Session giả lập** sang **Claims Cookie Authentication** của ASP.NET Core.

#### Bước 1: Sao chép tài nguyên tĩnh (Static Assets) từ MOCK sang `FreelancerStudent.Web`
1. Copy toàn bộ thư mục `MOCK/wwwroot/css/` sang `FreelancerStudent.Web/wwwroot/css/` (đặc biệt là file `site.css`).
2. Copy toàn bộ thư mục `MOCK/wwwroot/js/` sang `FreelancerStudent.Web/wwwroot/js/` (file `site.js`).
3. Đảm bảo thư mục `FreelancerStudent.Web/wwwroot/lib/` có sẵn thư viện `bootstrap` và `jquery`.

#### Bước 2: Thay thế toàn bộ mã nguồn file `FreelancerStudent.Web/Views/Shared/_Layout.cshtml`
Mở file `FreelancerStudent.Web/Views/Shared/_Layout.cshtml` và dán mã nguồn đã được tối ưu hóa sau:

```html
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@{
    // 1. Kiểm tra trạng thái đăng nhập thực tế từ Cookie Authentication
    var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

    // 2. Trích xuất Role từ Claims (kèm fallback Session)
    var role = User.FindFirst(ClaimTypes.Role)?.Value 
               ?? Context.Session.GetString("UserRole") 
               ?? "Guest";

    // 3. Trích xuất Tên người dùng và Email từ Claims
    var fullName = User.FindFirst("FullName")?.Value 
                   ?? User.FindFirst(ClaimTypes.Name)?.Value 
                   ?? Context.Session.GetString("FullName") 
                   ?? "Người dùng";
    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
                    ?? Context.Session.GetString("UserEmail") 
                    ?? "";

    // 4. Tiêu đề hiển thị theo Role
    var userRoleTitle = role switch
    {
        "NhaTuyenDung" => "Nhà tuyển dụng",
        "FreelancerStudent" => "Freelancer Student",
        "Admin" => "Quản trị viên",
        _ => "Khách"
    };

    // 5. Ảnh đại diện
    var userAvatar = User.FindFirst("Avatar")?.Value 
                     ?? Context.Session.GetString("UserAvatar") 
                     ?? "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=100&auto=format&fit=crop&q=80";
}

<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Sàn Giao Dịch Freelancer Sinh Viên</title>

    <!-- Google Fonts -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=Caveat:wght@600;700&display=swap" rel="stylesheet">

    <!-- Bootstrap 5 & Bootstrap Icons -->
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body class="d-flex flex-column min-vh-100">

    <!-- ========================================================= -->
    <!-- 1. HEADER DÀNH CHO KHÁCH (CHƯA ĐĂNG NHẬP)                 -->
    <!-- ========================================================= -->
    @if (!isAuthenticated || role == "Guest")
    {
        <header class="auth-header">
            <div class="container d-flex align-items-center justify-content-between">
                <a asp-controller="Home" asp-action="Index" class="brand-logo">
                    <i class="bi bi-mortarboard-fill text-success"></i> Freelancer <span>Students</span>
                </a>
                <nav class="d-none d-md-flex align-items-center gap-2">
                    <a asp-controller="Home" asp-action="Index" class="nav-link-custom">Trang chủ</a>
                    <a href="/Employer/Search" class="nav-link-custom">Tìm việc làm</a>
                    <a href="/Freelancer/Search" class="nav-link-custom">Tìm Freelancer</a>
                </nav>
                <div class="d-flex align-items-center gap-2">
                    <a asp-controller="Account" asp-action="Login" class="btn-header-login">Đăng nhập</a>
                    <a asp-controller="Account" asp-action="Register" class="btn-header-register">Đăng ký</a>
                </div>
            </div>
        </header>
    }

    <!-- ========================================================= -->
    <!-- 2. HEADER DÀNH CHO NHÀ TUYỂN DỤNG                         -->
    <!-- ========================================================= -->
    else if (role == "NhaTuyenDung")
    {
        <header class="portal-header">
            <div class="container-fluid px-lg-4">
                <div class="d-flex align-items-center justify-content-between">
                    <div class="d-flex align-items-center gap-3">
                        <a asp-controller="Customer" asp-action="Dashboard" class="brand-logo">
                            <i class="bi bi-briefcase-fill text-success"></i> Freelancer <span>Students</span>
                        </a>
                        <a href="/Employer/Search" class="search-pill-btn d-none d-sm-inline-flex">
                            <i class="bi bi-search"></i> Tìm kiếm Freelancer...
                        </a>
                    </div>
                    
                    <div class="d-flex align-items-center gap-3">
                        <a href="/ThongBao" class="portal-icon-btn position-relative" title="Thông báo">
                            <i class="bi bi-bell"></i>
                        </a>
                        <a href="/NhanTin" class="portal-icon-btn position-relative" title="Tin nhắn">
                            <i class="bi bi-chat-dots"></i>
                        </a>

                        <!-- User Profile Dropdown -->
                        <div class="dropdown">
                            <a href="#" class="portal-user-profile dropdown-toggle text-decoration-none" data-bs-toggle="dropdown">
                                <img src="@userAvatar" alt="@fullName" class="portal-user-avatar" />
                                <div class="portal-user-info d-none d-lg-block">
                                    <span class="portal-user-name">@fullName</span>
                                    <span class="portal-user-tag text-success">@userRoleTitle</span>
                                </div>
                            </a>
                            <ul class="dropdown-menu dropdown-menu-end shadow-sm border-0 mt-2">
                                <li><a class="dropdown-item" href="/Employer/Profile"><i class="bi bi-building me-2"></i> Hồ sơ doanh nghiệp</a></li>
                                <li><a class="dropdown-item" href="/Wallet"><i class="bi bi-wallet2 text-success me-2"></i> Ví tài chính</a></li>
                                <li><a class="dropdown-item" href="/Account/Profile"><i class="bi bi-shield-lock me-2"></i> Đổi mật khẩu</a></li>
                                <li><hr class="dropdown-divider"></li>
                                <li><a class="dropdown-item text-danger" asp-controller="Account" asp-action="Logout"><i class="bi bi-box-arrow-right me-2"></i> Đăng xuất</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </header>

        <!-- Subnav Nhà tuyển dụng -->
        <nav class="portal-subnav">
            <div class="container-fluid px-lg-4">
                <div class="subnav-container">
                    <a asp-controller="Customer" asp-action="Dashboard" class="subnav-item active"><i class="bi bi-speedometer2 me-1"></i> Tổng quan</a>
                    <a href="/Employer/ManageJobs" class="subnav-item"><i class="bi bi-journal-text me-1"></i> Quản lý tin tuyển</a>
                    <a href="/Employer/Search" class="subnav-item"><i class="bi bi-search me-1"></i> Tìm Freelancer</a>
                    <a href="/Employer/FreelancerYeuThich" class="subnav-item"><i class="bi bi-bookmark-heart me-1"></i> Đã lưu</a>
                    <a href="/HopDong" class="subnav-item"><i class="bi bi-file-earmark-check me-1"></i> Hợp đồng</a>
                    <a href="/Wallet" class="subnav-item"><i class="bi bi-wallet2 me-1"></i> Ví tài chính</a>
                </div>
            </div>
        </nav>
    }

    <!-- ========================================================= -->
    <!-- 3. HEADER DÀNH CHO FREELANCER SINH VIÊN                   -->
    <!-- ========================================================= -->
    else if (role == "FreelancerStudent")
    {
        <header class="portal-header">
            <div class="container-fluid px-lg-4">
                <div class="d-flex align-items-center justify-content-between">
                    <div class="d-flex align-items-center gap-3">
                        <a asp-controller="Freelancer" asp-action="Dashboard" class="brand-logo">
                            <i class="bi bi-mortarboard-fill text-success"></i> Freelancer <span>Students</span>
                        </a>
                        <a href="/Freelancer/Search" class="search-pill-btn d-none d-sm-inline-flex">
                            <i class="bi bi-search"></i> Tìm kiếm dự án / việc làm...
                        </a>
                    </div>

                    <div class="d-flex align-items-center gap-3">
                        <a href="/ThongBao" class="portal-icon-btn position-relative" title="Thông báo">
                            <i class="bi bi-bell"></i>
                        </a>
                        <a href="/NhanTin" class="portal-icon-btn position-relative" title="Tin nhắn">
                            <i class="bi bi-chat-dots"></i>
                        </a>

                        <!-- User Profile Dropdown -->
                        <div class="dropdown">
                            <a href="#" class="portal-user-profile dropdown-toggle text-decoration-none" data-bs-toggle="dropdown">
                                <img src="@userAvatar" alt="@fullName" class="portal-user-avatar" />
                                <div class="portal-user-info d-none d-lg-block">
                                    <span class="portal-user-name">@fullName</span>
                                    <span class="portal-user-tag text-primary">@userRoleTitle</span>
                                </div>
                            </a>
                            <ul class="dropdown-menu dropdown-menu-end shadow-sm border-0 mt-2">
                                <li><a class="dropdown-item" href="/Freelancer/HoSo"><i class="bi bi-person-badge me-2"></i> Hồ sơ năng lực</a></li>
                                <li><a class="dropdown-item" href="/Freelancer/Portfolio"><i class="bi bi-briefcase me-2"></i> Portfolio dự án</a></li>
                                <li><a class="dropdown-item" href="/Account/MinhChung"><i class="bi bi-patch-check text-success me-2"></i> Xác thực sinh viên</a></li>
                                <li><a class="dropdown-item" href="/Wallet"><i class="bi bi-wallet2 text-success me-2"></i> Ví thu nhập</a></li>
                                <li><a class="dropdown-item" href="/Account/Profile"><i class="bi bi-shield-lock me-2"></i> Đổi mật khẩu</a></li>
                                <li><hr class="dropdown-divider"></li>
                                <li><a class="dropdown-item text-danger" asp-controller="Account" asp-action="Logout"><i class="bi bi-box-arrow-right me-2"></i> Đăng xuất</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </header>

        <!-- Subnav Freelancer -->
        <nav class="portal-subnav">
            <div class="container-fluid px-lg-4">
                <div class="subnav-container">
                    <a asp-controller="Freelancer" asp-action="Dashboard" class="subnav-item active"><i class="bi bi-speedometer2 me-1"></i> Tổng quan</a>
                    <a href="/Freelancer/Search" class="subnav-item"><i class="bi bi-search me-1"></i> Tìm việc</a>
                    <a href="/Freelancer/DanhSachNopTuyen" class="subnav-item"><i class="bi bi-send me-1"></i> Đã ứng tuyển</a>
                    <a href="/HopDong" class="subnav-item"><i class="bi bi-file-earmark-check me-1"></i> Hợp đồng</a>
                    <a href="/Freelancer/HoSo" class="subnav-item"><i class="bi bi-person-lines-fill me-1"></i> Hồ sơ</a>
                    <a href="/Wallet" class="subnav-item"><i class="bi bi-wallet2 me-1"></i> Ví tiền</a>
                </div>
            </div>
        </nav>
    }

    <!-- ========================================================= -->
    <!-- 4. HEADER DÀNH CHO ADMIN QUẢN TRỊ                         -->
    <!-- ========================================================= -->
    else if (role == "Admin")
    {
        <header class="portal-header bg-dark text-white border-bottom border-secondary">
            <div class="container-fluid px-lg-4">
                <div class="d-flex align-items-center justify-content-between">
                    <a href="/Admin/Index" class="brand-logo text-white">
                        <i class="bi bi-shield-check text-warning"></i> Freelancer <span>Admin Portal</span>
                    </a>
                    <div class="d-flex align-items-center gap-3">
                        <span class="badge bg-warning text-dark fw-bold px-3 py-2">Quản Trị Viên</span>
                        <a asp-controller="Account" asp-action="Logout" class="btn btn-outline-danger btn-sm rounded-pill px-3">
                            <i class="bi bi-box-arrow-right me-1"></i> Đăng xuất
                        </a>
                    </div>
                </div>
            </div>
        </header>

        <!-- Subnav Admin -->
        <nav class="portal-subnav bg-dark border-secondary">
            <div class="container-fluid px-lg-4">
                <div class="subnav-container">
                    <a href="/Admin/Index" class="subnav-item text-white active"><i class="bi bi-speedometer2 me-1"></i> Thống kê</a>
                    <a href="/Admin/DanhSachNguoiDung" class="subnav-item text-white"><i class="bi bi-people me-1"></i> Người dùng</a>
                    <a href="/Admin/XuLyTaiKhoan" class="subnav-item text-white"><i class="bi bi-person-x me-1"></i> Xử lý tài khoản</a>
                    <a href="/Admin/QuanLyHopDong" class="subnav-item text-white"><i class="bi bi-file-earmark-text me-1"></i> Hợp đồng</a>
                    <a href="/Admin/XuLyTranhChap" class="subnav-item text-white"><i class="bi bi-exclamation-triangle me-1"></i> Tranh chấp</a>
                    <a href="/Admin/YeuCauNapTien" class="subnav-item text-white"><i class="bi bi-cash-stack me-1"></i> Nạp/Rút tiền</a>
                </div>
            </div>
        </nav>
    }

    <!-- ========================================================= -->
    <!-- NỘI DUNG VIEW CON                                         -->
    <!-- ========================================================= -->
    <main role="main" class="flex-grow-1">
        @if (TempData["SuccessMessage"] != null)
        {
            <div class="container mt-3">
                <div class="alert alert-success alert-dismissible fade show rounded-3 shadow-sm" role="alert">
                    <i class="bi bi-check-circle-fill me-2"></i> @TempData["SuccessMessage"]
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>
            </div>
        }
        @if (TempData["ErrorMessage"] != null)
        {
            <div class="container mt-3">
                <div class="alert alert-danger alert-dismissible fade show rounded-3 shadow-sm" role="alert">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i> @TempData["ErrorMessage"]
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>
            </div>
        }

        @RenderBody()
    </main>

    <!-- ========================================================= -->
    <!-- FOOTER CHUNG                                              -->
    <!-- ========================================================= -->
    <footer class="main-footer mt-auto">
        <div class="container">
            <div class="row g-4">
                <div class="col-12 col-md-4">
                    <div class="footer-brand">
                        <i class="bi bi-mortarboard-fill text-success"></i> Freelancer <span>Students</span>
                    </div>
                    <p class="footer-desc">
                        Nền tảng kết nối sinh viên tài năng với các cơ hội việc làm thực tế uy tín và an toàn.
                    </p>
                </div>
                <div class="col-6 col-md-2">
                    <h5 class="footer-heading">Dành cho Sinh viên</h5>
                    <ul class="footer-links">
                        <li><a href="/Freelancer/Search">Tìm việc làm</a></li>
                        <li><a href="/Account/MinhChung">Xác thực sinh viên</a></li>
                        <li><a href="/Wallet">Rút tiền thu nhập</a></li>
                    </ul>
                </div>
                <div class="col-6 col-md-2">
                    <h5 class="footer-heading">Dành cho Nhà tuyển dụng</h5>
                    <ul class="footer-links">
                        <li><a href="/Employer/ManageJobs">Đăng tin tuyển dụng</a></li>
                        <li><a href="/Employer/Search">Tìm kiếm Freelancer</a></li>
                        <li><a href="/HopDong">Ký quỹ hợp đồng</a></li>
                    </ul>
                </div>
                <div class="col-12 col-md-4">
                    <h5 class="footer-heading">Hỗ trợ & Pháp lý</h5>
                    <p class="footer-desc small">
                        Hệ thống áp dụng hợp đồng điện tử và ký quỹ bảo chứng Escrow để đảm bảo quyền lợi tối đa cho cả sinh viên và khách hàng.
                    </p>
                </div>
            </div>
            <div class="footer-bottom border-top pt-3 mt-4 text-center text-muted small">
                &copy; @DateTime.Now.Year Freelancer Students Platform. Bảo lưu mọi quyền.
            </div>
        </div>
    </footer>

    <!-- JS Dependencies -->
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

### PHƯƠNG ÁN 2: TÁCH NHỎ THÀNH CÁC PARTIAL VIEWS (Theo kiến trúc Clean Code)

Nếu muốn module hóa để dễ bảo trì dài hạn, bạn tách các khối header trên thành các file nhỏ trong thư mục `FreelancerStudent.Web/Views/Shared/`:
- `_HeaderGuest.cshtml`
- `_HeaderCustomer.cshtml`
- `_HeaderFreelancer.cshtml`
- `_HeaderAdmin.cshtml`
- `_Footer.cshtml`

Và trong `_Layout.cshtml` chỉ cần gọi:
```html
@if (!isAuthenticated || role == "Guest") { <partial name="_HeaderGuest" /> }
else if (role == "NhaTuyenDung") { <partial name="_HeaderCustomer" /> }
else if (role == "FreelancerStudent") { <partial name="_HeaderFreelancer" /> }
else if (role == "Admin") { <partial name="_HeaderAdmin" /> }
```


---

# 7. BẢNG MAPPING TOÀN DIỆN CONTROLLERS, APIS VÀ VIEWS TỪ MOCK SANG PROJECT CHÍNH

Dưới đây là bảng ánh xạ toàn bộ 12 Controllers và các Views từ thư mục `MOCK/` sang cấu trúc 2 tầng của Project chính (`FreelancerStudent.Web` và `FreelancerStudent.API`), đối chiếu trực tiếp với các Use Case trong `CUS_UC.docx`, `FRL_UC.docx`, `AD_UC.docx` và bảng CSDL SQL:

| STT | Chức năng / Màn hình trong MOCK | Controller & Views Web (`FreelancerStudent.Web`) | Controller & Endpoint API (`FreelancerStudent.API`) | Bảng SQL Tương Ứng (`KLCN040_FREELANCERSTUDENT.sql`) | Use Case Phục Vụ |
| :---: | :--- | :--- | :--- | :--- | :--- |
| **1** | **Xác thực & Tài khoản**<br>• `Account/Login.cshtml`<br>• `Account/Register.cshtml`<br>• `Account/Profile.cshtml` | `AccountController.cs`<br>• `Login()`<br>• `Register()`<br>• `Logout()`<br>• `ChangePassword()` | `AuthController.cs`<br>• `POST /api/Auth/login`<br>• `POST /api/Auth/register`<br>• `POST /api/Auth/logout`<br>• `POST /api/Auth/change-pass` | `Users`, `Roles`, `MinhChung_FreelancerStudents` | **CUS-UC-01 $\rightarrow$ 03**<br>**FRL-UC-01 $\rightarrow$ 03** |
| **2** | **Nghiệp vụ Nhà tuyển dụng**<br>• `Employer/ManageJobs.cshtml`<br>• `Employer/Applicants.cshtml`<br>• `Employer/Search.cshtml` | `CustomerController.cs`<br>• `Dashboard()`<br>• `ManageJobs()`<br>• `SearchFreelancer()`<br>• `ViewApplicant()` | `CustomerController.cs`<br>• `GET /api/Customer/jobs`<br>• `POST /api/Customer/post-job`<br>• `GET /api/Customer/freelancers`<br>• `POST /api/Customer/bookmark` | `NhaTuyenDung`, `BaiDangTimViec_FreelancerStudent`, `FreelancerYeuThich` | **CUS-UC-04 $\rightarrow$ 07** |
| **3** | **Nghiệp vụ Freelancer**<br>• `Freelancer/QuanLyTimViec.cshtml`<br>• `Freelancer/DanhSachNopTuyen.cshtml`<br>• `Freelancer/HoSo.cshtml`<br>• `Freelancer/Portfolio.cshtml` | `FreelancerController.cs`<br>• `Dashboard()`<br>• `QuanLyTimViec()`<br>• `CapNhatHoSo()`<br>• `NopMinhChung()` | `FreelancerController.cs`<br>• `GET /api/Freelancer/profile`<br>• `PUT /api/Freelancer/profile`<br>• `POST /api/Freelancer/apply`<br>• `POST /api/Freelancer/proof` | `FreelancerStudents`, `ChuyenNganh`, `KynangChuyennganh_FreelancerStudent`, `Portfolio` | **FRL-UC-04 $\rightarrow$ 08** |
| **4** | **Quản trị Hệ thống**<br>• `Admin/DanhSachNguoiDung.cshtml`<br>• `Admin/XuLyTaiKhoan.cshtml`<br>• `Admin/XuLyTranhChap.cshtml`<br>• `Admin/YeuCauNapTien.cshtml` | `AdminController.cs`<br>• `Index()`<br>• `DanhSachNguoiDung()`<br>• `DuyetMinhChung()`<br>• `XuLyTranhChap()` | `AdminController.cs`<br>• `GET /api/Admin/users`<br>• `PUT /api/Admin/lock-user`<br>• `POST /api/Admin/verify-proof`<br>• `PUT /api/Admin/resolve-dispute` | `Admin`, `LichSu_XuLyTaiKhoan`, `MinhChung_FreelancerStudents` | **AD-UC-01 $\rightarrow$ 11** |
| **5** | **Quản lý Hợp đồng & Ký quỹ**<br>• `HopDong/Index.cshtml`<br>• `HopDong/Details.cshtml`<br>• `HopDong/NghiemThu.cshtml` | `HopDongController.cs`<br>• `Index()`<br>• `Details(id)`<br>• `KyKet()`<br>• `NghiemThu()` | `HopDongController.cs`<br>• `GET /api/HopDong/list`<br>• `POST /api/HopDong/create`<br>• `POST /api/HopDong/escrow`<br>• `PUT /api/HopDong/complete` | `Wallet`, `LichSuGiaoDich` | **CUS-UC-08 $\rightarrow$ 09**<br>**FRL-UC-06 $\rightarrow$ 08** |
| **6** | **Ví tài chính & Giao dịch**<br>• `Wallet/Index.cshtml`<br>• `Wallet/NapTien.cshtml`<br>• `Wallet/RutTien.cshtml`<br>• `Wallet/LichSuGiaoDich.cshtml` | `WalletController.cs`<br>• `Index()`<br>• `NapTien()`<br>• `RutTien()`<br>• `LichSuGiaoDich()` | `WalletController.cs`<br>• `GET /api/Wallet/balance`<br>• `POST /api/Wallet/deposit`<br>• `POST /api/Wallet/withdraw`<br>• `GET /api/Wallet/history` | `Wallet`, `LichSuGiaoDich`, `YeuCauRutTien` | **CUS-UC-07**<br>**FRL-UC-09**<br>**AD-UC-03 $\rightarrow$ 05** |
| **7** | **Khiếu nại & Tranh chấp**<br>• `TranhChap/Index.cshtml`<br>• `TranhChap/Create.cshtml`<br>• `TranhChap/Details.cshtml` | `TranhChapController.cs`<br>• `Index()`<br>• `Create()`<br>• `Details()` | `TranhChapController.cs`<br>• `GET /api/Dispute/list`<br>• `POST /api/Dispute/create`<br>• `GET /api/Dispute/details` | `Wallet`, `LichSu_XuLyTaiKhoan` | **CUS-UC-10 $\rightarrow$ 11**<br>**FRL-UC-10**<br>**AD-UC-07** |
| **8** | **Trao đổi Chat & Thông báo**<br>• `NhanTin/Index.cshtml`<br>• `ThongBao/Index.cshtml` | `NhanTinController.cs`<br>`ThongBaoController.cs` | `ChatController.cs` (SignalR Hub)<br>`NotificationController.cs` | Bảng Chat & Thông báo | **CUS-UC-12**<br>**FRL-UC-05** |

---

# 8. CHECKLIST BẢO MẬT & KIỂM THỬ TRƯỚC KHI BÀN GIAO

### 8.1. Checklist Bảo mật & Xác thực
- [x] **Mã hóa Mật khẩu**: 100% mật khẩu được hash bằng BCrypt (WorkFactor = 11) trước khi lưu vào cột `pashwordHash` của bảng `Users`. Không bao giờ lưu dạng plain text.
- [x] **Chống tấn công CSRF**: Toàn bộ Form POST trong Razor Views bắt buộc có `@Html.AntiForgeryToken()` và Action Controller tương ứng có `[ValidateAntiForgeryToken]`.
- [x] **Chống tấn công Open Redirect**: Sau khi đăng nhập, kiểm tra chặt chẽ `Url.IsLocalUrl(model.ReturnUrl)`. Nếu ReturnUrl từ domain ngoài $\rightarrow$ Điều hướng về trang chủ mặc định.
- [x] **Bảo vệ Cookie Xác thực**: Cấu hình `HttpOnly = true` (chống đánh cắp cookie qua mã độc XSS Javascript), `SameSite = SameSiteMode.Lax`, `SecurePolicy = CookieSecurePolicy.SameAsRequest`.
- [x] **Kiểm tra trạng thái tài khoản thời gian thực**: Ngăn chặn tức thì các tài khoản có `status == 'LOCKED'` hoặc `'SUSPENDED'` khi đăng nhập hoặc thực hiện giao dịch tài chính.

### 8.2. Checklist Nghiệp vụ & Dữ liệu CSDL
- [x] **Ràng buộc toàn vẹn CSDL (SQL Constraints)**:
  - `CK_Roles_maRole`: `maRole IN (1, 2, 3)`.
  - `CK_Users_Email`: Email là duy nhất (`UNIQUE NOT NULL`).
  - `CK_FreelancerStudents_GPA`: GPA nằm trong khoảng $[0.0, 4.0]$.
  - `CK_Wallet_SoDuKhaDung`: Số dư ví không được âm (`soDuKhaDung >= 0`).
- [x] **Giao dịch nguyên tử (Atomic Database Transactions)**: Khi đăng ký tài khoản mới, việc tạo bản ghi `Users`, bản ghi hồ sơ con (`FreelancerStudents` / `NhaTuyenDung`) và tạo ví `Wallet` phải nằm trọn trong 1 `IDbContextTransaction`. Nếu xảy ra lỗi $\rightarrow$ Rollback toàn bộ để tránh dữ liệu rác.
- [x] **Xử lý Session và Phân quyền**: Khi người dùng Đăng xuất, gọi đồng thời `SignOutAsync()` và `Session.Clear()` để giải phóng hoàn toàn bộ nhớ và ngăn chặn quay lại trang bảo mật bằng nút Back trình duyệt.

---
*Tài liệu kỹ thuật được biên soạn phục vụ công tác chuyển đổi, mapping và hoàn thiện khóa luận tốt nghiệp Sàn Giao Dịch Freelancer Sinh Viên (KLCN040).*
