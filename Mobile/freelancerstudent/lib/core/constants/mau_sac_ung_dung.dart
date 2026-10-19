import 'package:flutter/material.dart';

/// Hệ thống màu sắc chủ đạo đồng bộ toàn bộ ứng dụng Flutter Mobile
/// Định hướng tông xanh lá tự nhiên, dịu mắt, hiện đại và chuyên nghiệp.
class AppColors {
  // --- BẢNG MÀU CHÍNH ĐỀ XUẤT ---
  /// Màu chính - Xanh lá tự nhiên (#2E7D32)
  static const Color primary = Color(0xFF2E7D32);

  /// Màu chính đậm - Xanh lá đậm (#1B5E20)
  static const Color primaryDark = Color(0xFF1B5E20);

  /// Màu xanh lá sáng / nhạt (#66BB6A)
  static const Color primaryLight = Color(0xFF66BB6A);

  /// Màu phụ - Sage Green dịu mắt (#81A88A)
  static const Color secondary = Color(0xFF81A88A);

  /// Màu nền ứng dụng - Trắng xanh rất nhạt (#F5F9F5)
  static const Color background = Color(0xFFF5F9F5);

  /// Màu nền thẻ, nội dung và dialog - Trắng (#FFFFFF)
  static const Color surface = Color(0xFFFFFFFF);

  /// Màu nền nhấn nhẹ - Xanh lá nhạt (#E8F5E9)
  static const Color lightGreenSurface = Color(0xFFE8F5E9);

  /// Màu đường viền - Xám xanh nhạt (#DCE7DD)
  static const Color border = Color(0xFFDCE7DD);

  /// Màu tiêu đề - Xám đen hơi xanh (#26352A)
  static const Color heading = Color(0xFF26352A);

  /// Màu nội dung - Xám trung tính (#526056)
  static const Color bodyText = Color(0xFF526056);

  /// Màu trạng thái thành công (#2E7D32)
  static const Color success = Color(0xFF2E7D32);

  /// Màu trạng thái cảnh báo - Vàng hổ phách (#F9A825)
  static const Color warning = Color(0xFFF9A825);

  /// Màu trạng thái lỗi - Đỏ trầm (#C62828)
  static const Color error = Color(0xFFC62828);

  /// Màu thông tin - Xanh dương dịu (#1976D2)
  static const Color info = Color(0xFF1976D2);

  /// Màu nền thông tin nhạt (#E3F2FD)
  static const Color infoLight = Color(0xFFE3F2FD);

  /// Màu viền thông tin nhạt (#BBDEFB)
  static const Color infoBorder = Color(0xFFBBDEFB);

  // --- ALIAS TƯƠNG THÍCH NGƯỢC ---
  static const Color textPrimary = heading;
  static const Color textSecondary = bodyText;
  static const Color surfaceLight = lightGreenSurface;
}
