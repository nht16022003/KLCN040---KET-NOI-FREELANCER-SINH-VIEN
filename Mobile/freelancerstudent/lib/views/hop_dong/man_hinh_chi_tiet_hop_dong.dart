import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_hop_dong.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_hop_dong.dart';
import '../chat/man_hinh_chi_tiet_chat.dart';
import '../tien_do/man_hinh_tien_do.dart';
import '../employer/man_hinh_tao_hop_dong.dart';
import '../../models/mo_hinh_ung_tuyen.dart';
import 'man_hinh_danh_sach_hop_dong.dart';

/// Chi tiết hợp đồng của Freelancer: `GET api/HopDong/chi-tiet/{maHD}`,
/// xác nhận / từ chối: `POST api/HopDong/xac-nhan?maUserFreelancer=`.
class HopDongDetailScreen extends StatefulWidget {
  final String maHD;

  const HopDongDetailScreen({super.key, required this.maHD});

  @override
  State<HopDongDetailScreen> createState() => _HopDongDetailScreenState();
}

class _HopDongDetailScreenState extends State<HopDongDetailScreen> {
  bool _dangXuLy = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _tai());
  }

  Future<void> _tai() => context.read<HopDongViewModel>().taiChiTietHopDong(
        widget.maHD,
      );

  void _thongBao(String noiDung) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(noiDung)),
    );
  }

  Future<void> _xacNhan(HopDongModel hd, bool dongY) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      _thongBao('Bạn cần đăng nhập.');
      return;
    }

    String? lyDo;
    if (!dongY) {
      final controller = TextEditingController();
      final xacNhanTuChoi = await showDialog<bool>(
        context: context,
        builder: (ctx) => AlertDialog(
          title: const Text('Từ chối hợp đồng'),
          content: TextField(
            controller: controller,
            maxLines: 3,
            decoration: const InputDecoration(
              labelText: 'Lý do từ chối',
              border: OutlineInputBorder(),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx, false),
              child: const Text('Hủy'),
            ),
            FilledButton(
              onPressed: () => Navigator.pop(ctx, true),
              child: const Text('Từ chối'),
            ),
          ],
        ),
      );
      lyDo = controller.text.trim();
      controller.dispose();
      if (xacNhanTuChoi != true) return;
    }

    if (!mounted) return;
    setState(() => _dangXuLy = true);
    final vm = context.read<HopDongViewModel>();
    final ok = await vm.xacNhanHopDong(
      hd.maHD,
      dongY,
      maUserFreelancer: user.userId,
      lyDoTuChoi: lyDo,
    );
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (ok) {
      _thongBao(dongY ? 'Đã xác nhận hợp đồng.' : 'Đã từ chối hợp đồng.');
      await _tai();
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Không thể xử lý hợp đồng.');
    }
  }

  Future<void> _kyQuy(HopDongModel hd) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      _thongBao('Bạn cần đăng nhập.');
      return;
    }

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Ký quỹ hợp đồng'),
        content: Text(
          'Bạn xác nhận ký quỹ số tiền ${AppFormatters.formatCurrency(hd.sotienkyquy)} từ ví điện tử của mình để bắt đầu hợp đồng?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(
              backgroundColor: AppColors.primary,
            ),
            child: const Text('Xác nhận ký quỹ'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    setState(() => _dangXuLy = true);
    final vm = context.read<HopDongViewModel>();
    final ok = await vm.kyQuyHopDong(hd.maHD, maUserNhaTuyenDung: user.userId);
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (ok) {
      _thongBao('Ký quỹ hợp đồng thành công! Dự án chuyển sang trạng thái Đang thực hiện.');
      await _tai();
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Ký quỹ thất bại. Vui lòng kiểm tra lại số dư ví.');
    }
  }

  Widget _dong(String nhan, String? giaTri) {
    if (giaTri == null || giaTri.trim().isEmpty) {
      return const SizedBox.shrink();
    }
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 130,
            child: Text(nhan, style: const TextStyle(color: Colors.black54)),
          ),
          Expanded(child: Text(giaTri)),
        ],
      ),
    );
  }

  String? _ngay(DateTime? d) =>
      d == null ? null : AppFormatters.formatDateTime(d).split(' ').last;

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<HopDongViewModel>();
    final user = context.watch<AuthViewModel>().currentUser;
    final hd = vm.hopDongHienTai?.maHD == widget.maHD
        ? vm.hopDongHienTai
        : null;

    Widget noiDung;
    if (hd == null) {
      noiDung = Center(
        child: vm.dangTai
            ? const CircularProgressIndicator()
            : Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(vm.thongBaoLoi ?? 'Không tải được hợp đồng.'),
                    const SizedBox(height: 12),
                    FilledButton(
                      onPressed: _tai,
                      child: const Text('Thử lại'),
                    ),
                  ],
                ),
              ),
      );
    } else {
      final mau = mauTrangThaiHopDong(hd.trangThai);
      final laFreelancerCuaHopDong =
          user != null && hd.maUserFreelancer == user.userId;
      final isNTD = user != null &&
          (user.role == 'NhaTuyenDung' ||
              user.role == 'Employer' ||
              hd.maUserNhaTuyenDung == user.userId);
      final choXacNhan = hd.trangThai == 'ChoXacNhan';
      final choKyQuy = hd.trangThai == 'ChoKyQuy';

      noiDung = RefreshIndicator(
        onRefresh: _tai,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            Text(
              hd.tenCongViec,
              style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Chip(
              label: Text(
                nhanTrangThaiHopDong(hd.trangThai),
                style: TextStyle(color: mau, fontWeight: FontWeight.w600),
              ),
              backgroundColor: mau.withValues(alpha: 0.12),
              side: BorderSide.none,
            ),
            if (hd.daBiTuChoi) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: AppColors.error.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColors.error.withValues(alpha: 0.3)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Row(
                      children: [
                        Icon(Icons.cancel_outlined, color: AppColors.error, size: 20),
                        SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            'Dự thảo hợp đồng đã bị Freelancer từ chối',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 15,
                              color: AppColors.error,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Lý do từ chối: ${(hd.lyDoTuChoi ?? '').trim().isNotEmpty ? hd.lyDoTuChoi! : 'Freelancer không cung cấp lý do cụ thể.'}',
                      style: const TextStyle(
                        color: AppColors.heading,
                        fontSize: 14,
                        height: 1.4,
                      ),
                    ),
                  ],
                ),
              ),
            ],
            const Divider(height: 24),
            _dong('Mã hợp đồng', hd.maHD),
            _dong('Mã công việc', hd.maJob),
            _dong('Nhà tuyển dụng', hd.tenNhaTuyenDung),
            _dong('Email NTD', hd.emailNhaTuyenDung),
            _dong('SĐT NTD', hd.sdtNhaTuyenDung),
            _dong('Freelancer', hd.tenFreelancer),
            _dong('Ngày bắt đầu', _ngay(hd.ngayBatDau)),
            _dong('Ngày kết thúc', _ngay(hd.ngayKetThuc)),
            _dong(
              'Tiền ký quỹ',
              AppFormatters.formatCurrency(hd.sotienkyquy),
            ),
            _dong('Hình thức', hd.hinhthuclamviec),
            _dong('Mô tả công việc', hd.moTaCongViec),
            _dong('Điều khoản', hd.dieuKhoan),
            if (hd.giaiDoans.isNotEmpty) ...[
              const SizedBox(height: 12),
              const Text(
                'Giai đoạn thanh toán',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
              ...hd.giaiDoans.map(
                (g) => ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: Text(g.tenGiaiDoan),
                  subtitle: Text(
                    [
                      if ((g.moTa ?? '').isNotEmpty) g.moTa!,
                      if (g.hanChot != null) 'Hạn: ${_ngay(g.hanChot)}',
                    ].join('\n'),
                  ),
                  trailing: Text(g.trangThai),
                ),
              ),
            ],
            const SizedBox(height: 16),
            if (choXacNhan && laFreelancerCuaHopDong)
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: _dangXuLy ? null : () => _xacNhan(hd, false),
                      child: const Text('Từ chối'),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: FilledButton(
                      onPressed: _dangXuLy ? null : () => _xacNhan(hd, true),
                      child: Text(_dangXuLy ? 'Đang xử lý...' : 'Xác nhận'),
                    ),
                  ),
                ],
              ),
            if (choKyQuy && isNTD)
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: _dangXuLy ? null : () => _kyQuy(hd),
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    padding: const EdgeInsets.symmetric(vertical: 12),
                  ),
                  icon: const Icon(Icons.account_balance_wallet_outlined),
                  label: Text(
                    _dangXuLy
                        ? 'Đang xử lý...'
                        : 'Ký quỹ hợp đồng (${AppFormatters.formatCurrency(hd.sotienkyquy)})',
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
              ),
            if (hd.daBiTuChoi && isNTD) ...[
              const SizedBox(height: 12),
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: () async {
                    final applicant = ApplicationModel(
                      id: hd.maUngTuyen?.toString() ?? '',
                      jobId: hd.maJob,
                      applicantId: hd.maFreelancerStudent.toString(),
                      tenUser: hd.tenFreelancer,
                      emailUser: hd.emailFreelancer,
                      sdtUser: hd.sdtFreelancer,
                      tieude: hd.tenCongViec,
                      proposedBudget: hd.sotienkyquy,
                      status: 'ChapNhan',
                    );
                    await Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => TaoHopDongScreen(applicant: applicant),
                      ),
                    );
                    if (mounted) _tai();
                  },
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    padding: const EdgeInsets.symmetric(vertical: 12),
                  ),
                  icon: const Icon(Icons.refresh),
                  label: const Text(
                    'Tạo lại hợp đồng',
                    style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                  ),
                ),
              ),
            ],
            if (hd.trangThai == 'DangThucHien')
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => TienDoScreen(
                         maHD: hd.maHD,
                         tenCongViec: hd.tenCongViec,
                      ),
                    ),
                  ),
                  icon: const Icon(Icons.timeline),
                  label: Text(
                    laFreelancerCuaHopDong
                        ? 'Cập nhật tiến độ dự án'
                        : 'Theo dõi tiến độ dự án',
                  ),
                ),
              ),
            if (isNTD && hd.maUserFreelancer != null) ...[
              const SizedBox(height: 8),
              SizedBox(
                width: double.infinity,
                child: OutlinedButton.icon(
                  onPressed: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => ChatDetailScreen(
                        receiverId: hd.maUserFreelancer.toString(),
                        receiverName: hd.tenFreelancer,
                        maJob: hd.maJob,
                      ),
                    ),
                  ),
                  icon: const Icon(Icons.chat_bubble_outline),
                  label: const Text('Chat với Freelancer'),
                ),
              ),
            ] else if (hd.maUserNhaTuyenDung != null) ...[
              const SizedBox(height: 8),
              SizedBox(
                width: double.infinity,
                child: OutlinedButton.icon(
                  onPressed: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => ChatDetailScreen(
                        receiverId: hd.maUserNhaTuyenDung.toString(),
                        receiverName: hd.tenNhaTuyenDung,
                        maJob: hd.maJob,
                      ),
                    ),
                  ),
                  icon: const Icon(Icons.chat_bubble_outline),
                  label: const Text('Chat với Nhà tuyển dụng'),
                ),
              ),
            ],
          ],
        ),
      );
    }

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Chi tiết hợp đồng'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: noiDung,
    );
  }
}
