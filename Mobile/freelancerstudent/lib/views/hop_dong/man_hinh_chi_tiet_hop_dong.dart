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
import '../../viewmodels/viewmodel_task_cong_viec.dart';
import 'package:flutter/services.dart';
import 'package:url_launcher/url_launcher.dart';
import 'man_hinh_danh_sach_hop_dong.dart';
import 'man_hinh_ban_giao_san_pham.dart';

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

  Future<void> _nghiemThu(HopDongModel hd) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      _thongBao('Bạn cần đăng nhập.');
      return;
    }

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Nghiệm thu hợp đồng'),
        content: const Text(
          'Bạn xác nhận nghiệm thu và chuyển hợp đồng sang trạng thái Đã hoàn thành (chờ thanh toán giải ngân)?',
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
            child: const Text('Xác nhận nghiệm thu'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    if (hd.giaiDoans.isNotEmpty &&
        hd.giaiDoans.any((g) => g.trangThai != 'Dat' && g.trangThai != 'DaNghiemThu')) {
      _thongBao('Vui lòng đánh giá Đạt tất cả các mốc giai đoạn trước khi nghiệm thu hoàn thành hợp đồng.');
      return;
    }

    setState(() => _dangXuLy = true);
    final vm = context.read<HopDongViewModel>();
    final ok = await vm.nghiemThuHopDong(hd.maHD, maUser: user.userId);
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (ok) {
      _thongBao('Đã nghiệm thu hợp đồng thành công! Hợp đồng chuyển sang trạng thái Đã hoàn thành.');
      await _tai();
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Nghiệm thu thất bại. Vui lòng thử lại.');
    }
  }

  Future<void> _danhGiaGiaiDoan(HopDongModel hd, GiaiDoanHopDongModel gd, String trangThai) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      _thongBao('Bạn cần đăng nhập.');
      return;
    }

    setState(() => _dangXuLy = true);
    final vm = context.read<TaskViewModel>();
    final loi = await vm.danhGiaGiaiDoan(
      maHD: hd.maHD,
      maUser: user.userId,
      maGiaiDoan: gd.maGiaiDoan,
      trangThai: trangThai,
    );
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (loi == null) {
      _thongBao(trangThai == 'Dat'
          ? 'Đã xác nhận mốc "${gd.tenGiaiDoan}" là Đạt.'
          : 'Đã đánh dấu mốc "${gd.tenGiaiDoan}" là Chưa đạt.');
      await _tai();
    } else {
      _thongBao(loi);
    }
  }

  String _lamSachMoTaGiaiDoan(String? moTa) {
    if (moTa == null || moTa.isEmpty) return '';
    var s = moTa.replaceAll(RegExp(r'\[Giải ngân:[^\]]*\]\s*'), '').trim();
    s = s.replaceAll(RegExp(r'\[LINK_SAN_PHAM\]:[^\n]*'), '').trim();
    return s;
  }

  String? _trichXuatLink(String? moTa) {
    if (moTa == null || moTa.isEmpty) return null;
    final match = RegExp(r'https?://[^\s]+').firstMatch(moTa);
    return match?.group(0);
  }

  Future<void> _moLink(String urlStr) async {
    final clean = urlStr.trim();
    if (clean.isEmpty) return;
    Uri? uri = Uri.tryParse(clean);
    if (uri != null && !uri.hasScheme) {
      uri = Uri.tryParse('https://$clean');
    }
    if (uri != null) {
      try {
        final launched =
            await launchUrl(uri, mode: LaunchMode.externalApplication);
        if (!launched) {
          await Clipboard.setData(ClipboardData(text: clean));
          _thongBao('Đã sao chép liên kết vào bộ nhớ tạm.');
        }
      } catch (_) {
        await Clipboard.setData(ClipboardData(text: clean));
        _thongBao('Đã sao chép liên kết vào bộ nhớ tạm.');
      }
    } else {
      await Clipboard.setData(ClipboardData(text: clean));
      _thongBao('Đã sao chép liên kết vào bộ nhớ tạm.');
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
              const SizedBox(height: 16),
              const Row(
                children: [
                  Icon(Icons.checklist_rounded, color: AppColors.primary, size: 20),
                  SizedBox(width: 8),
                  Text(
                    'Lộ trình các mốc bàn giao',
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              ...hd.giaiDoans.map(
                (g) {
                  final daDat = g.trangThai == 'Dat' || g.trangThai == 'DaNghiemThu';
                  final moTaSach = _lamSachMoTaGiaiDoan(g.moTa);
                  final linkMoc = (g.linkSanPham != null && g.linkSanPham!.trim().isNotEmpty)
                      ? g.linkSanPham!.trim()
                      : _trichXuatLink(g.moTa);
                  return Card(
                    margin: const EdgeInsets.only(bottom: 10),
                    elevation: 0,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                      side: BorderSide(color: Colors.grey.shade200),
                    ),
                    color: AppColors.surface,
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Expanded(
                                child: Text(
                                  g.tenGiaiDoan,
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 14,
                                    color: AppColors.heading,
                                  ),
                                ),
                              ),
                              Container(
                                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 3),
                                decoration: BoxDecoration(
                                  color: daDat ? AppColors.lightGreenSurface : Colors.orange.withAlpha(25),
                                  borderRadius: BorderRadius.circular(6),
                                ),
                                child: Text(
                                  daDat ? 'Đạt' : 'Chưa đạt',
                                  style: TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.bold,
                                    color: daDat ? AppColors.primary : Colors.orange.shade800,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          if (moTaSach.isNotEmpty) ...[
                            const SizedBox(height: 6),
                            Text(
                              moTaSach,
                              style: const TextStyle(fontSize: 13, color: AppColors.bodyText),
                            ),
                          ],
                          if (linkMoc != null && linkMoc.isNotEmpty) ...[
                            const SizedBox(height: 8),
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                              decoration: BoxDecoration(
                                color: AppColors.primary.withValues(alpha: 0.08),
                                borderRadius: BorderRadius.circular(8),
                                border: Border.all(color: AppColors.primary.withValues(alpha: 0.25)),
                              ),
                              child: Row(
                                children: [
                                  const Icon(Icons.link_rounded, size: 18, color: AppColors.primary),
                                  const SizedBox(width: 8),
                                  Expanded(
                                    child: Text(
                                      'Sản phẩm: $linkMoc',
                                      style: const TextStyle(
                                        fontSize: 12,
                                        fontWeight: FontWeight.w600,
                                        color: AppColors.primary,
                                        decoration: TextDecoration.underline,
                                      ),
                                      maxLines: 1,
                                      overflow: TextOverflow.ellipsis,
                                    ),
                                  ),
                                  const SizedBox(width: 6),
                                  InkWell(
                                    onTap: () => _moLink(linkMoc),
                                    child: Container(
                                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                      decoration: BoxDecoration(
                                        color: AppColors.primary,
                                        borderRadius: BorderRadius.circular(6),
                                      ),
                                      child: const Row(
                                        mainAxisSize: MainAxisSize.min,
                                        children: [
                                          Icon(Icons.open_in_new_rounded, size: 12, color: Colors.white),
                                          SizedBox(width: 4),
                                          Text(
                                            'Xem link',
                                            style: TextStyle(
                                              color: Colors.white,
                                              fontSize: 11,
                                              fontWeight: FontWeight.bold,
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                  const SizedBox(width: 6),
                                  IconButton(
                                    padding: EdgeInsets.zero,
                                    constraints: const BoxConstraints(),
                                    icon: const Icon(Icons.copy_rounded, size: 16),
                                    color: Colors.grey.shade700,
                                    tooltip: 'Sao chép link',
                                    onPressed: () {
                                      Clipboard.setData(ClipboardData(text: linkMoc));
                                      _thongBao('Đã sao chép link sản phẩm.');
                                    },
                                  ),
                                ],
                              ),
                            ),
                          ],
                          if (g.hanChot != null) ...[
                            const SizedBox(height: 4),
                            Text(
                              'Hạn bàn giao: ${_ngay(g.hanChot)}',
                              style: const TextStyle(fontSize: 12, color: AppColors.mutedText),
                            ),
                          ],
                          if (isNTD && hd.trangThai == 'DangThucHien') ...[
                            const SizedBox(height: 8),
                            const Divider(height: 1),
                            const SizedBox(height: 6),
                            Row(
                              mainAxisAlignment: MainAxisAlignment.end,
                              children: [
                                OutlinedButton.icon(
                                  onPressed: _dangXuLy || !daDat
                                      ? null
                                      : () => _danhGiaGiaiDoan(hd, g, 'ChuaDat'),
                                  icon: const Icon(Icons.close, size: 15),
                                  label: const Text('Chưa đạt', style: TextStyle(fontSize: 12)),
                                  style: OutlinedButton.styleFrom(
                                    visualDensity: VisualDensity.compact,
                                    foregroundColor: Colors.red,
                                    side: const BorderSide(color: Colors.red),
                                  ),
                                ),
                                const SizedBox(width: 8),
                                FilledButton.icon(
                                  onPressed: _dangXuLy || daDat
                                      ? null
                                      : () => _danhGiaGiaiDoan(hd, g, 'Dat'),
                                  icon: const Icon(Icons.check, size: 15),
                                  label: const Text('Đạt', style: TextStyle(fontSize: 12)),
                                  style: FilledButton.styleFrom(
                                    visualDensity: VisualDensity.compact,
                                    backgroundColor: AppColors.primary,
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ],
                      ),
                    ),
                  );
                },
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
            // BÀN GIAO VÀ NGHIỆM THU SẢN PHẨM (Tương tự sectionBanGiaoSanPham bên Web)
            if ((laFreelancerCuaHopDong || isNTD) &&
                (hd.trangThai == 'DangThucHien' ||
                    hd.trangThai == 'DaHoanThanh' ||
                    hd.trangThai == 'DaThanhToan')) ...[
              const SizedBox(height: 12),
              Card(
                elevation: 0,
                color: AppColors.surface,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                  side: BorderSide(
                    color: AppColors.primary.withValues(alpha: 0.35),
                  ),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Container(
                            padding: const EdgeInsets.all(8),
                            decoration: BoxDecoration(
                              color: AppColors.primary.withValues(alpha: 0.1),
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: const Icon(
                              Icons.inventory_2_outlined,
                              color: AppColors.primary,
                              size: 24,
                            ),
                          ),
                          const SizedBox(width: 12),
                          const Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  'Bàn giao & Nghiệm thu sản phẩm',
                                  style: TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 15,
                                    color: AppColors.heading,
                                  ),
                                ),
                                SizedBox(height: 3),
                                Text(
                                  'Quản lý các phiên bản bàn giao, yêu cầu chỉnh sửa và nghiệm thu sản phẩm.',
                                  style: TextStyle(
                                    fontSize: 12,
                                    color: AppColors.mutedText,
                                    height: 1.3,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 14),
                      SizedBox(
                        width: double.infinity,
                        child: FilledButton.icon(
                          onPressed: () async {
                            await Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (_) => ManHinhBanGiaoSanPham(
                                  maHD: hd.maHD,
                                  trangThaiHopDong: hd.trangThai,
                                  laFreelancer: laFreelancerCuaHopDong,
                                  laKhachHang: isNTD,
                                ),
                              ),
                            );
                            if (mounted) _tai();
                          },
                          style: FilledButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            padding: const EdgeInsets.symmetric(vertical: 12),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                          ),
                          icon: const Icon(Icons.inventory_2_rounded, size: 18),
                          label: const Text(
                            'Xem bàn giao sản phẩm',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 15,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
            if (hd.trangThai == 'DangThucHien') ...[
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
              if (isNTD) ...[
                const SizedBox(height: 8),
                SizedBox(
                  width: double.infinity,
                  child: FilledButton.icon(
                    onPressed: _dangXuLy ? null : () => _nghiemThu(hd),
                    icon: const Icon(Icons.verified),
                    label: const Text('Nghiệm thu hợp đồng'),
                    style: FilledButton.styleFrom(
                      backgroundColor: Colors.teal,
                      padding: const EdgeInsets.symmetric(vertical: 12),
                    ),
                  ),
                ),
              ],
            ],
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
