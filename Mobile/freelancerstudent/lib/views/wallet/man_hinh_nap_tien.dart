import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../services/dich_vu_vi_tien.dart';
import '../../viewmodels/viewmodel_vi_tien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../profile/man_hinh_minh_chung_the_sv.dart';

class DepositScreen extends StatefulWidget {
  const DepositScreen({super.key});

  @override
  State<DepositScreen> createState() => _DepositScreenState();
}

class _DepositScreenState extends State<DepositScreen> {
  final TextEditingController _soTienController = TextEditingController();
  final WalletService _walletService = WalletService();
  final ImagePicker _picker = ImagePicker();

  bool _dangXuLy = false;
  bool _daXacNhanMoPhong = false;
  bool _dangTaiBienLai = false;
  bool _daTaiBienLai = false;
  File? _anhBienLai;
  Map<String, dynamic>? _ketQuaGiaoDich;

  final List<double> _mucTienGoiY = [100000, 200000, 500000, 1000000, 2000000];

  Future<void> _chonAnhBienLai(ImageSource source) async {
    try {
      final picked = await _picker.pickImage(
        source: source,
        imageQuality: 85,
        maxWidth: 1920,
      );
      if (picked != null) {
        setState(() {
          _anhBienLai = File(picked.path);
        });
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Lỗi khi chọn ảnh: ${e.toString()}')),
      );
    }
  }

  Future<void> _guiAnhBienLai(String maGiaoDich) async {
    if (_anhBienLai == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn ảnh biên lai trước.')),
      );
      return;
    }

    setState(() => _dangTaiBienLai = true);

    final vm = context.read<WalletViewModel>();
    final res = await vm.uploadBienLaiNapTien(maGiaoDich, _anhBienLai!);

    if (!mounted) return;
    setState(() => _dangTaiBienLai = false);

    if (res.isSuccess) {
      setState(() => _daTaiBienLai = true);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message),
          backgroundColor: AppColors.primary,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message),
          backgroundColor: Colors.red[700],
        ),
      );
    }
  }

  @override
  void dispose() {
    _soTienController.dispose();
    super.dispose();
  }

  Future<void> _taoGiaoDich() async {
    final rawText = _soTienController.text.replaceAll(RegExp(r'[^0-9]'), '');
    final soTien = double.tryParse(rawText) ?? 0;

    if (soTien < 100000 || soTien % 50000 != 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Số tiền nạp tối thiểu là 100.000đ và phải là bội số của 50.000đ.',
          ),
        ),
      );
      return;
    }

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập để nạp tiền.')),
      );
      return;
    }

    setState(() => _dangXuLy = true);

    final res = await _walletService.taoGiaoDichNapTien(user.userId, soTien);

    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (res.isSuccess && res.data != null) {
      setState(() {
        _ketQuaGiaoDich = res.data;
      });
      context.read<WalletViewModel>().taiThongTinVi(user.userId);
    } else {
      final msg =
          res.message.isNotEmpty ? res.message : 'Tạo giao dịch nạp tiền thất bại.';
      if (msg.contains('xác thực') || msg.contains('xác minh')) {
        showDialog(
          context: context,
          builder: (ctx) => AlertDialog(
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
            title: const Row(
              children: [
                Icon(Icons.shield_outlined, color: Colors.orange),
                SizedBox(width: 8),
                Text(
                  'Yêu cầu xác thực',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
              ],
            ),
            content: Text(msg),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(ctx),
                child: const Text('Đóng'),
              ),
              ElevatedButton(
                style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
                onPressed: () {
                  Navigator.pop(ctx);
                  Navigator.push(
                    context,
                    MaterialPageRoute(builder: (_) => const ManHinhMinhChungTheSV()),
                  );
                },
                child: const Text(
                  'Xác thực ngay',
                  style: TextStyle(color: Colors.white),
                ),
              ),
            ],
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(msg)),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Nạp tiền vào ví'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: _ketQuaGiaoDich != null
          ? _buildKetQua(context)
          : ListView(
              padding: const EdgeInsets.all(16),
              children: [
                const Text(
                  'Chọn hoặc nhập số tiền cần nạp',
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: AppColors.heading,
                  ),
                ),
                const SizedBox(height: 12),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: _mucTienGoiY.map((tien) {
                    return ActionChip(
                      label: Text(AppFormatters.formatCurrency(tien)),
                      onPressed: () {
                        setState(() {
                          _soTienController.text = tien.toStringAsFixed(0);
                        });
                      },
                    );
                  }).toList(),
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: _soTienController,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    labelText: 'Số tiền (VNĐ)',
                    hintText: 'Tối thiểu 100.000đ',
                    border: OutlineInputBorder(),
                    prefixIcon: Icon(Icons.attach_money),
                  ),
                ),
                const SizedBox(height: 12),
                const Text(
                  'Quy định: Số tiền tối thiểu 100.000đ và là bội số của 50.000đ. Hệ thống xác thực tự động qua chuyển khoản SePay hoặc nút xác nhận mô phỏng bên dưới.',
                  style: TextStyle(fontSize: 12, color: AppColors.bodyText),
                ),
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  child: FilledButton(
                    onPressed: _dangXuLy ? null : _taoGiaoDich,
                    style: FilledButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      backgroundColor: AppColors.primary,
                    ),
                    child: _dangXuLy
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Text('Tạo giao dịch nạp tiền'),
                  ),
                ),
              ],
            ),
    );
  }

  Widget _buildKetQua(BuildContext context) {
    final kq = _ketQuaGiaoDich!;
    final maGiaoDich = kq['maGiaoDich']?.toString() ?? '';
    final noiDung = kq['noiDungChuyenKhoan']?.toString() ?? maGiaoDich;
    final soTien =
        (kq['soTien'] is num) ? (kq['soTien'] as num).toDouble() : 0.0;
    final qrUrl =
        'https://img.vietqr.io/image/MB-0378896290-compact2.png?amount=${soTien.toInt()}&addInfo=${Uri.encodeComponent(noiDung)}&accountName=${Uri.encodeComponent('HOANG TUAN')}';

    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.lightGreenSurface,
            borderRadius: BorderRadius.circular(16),
          ),
          child: Column(
            children: [
              Icon(
                _daXacNhanMoPhong
                    ? Icons.verified_rounded
                    : Icons.check_circle_outline,
                color: AppColors.primary,
                size: 56,
              ),
              const SizedBox(height: 12),
              Text(
                _daXacNhanMoPhong
                    ? 'Đã cộng tiền vào ví thành công!'
                    : 'Tạo đơn nạp tiền thành công!',
                style: const TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Số tiền: ${AppFormatters.formatCurrency(soTien)}',
                style: const TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: AppColors.primary,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),

        // Mã QR VietQR
        Card(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                const Text(
                  'Quét mã VietQR để chuyển khoản',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 15,
                    color: AppColors.heading,
                  ),
                ),
                const SizedBox(height: 12),
                ClipRRect(
                  borderRadius: BorderRadius.circular(10),
                  child: Image.network(
                    qrUrl,
                    height: 220,
                    fit: BoxFit.contain,
                    errorBuilder: (_, _, _) => const Padding(
                      padding: EdgeInsets.all(16),
                      child: Text('Không thể tải mã QR VietQR'),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 14),

        Card(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Thông tin chuyển khoản',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                ),
                const Divider(),
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: const Text('Mã giao dịch'),
                  subtitle: Text(
                    maGiaoDich,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: const Text('Nội dung chuyển khoản (bắt buộc chính xác)'),
                  subtitle: SelectableText(
                    noiDung,
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      color: Colors.red,
                      fontSize: 16,
                    ),
                  ),
                ),
                const SizedBox(height: 8),
                const Text(
                  'Vui lòng ghi đúng nội dung chuyển khoản ở trên để hệ thống tự động cộng tiền vào ví.',
                  style: TextStyle(fontSize: 13, color: Colors.black54),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 16),

        // Khối tải ảnh biên lai minh chứng (Đồng bộ với Website & Admin)
        Card(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Row(
                  children: [
                    Icon(Icons.receipt_long, color: AppColors.primary, size: 22),
                    SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        'Minh chứng chuyển khoản (Ảnh biên lai)',
                        style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                const Text(
                  'Sau khi hoàn tất chuyển khoản theo mã và nội dung ở trên, hãy đính kèm ảnh chụp biên lai ngân hàng để Quản trị viên đối chiếu và phê duyệt ngay.',
                  style: TextStyle(fontSize: 12.5, color: Colors.black54),
                ),
                const SizedBox(height: 14),

                if (_daTaiBienLai) ...[
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: const Color(0xFFF0FDF4),
                      borderRadius: BorderRadius.circular(8),
                      border: Border.all(color: const Color(0xFFBBF7D0)),
                    ),
                    child: const Row(
                      children: [
                        Icon(Icons.check_circle, color: Color(0xFF166534), size: 20),
                        SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            'Đã tải ảnh biên lai thành công! Quản trị viên đang đối chiếu và sẽ duyệt số dư vào ví.',
                            style: TextStyle(
                              color: Color(0xFF166534),
                              fontWeight: FontWeight.w600,
                              fontSize: 13,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ] else ...[
                  if (_anhBienLai != null) ...[
                    Center(
                      child: Stack(
                        children: [
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.file(
                              _anhBienLai!,
                              height: 180,
                              fit: BoxFit.cover,
                            ),
                          ),
                          Positioned(
                            top: 4,
                            right: 4,
                            child: InkWell(
                              onTap: () => setState(() => _anhBienLai = null),
                              child: Container(
                                padding: const EdgeInsets.all(4),
                                decoration: const BoxDecoration(
                                  color: Colors.black54,
                                  shape: BoxShape.circle,
                                ),
                                child: const Icon(Icons.close, color: Colors.white, size: 16),
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 12),
                    SizedBox(
                      width: double.infinity,
                      child: ElevatedButton.icon(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: AppColors.primary,
                          foregroundColor: Colors.white,
                          padding: const EdgeInsets.symmetric(vertical: 12),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                        ),
                        icon: _dangTaiBienLai
                            ? const SizedBox(
                                width: 18,
                                height: 18,
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                  color: Colors.white,
                                ),
                              )
                            : const Icon(Icons.cloud_upload_outlined, size: 20),
                        label: Text(
                          _dangTaiBienLai ? 'Đang tải lên...' : 'Gửi ảnh biên lai xác thực',
                          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                        ),
                        onPressed: _dangTaiBienLai ? null : () => _guiAnhBienLai(maGiaoDich),
                      ),
                    ),
                  ] else ...[
                    Row(
                      children: [
                        Expanded(
                          child: OutlinedButton.icon(
                            style: OutlinedButton.styleFrom(
                              foregroundColor: AppColors.primary,
                              side: const BorderSide(color: AppColors.primary),
                              padding: const EdgeInsets.symmetric(vertical: 10),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(8),
                              ),
                            ),
                            icon: const Icon(Icons.camera_alt_outlined, size: 18),
                            label: const Text('Chụp ảnh', style: TextStyle(fontWeight: FontWeight.bold)),
                            onPressed: () => _chonAnhBienLai(ImageSource.camera),
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: OutlinedButton.icon(
                            style: OutlinedButton.styleFrom(
                              foregroundColor: AppColors.primary,
                              side: const BorderSide(color: AppColors.primary),
                              padding: const EdgeInsets.symmetric(vertical: 10),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(8),
                              ),
                            ),
                            icon: const Icon(Icons.photo_library_outlined, size: 18),
                            label: const Text('Thư viện', style: TextStyle(fontWeight: FontWeight.bold)),
                            onPressed: () => _chonAnhBienLai(ImageSource.gallery),
                          ),
                        ),
                      ],
                    ),
                  ],
                ],
              ],
            ),
          ),
        ),
        const SizedBox(height: 16),

        // Khối trạng thái chờ Quản trị viên phê duyệt
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.blue.shade50,
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: Colors.blue.shade300),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Icon(Icons.hourglass_top_rounded, color: Colors.blue.shade800),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      'Trạng thái: Đang chờ Quản trị viên duyệt',
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        color: Colors.blue.shade900,
                        fontSize: 15,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Text(
                'Sau khi thực hiện chuyển khoản với mã và nội dung ở trên, Quản trị viên hệ thống sẽ đối chiếu và phê duyệt số dư vào ví của bạn.',
                style: TextStyle(fontSize: 13, color: Colors.blue.shade900),
              ),
            ],
          ),
        ),

        const SizedBox(height: 16),
        OutlinedButton(
          onPressed: () => Navigator.pop(context),
          style: OutlinedButton.styleFrom(
            padding: const EdgeInsets.symmetric(vertical: 14),
          ),
          child: const Text('Quay lại Ví điện tử'),
        ),
      ],
    );
  }
}
