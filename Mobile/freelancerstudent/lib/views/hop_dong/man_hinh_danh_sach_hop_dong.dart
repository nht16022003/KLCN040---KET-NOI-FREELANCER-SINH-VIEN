import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_hop_dong.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_ho_so_freelancer.dart';
import '../../viewmodels/viewmodel_hop_dong.dart';
import 'man_hinh_chi_tiet_hop_dong.dart';

import '../../services/dich_vu_nha_tuyen_dung.dart';

/// Nhãn hiển thị cho trạng thái hợp đồng do Backend trả về (không tự thêm trạng thái mới).
String nhanTrangThaiHopDong(String trangThai) {
  switch (trangThai) {
    case 'DuThao':
      return 'Dự thảo';
    case 'ChoXacNhan':
      return 'Chờ Freelancer xác nhận';
    case 'ChoKyQuy':
      return 'Chờ ký quỹ';
    case 'DangThucHien':
      return 'Đang thực hiện';
    case 'DaHuy':
      return 'Đã hủy';
    default:
      return trangThai;
  }
}

Color mauTrangThaiHopDong(String trangThai) {
  switch (trangThai) {
    case 'ChoXacNhan':
      return AppColors.warning;
    case 'ChoKyQuy':
      return AppColors.info;
    case 'DangThucHien':
      return AppColors.success;
    case 'DaHuy':
      return AppColors.error;
    default:
      return AppColors.bodyText;
  }
}

/// Danh sách hợp đồng:
/// - Freelancer: `GET api/HopDong/danh-sach-freelancer/{maFS}`
/// - Nhà tuyển dụng: `GET api/HopDong/danh-sach/{maNTD}`
class HopDongListScreen extends StatefulWidget {
  const HopDongListScreen({super.key});

  @override
  State<HopDongListScreen> createState() => _HopDongListScreenState();
}

class _HopDongListScreenState extends State<HopDongListScreen> {
  String? _loi;
  bool _dangKhoiTao = true;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _tai());
  }

  Future<void> _tai() async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      setState(() {
        _dangKhoiTao = false;
        _loi = 'Bạn cần đăng nhập để xem hợp đồng.';
      });
      return;
    }

    setState(() {
      _dangKhoiTao = true;
      _loi = null;
    });

    final hopDongVM = context.read<HopDongViewModel>();
    final isNTD = user.role == 'NhaTuyenDung' || user.role == 'Employer';

    if (isNTD) {
      // Tìm mã Nhà tuyển dụng
      final ntdService = NhaTuyenDungService();
      final ntdRes = await ntdService.layDanhSachNhaTuyenDung();
      final ntd = ntdRes.data?.firstWhere(
        (item) => item.maUser == user.userId,
        orElse: () => ntdRes.data!.isNotEmpty ? ntdRes.data!.first : throw Exception(),
      );

      final maNTD = ntd?.maNhaTuyenDung;
      if (maNTD != null && maNTD > 0) {
        await hopDongVM.taiDanhSachHopDongNTD(maNTD);
        if (!mounted) return;
        setState(() => _dangKhoiTao = false);
        return;
      }
    }

    if (!mounted) return;

    // Luồng Freelancer Student
    final hoSoVM = context.read<FreelancerProfileViewModel>();
    if (hoSoVM.profile == null) {
      await hoSoVM.loadProfile(user.userId);
    }
    final maFS = hoSoVM.profile?.maFreelancerStudents;
    if (maFS == null || maFS <= 0) {
      if (!mounted) return;
      setState(() {
        _dangKhoiTao = false;
        _loi = isNTD
            ? 'Chưa tìm thấy thông tin Nhà tuyển dụng của tài khoản này.'
            : (hoSoVM.errorMessage ??
                'Không xác định được hồ sơ Freelancer của tài khoản này.');
      });
      return;
    }

    await hopDongVM.taiDanhSachHopDongFreelancer(maFS);
    if (!mounted) return;
    setState(() => _dangKhoiTao = false);
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<HopDongViewModel>();
    final dangTai = _dangKhoiTao || vm.dangTai;
    final loi = _loi ?? vm.thongBaoLoi;
    final ds = vm.danhSachHopDong;

    Widget noiDung;
    if (dangTai && ds.isEmpty) {
      noiDung = const Center(child: CircularProgressIndicator());
    } else if (loi != null && ds.isEmpty) {
      noiDung = Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(loi, textAlign: TextAlign.center),
              const SizedBox(height: 12),
              FilledButton(onPressed: _tai, child: const Text('Thử lại')),
            ],
          ),
        ),
      );
    } else if (ds.isEmpty) {
      noiDung = ListView(
        children: const [
          SizedBox(height: 120),
          Center(child: Text('Bạn chưa có hợp đồng nào.')),
        ],
      );
    } else {
      noiDung = ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: ds.length,
        separatorBuilder: (_, _) => const SizedBox(height: 12),
        itemBuilder: (_, i) => _TheHopDong(
          hopDong: ds[i],
          onTap: () async {
            await Navigator.push(
              context,
              MaterialPageRoute(
                builder: (_) => HopDongDetailScreen(maHD: ds[i].maHD),
              ),
            );
            if (mounted) _tai();
          },
        ),
      );
    }

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Hợp đồng của tôi'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: RefreshIndicator(onRefresh: _tai, child: noiDung),
    );
  }
}

class _TheHopDong extends StatelessWidget {
  final HopDongModel hopDong;
  final VoidCallback onTap;

  const _TheHopDong({required this.hopDong, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final mau = mauTrangThaiHopDong(hopDong.trangThai);
    return Card(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                hopDong.tenCongViec,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 4),
              Text('Mã HĐ: ${hopDong.maHD}'),
              if ((hopDong.tenNhaTuyenDung ?? '').isNotEmpty)
                Text('Nhà tuyển dụng: ${hopDong.tenNhaTuyenDung}'),
              Text(
                'Tiền ký quỹ: ${AppFormatters.formatCurrency(hopDong.sotienkyquy)}',
              ),
              const SizedBox(height: 8),
              Chip(
                label: Text(
                  nhanTrangThaiHopDong(hopDong.trangThai),
                  style: TextStyle(color: mau, fontWeight: FontWeight.w600),
                ),
                backgroundColor: mau.withValues(alpha: 0.12),
                side: BorderSide.none,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
