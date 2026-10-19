import 'package:intl/intl.dart';

class AppFormatters {
  static String formatCurrency(double amount) {
    final format = NumberFormat.currency(
      locale: 'vi_VN',
      symbol: 'đ',
      decimalDigits: 0,
    );
    return format.format(amount);
  }

  static String formatDateTime(DateTime date) {
    return DateFormat('HH:mm dd/MM/yyyy').format(date);
  }

  static String timeAgo(DateTime date) {
    final difference = DateTime.now().difference(date);

    if (difference.inDays > 30) {
      return '${difference.inDays ~/ 30} tháng trước';
    }
    if (difference.inDays > 0) {
      return '${difference.inDays} ngày trước';
    }
    if (difference.inHours > 0) {
      return '${difference.inHours} giờ trước';
    }
    if (difference.inMinutes > 0) {
      return '${difference.inMinutes} phút trước';
    }
    return 'Vừa xong';
  }
}
