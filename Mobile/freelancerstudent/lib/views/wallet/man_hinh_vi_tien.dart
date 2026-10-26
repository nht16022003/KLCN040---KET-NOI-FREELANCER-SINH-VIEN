import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../services/dich_vu_minh_chung.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_vi_tien.dart';
import '../profile/man_hinh_minh_chung_the_sv.dart';
import 'man_hinh_nap_tien.dart';
import 'man_hinh_rut_tien.dart';
import 'man_hinh_lich_su_giao_dich.dart';

class WalletScreen extends StatefulWidget {
  const WalletScreen({super.key});

  @override
  State<WalletScreen> createState() => _WalletScreenState();
}

class _WalletScreenState extends State<WalletScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _taiVi());
  }

  Future<void> _taiVi() async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      await context.read<WalletViewModel>().taiThongTinVi(user.userId);
    }
  }

  Future<void> _kiemTraVaChuyenHuong(
    BuildContext context,
    int userId,
    Widget manHinhTiepTheo,
    String hanhDong,
  ) async {
    final user = context.read<AuthViewModel>().currentUser;
    // Nếu là Nhà tuyển dụng thì không cần minh chứng thẻ sinh viên
    if (user != null && user.role.toLowerCase() != 'nhatuyendung') {
      final minhChungService = MinhChungService();
      final xacThuc = await minhChungService.kiemTraTrangThaiXacThuc(userId);
      if (xacThuc.isSuccess && xacThuc.data != null && xacThuc.data!['daXacMinh'] != true) {
        if (!context.mounted) return;
        showDialog(
          context: context,
          builder: (ctx) => AlertDialog(
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
            title: const Row(
              children: [
                Icon(Icons.shield_outlined, color: Colors.orange),
                SizedBox(width: 8),
                Text('Yêu cầu xác thực', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
              ],
            ),
            content: Text(
              xacThuc.data!['thongBao']?.toString() ??
                  'Tài khoản sinh viên cần hoàn tất xác thực minh chứng Thẻ sinh viên (trạng thái "Đã xác minh") trước khi $hanhDong!',
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(ctx),
                child: const Text('Để sau'),
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
                child: const Text('Xác thực ngay', style: TextStyle(color: Colors.white)),
              ),
            ],
          ),
        );
        return;
      }
    }

    if (!context.mounted) return;
    Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => manHinhTiepTheo),
    );
  }

  @override
  Widget build(BuildContext context) {
    final user = context.watch<AuthViewModel>().currentUser;
    final walletVM = context.watch<WalletViewModel>();

    if (user == null) {
      return Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          title: const Text('Ví của tôi'),
          backgroundColor: AppColors.surface,
          foregroundColor: AppColors.heading,
          elevation: 0.5,
        ),
        body: const Center(
          child: Text('Vui lòng đăng nhập để xem thông tin ví.'),
        ),
      );
    }

    final vi = walletVM.vi;
    final dangTai = walletVM.dangTai;
    final loi = walletVM.thongBaoLoi;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Ví của tôi'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: RefreshIndicator(
        onRefresh: _taiVi,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            if (dangTai && vi == null)
              const Center(
                child: Padding(
                  padding: EdgeInsets.all(40),
                  child: CircularProgressIndicator(),
                ),
              )
            else if (loi != null && vi == null)
              Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(loi, textAlign: TextAlign.center),
                      const SizedBox(height: 12),
                      FilledButton(
                        onPressed: _taiVi,
                        child: const Text('Thử lại'),
                      ),
                    ],
                  ),
                ),
              )
            else ...[
              Card(
                elevation: 2,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(20),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text(
                            'Số dư khả dụng',
                            style: TextStyle(
                              fontSize: 15,
                              color: AppColors.bodyText,
                            ),
                          ),
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 8,
                              vertical: 4,
                            ),
                            decoration: BoxDecoration(
                              color: AppColors.lightGreenSurface,
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: Text(
                              vi?.loaiTienTe ?? 'VND',
                              style: const TextStyle(
                                fontSize: 12,
                                fontWeight: FontWeight.bold,
                                color: AppColors.primary,
                              ),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 8),
                      Text(
                        AppFormatters.formatCurrency(vi?.soDuKhaDung ?? 0),
                        style: const TextStyle(
                           fontSize: 30,
                           fontWeight: FontWeight.bold,
                           color: AppColors.heading,
                        ),
                      ),
                      const Divider(height: 24),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text(
                            'Số dư đóng băng (ký quỹ):',
                            style: TextStyle(fontSize: 13, color: AppColors.bodyText),
                          ),
                          Text(
                            AppFormatters.formatCurrency(vi?.soDuDongBang ?? 0),
                            style: const TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.bold,
                              color: AppColors.warning,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 20),
              Row(
                children: [
                  Expanded(
                    child: FilledButton.icon(
                      onPressed: () {
                        _kiemTraVaChuyenHuong(
                          context,
                          user.userId,
                          const DepositScreen(),
                          'nạp tiền vào ví',
                        );
                      },
                      icon: const Icon(Icons.add_circle_outline),
                      label: const Text('Nạp tiền'),
                      style: FilledButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 12),
                        backgroundColor: AppColors.primary,
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: FilledButton.icon(
                      onPressed: () {
                        _kiemTraVaChuyenHuong(
                          context,
                          user.userId,
                          const WithdrawScreen(),
                          'tạo yêu cầu rút tiền',
                        );
                      },
                      icon: const Icon(Icons.outbox_rounded),
                      label: const Text('Rút tiền'),
                      style: FilledButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 12),
                        backgroundColor: Colors.teal,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              OutlinedButton.icon(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const TransactionHistoryScreen(),
                    ),
                  );
                },
                icon: const Icon(Icons.history),
                label: const Text('Xem lịch sử giao dịch & Rút tiền'),
                style: OutlinedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  foregroundColor: AppColors.primary,
                  side: const BorderSide(color: AppColors.primary),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
