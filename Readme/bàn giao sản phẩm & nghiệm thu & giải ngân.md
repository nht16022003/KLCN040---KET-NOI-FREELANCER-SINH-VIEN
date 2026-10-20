# README – TỔNG HỢP FILE THÊM/SỬA VÀ MỤC ĐÍCH THAY ĐỔI

**Dự án:** FreelancerStudent – Nền tảng kết nối Freelancer sinh viên và nhà tuyển dụng  
**Nguồn đối chiếu:** `KLCN040---KET-NOI-FREELANCER-SINH-VIEN-main(20261020-083145).zip` (bản gửi ngày 20/10/2026).  
**Phạm vi:** Luồng ứng tuyển → hợp đồng → ký quỹ → bàn giao → nghiệm thu → giải ngân, cùng các cập nhật giao diện có liên quan.

> **Cách đọc:** Các mục “file tham gia triển khai” được xác nhận có trong source hiện tại; **không đồng nghĩa tất cả đều là file mới tạo**. Không có bản Git diff đầy đủ của từng mốc nên không thể xác định chắc chắn ngày tạo/sửa từng file. Các thay đổi được nhắc trong cuộc trao đổi nhưng khác với ZIP được ghi rõ ở phần **Đối chiếu trạng thái**.

## 1. Tổng quan quy trình nghiệp vụ

1. Nhà tuyển dụng đăng công việc (`JobPost`).
2. Freelancer nộp đơn (`UngTuyen`); nhà tuyển dụng duyệt hoặc từ chối. Người bị từ chối có thể ứng tuyển lại theo logic hiện có.
3. Nhà tuyển dụng lập hợp đồng (`HopDong`) cho Freelancer được chọn.
4. Freelancer xác nhận hợp đồng. Hợp đồng chuyển sang chờ ký quỹ (`ChoKyQuy`).
5. Nhà tuyển dụng ký quỹ: trừ số dư khả dụng và tăng số dư đóng băng trong ví; hợp đồng chuyển `DangThucHien`.
6. Freelancer bàn giao sản phẩm, có thể nộp lại phiên bản sau yêu cầu chỉnh sửa.
7. Nhà tuyển dụng nghiệm thu bản bàn giao hợp lệ; hợp đồng chuyển `DaHoanThanh`.
8. **Trong cùng giao dịch cơ sở dữ liệu**, hệ thống giảm tiền đóng băng của nhà tuyển dụng, cộng tiền khả dụng cho Freelancer và ghi lịch sử giao dịch hai bên.

## 2. Danh mục file theo nhóm thay đổi

Các đường dẫn dưới đây tính từ thư mục gốc dự án trong ZIP.

### 2.1. Quản lý bài đăng và ứng tuyển

| File | Vai trò / mục đích triển khai |
|---|---|
| `FreelancerStudent.API/Services/JobPostService.cs` | Xử lý tạo/sửa bài đăng, nộp đơn, duyệt/từ chối, kiểm tra đơn trùng và cho phép ứng tuyển lại khi bị từ chối. **ZIP hiện tại còn chứa kiểm tra tuyển đủ theo nghiệm thu** (xem mục 5). |
| `FreelancerStudent.API/Repositories/JobPostRepository.cs` | Truy xuất bài đăng, đơn ứng tuyển và cập nhật trạng thái trong cơ sở dữ liệu. |
| `FreelancerStudent.API/Repositories/Interfaces/IJobPostRepository.cs` | Hợp đồng phương thức cho repository bài đăng. |
| `FreelancerStudent.API/Controllers/JobPostController.cs` | API cho các thao tác bài đăng và ứng tuyển. |
| `FreelancerStudent.API/Controllers/UngTuyenController.cs` | Các endpoint liên quan đến đơn ứng tuyển. |
| `FreelancerStudent.API/DTOs/RequestDTOs/UngTuyen_RequestDTO.cs` | Dữ liệu đầu vào khi Freelancer nộp đơn. |
| `FreelancerStudent.API/DTOs/RequestDTOs/DuyetUngTuyen_RequestDTO.cs` | Dữ liệu yêu cầu duyệt hoặc từ chối đơn. |
| `FreelancerStudent.API/DTOs/ReponseDTOs/UngTuyen_ReponseDTO.cs` | Dữ liệu trả về về đơn ứng tuyển, bao gồm thông tin liên quan đến hợp đồng. |
| `FreelancerStudent.API/DTOs/ReponseDTOs/JobPost_ReponseDTO.cs` | Dữ liệu bài đăng trả về cho Web; ZIP hiện tại có thuộc tính liên quan đến tuyển đủ. |
| `FreelancerStudent.Web/Controllers/JobPostController.cs` | Điều phối trang danh sách, chi tiết, tạo bài và nộp đơn; kiểm tra trạng thái ứng tuyển. |
| `FreelancerStudent.Web/Services/JobPostWebService.cs` | Gọi API bài đăng từ ứng dụng MVC. |
| `FreelancerStudent.Web/ViewModels/JobPostViewModel.cs` | Dữ liệu bài đăng dùng trên Razor View. |
| `FreelancerStudent.Web/ViewModels/UngTuyenViewModel.cs` | Dữ liệu hiển thị và gửi form ứng tuyển. |
| `FreelancerStudent.Web/Views/JobPost/Index.cshtml` | Danh sách công việc và nút `Ứng tuyển` / `Đã nộp đơn` / `Đã được duyệt` / `Ứng tuyển lại`. |
| `FreelancerStudent.Web/Views/JobPost/Detail.cshtml` | Chi tiết công việc và hành động ứng tuyển. |
| `FreelancerStudent.Web/Views/JobPost/UngTuyen.cshtml` | Form ứng tuyển. |
| `FreelancerStudent.Web/Views/JobPost/Create.cshtml` | Form đăng tin; ZIP hiện tại hiển thị số lượng `1` ở ô chỉ đọc và gửi hidden `soluongtuyen=1`. |
| `FreelancerStudent.Web/Views/JobPost/Edit.cshtml` | Giao diện chỉnh sửa bài đăng. |
| `FreelancerStudent.Web/Views/JobPost/QuanLyTinTuyenDung.cshtml` | Giao diện quản lý bài đăng phía nhà tuyển dụng. |

**Ý nghĩa:** Duy trì quy trình nhiều người có thể ứng tuyển, tránh nộp trùng của cùng Freelancer và hỗ trợ ứng tuyển lại khi bị từ chối. `soluongtuyen=1` trên form chỉ là giá trị gửi từ giao diện, **không tự động giới hạn số hợp đồng**.

### 2.2. Quản lý hợp đồng và ký quỹ

| File | Vai trò / mục đích triển khai |
|---|---|
| `FreelancerStudent.API/Models/HopDong.cs` | Entity hợp đồng, quan hệ với bài đăng và Freelancer. |
| `FreelancerStudent.API/Models/GiaiDoan_HopDong.cs` | Entity các giai đoạn hợp đồng. |
| `FreelancerStudent.API/DTOs/RequestDTOs/TaoHopDong_RequestDTO.cs` | Dữ liệu tạo dự thảo hợp đồng. |
| `FreelancerStudent.API/DTOs/RequestDTOs/XacNhanHopDong_RequestDTO.cs` | Dữ liệu Freelancer xác nhận hoặc từ chối hợp đồng. |
| `FreelancerStudent.API/DTOs/RequestDTOs/KyQuyHopDong_RequestDTO.cs` | Dữ liệu yêu cầu ký quỹ. |
| `FreelancerStudent.API/DTOs/RequestDTOs/HopDongFilterRequestDTO.cs` | Tham số lọc danh sách hợp đồng. |
| `FreelancerStudent.API/DTOs/ReponseDTOs/HopDongResponseDTO.cs` | Dữ liệu hợp đồng trả về API. |
| `FreelancerStudent.API/Repositories/HopDongRepository.cs` | Lấy danh sách/chi tiết, tạo hợp đồng, cập nhật trạng thái và xử lý các truy vấn hợp đồng. |
| `FreelancerStudent.API/Services/Interfaces/IHopDongService.cs` | Khai báo nghiệp vụ hợp đồng. |
| `FreelancerStudent.API/Services/HopDongService.cs` | Tạo dự thảo, xác nhận, ký quỹ; trong `KyQuyHopDongAsync` dùng transaction và cập nhật ví/hợp đồng. |
| `FreelancerStudent.API/Controllers/HopDongController.cs` | API danh sách, chi tiết, tạo, xác nhận và ký quỹ. |
| `FreelancerStudent.Web/Services/Interfaces/IHopDongWebService.cs` | Giao diện dịch vụ Web gọi API hợp đồng. |
| `FreelancerStudent.Web/Services/HopDongWebService.cs` | Gửi request API và nhận kết quả hợp đồng/ký quỹ. |
| `FreelancerStudent.Web/ViewModels/HopDongViewModel.cs` | ViewModel cho các màn hình hợp đồng. |
| `FreelancerStudent.Web/Controllers/HopDongController.cs` | Điều phối danh sách, tạo, chi tiết, xác nhận, ký quỹ và thao tác task/tiến độ. |
| `FreelancerStudent.Web/Views/HopDong/Index.cshtml` | Danh sách hợp đồng, trạng thái và tìm kiếm/lọc. |
| `FreelancerStudent.Web/Views/HopDong/TaoHopDong.cshtml` | Form tạo dự thảo hợp đồng. |
| `FreelancerStudent.Web/Views/HopDong/ChiTiet.cshtml` | Chi tiết hợp đồng và thao tác theo vai trò/trạng thái. |

**Điểm kỹ thuật quan trọng:** `HopDongService.KyQuyHopDongAsync` kiểm tra trạng thái chờ ký quỹ, chuyển hợp đồng sang `DangThucHien`, trừ `Wallet.soDuKhaDung`, tăng `Wallet.soDuDongBang` và ghi lịch sử giao dịch. Đây là **khóa tiền**, chưa phải giải ngân.

### 2.3. Freelancer bàn giao sản phẩm và yêu cầu chỉnh sửa

| File | Vai trò / mục đích triển khai |
|---|---|
| `FreelancerStudent.API/Models/BanGiao_SanPham.cs` | Entity lưu mã hợp đồng, phiên bản, file/link demo, mô tả, trạng thái và phản hồi. |
| `FreelancerStudent.API/DTOs/RequestDTOs/TaoBanGiaoRequestDTO.cs` | Dữ liệu Freelancer gửi bản bàn giao. |
| `FreelancerStudent.API/DTOs/ReponseDTOs/BanGiaoResponseDTO.cs` | DTO phản hồi thông tin bàn giao/yêu cầu chỉnh sửa. |
| `FreelancerStudent.API/Repositories/BanGiaoSanPhamRepository.cs` | Truy xuất bản bàn giao theo hợp đồng, theo ID, bản mới nhất; thêm và lưu dữ liệu. |
| `FreelancerStudent.API/Services/BanGiaoSanPhamService.cs` | Kiểm tra quyền, tạo bản bàn giao, quản lý phiên bản, yêu cầu chỉnh sửa, lấy lịch sử, nghiệm thu. |
| `FreelancerStudent.API/Controllers/BanGiaoSanPhamController.cs` | API tạo, xem, yêu cầu chỉnh sửa, nghiệm thu (`PUT nghi​emthu/{maBanGiao}`). |
| `FreelancerStudent.Web/Services/Interfaces/IBanGiaoSanPhamWebService.cs` | Khai báo các phương thức gọi API bàn giao. |
| `FreelancerStudent.Web/Services/BanGiaoSanPhamWebService.cs` | Gửi request từ Web đến API bàn giao. |
| `FreelancerStudent.Web/ViewModels/BanGiaoSanPhamViewModels.cs` | ViewModel bàn giao, phản hồi, yêu cầu chỉnh sửa. |
| `FreelancerStudent.Web/Controllers/BanGiaoSanPhamController.cs` | Các action `Index`, `TaoBanGiao`, `GuiYeuCau`, `NghiemThu`. |
| `FreelancerStudent.Web/Views/BanGiaoSanPham/Index.cshtml` | Giao diện xem lịch sử, nộp sản phẩm, yêu cầu chỉnh sửa và nghiệm thu. |

**Các trạng thái thấy trong code:** `ChoNghiemThu`, `YeuCauChinhSua`, `ChoChinhSua`, `DaNopLai`, `DaNghiemThu` (tùy bảng bàn giao/yêu cầu chỉnh sửa). Khi nộp lại, dịch vụ tăng phiên bản `v1.0`, `v2.0`… và cập nhật yêu cầu chỉnh sửa liên quan. Bản được nghiệm thu phải là bản mới nhất còn hợp lệ.

### 2.4. Giải ngân ký quỹ và cập nhật ví Freelancer

**Không có service giải ngân riêng trong source hiện tại:** nghiệp vụ này nằm trực tiếp tại `FreelancerStudent.API/Services/BanGiaoSanPhamService.cs`, phương thức **`NghiemThuAsync(int maBanGiao, int maUser)`**.

| File | Vai trò / mục đích triển khai |
|---|---|
| `FreelancerStudent.API/Services/BanGiaoSanPhamService.cs` | Sau nghiệm thu, hoàn tất hợp đồng, trừ tiền đóng băng của khách hàng, cộng tiền khả dụng cho Freelancer, ghi lịch sử hai ví trong cùng transaction. |
| `FreelancerStudent.API/Models/Wallet.cs` | Entity ví với các trường số dư khả dụng và đóng băng. |
| `FreelancerStudent.API/Repositories/WalletRepository.cs` | Lấy/tạo/cập nhật ví. |
| `FreelancerStudent.API/Repositories/Interfaces/IWalletRepository.cs` | Hợp đồng truy xuất ví. |
| `FreelancerStudent.API/Services/WalletService.cs` | Nghiệp vụ truy vấn ví. |
| `FreelancerStudent.API/Services/Interfaces/IWalletService.cs` | Giao diện nghiệp vụ ví. |
| `FreelancerStudent.API/Controllers/WalletController.cs` | API lấy thông tin ví. |
| `FreelancerStudent.Web/Controllers/WalletController.cs` | Trang ví và thao tác giao dịch trên Web. |
| `FreelancerStudent.Web/Services/WalletWebService.cs` | Dịch vụ gọi API ví. |
| `FreelancerStudent.Web/ViewModels/WalletViewModel.cs` | Dữ liệu số dư hiển thị trên giao diện. |
| `FreelancerStudent.Web/Views/Wallet/Index.cshtml` | Giao diện ví điện tử. |
| `FreelancerStudent.API/Data/ApplicationDBContext.cs` | DbContext chứa các bảng hợp đồng, bàn giao, ví và lịch sử giao dịch được sử dụng trong luồng. |

**Chi tiết transaction trong `NghiemThuAsync`:**

1. Kiểm tra người thao tác, hợp đồng `DangThucHien`, bản bàn giao `ChoNghiemThu`, số tiền ký quỹ hợp lệ và tài khoản hai bên.
2. Cập nhật `HopDong.trangThai = DaHoanThanh` với điều kiện trạng thái cũ `DangThucHien`.
3. Cập nhật `BanGiao_SanPham.trangThai = DaNghiemThu`, chỉ khi đó là phiên bản mới nhất.
4. `Wallet` khách hàng: `soDuDongBang = soDuDongBang - soTien` và kiểm tra số dư đủ.
5. `Wallet` Freelancer: `soDuKhaDung = soDuKhaDung + soTien`.
6. Tạo hai lịch sử giao dịch: `GiaiNganHopDong` (khách hàng) và `NhanTienHopDong` (Freelancer), phương thức `ViNoiBo`.
7. `SaveChangesAsync()` và `CommitAsync()`; khi có lỗi, transaction được rollback.

**Số tiền giải ngân trong bản này:** lấy từ `HopDong.sotienkyquy`; đoạn giải ngân hiện tại không thể hiện khấu trừ phí dịch vụ trước khi cộng vào ví Freelancer.

### 2.5. Cấu hình kết nối các tầng

| File | Vai trò / mục đích triển khai |
|---|---|
| `FreelancerStudent.API/Program.cs` | Đăng ký dependency injection cho repository/service/API. |
| `FreelancerStudent.Web/Program.cs` | Đăng ký dịch vụ Web và cấu hình ứng dụng MVC. |
| `FreelancerStudent.API/Data/ApplicationDBContext.cs` | Ánh xạ các entity sang bảng SQL Server. |

## 3. Tóm tắt mục đích sửa theo thứ tự triển khai nghiệp vụ

| Giai đoạn | Thay đổi chính | Kết quả mong muốn |
|---|---|---|
| 1. Ứng tuyển | Kiểm tra nộp trùng, cho phép ứng tuyển lại sau `TuChoi` | Một Freelancer không tạo nhiều đơn chờ duyệt cho cùng Job. |
| 2. Duyệt hồ sơ | Liên kết đơn được duyệt với bước tạo hợp đồng | Nhà tuyển dụng chọn ứng viên và lập dự thảo. |
| 3. Hợp đồng | Tạo, xác nhận/từ chối, hiển thị trạng thái và chi tiết | Hai bên thống nhất công việc trước khi ký quỹ. |
| 4. Ký quỹ | Khóa tiền trong ví nhà tuyển dụng và kích hoạt hợp đồng | Có tiền bảo đảm trước khi Freelancer thực hiện. |
| 5. Bàn giao | Tạo bản bàn giao có phiên bản, file/link và mô tả | Freelancer gửi sản phẩm để nhà tuyển dụng xem xét. |
| 6. Chỉnh sửa | Lưu yêu cầu sửa và hỗ trợ nộp lại | Có vòng phản hồi trước nghiệm thu. |
| 7. Nghiệm thu | Kiểm tra bản mới nhất, chuyển trạng thái hợp đồng/bàn giao | Chốt sản phẩm hợp lệ, tránh nghiệm thu lặp. |
| 8. Giải ngân | Chuyển tiền đóng băng của khách hàng sang số dư khả dụng Freelancer | Thanh toán nội bộ và có lịch sử giao dịch. |
| 9. Giao diện đăng tin | Đưa số lượng tuyển về 1 ở form | Đơn giản hóa mô hình Job mà vẫn giữ trường dữ liệu. |

## 4. Kiểm thử đề xuất

- [ ] Freelancer nộp đơn lần đầu → `ChoDuyet`; nộp lại khi đang chờ → bị chặn.
- [ ] Nhà tuyển dụng từ chối → Freelancer có thể ứng tuyển lại.
- [ ] Nhà tuyển dụng tạo hợp đồng → Freelancer xác nhận → `ChoKyQuy`.
- [ ] Nhà tuyển dụng ký quỹ đủ tiền → hợp đồng `DangThucHien`; ví khả dụng giảm, ví đóng băng tăng.
- [ ] Freelancer nộp sản phẩm lần đầu → phiên bản `v1.0`, `ChoNghiemThu`.
- [ ] Nhà tuyển dụng yêu cầu chỉnh sửa → Freelancer nộp lại phiên bản mới.
- [ ] Nhà tuyển dụng nghiệm thu bản mới nhất → hợp đồng `DaHoanThanh`, bàn giao `DaNghiemThu`.
- [ ] Tiền đóng băng nhà tuyển dụng giảm đúng bằng số tiền ký quỹ; số dư khả dụng Freelancer tăng tương ứng.
- [ ] Có hai bản ghi lịch sử giao dịch (`GiaiNganHopDong`, `NhanTienHopDong`).
- [ ] Gửi nghiệm thu lần hai hoặc nghiệm thu bản cũ → không giải ngân lần hai.
- [ ] Thử lỗi ví không đủ tiền hoặc ví Freelancer không hợp lệ → transaction rollback, không hoàn tất hợp đồng dở dang.

## 5. Đối chiếu source ZIP với yêu cầu cuối cùng trong cuộc trao đổi

**Quan trọng – bản ZIP vẫn còn một số code của phương án mà bạn đã yêu cầu hủy:**

1. `FreelancerStudent.API/Services/JobPostService.cs` vẫn có truy vấn `maJobsDaNghiemThu`, `soDaNghiemThuTheoJob`, phép gán `DaTuyenDu` trong `layDanhSachJobPostAsync()` và đoạn chặn ứng tuyển khi đã nghiệm thu đủ trong `nopDonUngTuyenAsync()`. Bạn đã yêu cầu **không áp dụng** quy tắc này. Do đó, nếu mục tiêu là bản cuối đã hoàn tác, cần kiểm tra/xóa những đoạn trên và các tham chiếu `DaTuyenDu` ở DTO/Web/View tương ứng. README ghi nhận đây là **phần còn sót trong ZIP**, không xem là nghiệp vụ cuối cùng đã thống nhất.
2. `FreelancerStudent.Web/Views/JobPost/Create.cshtml` trong ZIP đang có **ô số lượng `readonly` hiển thị `1`**, đồng thời có `<input type="hidden" asp-for="soluongtuyen" value="1" />`. Bạn muốn **ẩn hoàn toàn**, nên có thể bỏ khối label + input `readonly`, chỉ giữ input hidden.
3. `FreelancerStudent.API/Services/JobPostService.cs` vẫn gán `soluongtuyen = request.soluongtuyen` trong `taoJobPostAsync()`. Điều này nhận giá trị `1` từ form nhưng **không khóa cứng ở API**. Nếu cần mọi request đều chỉ tạo Job 1 người, đổi thành `soluongtuyen = 1` và cân nhắc tương tự trong cập nhật Job.
4. Không thể xác nhận việc build/chạy tích hợp thành công của **toàn bộ bản ZIP mới nhất** chỉ từ việc đọc source; cần build và test theo mục 4.

## 6. Những gì README này không khẳng định

- Không khẳng định mọi file liệt kê đều do một cá nhân tạo mới, vì ZIP không chứa lịch sử commit đầy đủ để chứng minh.
- Không khẳng định những đoạn đã thảo luận đều đã được hoàn tác; mục 5 nêu chính xác các điểm chưa khớp.
- Không khẳng định có hệ thống thanh toán ngân hàng thực: luồng giải ngân đang dùng ví nội bộ (`ViNoiBo`).

---

**Tóm tắt:** Hai nghiệp vụ trọng tâm cuối quy trình là **bàn giao/đề nghị chỉnh sửa/nghiệm thu sản phẩm** và **giải ngân ký quỹ vào ví Freelancer**. Trong source mới, nghiệp vụ giải ngân được thực hiện ngay trong `BanGiaoSanPhamService.NghiemThuAsync`, dùng transaction để đảm bảo việc hoàn tất hợp đồng và chuyển số dư ví diễn ra nhất quán.
