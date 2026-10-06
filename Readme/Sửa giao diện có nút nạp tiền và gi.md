CHỨC NĂNG NẠP TIỀN VÀO VÍ
DỰ ÁN FREELANCER STUDENT

==================================================
A. FRONTEND - HIỂN THỊ VÍ
==================================================

1. TẠO FILE:
FreelancerStudent.Web/ViewModels/WalletViewModel.cs

- Tạo ViewModel chứa thông tin ví.
- Các thông tin:
  + maWallet
  + tenUsers
  + soDuKhaDung
  + soDuDongBang


2. TẠO FILE:
FreelancerStudent.Web/Services/Interfaces/IWalletWebService.cs

- Tạo Interface xử lý Wallet phía Web.


3. TẠO FILE:
FreelancerStudent.Web/Services/WalletWebService.cs

- Gọi API Backend để lấy thông tin Wallet.


4. SỬA FILE:
FreelancerStudent.Web/Program.cs

- Đăng ký Dependency Injection:

builder.Services.AddScoped<
    IWalletWebService,
    WalletWebService>();


5. SỬA FILE:
FreelancerStudent.Web/Views/Shared/_Layout.cshtml

- Thêm nút "Nạp tiền".
- Hiển thị số dư khả dụng.
- Nút "Nạp tiền" chuyển đến /Wallet.


==================================================
B. DATABASE - GIAO DỊCH NẠP TIỀN
==================================================

6. TẠO BẢNG:
GiaoDichNapTien

Các cột:

- maNapTien
- maWallet
- maGiaoDich
- soTien
- noiDungChuyenKhoan
- trangThai
- ngayTao
- ngayHetHan
- ngayHoanThanh
- maGiaoDichNganHang
- maAdminXuLy


Trạng thái sử dụng:

- DangXuLy
- HoanThanh
- HetHan


==================================================
C. BACKEND - MODEL
==================================================

7. TẠO FILE:
FreelancerStudent.API/Models/GiaoDichNapTien.cs

- Tạo Model tương ứng bảng GiaoDichNapTien.


8. SỬA FILE:
FreelancerStudent.API/Data/ApplicationDBContext.cs

- Thêm:

DbSet<GiaoDichNapTien> GiaoDichNapTiens

- Thêm cấu hình Entity.

- Map đúng tên bảng:

entity.ToTable("GiaoDichNapTien");


==================================================
D. BACKEND - DTO
==================================================

9. TẠO FILE:
FreelancerStudent.API/DTOs/Request/
TaoGiaoDichNapTien_RequestDTO.cs

- Nhận số tiền người dùng muốn nạp.


10. TẠO FILE:
FreelancerStudent.API/DTOs/Reponse/
GiaoDichNapTien_ReponseDTO.cs

- Trả thông tin giao dịch:

  + maNapTien
  + maGiaoDich
  + soTien
  + noiDungChuyenKhoan
  + trangThai
  + ngayTao
  + ngayHetHan


==================================================
E. BACKEND - REPOSITORY GIAO DỊCH
==================================================

11. TẠO FILE:
FreelancerStudent.API/Repositories/Interfaces/
IGiaoDichNapTienRepository.cs

- Thêm các hàm:

themGiaoDichNapTienAsync()

layGiaoDichTheoMaAsync()

capNhatGiaoDichAsync()

xuLyNapTienSePayAsync()


12. TẠO FILE:
FreelancerStudent.API/Repositories/
GiaoDichNapTienRepository.cs

- Code xử lý:

  + Thêm giao dịch.
  + Tìm giao dịch theo maGiaoDich.
  + Cập nhật giao dịch.


13. SỬA FILE:
FreelancerStudent.API/Repositories/
GiaoDichNapTienRepository.cs

- Thêm hàm:

xuLyNapTienSePayAsync()

- Hàm này xử lý:

  + Tìm GiaoDichNapTien.
  + Kiểm tra trạng thái.
  + Kiểm tra thời gian hết hạn.
  + Kiểm tra đúng số tiền.
  + Kiểm tra mã giao dịch ngân hàng.
  + Kiểm tra giao dịch đã xử lý chưa.
  + Tìm Wallet.
  + Cộng tiền vào Wallet.
  + Chuyển trạng thái thành HoanThanh.
  + Gán ngayHoanThanh.
  + Lưu maGiaoDichNganHang.


14. SỬA FILE:
FreelancerStudent.API/Repositories/
GiaoDichNapTienRepository.cs

- Thêm Database Transaction:

BeginTransactionAsync(
    IsolationLevel.Serializable
)

- Nếu xử lý thành công:
  CommitAsync()

- Nếu xảy ra lỗi:
  RollbackAsync()

- Mục đích:
  Không để xảy ra trường hợp đã cộng Wallet
  nhưng chưa cập nhật GiaoDichNapTien hoặc ngược lại.


==================================================
F. BACKEND - WALLET REPOSITORY
==================================================

15. SỬA FILE:
FreelancerStudent.API/Repositories/Interfaces/
IWalletRepository.cs

- Thêm:

layViTheoMaWallet()

capNhatViAsync()


16. SỬA FILE:
FreelancerStudent.API/Repositories/
WalletRepository.cs

- Thêm xử lý:

  + Tìm Wallet theo maWallet.
  + Cập nhật số dư Wallet.


==================================================
G. BACKEND - SERVICE
==================================================

17. TẠO FILE:
FreelancerStudent.API/Services/Interfaces/
IGiaoDichNapTienService.cs

- Thêm:

taoGiaoDichNapTienAsync()

hoanThanhGiaoDichNapTienAsync()

layTrangThaiGiaoDichAsync()


18. TẠO FILE:
FreelancerStudent.API/Services/
GiaoDichNapTienService.cs

- Code tạo giao dịch nạp tiền.

- Kiểm tra:

  + Số tiền >= 100.000đ.
  + Số tiền là bội số 50.000đ.

- Tạo mã:

NAP_xxx

- Trạng thái ban đầu:

DangXuLy

- Thời gian hết hạn:

30 phút


19. SỬA FILE:
FreelancerStudent.API/Services/
GiaoDichNapTienService.cs

- Thêm xử lý hoàn thành giao dịch.

- Thêm xử lý kiểm tra trạng thái.

- Nếu quá thời gian:
  DangXuLy -> HetHan.


20. SỬA FILE:
FreelancerStudent.API/Services/Interfaces/
IGiaoDichNapTienService.cs

- Thêm hàm:

xuLyWebhookSePayAsync(
    string maGiaoDich,
    decimal soTien,
    string maGiaoDichNganHang
)


21. SỬA FILE:
FreelancerStudent.API/Services/
GiaoDichNapTienService.cs

- Thêm hàm:

xuLyWebhookSePayAsync()

- Gọi:

_giaoDichNapTienRepository
    .xuLyNapTienSePayAsync(...)


==================================================
H. BACKEND - CONTROLLER
==================================================

22. TẠO FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Tạo API:

POST
/api/GiaoDichNapTien/TaoGiaoDich/{maUser}

- Tạo API:

GET
/api/GiaoDichNapTien/TrangThai/{maGiaoDich}


23. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Tạo API test:

POST
/api/GiaoDichNapTien/HoanThanhGiaoDich/{maGiaoDich}

- API này ban đầu dùng để test cộng tiền
  mà chưa cần ngân hàng.


24. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- COMMENT API:

HoanThanhGiaoDich/{maGiaoDich}

- Không cho phép dùng API test này
  khi đã tích hợp thanh toán thật.


==================================================
I. BACKEND - DEPENDENCY INJECTION
==================================================

25. SỬA FILE:
FreelancerStudent.API/Program.cs

- Đăng ký:

builder.Services.AddScoped<
    IGiaoDichNapTienRepository,
    GiaoDichNapTienRepository>();

builder.Services.AddScoped<
    IGiaoDichNapTienService,
    GiaoDichNapTienService>();


==================================================
J. FRONTEND - GIAO DỊCH NẠP TIỀN
==================================================

26. TẠO FILE:
FreelancerStudent.Web/ViewModels/
GiaoDichNapTienViewModel.cs

- Chứa thông tin giao dịch nạp tiền.


27. TẠO FILE:
FreelancerStudent.Web/ViewModels/
TaoGiaoDichNapTienRequest.cs

- Chứa số tiền gửi từ giao diện.


28. TẠO FILE:
FreelancerStudent.Web/Services/Interfaces/
IGiaoDichNapTienWebService.cs

- Thêm:

taoGiaoDichNapTien()

layTrangThaiGiaoDich()


29. TẠO FILE:
FreelancerStudent.Web/Services/
GiaoDichNapTienWebService.cs

- Gọi Backend:

POST
api/GiaoDichNapTien/TaoGiaoDich/{maUser}

- Gọi Backend:

GET
api/GiaoDichNapTien/TrangThai/{maGiaoDich}


30. SỬA FILE:
FreelancerStudent.Web/Program.cs

- Đăng ký:

builder.Services.AddScoped<
    IGiaoDichNapTienWebService,
    GiaoDichNapTienWebService>();


==================================================
K. FRONTEND - WALLET CONTROLLER
==================================================

31. TẠO/SỬA FILE:
FreelancerStudent.Web/Controllers/
WalletController.cs

- Thêm Index().

- Thêm TaoGiaoDich().

- Thêm TrangThai().

- Lấy maUser từ Session.

- Gọi GiaoDichNapTienWebService.


==================================================
L. FRONTEND - GIAO DIỆN NẠP TIỀN
==================================================

32. TẠO/SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Tạo giao diện nạp tiền.

- Thêm mức tiền:

  + 100.000đ
  + 200.000đ
  + 500.000đ
  + 1.000.000đ

- Thêm nhập số tiền khác.


33. SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Thêm validate:

  + Tối thiểu 100.000đ.
  + Phải là bội số 50.000đ.

- Thêm thông báo lỗi:

"Số tiền nạp phải từ 100.000đ trở lên
và phải là bội số của 50.000đ."


34. SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Thêm JavaScript gọi:

POST /Wallet/TaoGiaoDich

- Sau khi tạo thành công hiển thị:

  + Mã giao dịch.
  + Số tiền.
  + Nội dung chuyển khoản.
  + Trạng thái.


==================================================
M. FRONTEND - VIETQR
==================================================

35. SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Thêm QR VietQR.


36. SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Ban đầu sử dụng Vietcombank.

- Sau đó đổi sang:

  Ngân hàng: MB Bank
  STK: 0378896290
  Chủ tài khoản: DANG TUAN ANH

- QR truyền:

  + Ngân hàng.
  + Số tài khoản.
  + Số tiền.
  + Nội dung NAP_xxx.


==================================================
N. FRONTEND - POLLING TRẠNG THÁI
==================================================

37. SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Thêm JavaScript kiểm tra trạng thái:

GET
/Wallet/TrangThai?maGiaoDich=NAP_xxx

- Kiểm tra khoảng 5 giây/lần.


38. SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Nếu DangXuLy:
  tiếp tục kiểm tra.

- Nếu HoanThanh:
  dừng kiểm tra và báo thành công.

- Nếu HetHan:
  dừng kiểm tra và báo hết hạn.


39. SỬA FILE:
FreelancerStudent.Web/Views/Wallet/Index.cshtml

- Thêm countdown 30 phút.

- Khi HoanThanh:
  dừng countdown.

- Khi HetHan:
  dừng countdown.


==================================================
O. TÍCH HỢP SEPAY
==================================================

40. CẤU HÌNH SEPAY:

- Kết nối tài khoản MB Bank với SePay.

- Tài khoản:

0378896290

- Đã xác nhận SePay nhận được
  biến động giao dịch MB thật.


41. CẤU HÌNH NGROK:

- Public Backend API:

localhost:5152

ra Internet để SePay có thể gọi Webhook.


42. CẤU HÌNH WEBHOOK TRÊN SEPAY:

- Tạo Webhook:

POST
/api/GiaoDichNapTien/Webhook

- Chọn:

  + Tiền vào.
  + JSON.
  + MB Bank.
  + HMAC-SHA256.


==================================================
P. BACKEND - DTO SEPAY
==================================================

43. TẠO FILE:
FreelancerStudent.API/DTOs/Request/
SePayWebhook_RequestDTO.cs

- Thêm:

  + id
  + gateway
  + transactionDate
  + accountNumber
  + subAccount
  + code
  + content
  + transferType
  + transferAmount
  + accumulated
  + description
  + referenceCode


==================================================
Q. BACKEND - WEBHOOK SEPAY
==================================================

44. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Inject thêm:

IConfiguration

- Dùng để đọc:

SePay:WebhookSecret


45. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Thêm using:

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


46. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Thêm API:

[HttpPost("Webhook")]

- Đọc raw body của request.


47. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Thêm xác thực:

X-SePay-Signature

X-SePay-Timestamp


48. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Kiểm tra Timestamp.

- Chỉ chấp nhận request trong khoảng:

300 giây.


49. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Tạo lại chữ ký:

timestamp + "." + rawBody

- Tạo HMACSHA256.

- So sánh chữ ký bằng:

CryptographicOperations.FixedTimeEquals()


50. CẤU HÌNH USER SECRETS:

- Khởi tạo:

dotnet user-secrets init

- Lưu:

SePay:WebhookSecret

- Không lưu Secret Key trực tiếp
  trong source code.


==================================================
R. BACKEND - XỬ LÝ DỮ LIỆU SEPAY
==================================================

51. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Deserialize raw JSON thành:

SePayWebhook_RequestDTO


52. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Kiểm tra:

body.transferType == "in"

- Chỉ xử lý giao dịch tiền vào.


53. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Tìm mã NAP trong:

body.content


54. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Ban đầu Regex:

@"NAP_[A-Fa-f0-9]{10}"


==================================================
S. TEST WEBHOOK THẬT
==================================================

55. TEST:

- Chuyển khoản thật qua MB.

- SePay gọi:

POST /api/GiaoDichNapTien/Webhook

- Kết quả:

200 OK


56. KIỂM TRA CMD:

- Xác thực thành công:

HMAC-SHA256: HỢP LỆ

- Nhận được:

  + Ngân hàng MBBank.
  + Tài khoản MB.
  + Số tiền.
  + Nội dung giao dịch.
  + referenceCode.


57. TEST GIAO DỊCH:

maGiaoDich:
NAP_E3FDC9FECD

soTien:
100.000đ


58. KIỂM TRA DATABASE:

- GiaoDichNapTien vẫn:

trangThai = DangXuLy

- Wallet chưa được cộng tiền.


==================================================
T. SỬA LỖI MÃ NAP TỪ MB
==================================================

59. PHÁT HIỆN:

- Hệ thống tạo:

NAP_E3FDC9FECD

- Nhưng nội dung MB/SePay nhận được:

NAPE3FDC9FECD

- MB đã loại bỏ dấu "_".


60. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Đổi Regex cũ:

@"NAP_[A-Fa-f0-9]{10}"

- Thành:

@"NAP_?[A-Fa-f0-9]{10}"

- Cho phép mã NAP có hoặc không có "_".


61. SỬA FILE:
FreelancerStudent.API/Controllers/
GiaoDichNapTienController.cs

- Thêm chuẩn hóa mã:

string maGiaoDich =
    match.Value.ToUpperInvariant();

if (!maGiaoDich.StartsWith("NAP_"))
{
    maGiaoDich =
        "NAP_" + maGiaoDich.Substring(3);
}

- Kết quả:

NAPE3FDC9FECD

được chuyển lại thành:

NAP_E3FDC9FECD


==================================================
U. DATABASE KIỂM TRA
==================================================

62. KIỂM TRA DATABASE:

API đang sử dụng:

KETNOI_FREELANCERSV


63. KIỂM TRA TABLE:

- GiaoDichNapTien
- Wallet

Lưu ý:

Tên DbSet EF:

Wallets

nhưng tên bảng SQL thật:

Wallet


==================================================
V. TRẠNG THÁI HIỆN TẠI
==================================================

64. ĐÃ TẠO:

- WalletViewModel.cs
- IWalletWebService.cs
- WalletWebService.cs
- GiaoDichNapTien.cs
- TaoGiaoDichNapTien_RequestDTO.cs
- GiaoDichNapTien_ReponseDTO.cs
- IGiaoDichNapTienRepository.cs
- GiaoDichNapTienRepository.cs
- IGiaoDichNapTienService.cs
- GiaoDichNapTienService.cs
- GiaoDichNapTienViewModel.cs
- TaoGiaoDichNapTienRequest.cs
- IGiaoDichNapTienWebService.cs
- GiaoDichNapTienWebService.cs
- SePayWebhook_RequestDTO.cs
- GiaoDichNapTienController.cs


65. ĐÃ SỬA:

- ApplicationDBContext.cs
- IWalletRepository.cs
- WalletRepository.cs
- FreelancerStudent.API/Program.cs
- FreelancerStudent.Web/Program.cs
- WalletController.cs
- Views/Wallet/Index.cshtml
- Views/Shared/_Layout.cshtml
- IGiaoDichNapTienRepository.cs
- GiaoDichNapTienRepository.cs
- IGiaoDichNapTienService.cs
- GiaoDichNapTienService.cs
- GiaoDichNapTienController.cs


66. ĐÃ HOÀN THÀNH:

- Tạo giao dịch nạp tiền.
- Validate số tiền.
- Tạo QR.
- MB Bank.
- Countdown.
- Polling trạng thái.
- Kết nối SePay.
- Webhook.
- ngrok.
- HMAC-SHA256.
- User Secrets.
- Nhận giao dịch ngân hàng thật.
- Nhận Webhook thật.
- Xác định lỗi dấu "_" của MB.
- Sửa Regex.
- Chuẩn hóa lại mã NAP.


==================================================
W. BƯỚC ĐANG LÀM
==================================================

67. Build và chạy lại:

FreelancerStudent.API

sau khi sửa Regex.


68. Kiểm tra lại giao dịch:

NAP_E3FDC9FECD


69. Kết quả cần đạt trong GiaoDichNapTien:

trangThai = HoanThanh

ngayHoanThanh != NULL

maGiaoDichNganHang != NULL


70. Kết quả cần đạt trong Wallet:

Wallet tương ứng được cộng:

+100.000đ


71. Sau khi xác nhận bước trên thành công:

- Kiểm tra Frontend tự nhận HoanThanh.
- Kiểm tra số dư mới.
- Sau đó tiếp tục làm lịch sử giao dịch.