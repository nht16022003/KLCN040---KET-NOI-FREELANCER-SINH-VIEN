import 'package:dio/dio.dart';

/// Trích thông báo lỗi dễ đọc từ lỗi gọi API (ưu tiên `message` do Backend trả về).
String layThongBaoLoiApi(Object loi, {String macDinh = 'Đã xảy ra lỗi.'}) {
  if (loi is DioException) {
    final du = loi.response?.data;
    if (du is Map && du['message'] != null) {
      final tinNhan = du['message'].toString().trim();
      if (tinNhan.isNotEmpty) return tinNhan;
    }
    if (du is String && du.trim().isNotEmpty && du.length < 300) {
      return du.trim();
    }
    switch (loi.type) {
      case DioExceptionType.connectionError:
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.receiveTimeout:
      case DioExceptionType.sendTimeout:
        return 'Không kết nối được máy chủ. Vui lòng kiểm tra API đang chạy.';
      default:
        break;
    }
    final ma = loi.response?.statusCode;
    return ma != null ? '$macDinh (HTTP $ma)' : macDinh;
  }
  return macDinh;
}
