# Đánh giá

## API
- `FreelancerStudent.API/Controllers/DanhGiaController.cs`
- `FreelancerStudent.API/DTOs/ReponseDTOs/ChuyenNganh_ReponseDTO.cs`
- `FreelancerStudent.API/DTOs/ReponseDTOs/DanhGiaThongKe_ReponseDTO.cs`
- `FreelancerStudent.API/DTOs/ReponseDTOs/DanhGia_FormInfo_ReponseDTO.cs`
- `FreelancerStudent.API/DTOs/ReponseDTOs/DanhGia_ReponseDTO.cs`
- `FreelancerStudent.API/DTOs/ReponseDTOs/HopDongDanhGia_ReponseDTO.cs`
- `FreelancerStudent.API/DTOs/RequestDTOs/DanhGia_RequestDTO.cs`
- `FreelancerStudent.API/Models/DanhGia_NhanXet.cs`
- `FreelancerStudent.API/Models/DanhGia_NhanXet_LichSu.cs`
- `FreelancerStudent.API/Repositories/DanhGiaRepository.cs`
- `FreelancerStudent.API/Repositories/Interfaces/IDanhGiaRepository.cs`
- `FreelancerStudent.API/Services/DanhGiaService.cs`
- `FreelancerStudent.API/Services/Interfaces/IDanhGiaService.cs`
- `FreelancerStudent.API/Options/DanhGiaPolicy.cs`

## Web
- `FreelancerStudent.Web/Controllers/DanhGiaController.cs`
- `FreelancerStudent.Web/Services/DanhGiaWebService.cs`
- `FreelancerStudent.Web/Services/Interfaces/IDanhGiaWebService.cs`
- `FreelancerStudent.Web/ViewModels/DanhGiaViewModels.cs`
- `FreelancerStudent.Web/Views/DanhGia/CongViecDaHoanThanh.cshtml`
- `FreelancerStudent.Web/Views/DanhGia/_DanhSachDanhGia.cshtml`
- `FreelancerStudent.Web/Views/DanhGia/_FormDanhGia.cshtml`
- `FreelancerStudent.Web/Views/HopDong/ChiTiet.cshtml`
- `FreelancerStudent.Web/Views/FreelancerStudent/Profile.cshtml`

## SQL
- `Database/KLCN040_FREELANCERSTUDENT.sql`
- `FreelancerStudent.API/Database/Migrations/20261022_HoanThienThanhToanDanhGia.sql`

## Luồng chạy chức năng đánh giá
1. Người dùng mở hợp đồng đã nghiệm thu, thanh toán hoặc hoàn tiền và chọn đánh giá đối tác.
2. Web tải form đánh giá theo hợp đồng; API kiểm tra người dùng có thuộc hợp đồng và còn thời hạn đánh giá.
3. Người dùng chọn từ 1 đến 5 sao tổng thể; có thể chấm riêng chất lượng, đúng hạn, giao tiếp và nhập nhận xét.
4. Web gửi đánh giá mới hoặc cập nhật đánh giá hiện có qua API.
5. API kiểm tra quyền, trạng thái hợp đồng, thời hạn, giới hạn sao và nhận xét; lưu đánh giá trong cơ sở dữ liệu.
6. API cập nhật điểm trung bình và số lượt đánh giá của hồ sơ đối tác, đồng thời tạo thông báo.
7. Web hiển thị danh sách đánh giá và thống kê trên hồ sơ đối tác.

## Cơ chế tính số sao
- Điểm tổng thể của hồ sơ là trung bình cộng số sao tổng thể của các đánh giá đã nhận: tổng số sao chia tổng lượt đánh giá.
- Điểm trung bình được làm tròn 2 chữ số thập phân khi trả thống kê; trường hồ sơ Freelancer được làm tròn 1 chữ số khi cập nhật.
- Điểm từng tiêu chí là trung bình cộng riêng các đánh giá có chấm tiêu chí đó; lượt bỏ trống không được tính.
- Phân bố sao đếm số đánh giá có từng mức từ 1 đến 5; tỷ lệ hài lòng là phần trăm đánh giá từ 4 sao trở lên.
- Không dùng thuật toán xếp hạng hay trọng số; sử dụng phép tính trung bình cộng và làm tròn.