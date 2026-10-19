# HƯỚNG DẪN CHI TIẾT: LUỒNG HOẠT ĐỘNG TỪ GIAO DIỆN MOBILE (FLUTTER) ĐẾN BACKEND API (.NET)

> **Dành cho:** Lập trình viên đã quen thuộc với mô hình **MVC (Model - View - Controller)** trong ASP.NET Core và muốn hiểu cặn kẽ cách ứng dụng **Flutter Mobile** hoạt động từ lúc người dùng thao tác đến khi gọi API và xử lý dữ liệu.

---

## 📑 MỤC LỤC
1. [Bản chất khác biệt: Web MVC vs Mobile Flutter](#1-bản-chất-khác-biệt-web-mvc-vs-mobile-flutter)
2. [Bảng đối chiếu 1-1: MVC (.NET Web) sang MVVM (Flutter Mobile)](#2-bảng-đối-chiếu-1-1-mvc-net-web-sang-mvvm-flutter-mobile)
3. [Sơ đồ luồng tổng quát (End-to-End Architecture)](#3-sơ-đồ-luồng-tổng-quát-end-to-end-architecture)
4. [Mổ xẻ 6 tầng kiến trúc trong mã nguồn Mobile](#4-mổ-xẻ-6-tầng-kiến-trúc-trong-mã-nguồn-mobile)
5. [Trình tự từng bước qua Ví dụ thực tế 1: Luồng Đăng nhập (Auth)](#5-trình-tự-từng-bước-qua-ví-dụ-thực-tế-1-luồng-đăng-nhập-auth)
6. [Trình tự từng bước qua Ví dụ thực tế 2: Luồng Tải danh sách việc làm (Job Feed)](#6-trình-tự-từng-bước-qua-ví-dụ-thực-tế-2-luồng-tải-danh-sách-việc-làm-job-feed)
7. [Luồng thời gian thực: Chat SignalR Hub trên Mobile](#7-luồng-thời-gian-thực-chat-signalr-hub-trên-mobile)
8. [Cẩm nang chuyển giao tư duy: Những lưu ý sống còn cho dân Backend](#8-cẩm-nang-chuyển-giao-tư-duy-những-lưu-ý-sống-còn-cho-dân-backend)

---

## 1. Bản chất khác biệt: Web MVC vs Mobile Flutter

Nếu bạn đã làm việc với **ASP.NET Core MVC** (`FreelancerStudent.Web`), bạn đã quen với luồng **Server-Side Rendering (SSR)**:

```text
[Trình duyệt Web] -----(HTTP GET /JobPost)-----> [.NET MVC Controller]
                                                         │
                                                  (Query SQL)
                                                         │
[Trình duyệt Web] <====(Trả về file HTML đã vẽ)== [View .cshtml]
```
👉 *Đặc điểm Web MVC:* Mỗi lần chuyển trang hoặc bấm nút, trình duyệt gửi request lên Server, Server render ra một trang HTML hoàn chỉnh rồi gửi về cho trình duyệt hiển thị lại toàn bộ trang.

---

Trong khi đó, **Flutter Mobile** hoạt động theo mô hình **Client-Side Reactive (CSR)**:

```text
[Ứng dụng Flutter trên điện thoại]              [Backend .NET Web API]
 (Giao diện là code Dart vẽ sẵn)                 (Chỉ làm nhiệm vụ Data)
        │                                                  │
   [Người dùng bấm nút]                                    │
        │                                                  │
  Gọi hàm ViewModel                                        │
        │                                                  │
  Gửi HTTP Request ────────(Chỉ gửi nhận JSON thô)────────>│ [API Controller]
        │                                                  │       │
  Nhận lại JSON                                            │   Query DB
  Parse thành Model DTO <──(Chuỗi JSON: {"id":1, ...})─────┘       │
        │                                                  │
  Cập nhật State (notify)                                  │
        │                                                  │
  Flutter tự VẼ LẠI đúng cái ô đó trên màn hình (Rebuild Widget)
```
👉 *Đặc điểm Flutter Mobile:*
1. **Giao diện không phụ thuộc HTML**: Màn hình điện thoại được vẽ bởi engine đồ họa (Skia/Impeller) của Flutter.
2. **Backend chỉ trả về JSON**: Backend không trả về HTML nữa, mà trả về chuỗi JSON thô (`{ "isSuccess": true, "data": [...] }`).
3. **Màn hình tự cập nhật cục bộ**: Khi có dữ liệu mới, Flutter chỉ vẽ lại (rebuild) đúng thành phần cần hiển thị (ví dụ danh sách việc làm hoặc vòng xoay loading), người dùng không hề bị chớp màn hình hay tải lại cả trang.

---

## 2. Bảng đối chiếu 1-1: MVC (.NET Web) sang MVVM (Flutter Mobile)

Dự án Mobile này áp dụng mô hình **MVVM (Model - View - ViewModel)** kết hợp với **Provider** để quản lý trạng thái. Dưới đây là bảng so sánh giúp bạn ánh xạ ngay lập tức sang những gì bạn đã biết:

| Khái niệm trong ASP.NET Core MVC | Tương đương trong Flutter Mobile | Vai trò & Trách nhiệm | File thực tế trong dự án |
|---|---|---|---|
| **View (`.cshtml`)** | **View / Screen (`Widget`)** | Vẽ giao diện, hiển thị nút bấm, form nhập liệu, danh sách và lắng nghe cử chỉ chạm của người dùng. | `views/auth/login_screen.dart`<br>`views/home/home_screen.dart` |
| **Controller (`*Controller.cs`)** | **ViewModel (`*ViewModel.dart`)** | Tiếp nhận lệnh từ giao diện, quản lý trạng thái (`isLoading`, `errorMessage`, `data`), điều phối gọi Service và thông báo cho giao diện cập nhật. | `viewmodels/auth_viewmodel.dart`<br>`viewmodels/job_viewmodel.dart` |
| **Service Web (`*WebService.cs`)** | **Service Mobile (`*Service.dart`)** | Chứa logic nghiệp vụ kết nối mạng: gọi `ApiClient`, xử lý response, lưu trữ token hoặc session. | `services/auth_service.dart`<br>`services/job_service.dart` |
| **`HttpClient` / `IHttpClientFactory`** | **`ApiClient` (dùng thư viện `Dio`)** | Đối tượng gửi request HTTP (`GET`, `POST`, `PUT`, `DELETE`), cấu hình timeout, base URL. | `core/network/api_client.dart` |
| **`DelegatingHandler` (HTTP Message Handler)** | **`DioInterceptor`** | Can thiệp trước khi gửi request để tự động móc Token `Bearer {JWT}` gắn vào Header. | `core/network/dio_interceptor.dart` |
| **ViewModel / DTO (`*ViewModel.cs`, `*DTO.cs`)** | **Model (`*Model.dart`)** | Định nghĩa cấu trúc dữ liệu, có 2 hàm: `fromJson()` (biến JSON thành Object) và `toJson()` (biến Object thành JSON). | `models/auth_response_model.dart`<br>`models/job_post_model.dart` |
| **Session / Cookies** | **`SharedPreferences` & `FlutterSecureStorage`** | Lưu trữ mã phiên đăng nhập, JWT Token, ID người dùng an toàn vào bộ nhớ điện thoại. | `core/storage/pref_keys.dart` |
| **`appsettings.json`** | **`ApiEndpoints`** | Khai báo tập trung các đường link URL kết nối tới Backend API. | `core/constants/api_endpoints.dart` |
| **`RedirectToAction`** | **`Navigator.push` / `pushReplacement`** | Chuyển từ màn hình này sang màn hình khác trên điện thoại. | `Navigator.of(context).push(...)` |

---

## 3. Sơ đồ luồng tổng quát (End-to-End Architecture)

Dưới đây là hành trình 2 chiều từ lúc người dùng nhấn nút trên màn hình điện thoại cho đến cơ sở dữ liệu SQL Server:

```mermaid
sequenceDiagram
    autonumber
    actor User as Người dùng Mobile
    participant View as View (Widget Screen)
    participant VM as ViewModel (ChangeNotifier)
    participant Svc as Service Layer
    participant Net as ApiClient + DioInterceptor
    participant API as Backend ASP.NET Core API
    participant DB as SQL Server Database

    User->>View: 1. Chạm nút (Ví dụ: "Đăng nhập")
    View->>VM: 2. Gọi hàm authViewModel.login(email, pass)
    VM->>VM: 3. Set _isLoading = true, notifyListeners()
    Note over View: Giao diện tự động hiện vòng xoay xoay (Loading)
    VM->>Svc: 4. Gọi authService.login(email, pass)
    Svc->>Net: 5. Gửi request POST qua ApiClient
    Note over Net: Interceptor tự động gắn Bearer Token nếu có
    Net->>API: 6. Gửi gói tin HTTP qua mạng (http://10.0.2.2:5152/api/...)
    API->>DB: 7. Truy vấn SQL Server / Xác thực / Tính toán
    DB-->>API: 8. Trả kết quả bản ghi dữ liệu
    API-->>Net: 9. Phản hồi HTTP 200 OK kèm JSON Body
    Net-->>Svc: 10. Trả về Dio Response
    Svc->>Svc: 11. Parse JSON sang Model (Model.fromJson) & Lưu Token vào Bộ nhớ
    Svc-->>VM: 12. Trả ApiResponse(data: Model, isSuccess: true)
    VM->>VM: 13. Cập nhật _currentUser, tắt _isLoading = false, gọi notifyListeners()
    VM-->>View: 14. Thông báo State đã thay đổi!
    Note over View: View tự động Rebuild: Ẩn loading, điều hướng sang HomeScreen
    View-->>User: 15. Hiển thị màn hình chính cho người dùng
```

---

## 4. Mổ xẻ 6 tầng kiến trúc trong mã nguồn Mobile

### Tầng 1: View (Giao diện UI / Widget)
- **Vị trí thư mục:** `lib/views/`
- **Các file tiêu biểu:**
  - [views/auth/login_screen.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/views/auth/login_screen.dart)
  - [views/home/home_screen.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/views/home/home_screen.dart)
- **Nhiệm vụ:**
  - Xây dựng bố cục giao diện bằng các Widget lồng nhau (`Scaffold`, `Column`, `ListView`, `TextField`, `ElevatedButton`).
  - Tuyệt đối **KHÔNG** viết logic gọi mạng trực tiếp ở đây.
  - Lắng nghe ViewModel qua 2 phương thức của thư viện Provider:
    - `context.read<MyViewModel>()`: Dùng khi **chỉ muốn gọi hàm** (ví dụ khi bấm nút: `context.read<AuthViewModel>().login(...)`). Nó không đăng ký lắng nghe nên khi ViewModel thay đổi, nút này không bị vẽ lại thừa thãi.
    - `context.watch<MyViewModel>()`: Dùng khi **cần lấy dữ liệu để vẽ** (ví dụ: `final jobs = context.watch<JobViewModel>().jobs;`). Bất cứ khi nào ViewModel có dữ liệu mới, hàm `build()` sẽ tự động chạy lại để hiển thị dữ liệu mới.

---

### Tầng 2: ViewModel (Quản lý trạng thái - State Management)
- **Vị trí thư mục:** `lib/viewmodels/`
- **Các file tiêu biểu:**
  - [viewmodels/auth_viewmodel.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/viewmodels/auth_viewmodel.dart)
  - [viewmodels/job_viewmodel.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/viewmodels/job_viewmodel.dart)
- **Nhiệm vụ:**
  - Kế thừa class `ChangeNotifier` của Flutter.
  - Chứa các biến trạng thái giao diện:
    - `bool _isLoading`: Đang tải dữ liệu hay đã xong.
    - `String? _errorMessage`: Lưu câu thông báo lỗi nếu có.
    - `List<JobPostModel> _jobs`: Danh sách dữ liệu hứng được từ API.
  - **Vũ khí quan trọng nhất:** Lệnh **`notifyListeners()`**.
    - Mỗi khi biến dữ liệu thay đổi (ví dụ: gán danh sách mới, hoặc bật/tắt loading), ViewModel gọi `notifyListeners()`.
    - Lệnh này giống như việc "bấm chuông" thông báo cho tất cả các View đang `watch` biết: *"Dữ liệu đã có thay đổi, hãy vẽ lại màn hình đi!"*.

---

### Tầng 3: Service (Nghiệp vụ ứng dụng)
- **Vị trí thư mục:** `lib/services/`
- **Các file tiêu biểu:**
  - [services/auth_service.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/services/auth_service.dart)
  - [services/job_service.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/services/job_service.dart)
- **Nhiệm vụ:**
  - Tách biệt logic giao tiếp dữ liệu khỏi ViewModel.
  - Chuẩn bị dữ liệu gửi đi (Payload/Parameters).
  - Gọi xuống tầng mạng `ApiClient`.
  - Nhận kết quả phản hồi, bọc trong lớp `ApiResponse<T>` chuẩn hóa (gồm `isSuccess`, `data`, `message`, `statusCode`).
  - Xử lý lưu trữ an toàn: Ghi nhớ Token JWT, vai trò (`Role`), ID người dùng vào bộ nhớ máy điện thoại.

---

### Tầng 4: Core Network & Security (Tầng mạng & Bảo mật)
- **Vị trí thư mục:** `lib/core/network/` và `lib/core/storage/`
- **Các file tiêu biểu:**
  - [core/network/api_client.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/core/network/api_client.dart): Singleton quản lý `Dio` (tương đương `HttpClient` trong C#).
  - [core/network/dio_interceptor.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/core/network/dio_interceptor.dart): Bộ đón chặn gói tin.
  - [core/constants/api_endpoints.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/core/constants/api_endpoints.dart): Cấu hình URL.
- **Nhiệm vụ:**
  - **Tự động gắn Token**: Khi bạn gọi bất kỳ API nào cần đăng nhập, `dio_interceptor.dart` sẽ tự động đọc Token từ bộ nhớ ngầm và đính kèm vào Header HTTP:
    ```dart
    options.headers['Authorization'] = 'Bearer $token';
    ```
    *(Nhờ đó, lập trình viên không bao giờ phải gán thủ công header Token ở từng màn hình!)*
  - **Cấu hình Base URL kết nối**:
    - Khi chạy trên **Android Emulator**, IP máy tính của bạn được máy ảo map thành: `http://10.0.2.2:5152/api`.
    - Khi chạy trên **Chrome / Windows App**, URL là: `http://localhost:5152/api`.

---

### Tầng 5: Model (Ánh xạ DTO / Serialization)
- **Vị trí thư mục:** `lib/models/`
- **Các file tiêu biểu:**
  - [models/auth_response_model.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/models/auth_response_model.dart)
  - [models/job_post_model.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/models/job_post_model.dart)
- **Nhiệm vụ:**
  - Định nghĩa kiểu dữ liệu chặt chẽ (strongly-typed) tương ứng với DTO của Backend.
  - **Hàm `fromJson(Map<String, dynamic> json)`**: Nhận chuỗi từ điển JSON trả về từ API và gán vào các thuộc tính Dart.
  - **Hàm `toJson()`**: Chuyển các thuộc tính Dart thành Map để chuẩn bị serialize thành chuỗi JSON gửi lên API.

---

### Tầng 6: Backend .NET Web API
- **Vị trí thư mục:** `FreelancerStudent.API/`
- **Các file tiêu biểu:**
  - `Controllers/AuthController.cs`
  - `Controllers/JobPostController.cs`
  - `Controllers/HopDongController.cs`
- **Nhiệm vụ:**
  - Nhận HTTP Request từ Mobile gửi tới.
  - Kiểm tra tính hợp lệ qua Model Validation / JWT Token Authentication.
  - Gọi Repository truy vấn dữ liệu SQL Server.
  - Trả về mã phản hồi HTTP (`200 OK`, `400 BadRequest`, `401 Unauthorized`) kèm nội dung JSON.

---

## 5. Trình tự từng bước qua Ví dụ thực tế 1: Luồng Đăng nhập (Auth)

Hãy cùng theo dõi từng miligiây khi người dùng đăng nhập tài khoản trên ứng dụng:

```text
[Màn hình Login] ──> [AuthViewModel] ──> [AuthService] ──> [Dio Client] ──> [.NET Web API]
```

### Bước 1: Người dùng nhấn nút "Đăng nhập"
- Tại [login_screen.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/views/auth/login_screen.dart):
  Người dùng nhập `email` và `password`, sau đó ấn nút **Đăng nhập**. Hàm `_handleLogin()` được kích hoạt:
  ```dart
  final authViewModel = context.read<AuthViewModel>();
  final success = await authViewModel.login(email, password);
  ```

### Bước 2: ViewModel kích hoạt trạng thái "Đang tải"
- Tại [auth_viewmodel.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/viewmodels/auth_viewmodel.dart):
  ```dart
  _isLoading = true;
  _errorMessage = null;
  notifyListeners(); // <-- BÁO HIỆU CHO VIEW VẼ LẠI
  ```
- **Hiện tượng trên màn hình:** View nhận được tín hiệu `_isLoading == true`, lập tức đổi nút bấm thành hình tròn xoay xoay `CircularProgressIndicator` và khóa các ô nhập liệu để người dùng không bấm nhiều lần.

### Bước 3: Chuyển dữ liệu xuống AuthService
- ViewModel gọi:
  ```dart
  final response = await _authService.login(email, password);
  ```

### Bước 4: Đóng gói và gửi qua mạng (Tầng Network)
- Tại [auth_service.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/services/auth_service.dart):
  ```dart
  final response = await _apiClient.post<Map<String, dynamic>>(
    ApiEndpoints.authLogin, // 'http://10.0.2.2:5152/api/Auth/DangNhap'
    {'tentaikhoan_email': email.trim(), 'password': password},
  );
  ```
- `ApiClient` dùng thư viện `Dio` mở kết nối HTTP POST, serialize payload thành chuỗi JSON:
  ```json
  {"tentaikhoan_email": "tuan@gmail.com", "password": "123"}
  ```

### Bước 5: Backend ASP.NET Core API xử lý
- Request bay qua mạng và tới `AuthController.cs` trên máy chủ .NET:
  ```csharp
  [HttpPost("DangNhap")]
  public async Task<IActionResult> DangNhap([FromBody] DangNhap_RequestDTO request)
  ```
- Backend hash mật khẩu, kiểm tra với database SQL Server.
- Nếu hợp lệ, Backend sinh mã JWT Token và trả về HTTP 200 kèm JSON:
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6...",
    "email": "tuan@gmail.com",
    "role": "FreelancerStudent",
    "userId": 15,
    "fullName": "Nguyễn Hoàng Tuấn"
  }
  ```

### Bước 6: Mobile nhận JSON và Parse thành Đối tượng
- `ApiClient` nhận được JSON từ Backend.
- `AuthService` chuyển JSON vào model:
  ```dart
  final user = AuthResponseModel.fromJson(response.data!);
  ```
- `AuthService` lưu phiên đăng nhập vào bộ nhớ điện thoại để lần sau mở app không cần đăng nhập lại:
  ```dart
  await prefs.setString(PrefKeys.accessToken, user.token);
  await prefs.setString(PrefKeys.userRole, user.role);
  ```

### Bước 7: ViewModel tắt loading và thông báo View chuyển trang
- Tại `AuthViewModel`:
  ```dart
  _currentUser = response.data;
  _isLoading = false;
  notifyListeners(); // <-- BÁO HIỆU XONG
  return true;
  ```
- Tại `LoginScreen`:
  Hàm `await authViewModel.login(...)` trả về `true`. Giao diện kiểm tra vai trò:
  - Nếu là `NhaTuyenDung` ➔ Điều hướng tới [EmployerHomeScreen](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/views/employer/employer_home_screen.dart).
  - Nếu là `FreelancerStudent` ➔ Điều hướng tới [HomeScreen](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/views/home/home_screen.dart).

---

## 6. Trình tự từng bước qua Ví dụ thực tế 2: Luồng Tải danh sách việc làm (Job Feed)

Ví dụ này minh họa cách Flutter lấy danh sách dữ liệu từ Backend và hiển thị lên màn hình (tương đương với trang `Index.cshtml` của Web MVC):

### Bước 1: Mở màn hình HomeScreen
- Khi màn hình [home_screen.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/views/home/home_screen.dart) khởi tạo (`initState`), nó gửi yêu cầu nạp dữ liệu:
  ```dart
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<JobViewModel>().fetchJobs();
    });
  }
  ```

### Bước 2: JobViewModel gọi JobService
- Tại [job_viewmodel.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/viewmodels/job_viewmodel.dart):
  ```dart
  Future<void> fetchJobs({int page = 1, String keyword = ''}) async {
    _isLoading = true;
    notifyListeners(); // Báo View hiện thanh cuộn Skeleton / Loading
    
    final response = await _jobService.getJobs(page: page, keyword: keyword);
  ```

### Bước 3: Interceptor gắn Token và gọi HTTP GET
- `JobService` gọi `_apiClient.get(ApiEndpoints.jobs, ...)`.
- Trước khi gói tin bay đi, [ApiDioInterceptor](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/core/network/dio_interceptor.dart) tự động móc Token đã lưu ở Bước đăng nhập gắn vào Header:
  ```text
  GET /api/JobPost?page=1&keyword= HTTP/1.1
  Host: 10.0.2.2:5152
  Authorization: Bearer eyJhbGciOiJIUzI1Ni...
  ```

### Bước 4: Backend .NET truy vấn SQL và trả mảng JSON
- `JobPostController.cs` nhận request, gọi `JobPostRepository.cs` truy vấn bảng `JobPost` kèm bảng `Users` và `KyNang`.
- Trả về danh sách JSON:
  ```json
  [
    {
      "maJob": 101,
      "tieuDe": "Thiết kế giao diện Mobile Flutter",
      "nganSach": 5000000,
      "hanNopHoSo": "2026-11-01T00:00:00",
      "tenNhaTuyenDung": "Công ty Công Nghệ ABC"
    }
  ]
  ```

### Bước 5: Parse mảng JSON thành `List<JobPostModel>`
- Tại [job_service.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/services/job_service.dart):
  ```dart
  final List rawList = response.data;
  final jobs = rawList.map((item) => JobPostModel.fromJson(item)).toList();
  return ApiResponse.success(jobs);
  ```

### Bước 6: ViewModel lưu danh sách và báo cho Widget vẽ lại
- Tại `JobViewModel`:
  ```dart
  _jobs = response.data;
  _isLoading = false;
  notifyListeners(); // <-- KÍCH HOẠT VẼ LẠI MÀN HÌNH
  ```

### Bước 7: Cây Widget tự động render danh sách
- Trong [home_screen.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/views/home/home_screen.dart), đoạn code sau đang quan sát ViewModel:
  ```dart
  final jobViewModel = context.watch<JobViewModel>();
  final jobs = jobViewModel.jobs;
  ```
- Ngay khi `notifyListeners()` được gọi, `build()` chạy lại:
  - Nếu `jobViewModel.isLoading == true` ➔ Hiển thị vòng xoay `CircularProgressIndicator`.
  - Nếu `jobs.isEmpty` ➔ Hiển thị text `"Chưa có công việc nào được đăng"`.
  - Nếu có dữ liệu ➔ Dùng `ListView.builder` hiển thị từng thẻ công việc (Job Card) đẹp mắt, mượt mà.

---

## 7. Luồng thời gian thực: Chat SignalR Hub trên Mobile

Ngoài HTTP thông thường (Request / Response), ứng dụng Mobile này còn có tính năng **Chat trực tiếp thời gian thực** kết nối tới `ChatHub.cs` của Backend .NET qua WebSocket:

- **File quản lý:** [services/chat_signalr_service.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/services/chat_signalr_service.dart)
- **Cơ chế hoạt động:**
  1. **Khởi tạo kết nối vĩnh viễn (Persistent Connection):**
     Mobile tạo một kết nối `HubConnectionBuilder`:
     ```dart
     final hubConnection = HubConnectionBuilder()
         .withUrl('http://10.0.2.2:5152/api/chatHub', 
                  options: HttpConnectionOptions(accessTokenFactory: () async => token))
         .build();
     await hubConnection.start();
     ```
  2. **Lắng nghe tin nhắn mới từ Server (Server Push):**
     Khi có người gửi tin nhắn, Backend .NET bắn sự kiện `ReceiveMessage` xuống:
     ```dart
     hubConnection.on('ReceiveMessage', (arguments) {
       final messageJson = arguments[0];
       // Thêm tin nhắn mới vào danh sách chat và rung chuông thông báo
     });
     ```
  3. **Gửi tin nhắn đi mà không cần gọi API HTTP:**
     ```dart
     await hubConnection.invoke('SendMessage', args: [receiverId, content]);
     ```

---

## 8. Cẩm nang chuyển giao tư duy: Những lưu ý sống còn cho dân Backend

Khi từ Backend/MVC chuyển sang lập trình Flutter, hãy luôn ghi nhớ 5 nguyên tắc sau:

### 1. Phân biệt `10.0.2.2` và `localhost`
- Nếu bạn mở trình duyệt trên máy tính gõ `http://localhost:5152`, nó trỏ tới chính máy tính của bạn.
- Nhưng **máy ảo Android Emulator là một chiếc điện thoại ảo độc lập**. Nếu trong code Flutter bạn để `localhost`, máy ảo sẽ tự trỏ vào chính nó chứ không trỏ ra máy tính chạy backend!
- 👉 **Quy tắc:** Trên Android Emulator, luôn dùng `10.0.2.2` để trỏ về `localhost` của máy tính.

### 2. Không gọi `setState` lung tung ở màn hình lớn
- Trong Web, bạn có thể F5 lại trang. Trong Flutter, gọi `setState()` ở widget cha sẽ ép toàn bộ màn hình phải tính toán và vẽ lại từ đầu, gây lag/giật.
- 👉 **Quy tắc:** Hãy để ViewModel quản lý biến, và chỉ cập nhật thông qua `notifyListeners()` kết hợp với các Widget con thích hợp.

### 3. Nguyên tắc kiểm tra `if (!mounted) return;`
- Khi người dùng bấm nút gọi API, mạng có thể mất 2-3 giây. Trong thời gian chờ đó, người dùng hoàn toàn có thể nhấn nút Back để thoát ra màn hình trước.
- Khi API trả về, nếu bạn lập tức gọi `Navigator.push(...)` hay `ScaffoldMessenger...` mà không kiểm tra xem màn hình đó còn hiển thị trên điện thoại hay không, ứng dụng sẽ bị văng (Crash).
- 👉 **Quy tắc:** Luôn thêm dòng `if (!mounted) return;` ngay sau mọi lệnh `await` trước khi can thiệp vào `context`.

### 4. Đừng bao giờ hardcode chuỗi JSON
- Trong C#, bạn dùng DTO mạnh (`class LoginDTO { public string Email { get; set; } }`).
- Trong Flutter, luôn tạo Model có đầy đủ `fromJson` và `toJson` (như [models/auth_response_model.dart](file:///d:/KLTN/KLTN/Improve/Mobile/freelancerstudent/lib/models/auth_response_model.dart)). Tránh việc truy cập trực tiếp `response.data['user']['name']` vì rất dễ gõ sai chính tả và gây lỗi runtime.

### 5. Luồng dữ liệu một chiều (Unidirectional Data Flow)
- Dữ liệu luôn đi theo một vòng tròn khép kín:
  **`UI Event (Chạm)` ➔ `ViewModel (State)` ➔ `Service (Logic)` ➔ `API (.NET)` ➔ `Model (Data)` ➔ `ViewModel (Notify)` ➔ `UI Render (Vẽ lại)`**.

---
*Tài liệu được biên soạn đồng bộ với kiến trúc thư mục thực tế tại `d:\KLTN\KLTN\Improve\Mobile\freelancerstudent`.*
