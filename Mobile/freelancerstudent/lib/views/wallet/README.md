# Màn hình Ví & Thanh toán (`lib/views/wallet/`)

## 📄 Danh sách màn hình & Nhiệm vụ:
- `wallet_screen.dart`:
  - Thẻ hiển thị số dư ví khả dụng (VNĐ).
  - Các nút thao tác nhanh: **"Nạp tiền"**, **"Rút tiền / Thanh toán"**, **"Lịch sử"**.
- `deposit_screen.dart`:
  - Nhập số tiền cần nạp.
  - Chọn cổng thanh toán (VNPAY, MoMo, Chuyển khoản QR code ngân hàng).
  - Điều hướng tới WebView hoặc mở app thanh toán tương ứng.
- `transaction_history_screen.dart`:
  - Danh sách lịch sử biến động số dư (Nạp tiền, Tạm giữ tiền đặt cọc dự án, Nhận tiền thanh toán dự án hoàn thành).
  - Chi tiết từng mã giao dịch, thời gian và trạng thái (`Thành công`, `Thất bại`, `Đang xử lý`).
