import 'package:flutter/material.dart';

class DialogUtils {
  static Future<void> showLoading(
    BuildContext context, {
    String message = 'Đang xử lý...',
  }) async {
    await showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => AlertDialog(
        content: Row(
          children: [
            const CircularProgressIndicator(),
            const SizedBox(width: 16),
            Expanded(child: Text(message)),
          ],
        ),
      ),
    );
  }

  static Future<void> showConfirmDialog(
    BuildContext context, {
    required String title,
    required String content,
    required VoidCallback onConfirm,
  }) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(title),
        content: Text(content),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Hủy'),
          ),
          TextButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Xác nhận'),
          ),
        ],
      ),
    );

    if (result == true) {
      onConfirm();
    }
  }

  static void showToast(String message, {BuildContext? context}) {
    final scaffoldMessenger = context == null
        ? null
        : ScaffoldMessenger.maybeOf(context);

    scaffoldMessenger?.showSnackBar(
      SnackBar(content: Text(message), behavior: SnackBarBehavior.floating),
    );
  }
}
