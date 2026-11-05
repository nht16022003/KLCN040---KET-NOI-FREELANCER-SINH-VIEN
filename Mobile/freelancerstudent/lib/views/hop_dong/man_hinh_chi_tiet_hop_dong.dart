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
import 'package:intl/intl.dart';
import '../../models/mo_hinh_danh_gia.dart';
import '../../services/dich_vu_danh_gia.dart';
import '../../services/dich_vu_minh_chung.dart';
import '../../services/dich_vu_vi_tien.dart';
import '../danh_gia/man_hinh_danh_gia.dart';

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

    if (dongY) {
      setState(() => _dangXuLy = true);
      final check = await MinhChungService().kiemTraTrangThaiXacThuc(user.userId);
      if (!mounted) return;
      setState(() => _dangXuLy = false);

      final daXacMinh = check.isSuccess && check.data?['daXacMinh'] == true;
      if (!daXacMinh) {
        _thongBao('Tài khoản sinh viên cần hoàn tất xác thực minh chứng Thẻ sinh viên (trạng thái "Đã xác minh") để xác nhận tham gia hợp đồng!');
        return;
      }
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

    setState(() => _dangXuLy = true);
    final viRes = await WalletService().layViTheoUser(user.userId);
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    final soDuKhaDung = viRes.data?.soDuKhaDung ?? 0.0;
    final khongDuTien = soDuKhaDung < hd.sotienkyquy;

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Ký quỹ hợp đồng'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Xác nhận ký quỹ số tiền ${AppFormatters.formatCurrency(hd.sotienkyquy)} từ ví điện tử của bạn để kích hoạt hợp đồng?',
            ),
            const SizedBox(height: 12),
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: khongDuTien ? AppColors.error.withValues(alpha: 0.08) : Colors.grey.shade100,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(
                  color: khongDuTien ? AppColors.error.withValues(alpha: 0.3) : Colors.grey.shade300,
                ),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Số dư khả dụng:', style: TextStyle(fontSize: 13, color: Colors.black54)),
                      Text(
                        AppFormatters.formatCurrency(soDuKhaDung),
                        style: TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.bold,
                          color: khongDuTien ? AppColors.error : AppColors.heading,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Cần ký quỹ:', style: TextStyle(fontSize: 13, color: Colors.black54)),
                      Text(
                        AppFormatters.formatCurrency(hd.sotienkyquy),
                        style: const TextStyle(fontSize: 13, fontWeight: FontWeight.bold, color: AppColors.primary),
                      ),
                    ],
                  ),
                  if (khongDuTien) ...[
                    const SizedBox(height: 8),
                    const Text(
                      'Số dư ví không đủ để ký quỹ. Vui lòng nạp thêm tiền vào ví trước khi tiếp tục.',
                      style: TextStyle(fontSize: 12, color: AppColors.error, fontWeight: FontWeight.w500),
                    ),
                  ],
                ],
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: khongDuTien ? null : () => Navigator.pop(ctx, true),
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

  Future<void> _moHopThoaiThanhToan(HopDongModel hd) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      _thongBao('Bạn cần đăng nhập.');
      return;
    }

    setState(() => _dangXuLy = true);
    final vm = context.read<HopDongViewModel>();
    final thongTin = await vm.taiThongTinThanhToan(hd.maHD, maUser: user.userId);
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (thongTin == null) {
      _thongBao(vm.thongBaoLoi ?? 'Không thể tải thông tin giải ngân thanh toán.');
      return;
    }

    final xacNhan = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) {
        return Padding(
          padding: const EdgeInsets.all(20),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text(
                    'Xác nhận giải ngân thù lao',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () => Navigator.pop(ctx, false),
                  ),
                ],
              ),
              Text(
                'Hợp đồng: ${hd.maHD} · Freelancer: ${hd.tenFreelancer}',
                style: const TextStyle(fontSize: 13, color: AppColors.mutedText),
              ),
              const SizedBox(height: 16),
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: Colors.grey.shade50,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: Column(
                  children: [
                    _dongTaiChinh('Tiền ký quỹ (Escrow):', AppFormatters.formatCurrency(thongTin.sotienkyquy)),
                    const SizedBox(height: 8),
                    _dongTaiChinh('Phí nền tảng (nếu có):', AppFormatters.formatCurrency(thongTin.soTienHoaHong)),
                    const Divider(height: 18),
                    _dongTaiChinh(
                      'Freelancer thực nhận:',
                      AppFormatters.formatCurrency(thongTin.soTienFreelancerNhan),
                      isHighlight: true,
                    ),
                    const SizedBox(height: 8),
                    _dongTaiChinh('Số dư đang đóng băng:', AppFormatters.formatCurrency(thongTin.soDuDongBang)),
                  ],
                ),
              ),
              const SizedBox(height: 14),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.blue.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.blue.withValues(alpha: 0.25)),
                ),
                child: const Row(
                  children: [
                    Icon(Icons.info_outline, color: Colors.blue, size: 20),
                    SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        'Xác nhận giải ngân sẽ chuyển khoản tiền ký quỹ đóng băng cho Freelancer và hoàn tất hợp đồng. Thao tác không thể hoàn tác.',
                        style: TextStyle(fontSize: 12, color: Colors.blueGrey, height: 1.3),
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 20),
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: () => Navigator.pop(ctx, true),
                  style: FilledButton.styleFrom(
                    backgroundColor: Colors.teal,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                  icon: const Icon(Icons.check_circle_outline),
                  label: const Text(
                    'Xác nhận giải ngân ngay',
                    style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                  ),
                ),
              ),
            ],
          ),
        );
      },
    );

    if (xacNhan != true || !mounted) return;

    setState(() => _dangXuLy = true);
    final ok = await vm.thanhToanHopDong(hd.maHD, maUser: user.userId);
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (ok) {
      _thongBao('Giải ngân thanh toán thành công! Hợp đồng đã hoàn tất.');
      await _tai();
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Giải ngân thanh toán thất bại.');
    }
  }

  Widget _dongTaiChinh(String nhan, String giaTri, {bool isHighlight = false}) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          nhan,
          style: TextStyle(
            fontSize: isHighlight ? 14 : 13,
            fontWeight: isHighlight ? FontWeight.bold : FontWeight.normal,
            color: isHighlight ? AppColors.heading : Colors.black54,
          ),
        ),
        Text(
          giaTri,
          style: TextStyle(
            fontSize: isHighlight ? 15 : 13,
            fontWeight: FontWeight.bold,
            color: isHighlight ? Colors.teal : AppColors.heading,
          ),
        ),
      ],
    );
  }

  Future<void> _xoaHopDong(HopDongModel hd) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      _thongBao('Bạn cần đăng nhập.');
      return;
    }

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Xác nhận xóa hợp đồng'),
        content: Text(
          'Bạn chắc chắn muốn xóa hợp đồng "${hd.maHD}" khỏi hệ thống? Thao tác này không thể hoàn tác.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(backgroundColor: AppColors.error),
            child: const Text('Xóa hợp đồng'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    setState(() => _dangXuLy = true);
    final vm = context.read<HopDongViewModel>();
    final ok = await vm.xoaHopDong(hd.maHD, maUserNTD: user.userId);
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (ok) {
      _thongBao('Đã xóa hợp đồng thành công.');
      Navigator.pop(context);
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Không thể xóa hợp đồng.');
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

    String? lyDo;
    if (trangThai == 'ChuaDat') {
      final lyDoController = TextEditingController();
      final formKeyLyDo = GlobalKey<FormState>();
      final ok = await showDialog<bool>(
        context: context,
        barrierDismissible: false,
        builder: (ctx) => AlertDialog(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          title: Row(
            children: [
              Icon(Icons.report_problem_outlined, color: Colors.orange.shade800),
              const SizedBox(width: 8),
              const Text('Đánh giá Chưa đạt', style: TextStyle(fontWeight: FontWeight.bold)),
            ],
          ),
          content: Form(
            key: formKeyLyDo,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Vui lòng nêu rõ lý do mốc "${gd.tenGiaiDoan}" chưa đạt yêu cầu để Freelancer chỉnh sửa:'),
                const SizedBox(height: 12),
                TextFormField(
                  controller: lyDoController,
                  maxLines: 3,
                  autofocus: true,
                  decoration: const InputDecoration(
                    labelText: 'Lý do đánh giá chưa đạt *',
                    hintText: 'Ví dụ: Thiếu tài liệu hướng dẫn, chức năng X bị lỗi...',
                    border: OutlineInputBorder(),
                  ),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Vui lòng nhập lý do chưa đạt';
                    }
                    return null;
                  },
                ),
              ],
            ),
          ),
          actions: [
            OutlinedButton(
              onPressed: () => Navigator.of(ctx).pop(false),
              child: const Text('Hủy'),
            ),
            FilledButton(
              onPressed: () {
                if (formKeyLyDo.currentState!.validate()) {
                  Navigator.of(ctx).pop(true);
                }
              },
              style: FilledButton.styleFrom(backgroundColor: Colors.orange.shade800),
              child: const Text('Xác nhận Chưa đạt'),
            ),
          ],
        ),
      );
      if (ok != true || !mounted) return;
      lyDo = lyDoController.text.trim();
    }

    setState(() => _dangXuLy = true);
    final vm = context.read<TaskViewModel>();
    final loi = await vm.danhGiaGiaiDoan(
      maHD: hd.maHD,
      maUser: user.userId,
      maGiaiDoan: gd.maGiaiDoan,
      trangThai: trangThai,
      lyDo: lyDo,
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

  /// Freelancer cập nhật lại bàn giao cho một mốc tiến độ
  Future<void> _hienFormCapNhatMoc(HopDongModel hd, GiaiDoanHopDongModel gd) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      _thongBao('Bạn cần đăng nhập.');
      return;
    }

    final linkController = TextEditingController(text: gd.linkSanPham ?? '');
    final moTaClean = (gd.moTa ?? '').replaceAll(RegExp(r'\[Lý do chưa đạt: [^\]]+\]\s*'), '').trim();
    final moTaController = TextEditingController(text: moTaClean);
    final formKey = GlobalKey<FormState>();

    final xacNhan = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) => Padding(
        padding: EdgeInsets.only(
          left: 20,
          right: 20,
          top: 20,
          bottom: MediaQuery.of(ctx).viewInsets.bottom + 20,
        ),
        child: Form(
          key: formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  const Icon(Icons.sync_problem, color: AppColors.primary),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      'Cập nhật bàn giao: ${gd.tenGiaiDoan}',
                      style: const TextStyle(
                        fontSize: 17,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () => Navigator.pop(ctx, false),
                  ),
                ],
              ),
              if ((gd.lyDoChuaDat ?? '').isNotEmpty) ...[
                const SizedBox(height: 10),
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: Colors.red.shade50,
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(color: Colors.red.shade300),
                  ),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Icon(Icons.error_outline, color: Colors.red.shade700, size: 20),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'Lý do Nhà tuyển dụng đánh giá Chưa đạt:',
                              style: TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 13,
                                color: Colors.red.shade800,
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text(
                              gd.lyDoChuaDat!,
                              style: TextStyle(fontSize: 13, color: Colors.red.shade900),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
              const SizedBox(height: 14),
              TextFormField(
                controller: linkController,
                decoration: const InputDecoration(
                  labelText: 'Link sản phẩm / tài liệu mốc *',
                  hintText: 'https://github.com/... hoặc Drive link',
                  prefixIcon: Icon(Icons.link),
                  border: OutlineInputBorder(),
                ),
                keyboardType: TextInputType.url,
                validator: (v) {
                  if (v == null || v.trim().isEmpty) {
                    return 'Vui lòng nhập link sản phẩm hoặc tài liệu';
                  }
                  if (!v.trim().startsWith('http://') && !v.trim().startsWith('https://')) {
                    return 'Link phải bắt đầu bằng http:// hoặc https://';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: moTaController,
                decoration: const InputDecoration(
                  labelText: 'Ghi chú / Mô tả nội dung cập nhật (Tùy chọn)',
                  hintText: 'Mô tả nội dung đã chỉnh sửa hoặc hoàn thiện thêm...',
                  prefixIcon: Icon(Icons.notes),
                  border: OutlineInputBorder(),
                ),
                maxLines: 3,
              ),
              const SizedBox(height: 18),
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: () {
                    if (formKey.currentState!.validate()) {
                      Navigator.pop(ctx, true);
                    }
                  },
                  icon: const Icon(Icons.send_rounded),
                  label: const Text('Gửi bàn giao lại mốc này'),
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    padding: const EdgeInsets.symmetric(vertical: 13),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );

    if (xacNhan != true || !mounted) return;

    setState(() => _dangXuLy = true);
    final vm = context.read<TaskViewModel>();
    final loi = await vm.capNhatMocBanGiao(
      maHD: hd.maHD,
      maGiaiDoan: gd.maGiaiDoan,
      linkSanPham: linkController.text.trim(),
      moTa: moTaController.text.trim().isNotEmpty ? moTaController.text.trim() : null,
      maUser: user.userId,
    );
    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (loi == null) {
      _thongBao('Đã cập nhật lại mốc bàn giao "${gd.tenGiaiDoan}" thành công! Mốc đã chuyển sang trạng thái Chờ NTD đánh giá.');
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
                  final bool daDat = g.trangThai == 'Dat' || g.trangThai == 'DaNghiemThu';
                  final bool choDanhGia = g.trangThai == 'ChoDanhGia';
                  final bool chuaDat = g.trangThai == 'ChuaDat';
                  final moTaSach = _lamSachMoTaGiaiDoan(g.moTa);
                  final linkMoc = (g.linkSanPham != null && g.linkSanPham!.trim().isNotEmpty)
                      ? g.linkSanPham!.trim()
                      : _trichXuatLink(g.moTa);
                  return Card(
                    margin: const EdgeInsets.only(bottom: 10),
                    elevation: 0,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                      side: BorderSide(
                        color: daDat
                            ? Colors.green.shade300
                            : (chuaDat
                                ? Colors.red.shade300
                                : (choDanhGia ? Colors.blue.shade300 : Colors.grey.shade200)),
                        width: (daDat || chuaDat || choDanhGia) ? 1.5 : 1.0,
                      ),
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
                                  color: daDat
                                      ? AppColors.lightGreenSurface
                                      : (choDanhGia
                                          ? Colors.blue.shade50
                                          : (chuaDat ? Colors.red.shade50 : Colors.orange.withAlpha(25))),
                                  borderRadius: BorderRadius.circular(6),
                                  border: Border.all(
                                    color: daDat
                                        ? AppColors.primary
                                        : (choDanhGia
                                            ? Colors.blue.shade400
                                            : (chuaDat ? Colors.red.shade400 : Colors.orange.shade400)),
                                  ),
                                ),
                                child: Text(
                                  daDat
                                      ? 'ĐÃ ĐẠT'
                                      : (choDanhGia
                                          ? 'CHỜ NTD ĐÁNH GIÁ'
                                          : (chuaDat ? 'CHƯA ĐẠT' : 'CHƯA ĐẠT')),
                                  style: TextStyle(
                                    fontSize: 11,
                                    fontWeight: FontWeight.bold,
                                    color: daDat
                                        ? AppColors.primaryDark
                                        : (choDanhGia
                                            ? Colors.blue.shade900
                                            : (chuaDat ? Colors.red.shade900 : Colors.orange.shade800)),
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
                          // HỘP LÝ DO CHƯA ĐẠT NẾU CÓ
                          if ((g.lyDoChuaDat ?? '').isNotEmpty) ...[
                            const SizedBox(height: 8),
                            Container(
                              padding: const EdgeInsets.all(10),
                              decoration: BoxDecoration(
                                color: Colors.red.shade50,
                                borderRadius: BorderRadius.circular(8),
                                border: Border.all(color: Colors.red.shade200),
                              ),
                              child: Row(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Icon(Icons.info_outline, color: Colors.red.shade700, size: 16),
                                  const SizedBox(width: 8),
                                  Expanded(
                                    child: RichText(
                                      text: TextSpan(
                                        children: [
                                          TextSpan(
                                            text: 'Lý do Chưa đạt: ',
                                            style: TextStyle(
                                              fontWeight: FontWeight.bold,
                                              color: Colors.red.shade900,
                                              fontSize: 12,
                                            ),
                                          ),
                                          TextSpan(
                                            text: g.lyDoChuaDat!,
                                            style: TextStyle(
                                              color: Colors.red.shade800,
                                              fontSize: 12,
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                ],
                              ),
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
                          // NÚT CẬP NHẬT LẠI BÀN GIAO CHO FREELANCER KHI CHƯA ĐẠT HOẶC CẦN NỘP
                          if (laFreelancerCuaHopDong && hd.trangThai == 'DangThucHien' && !daDat) ...[
                            const SizedBox(height: 8),
                            SizedBox(
                              width: double.infinity,
                              child: OutlinedButton.icon(
                                onPressed: _dangXuLy ? null : () => _hienFormCapNhatMoc(hd, g),
                                icon: Icon(
                                  chuaDat ? Icons.sync_problem : Icons.upload_file,
                                  size: 15,
                                  color: chuaDat ? Colors.red.shade700 : AppColors.primary,
                                ),
                                label: Text(
                                  chuaDat
                                      ? 'Cập nhật lại bàn giao mốc này'
                                      : (choDanhGia
                                          ? 'Chỉnh sửa lại link bàn giao'
                                          : 'Bàn giao mốc này'),
                                  style: TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.bold,
                                    color: chuaDat ? Colors.red.shade800 : AppColors.primary,
                                  ),
                                ),
                                style: OutlinedButton.styleFrom(
                                  visualDensity: VisualDensity.compact,
                                  side: BorderSide(
                                    color: chuaDat ? Colors.red.shade400 : AppColors.primary,
                                  ),
                                  backgroundColor: chuaDat ? Colors.red.shade50 : AppColors.surface,
                                ),
                              ),
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
            // GIẢI NGÂN THÙ LAO (KHI HỢP ĐỒNG ĐÃ ĐƯỢC NGHIỆM THU HOÀN THÀNH)
            if (hd.trangThai == 'DaHoanThanh' && isNTD) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: Colors.teal.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.teal.withValues(alpha: 0.35)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Row(
                      children: [
                        Icon(Icons.check_circle_outline, color: Colors.teal, size: 22),
                        SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            'Hợp đồng đã hoàn thành nghiệm thu',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 15,
                              color: Colors.teal,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 6),
                    const Text(
                      'Tất cả sản phẩm đã được chấp thuận. Vui lòng giải ngân thù lao từ khoản ký quỹ cho Freelancer để hoàn tất hợp đồng.',
                      style: TextStyle(fontSize: 13, color: AppColors.bodyText, height: 1.3),
                    ),
                    const SizedBox(height: 14),
                    SizedBox(
                      width: double.infinity,
                      child: FilledButton.icon(
                        onPressed: _dangXuLy ? null : () => _moHopThoaiThanhToan(hd),
                        style: FilledButton.styleFrom(
                          backgroundColor: Colors.teal,
                          padding: const EdgeInsets.symmetric(vertical: 12),
                        ),
                        icon: const Icon(Icons.payments_outlined),
                        label: Text(
                          _dangXuLy ? 'Đang xử lý...' : 'Giải ngân thù lao cho Freelancer',
                          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
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
              const SizedBox(height: 8),
              SizedBox(
                width: double.infinity,
                child: OutlinedButton.icon(
                  onPressed: _dangXuLy ? null : () => _xoaHopDong(hd),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppColors.error,
                    side: const BorderSide(color: AppColors.error),
                    padding: const EdgeInsets.symmetric(vertical: 12),
                  ),
                  icon: const Icon(Icons.delete_outline, size: 18),
                  label: const Text('Xóa dự thảo hợp đồng này'),
                ),
              ),
            ],
            // ĐÁNH GIÁ ĐỐI TÁC (Khi hợp đồng hoàn thành hoặc đã thanh toán)
            if (hd.trangThai == 'DaHoanThanh' || hd.trangThai == 'DaThanhToan') ...[
              const SizedBox(height: 12),
              _DanhGiaHopDongSection(
                hd: hd,
                isNTD: isNTD,
                laFreelancer: laFreelancerCuaHopDong,
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
                      if (hd.trangThai == 'DangThucHien' && !hd.tatCaMocDaDat) ...[
                        Container(
                          margin: const EdgeInsets.only(bottom: 12),
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: Colors.amber.shade50,
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(color: Colors.amber.shade400),
                          ),
                          child: Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Icon(Icons.warning_amber_rounded, color: Colors.amber.shade900, size: 20),
                              const SizedBox(width: 8),
                              Expanded(
                                child: Text(
                                  'Chưa đủ điều kiện bàn giao sản phẩm: Toàn bộ ${hd.giaiDoans.length} mốc tiến độ của dự án phải được Nhà tuyển dụng đánh giá Đạt trước khi bàn giao sản phẩm hoàn chỉnh.',
                                  style: TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.w600,
                                    color: Colors.amber.shade900,
                                    height: 1.3,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
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
                                  tatCaMocDaDat: hd.tatCaMocDaDat,
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

    final laNtdChuHopDong = hd != null &&
        user != null &&
        (user.role == 'NhaTuyenDung' ||
            user.role == 'Employer' ||
            hd.maUserNhaTuyenDung == user.userId);

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Chi tiết hợp đồng'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
        actions: [
          if (hd != null &&
              laNtdChuHopDong &&
              (hd.trangThai == 'DaHuy' ||
                  hd.trangThai == 'TuChoi' ||
                  hd.trangThai == 'DuThao' ||
                  hd.trangThai == 'ChoXacNhan'))
            IconButton(
              icon: const Icon(Icons.delete_outline, color: AppColors.error),
              tooltip: 'Xóa hợp đồng',
              onPressed: _dangXuLy ? null : () => _xoaHopDong(hd),
            ),
        ],
      ),
      body: noiDung,
    );
  }
}



class _DanhGiaHopDongSection extends StatefulWidget {
  final HopDongModel hd;
  final bool isNTD;
  final bool laFreelancer;

  const _DanhGiaHopDongSection({
    required this.hd,
    required this.isNTD,
    required this.laFreelancer,
  });

  @override
  State<_DanhGiaHopDongSection> createState() => _DanhGiaHopDongSectionState();
}

class _DanhGiaHopDongSectionState extends State<_DanhGiaHopDongSection> {
  final DanhGiaService _service = DanhGiaService();
  bool _loading = true;
  FormDanhGiaModel? _formInfo;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadRatingInfo();
  }

  Future<void> _loadRatingInfo() async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      setState(() => _loading = false);
      return;
    }
    setState(() {
      _loading = true;
      _error = null;
    });

    final res = await _service.layFormDanhGia(
      maHD: widget.hd.maHD,
      maUser: user.userId,
    );

    if (mounted) {
      setState(() {
        _loading = false;
        if (res.isSuccess) {
          _formInfo = res.data;
        } else {
          _error = res.message;
        }
      });
    }
  }

  void _openRatingSheet() {
    final info = _formInfo;
    final currentReview = info?.danhGiaHienTai;

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
      builder: (ctx) => Padding(
        padding: EdgeInsets.only(
          bottom: MediaQuery.of(ctx).viewInsets.bottom,
        ),
        child: FormDanhGiaWidget(
          maHD: widget.hd.maHD,
          tenCongViec: widget.hd.tenCongViec,
          tenDoiTac: widget.isNTD
              ? (widget.hd.tenFreelancer.trim().isNotEmpty
                  ? widget.hd.tenFreelancer.trim()
                  : 'Freelancer')
              : ((widget.hd.tenNhaTuyenDung ?? '').trim().isNotEmpty
                  ? widget.hd.tenNhaTuyenDung!.trim()
                  : 'Nhà tuyển dụng'),
          vaiTroDoiTac: widget.isNTD ? 'FreelancerStudent' : 'Nhà tuyển dụng',
          danhGiaBanDau: currentReview,
          onThanhCong: () {
            Navigator.pop(ctx);
            _loadRatingInfo();
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Lưu đánh giá thành công!')),
            );
          },
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    if (_loading) {
      return Card(
        elevation: 0,
        color: AppColors.surface,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
          side: BorderSide(color: AppColors.primary.withValues(alpha: 0.35)),
        ),
        child: const Padding(
          padding: EdgeInsets.all(20),
          child: Center(child: CircularProgressIndicator()),
        ),
      );
    }

    if (_error != null && _formInfo == null) {
      return const SizedBox.shrink();
    }

    final info = _formInfo;
    final review = info?.danhGiaHienTai;
    final canRate = info?.coTheDanhGia == true;
    final canEdit = review?.coTheChinhSua == true;

    return Card(
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
                    color: Colors.amber.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: const Icon(
                    Icons.star_rounded,
                    color: Colors.amber,
                    size: 24,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'Đánh giá & Nhận xét đối tác',
                        style: TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 15,
                          color: AppColors.heading,
                        ),
                      ),
                      const SizedBox(height: 3),
                      Text(
                        review != null
                            ? 'Bạn đã gửi đánh giá cho đối tác trên hợp đồng này.'
                            : 'Chia sẻ trải nghiệm làm việc cùng đối tác sau khi hoàn tất hợp đồng.',
                        style: const TextStyle(
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

            // Đánh giá của mình đã gửi
            if (review != null) ...[
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.grey.shade50,
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text(
                          'Đánh giá của bạn:',
                          style: TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 13,
                          ),
                        ),
                        Row(
                          children: [
                            Row(
                              children: List.generate(
                                5,
                                (i) => Icon(
                                  i < review.soSaoTongThe
                                      ? Icons.star
                                      : Icons.star_border,
                                  size: 16,
                                  color: Colors.amber,
                                ),
                              ),
                            ),
                            const SizedBox(width: 4),
                            Text(
                              '(${review.soSaoTongThe}/5)',
                              style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 13,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                    if (review.ngayDanhGia != null) ...[
                      const SizedBox(height: 2),
                      Text(
                        'Ngày gửi: ${DateFormat("dd/MM/yyyy HH:mm").format(review.ngayDanhGia!)}',
                        style: TextStyle(
                          fontSize: 11,
                          color: Colors.grey.shade600,
                        ),
                      ),
                    ],
                    if ((review.nhanXet ?? '').trim().isNotEmpty) ...[
                      const SizedBox(height: 6),
                      Text(
                        '"${review.nhanXet}"',
                        style: const TextStyle(
                          fontStyle: FontStyle.italic,
                          fontSize: 13,
                          color: AppColors.bodyText,
                        ),
                      ),
                    ],
                  ],
                ),
              ),
              if (canEdit) ...[
                const SizedBox(height: 10),
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton.icon(
                    onPressed: _openRatingSheet,
                    icon: const Icon(Icons.edit, size: 16),
                    label: const Text('Chỉnh sửa đánh giá'),
                    style: OutlinedButton.styleFrom(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                  ),
                ),
              ],
            ] else if (canRate) ...[
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: _openRatingSheet,
                  style: FilledButton.styleFrom(
                    backgroundColor: Colors.amber.shade700,
                    padding: const EdgeInsets.symmetric(vertical: 12),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(8),
                    ),
                  ),
                  icon: const Icon(Icons.rate_review, size: 18),
                  label: const Text(
                    'Gửi đánh giá đối tác',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 15,
                    ),
                  ),
                ),
              ),
            ] else ...[
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.grey.shade100,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: const Text(
                  'Đã hết thời hạn gửi đánh giá (quá 30 ngày kể từ khi hoàn tất).',
                  style: TextStyle(fontSize: 12, color: Colors.black54),
                  textAlign: TextAlign.center,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
