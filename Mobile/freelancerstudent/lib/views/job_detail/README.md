# Màn hình Chi tiết Công việc & Ứng tuyển (`lib/views/job_detail/`)

## 📄 Danh sách màn hình & Nhiệm vụ:
- `job_detail_screen.dart`:
  - Hiển thị toàn bộ thông tin chi tiết: Mô tả dự án, Yêu cầu kỹ năng, Quyền lợi, Ngân sách chi trả, Thông tin Nhà tuyển dụng.
  - Nút cố định bên dưới màn hình: **"Ứng tuyển ngay"** (dành cho Freelancer) hoặc **"Xem danh sách ứng viên"** (nếu là NTD sở hữu bài đăng).
- `apply_job_bottom_sheet.dart`:
  - Modal bật lên để Freelancer điền thông tin nộp đơn:
    1. Lời giới thiệu / Thư ứng tuyển (Cover letter).
    2. Chi phí đề xuất (VNĐ).
    3. Ngày cam kết hoàn thành (Deadline đề xuất - theo yêu cầu ràng buộc nghiệp vụ).
    4. Nút bấm xác nhận gửi hồ sơ ứng tuyển (`UngTuyenController`).
