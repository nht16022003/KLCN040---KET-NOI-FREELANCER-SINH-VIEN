import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../viewmodels/viewmodel_nha_tuyen_dung.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../models/mo_hinh_danh_gia.dart';
import '../../services/dich_vu_danh_gia.dart';
import '../danh_gia/man_hinh_danh_sach_tat_ca_danh_gia.dart';

class HoSoNhaTuyenDungScreen extends StatefulWidget {
  final int? maNhaTuyenDung;
  final int? maUser;

  const HoSoNhaTuyenDungScreen({
    super.key,
    this.maNhaTuyenDung,
    this.maUser,
  });

  @override
  State<HoSoNhaTuyenDungScreen> createState() => _HoSoNhaTuyenDungScreenState();
}

class _HoSoNhaTuyenDungScreenState extends State<HoSoNhaTuyenDungScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _taiDuLieu());
  }

  void _taiDuLieu() {
    final vm = context.read<NhaTuyenDungViewModel>();
    if (widget.maNhaTuyenDung != null && widget.maNhaTuyenDung! > 0) {
      vm.taiProfileNhaTuyenDung(widget.maNhaTuyenDung!);
    } else {
      final user = context.read<AuthViewModel>().currentUser;
      final userId = widget.maUser ?? user?.userId;
      if (userId != null && userId > 0) {
        vm.taiProfileNhaTuyenDungTheoUser(userId);
      }
    }
  }

  Widget _dongThongTin(String nhan, String? giaTri) {
    if (giaTri == null || giaTri.trim().isEmpty) {
      return const SizedBox.shrink();
    }
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 140,
            child: Text(
              nhan,
              style: const TextStyle(color: Colors.black54, fontSize: 13),
            ),
          ),
          Expanded(
            child: Text(
              giaTri,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w500,
                color: AppColors.heading,
              ),
            ),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<NhaTuyenDungViewModel>();
    final profile = vm.profileChiTiet;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Hồ sơ nhà tuyển dụng'),
        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
      ),
      body: RefreshIndicator(
        onRefresh: () async => _taiDuLieu(),
        child: Builder(
          builder: (context) {
            if (vm.dangTai && profile == null) {
              return const Center(child: CircularProgressIndicator());
            }

            if (vm.thongBaoLoi != null && profile == null) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        vm.thongBaoLoi!,
                        textAlign: TextAlign.center,
                        style: const TextStyle(color: AppColors.error),
                      ),
                      const SizedBox(height: 12),
                      OutlinedButton(
                        onPressed: _taiDuLieu,
                        style: OutlinedButton.styleFrom(
                          shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.zero,
                          ),
                        ),
                        child: const Text('Thử lại'),
                      ),
                    ],
                  ),
                ),
              );
            }

            if (profile == null) {
              return const Center(child: Text('Không tìm thấy thông tin hồ sơ.'));
            }

            final ntd = profile.nhaTuyenDung;
            final dsJob = profile.danhSachTinTuyenDung;

            final tenCongTy = ntd.tenHienThi;
            final nguoiDaiDien = ntd.hotenUser ?? '';
            final email = ntd.emailUser ?? '';
            final sdt = ntd.sdtUser ?? '';
            final linhVuc = ntd.linhVucHienThi;
            final diaChi = ntd.diaChiHienThi;
            final gioiThieu = ntd.gioithieu ?? '';
            final soSao = ntd.diemDanhGia;
            final tongTin = profile.tongTinTuyenDung;
            final hopDongXong = profile.soHopDongHoanThanh;
            final hopTac = profile.soFreelancerHopTac;

            return ListView(
              padding: const EdgeInsets.all(16),
              children: [
                // Header Doanh nghiệp
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: AppColors.surface,
                    border: Border.all(color: AppColors.cardBorder),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        tenCongTy,
                        style: const TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.bold,
                          color: AppColors.heading,
                        ),
                      ),
                      const SizedBox(height: 6),
                      Text(
                        'Lĩnh vực: $linhVuc',
                        style: const TextStyle(
                          color: AppColors.primary,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'Đánh giá: ${soSao.toStringAsFixed(1)} / 5.0 sao',
                        style: const TextStyle(
                          color: Colors.amber,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 12),

                // Thống kê nhanh
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: AppColors.surface,
                    border: Border.all(color: AppColors.cardBorder),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                    children: [
                      _itemThongKe('Tin tuyển dụng', tongTin.toString()),
                      _itemThongKe('HĐ hoàn thành', hopDongXong.toString()),
                      _itemThongKe('Freelancer hợp tác', hopTac.toString()),
                    ],
                  ),
                ),
                const SizedBox(height: 12),

                // Chi tiết thông tin liên hệ
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: AppColors.surface,
                    border: Border.all(color: AppColors.cardBorder),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'Thông tin liên hệ & địa chỉ',
                        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                      const Divider(height: 20),
                      _dongThongTin('Người đại diện:', nguoiDaiDien),
                      _dongThongTin('Email:', email),
                      _dongThongTin('Số điện thoại:', sdt),
                      _dongThongTin('Địa chỉ:', diaChi),
                      if (gioiThieu.isNotEmpty) ...[
                        const SizedBox(height: 8),
                        const Text(
                          'Giới thiệu công ty:',
                          style: TextStyle(fontSize: 13, color: Colors.black54),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          gioiThieu,
                          style: const TextStyle(fontSize: 14, height: 1.4),
                        ),
                      ],
                    ],
                  ),
                ),
                const SizedBox(height: 12),

                _NhaTuyenDungReviewSection(
                  maNhaTuyenDung: ntd.maNhaTuyenDung > 0
                      ? ntd.maNhaTuyenDung
                      : (widget.maNhaTuyenDung ?? 0),
                  tenNhaTuyenDung: tenCongTy,
                ),
                const SizedBox(height: 12),

                // Danh sách việc làm đã đăng
                if (dsJob.isNotEmpty) ...[
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: AppColors.surface,
                      border: Border.all(color: AppColors.cardBorder),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Tin tuyển dụng (${dsJob.length})',
                          style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                        ),
                        const Divider(height: 20),
                        ...dsJob.map((job) {
                          final tieuDe = job.tieude;
                          final thuLao = job.thulao;
                          final status = job.status;
                          final isDangTuyen = status == 'DangTuyen' ||
                              status.toLowerCase().contains('đang tuyển');

                          return Container(
                            margin: const EdgeInsets.only(bottom: 10),
                            padding: const EdgeInsets.all(12),
                            decoration: BoxDecoration(
                              color: Colors.grey.shade50,
                              border: Border.all(color: Colors.grey.shade200),
                            ),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  tieuDe,
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 14,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Row(
                                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                  children: [
                                    if (thuLao > 0)
                                      Text(
                                        AppFormatters.formatCurrency(thuLao),
                                        style: const TextStyle(
                                          color: AppColors.primary,
                                          fontWeight: FontWeight.w600,
                                          fontSize: 13,
                                        ),
                                      ),
                                    Container(
                                      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                                      decoration: BoxDecoration(
                                        color: isDangTuyen ? Colors.green.shade50 : Colors.grey.shade200,
                                        border: Border.all(
                                          color: isDangTuyen ? Colors.green : Colors.grey,
                                        ),
                                      ),
                                      child: Text(
                                        isDangTuyen ? 'Đang tuyển' : status,
                                        style: TextStyle(
                                          fontSize: 11,
                                          fontWeight: FontWeight.w600,
                                          color: isDangTuyen ? Colors.green.shade800 : Colors.black87,
                                        ),
                                      ),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          );
                        }),
                      ],
                    ),
                  ),
                ],
              ],
            );
          },
        ),
      ),
    );
  }

  Widget _itemThongKe(String nhan, String giaTri) {
    return Column(
      children: [
        Text(
          giaTri,
          style: const TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.bold,
            color: AppColors.primary,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          nhan,
          style: const TextStyle(fontSize: 12, color: Colors.black54),
        ),
      ],
    );
  }
}

class _NhaTuyenDungReviewSection extends StatefulWidget {
  final int maNhaTuyenDung;
  final String tenNhaTuyenDung;

  const _NhaTuyenDungReviewSection({
    required this.maNhaTuyenDung,
    required this.tenNhaTuyenDung,
  });

  @override
  State<_NhaTuyenDungReviewSection> createState() => _NhaTuyenDungReviewSectionState();
}

class _NhaTuyenDungReviewSectionState extends State<_NhaTuyenDungReviewSection> {
  final DanhGiaService _service = DanhGiaService();
  bool _loading = true;
  ThongKeDanhGiaModel? _thongKe;

  @override
  void initState() {
    super.initState();
    _loadReviews();
  }

  Future<void> _loadReviews() async {
    if (widget.maNhaTuyenDung <= 0) {
      setState(() => _loading = false);
      return;
    }
    setState(() {
      _loading = true;
    });

    final res = await _service.layDanhGiaNhaTuyenDung(
      maNhaTuyenDung: widget.maNhaTuyenDung,
      trang: 1,
      kichThuocTrang: 5,
    );

    if (mounted) {
      setState(() {
        _loading = false;
        if (res.isSuccess) {
          _thongKe = res.data;
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_loading) {
      return Container(
        padding: const EdgeInsets.all(20),
        decoration: BoxDecoration(
          color: AppColors.surface,
          border: Border.all(color: AppColors.cardBorder),
        ),
        child: const Center(child: CircularProgressIndicator()),
      );
    }

    final stats = _thongKe;
    final total = stats?.tongLuotDanhGia ?? 0;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.surface,
        border: Border.all(color: AppColors.cardBorder),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Đánh giá từ Freelancer ($total)',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              if (total > 0)
                TextButton(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => ManHinhDanhSachTatCaDanhGia(
                          maNhaTuyenDung: widget.maNhaTuyenDung,
                          tenDoiTuong: widget.tenNhaTuyenDung,
                          laNhaTuyenDung: true,
                        ),
                      ),
                    );
                  },
                  child: const Text('Xem tất cả'),
                ),
            ],
          ),
          const Divider(height: 20),
          if (total == 0)
            const Padding(
              padding: EdgeInsets.symmetric(vertical: 12),
              child: Text(
                'Chưa có đánh giá nào từ Freelancer Sinh Viên.',
                style: TextStyle(color: Colors.black54),
              ),
            )
          else ...[
            Row(
              children: [
                Text(
                  stats!.diemTrungBinh.toStringAsFixed(1),
                  style: const TextStyle(
                    fontSize: 32,
                    fontWeight: FontWeight.bold,
                    color: AppColors.heading,
                  ),
                ),
                const SizedBox(width: 10),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: List.generate(
                        5,
                        (index) => Icon(
                          index < stats.diemTrungBinh.round()
                              ? Icons.star
                              : Icons.star_border,
                          size: 18,
                          color: Colors.amber,
                        ),
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      '$total lượt đánh giá',
                      style: const TextStyle(fontSize: 12, color: Colors.black54),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 14),
            ...stats.danhSach.take(3).map((item) {
              return Container(
                margin: const EdgeInsets.only(bottom: 10),
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.grey.shade50,
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          item.tenNguoiDanhGia,
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 13,
                          ),
                        ),
                        Row(
                          children: [
                            const Icon(Icons.star, size: 14, color: Colors.amber),
                            const SizedBox(width: 2),
                            Text(
                              item.soSaoTongThe.toString(),
                              style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 13,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                    const SizedBox(height: 2),
                    Text(
                      'Dự án: ${item.tenCongViec.isNotEmpty ? item.tenCongViec : 'Hợp đồng'}',
                      style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
                    ),
                    if ((item.nhanXet ?? '').trim().isNotEmpty) ...[
                      const SizedBox(height: 6),
                      Text(
                        item.nhanXet!,
                        style: const TextStyle(fontSize: 13, height: 1.3),
                      ),
                    ],
                  ],
                ),
              );
            }),
          ],
        ],
      ),
    );
  }
}
