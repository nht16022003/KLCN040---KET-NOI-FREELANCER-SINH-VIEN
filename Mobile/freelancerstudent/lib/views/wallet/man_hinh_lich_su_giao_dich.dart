import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_vi_tien.dart';
import '../../viewmodels/viewmodel_vi_tien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';

class TransactionHistoryScreen extends StatefulWidget {
  const TransactionHistoryScreen({super.key});

  @override
  State<TransactionHistoryScreen> createState() =>
      _TransactionHistoryScreenState();
}

class _TransactionHistoryScreenState extends State<TransactionHistoryScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    WidgetsBinding.instance.addPostFrameCallback((_) => _taiDuLieu());
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _taiDuLieu() async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null && user.userId > 0) {
      final vm = context.read<WalletViewModel>();
      await Future.wait([
        vm.taiLichSuGiaoDich(user.userId),
        vm.taiDanhSachRutTien(user.userId),
      ]);
    }
  }

  Future<void> _xuLyRutTienMoPhong(
    YeuCauRutTienModel item, {
    required bool chapNhan,
  }) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    final vm = context.read<WalletViewModel>();
    final res = await vm.xacNhanRutTienMoPhong(
      item.maRutTien,
      user.userId,
      chapNhan: chapNhan,
      lyDo: chapNhan ? null : 'Người dùng hủy yêu cầu rút tiền mô phỏng',
    );

    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(res.message),
        backgroundColor: res.isSuccess ? AppColors.primary : AppColors.error,
      ),
    );
  }

  String _formatLoaiGD(String loai) {
    switch (loai) {
      case 'NapTien':
        return 'Nạp tiền vào ví';
      case 'RutTien':
        return 'Rút tiền về ngân hàng';
      case 'GiuTienRut':
        return 'Tạm giữ tiền chờ rút';
      case 'HoanTienRut':
        return 'Hoàn tiền yêu cầu rút bị hủy';
      case 'ThanhToanHopDong':
      case 'KyQuyHopDong':
        return 'Ký quỹ hợp đồng';
      case 'GiaiNganHopDong':
        return 'Giải ngân hợp đồng';
      case 'NhanTienHopDong':
      case 'NhanThuLao':
        return 'Nhận thù lao nghiệm thu hợp đồng';
      case 'HoanTien':
      case 'HoanTienHopDong':
        return 'Hoàn tiền ký quỹ hợp đồng';
      case 'DieuChinhCong':
        return 'Admin điều chỉnh cộng tiền';
      case 'DieuChinhTru':
        return 'Admin điều chỉnh trừ tiền';
      default:
        return loai;
    }
  }

  String _formatTrangThaiRutTien(String status) {
    switch (status) {
      case 'ChoXuLy':
        return 'Chờ phê duyệt';
      case 'DaDuyet':
      case 'HoanThanh':
        return 'Đã giải ngân';
      case 'TuChoi':
      case 'DaHuy':
        return 'Đã hủy / Từ chối';
      default:
        return status;
    }
  }

  Color _colorTrangThaiRutTien(String status) {
    switch (status) {
      case 'DaDuyet':
      case 'HoanThanh':
        return AppColors.success;
      case 'TuChoi':
      case 'DaHuy':
        return AppColors.error;
      default:
        return AppColors.warning;
    }
  }

  @override
  Widget build(BuildContext context) {
    final walletVM = context.watch<WalletViewModel>();
    final dsGiaoDich = walletVM.danhSachGiaoDich;
    final dsRutTien = walletVM.danhSachRutTien;
    final dangTai = walletVM.dangTai;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Lịch sử giao dịch & Rút tiền'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
        bottom: TabBar(
          controller: _tabController,
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.bodyText,
          indicatorColor: AppColors.primary,
          tabs: const [
            Tab(text: 'Biến động số dư'),
            Tab(text: 'Yêu cầu rút tiền'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          // TAB 1: BIẾN ĐỘNG SỐ DƯ
          RefreshIndicator(
            onRefresh: _taiDuLieu,
            child: dangTai && dsGiaoDich.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : dsGiaoDich.isEmpty
                    ? const Center(
                        child: Text(
                          'Chưa có giao dịch nào được ghi nhận.',
                          style: TextStyle(color: AppColors.bodyText),
                        ),
                      )
                    : ListView.separated(
                        padding: const EdgeInsets.all(16),
                        itemCount: dsGiaoDich.length,
                        separatorBuilder: (_, _) => const SizedBox(height: 12),
                        itemBuilder: (ctx, index) {
                          final item = dsGiaoDich[index];
                          final isAdd = item.laGiaoDichCong;
                          final color =
                              isAdd ? AppColors.success : AppColors.error;

                          return Card(
                            elevation: 1,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: Padding(
                              padding: const EdgeInsets.all(16),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Expanded(
                                        child: Text(
                                          _formatLoaiGD(item.loaiGiaoDich),
                                          style: const TextStyle(
                                            fontWeight: FontWeight.bold,
                                            fontSize: 15,
                                            color: AppColors.heading,
                                          ),
                                        ),
                                      ),
                                      Text(
                                        '${isAdd ? '+' : '-'}${AppFormatters.formatCurrency(item.soTien)}',
                                        style: TextStyle(
                                          fontWeight: FontWeight.bold,
                                          fontSize: 16,
                                          color: color,
                                        ),
                                      ),
                                    ],
                                  ),
                                  if ((item.noiDung ?? '').isNotEmpty) ...[
                                    const SizedBox(height: 6),
                                    Text(
                                      item.noiDung!,
                                      style: const TextStyle(
                                        color: AppColors.bodyText,
                                        fontSize: 13,
                                      ),
                                    ),
                                  ],
                                  const Divider(height: 18),
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Text(
                                        item.ngayTao != null
                                            ? AppFormatters.formatDateTime(
                                                item.ngayTao!)
                                            : '',
                                        style: const TextStyle(
                                          color: Colors.black45,
                                          fontSize: 12,
                                        ),
                                      ),
                                      Text(
                                        'Số dư: ${AppFormatters.formatCurrency(item.soDuSauGD)}',
                                        style: const TextStyle(
                                          color: AppColors.heading,
                                          fontWeight: FontWeight.w500,
                                          fontSize: 13,
                                        ),
                                      ),
                                    ],
                                  ),
                                ],
                              ),
                            ),
                          );
                        },
                      ),
          ),

          // TAB 2: DANH SÁCH YÊU CẦU RÚT TIỀN
          RefreshIndicator(
            onRefresh: _taiDuLieu,
            child: dangTai && dsRutTien.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : dsRutTien.isEmpty
                    ? const Center(
                        child: Text(
                          'Chưa có yêu cầu rút tiền nào.',
                          style: TextStyle(color: AppColors.bodyText),
                        ),
                      )
                    : ListView.separated(
                        padding: const EdgeInsets.all(16),
                        itemCount: dsRutTien.length,
                        separatorBuilder: (_, _) => const SizedBox(height: 12),
                        itemBuilder: (ctx, index) {
                          final item = dsRutTien[index];
                          final statusColor =
                              _colorTrangThaiRutTien(item.trangThai);
                          final isPending = item.trangThai == 'ChoXuLy';

                          return Card(
                            elevation: 1,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: Padding(
                              padding: const EdgeInsets.all(16),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Text(
                                        'Rút: ${AppFormatters.formatCurrency(item.soTienRut)}',
                                        style: const TextStyle(
                                          fontWeight: FontWeight.bold,
                                          fontSize: 16,
                                          color: AppColors.heading,
                                        ),
                                      ),
                                      Container(
                                        padding: const EdgeInsets.symmetric(
                                          horizontal: 8,
                                          vertical: 4,
                                        ),
                                        decoration: BoxDecoration(
                                          color: statusColor.withValues(
                                              alpha: 0.1),
                                          borderRadius:
                                              BorderRadius.circular(6),
                                        ),
                                        child: Text(
                                          _formatTrangThaiRutTien(
                                              item.trangThai),
                                          style: TextStyle(
                                            color: statusColor,
                                            fontWeight: FontWeight.bold,
                                            fontSize: 12,
                                          ),
                                        ),
                                      ),
                                    ],
                                  ),
                                  const SizedBox(height: 8),
                                  Text(
                                    'Ngân hàng: ${item.tenNganHang} - STK: ${item.soTaiKhoan}',
                                    style: const TextStyle(
                                      color: AppColors.bodyText,
                                      fontSize: 14,
                                    ),
                                  ),
                                  Text(
                                    'Chủ tài khoản: ${item.tenChuTaiKhoan}',
                                    style: const TextStyle(
                                      color: AppColors.bodyText,
                                      fontSize: 13,
                                    ),
                                  ),
                                  if ((item.lyDoTuChoi ?? '').isNotEmpty) ...[
                                    const SizedBox(height: 6),
                                    Text(
                                      'Lý do từ chối: ${item.lyDoTuChoi}',
                                      style: const TextStyle(
                                        color: AppColors.error,
                                        fontSize: 13,
                                      ),
                                    ),
                                  ],
                                  const Divider(height: 18),
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Expanded(
                                        child: Text(
                                          item.ngayYeuCau != null
                                              ? 'Yêu cầu lúc: ${AppFormatters.formatDateTime(item.ngayYeuCau!)}'
                                              : '',
                                          style: const TextStyle(
                                            color: Colors.black45,
                                            fontSize: 12,
                                          ),
                                        ),
                                      ),
                                    ],
                                  ),
                                  if (isPending && item.maRutTien > 0) ...[
                                    const SizedBox(height: 12),
                                    Container(
                                      padding: const EdgeInsets.all(10),
                                      decoration: BoxDecoration(
                                        color: Colors.amber.shade50,
                                        borderRadius: BorderRadius.circular(8),
                                        border: Border.all(
                                            color: Colors.amber.shade300),
                                      ),
                                      child: Column(
                                        crossAxisAlignment:
                                            CrossAxisAlignment.start,
                                        children: [
                                          Text(
                                            'Mô phỏng xử lý rút tiền (Demo):',
                                            style: TextStyle(
                                              fontSize: 12,
                                              fontWeight: FontWeight.bold,
                                              color: Colors.amber.shade900,
                                            ),
                                          ),
                                          const SizedBox(height: 8),
                                          Row(
                                            children: [
                                              Expanded(
                                                child: OutlinedButton(
                                                  onPressed: () =>
                                                      _xuLyRutTienMoPhong(
                                                    item,
                                                    chapNhan: false,
                                                  ),
                                                  style: OutlinedButton
                                                      .styleFrom(
                                                    foregroundColor:
                                                        AppColors.error,
                                                    side: const BorderSide(
                                                        color: AppColors.error),
                                                  ),
                                                  child: const Text(
                                                    'Hủy / Hoàn tiền',
                                                    style:
                                                        TextStyle(fontSize: 12),
                                                  ),
                                                ),
                                              ),
                                              const SizedBox(width: 8),
                                              Expanded(
                                                child: FilledButton(
                                                  onPressed: () =>
                                                      _xuLyRutTienMoPhong(
                                                    item,
                                                    chapNhan: true,
                                                  ),
                                                  style: FilledButton.styleFrom(
                                                    backgroundColor:
                                                        AppColors.primary,
                                                  ),
                                                  child: const Text(
                                                    'Duyệt giải ngân',
                                                    style:
                                                        TextStyle(fontSize: 12),
                                                  ),
                                                ),
                                              ),
                                            ],
                                          ),
                                        ],
                                      ),
                                    ),
                                  ],
                                ],
                              ),
                            ),
                          );
                        },
                      ),
          ),
        ],
      ),
    );
  }
}
