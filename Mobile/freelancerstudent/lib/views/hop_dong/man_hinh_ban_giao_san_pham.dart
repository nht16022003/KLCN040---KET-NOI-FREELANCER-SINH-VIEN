import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_ban_giao.dart';
import '../../viewmodels/viewmodel_ban_giao.dart';

class ManHinhBanGiaoSanPham extends StatefulWidget {
  final String maHD;
  final String trangThaiHopDong;
  final bool laFreelancer;
  final bool laKhachHang;

  const ManHinhBanGiaoSanPham({
    super.key,
    required this.maHD,
    required this.trangThaiHopDong,
    required this.laFreelancer,
    required this.laKhachHang,
  });

  @override
  State<ManHinhBanGiaoSanPham> createState() => _ManHinhBanGiaoSanPhamState();
}

class _ManHinhBanGiaoSanPhamState extends State<ManHinhBanGiaoSanPham> {
  final _formKey = GlobalKey<FormState>();
  final _fileUrlController = TextEditingController();
  final _linkDemoController = TextEditingController();
  final _moTaController = TextEditingController();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<BanGiaoViewModel>().taiDuLieu(widget.maHD);
    });
  }

  @override
  void dispose() {
    _fileUrlController.dispose();
    _linkDemoController.dispose();
    _moTaController.dispose();
    super.dispose();
  }

  void _thongBao(String msg, {bool isError = false}) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(msg),
        backgroundColor: isError ? AppColors.error : AppColors.primary,
      ),
    );
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

  Future<void> _guiBanGiao() async {
    if (!_formKey.currentState!.validate()) return;

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Xác nhận bàn giao'),
        content: const Text(
          'Bạn chắc chắn muốn gửi phiên bản bàn giao này cho Nhà tuyển dụng kiểm tra?',
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
            child: const Text('Gửi bàn giao'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    final vm = context.read<BanGiaoViewModel>();
    final ok = await vm.taoBanGiao(
      maHD: widget.maHD,
      fileSanPhamUrl: _fileUrlController.text.trim(),
      linkDemo: _linkDemoController.text.trim().isNotEmpty
          ? _linkDemoController.text.trim()
          : null,
      moTaBanGiao: _moTaController.text.trim(),
    );

    if (!mounted) return;
    if (ok) {
      _thongBao('Bàn giao sản phẩm thành công!');
      _fileUrlController.clear();
      _linkDemoController.clear();
      _moTaController.clear();
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Không thể gửi bàn giao sản phẩm.',
          isError: true);
    }
  }

  Future<void> _moHopThoaiYeuCauChinhSua(BanGiaoModel banGiao) async {
    final controller = TextEditingController();
    final formKeyDialog = GlobalKey<FormState>();

    final xacNhan = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) {
        return Padding(
          padding: EdgeInsets.only(
            left: 20,
            right: 20,
            top: 20,
            bottom: MediaQuery.of(ctx).viewInsets.bottom + 20,
          ),
          child: Form(
            key: formKeyDialog,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text(
                      'Yêu cầu chỉnh sửa',
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
                  'Phiên bản ${banGiao.phienBan}',
                  style: const TextStyle(fontSize: 13, color: AppColors.mutedText),
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: controller,
                  maxLines: 4,
                  autofocus: true,
                  decoration: const InputDecoration(
                    labelText: 'Nội dung chi tiết cần chỉnh sửa *',
                    hintText:
                        'Ghi rõ các điểm cần sửa đổi, bổ sung cho Freelancer...',
                    border: OutlineInputBorder(),
                    alignLabelWithHint: true,
                  ),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Vui lòng nhập nội dung yêu cầu chỉnh sửa';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                SizedBox(
                  width: double.infinity,
                  child: FilledButton.icon(
                    onPressed: () {
                      if (formKeyDialog.currentState!.validate()) {
                        Navigator.pop(ctx, true);
                      }
                    },
                    style: FilledButton.styleFrom(
                      backgroundColor: Colors.orange.shade700,
                      padding: const EdgeInsets.symmetric(vertical: 12),
                    ),
                    icon: const Icon(Icons.send_rounded),
                    label: const Text(
                      'Gửi yêu cầu chỉnh sửa',
                      style: TextStyle(fontWeight: FontWeight.bold),
                    ),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );

    if (xacNhan != true || !mounted) return;

    final vm = context.read<BanGiaoViewModel>();
    final ok = await vm.guiYeuCauChinhSua(
      maHD: widget.maHD,
      maBanGiao: banGiao.maBanGiao,
      noiDungYeuCau: controller.text.trim(),
    );

    if (!mounted) return;
    if (ok) {
      _thongBao('Đã gửi yêu cầu chỉnh sửa cho Freelancer.');
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Không thể gửi yêu cầu chỉnh sửa.',
          isError: true);
    }
  }

  Future<void> _xacNhanNghiemThu(BanGiaoModel banGiao) async {
    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Nghiệm thu sản phẩm'),
        content: Text(
          'Bạn xác nhận Phiên bản ${banGiao.phienBan} đạt yêu cầu và đồng ý nghiệm thu hoàn thành?',
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
            child: const Text('Đồng ý nghiệm thu'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    final vm = context.read<BanGiaoViewModel>();
    final ok = await vm.nghiemThu(
      maHD: widget.maHD,
      maBanGiao: banGiao.maBanGiao,
    );

    if (!mounted) return;
    if (ok) {
      _thongBao('Nghiệm thu sản phẩm thành công!');
    } else {
      _thongBao(vm.thongBaoLoi ?? 'Nghiệm thu thất bại.', isError: true);
    }
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<BanGiaoViewModel>();
    final coTheNop = vm.coTheNop(widget.laFreelancer, widget.trangThaiHopDong);
    final coTheDanhGia =
        vm.coTheDanhGia(widget.laKhachHang, widget.trangThaiHopDong);

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Bàn giao sản phẩm',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            Text(
              'Hợp đồng #${widget.maHD}',
              style: const TextStyle(fontSize: 12, color: AppColors.mutedText),
            ),
          ],
        ),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => vm.taiDuLieu(widget.maHD),
            tooltip: 'Làm mới',
          ),
        ],
      ),
      body: vm.dangTai
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: () => vm.taiDuLieu(widget.maHD),
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  // THÔNG TIN HỢP ĐỒNG & TRẠNG THÁI
                  _buildHeaderCard(),

                  // BANNER NẾU ĐÃ HOÀN THÀNH
                  if (widget.trangThaiHopDong == 'DaHoanThanh')
                    _buildCompletedBanner(),

                  const SizedBox(height: 16),

                  // FORM FREELANCER NỘP BÀN GIAO
                  if (coTheNop) ...[
                    _buildSubmissionForm(vm),
                    const SizedBox(height: 16),
                  ],

                  // DANH SÁCH LỊCH SỬ BÀN GIAO
                  _buildDeliveryHistory(vm, coTheDanhGia),
                ],
              ),
            ),
    );
  }

  Widget _buildHeaderCard() {
    final bool isCompleted = widget.trangThaiHopDong == 'DaHoanThanh' ||
        widget.trangThaiHopDong == 'DaThanhToan';
    final statusColor = isCompleted ? AppColors.primary : Colors.blue.shade700;
    final statusText = isCompleted
        ? 'Hợp đồng đã hoàn thành'
        : 'Hợp đồng đang thực hiện';

    return Card(
      elevation: 0,
      color: AppColors.surface,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: Colors.grey.shade200),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: AppColors.primary.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(10),
              ),
              child: const Icon(
                Icons.inventory_2_outlined,
                color: AppColors.primary,
                size: 26,
              ),
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Bàn giao & Nghiệm thu sản phẩm',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Quản lý các phiên bản bàn giao, yêu cầu chỉnh sửa và nghiệm thu sản phẩm.',
                    style: TextStyle(
                      fontSize: 13,
                      color: Colors.grey.shade700,
                      height: 1.3,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: statusColor.withValues(alpha: 0.1),
                      borderRadius: BorderRadius.circular(6),
                    ),
                    child: Text(
                      statusText,
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: statusColor,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildCompletedBanner() {
    return Container(
      margin: const EdgeInsets.only(top: 12),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.lightGreenSurface,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: AppColors.primary.withValues(alpha: 0.3)),
      ),
      child: const Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(Icons.check_circle_outline, color: AppColors.primary, size: 22),
          SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Sản phẩm đã được nghiệm thu hoàn tất!',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                    fontSize: 14,
                  ),
                ),
                SizedBox(height: 4),
                Text(
                  'Sản phẩm bàn giao đã đạt yêu cầu. Hợp đồng sẵn sàng để giải ngân thù lao cho Freelancer.',
                  style: TextStyle(fontSize: 13, color: AppColors.bodyText),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSubmissionForm(BanGiaoViewModel vm) {
    final bool isRevision = vm.moiNhat != null;
    final formTitle = isRevision
        ? 'Nộp lại sản phẩm sau chỉnh sửa'
        : 'Bàn giao sản phẩm lần đầu';

    return Card(
      elevation: 0,
      color: AppColors.surface,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: AppColors.primary.withValues(alpha: 0.4)),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Icon(
                    isRevision ? Icons.published_with_changes : Icons.cloud_upload_outlined,
                    color: AppColors.primary,
                    size: 22,
                  ),
                  const SizedBox(width: 8),
                  Text(
                    formTitle,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                ],
              ),
              const Divider(height: 20),
              // ĐƯỜNG DẪN SẢN PHẨM
              TextFormField(
                controller: _fileUrlController,
                decoration: const InputDecoration(
                  labelText: 'Đường dẫn sản phẩm *',
                  hintText: 'https://drive.google.com/... hoặc GitHub repository',
                  prefixIcon: Icon(Icons.link_rounded),
                  border: OutlineInputBorder(),
                  helperText: 'Nhập link Google Drive, GitHub hoặc nơi lưu trữ sản phẩm',
                ),
                keyboardType: TextInputType.url,
                validator: (v) {
                  if (v == null || v.trim().isEmpty) {
                    return 'Vui lòng nhập đường dẫn sản phẩm';
                  }
                  if (!v.trim().startsWith('http://') &&
                      !v.trim().startsWith('https://')) {
                    return 'Đường dẫn phải bắt đầu bằng http:// hoặc https://';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 14),

              // LINK DEMO
              TextFormField(
                controller: _linkDemoController,
                decoration: const InputDecoration(
                  labelText: 'Link Demo (Tùy chọn)',
                  hintText: 'https://...',
                  prefixIcon: Icon(Icons.play_circle_outline_rounded),
                  border: OutlineInputBorder(),
                ),
                keyboardType: TextInputType.url,
              ),
              const SizedBox(height: 14),

              // MÔ TẢ BÀN GIAO
              TextFormField(
                controller: _moTaController,
                maxLines: 4,
                decoration: const InputDecoration(
                  labelText: 'Mô tả bàn giao *',
                  hintText: 'Mô tả chi tiết những nội dung công việc đã hoàn thành...',
                  border: OutlineInputBorder(),
                  alignLabelWithHint: true,
                ),
                validator: (v) {
                  if (v == null || v.trim().isEmpty) {
                    return 'Vui lòng nhập mô tả bàn giao';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // NÚT GỬI
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: vm.dangXuLy ? null : _guiBanGiao,
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    padding: const EdgeInsets.symmetric(vertical: 12),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(8),
                    ),
                  ),
                  icon: vm.dangXuLy
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(
                            color: Colors.white,
                            strokeWidth: 2,
                          ),
                        )
                      : const Icon(Icons.send_rounded),
                  label: Text(
                    vm.dangXuLy ? 'Đang gửi...' : 'Gửi bàn giao sản phẩm',
                    style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildDeliveryHistory(BanGiaoViewModel vm, bool coTheDanhGia) {
    final list = vm.danhSachBanGiao;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Row(
          children: [
            Icon(Icons.history_rounded, color: AppColors.primary, size: 20),
            SizedBox(width: 8),
            Text(
              'Lịch sử bàn giao sản phẩm',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
                color: AppColors.heading,
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        if (list.isEmpty)
          Container(
            padding: const EdgeInsets.symmetric(vertical: 36, horizontal: 20),
            alignment: Alignment.center,
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.grey.shade200),
            ),
            child: Column(
              children: [
                Icon(Icons.inbox_rounded, size: 48, color: Colors.grey.shade400),
                const SizedBox(height: 10),
                Text(
                  'Chưa có sản phẩm nào được bàn giao.',
                  style: TextStyle(color: Colors.grey.shade600, fontSize: 14),
                ),
              ],
            ),
          )
        else
          ...list.map((item) {
            final isLatest = vm.moiNhat?.maBanGiao == item.maBanGiao;
            final ycs = vm.danhSachYeuCau
                .where((yc) => yc.maBanGiao == item.maBanGiao)
                .toList();

            return _buildDeliveryItemCard(
              item: item,
              isLatest: isLatest,
              changeRequests: ycs,
              canReview: coTheDanhGia && isLatest,
              isProcessing: vm.dangXuLy,
            );
          }),
      ],
    );
  }

  Widget _buildDeliveryItemCard({
    required BanGiaoModel item,
    required bool isLatest,
    required List<YeuCauChinhSuaModel> changeRequests,
    required bool canReview,
    required bool isProcessing,
  }) {
    Color badgeColor;
    String badgeText;

    if (item.trangThai == 'DaNghiemThu') {
      badgeColor = AppColors.primary;
      badgeText = 'Đã nghiệm thu';
    } else if (item.trangThai == 'YeuCauChinhSua') {
      badgeColor = Colors.orange.shade800;
      badgeText = 'Yêu cầu chỉnh sửa';
    } else {
      badgeColor = Colors.blue.shade700;
      badgeText = 'Chờ nghiệm thu';
    }

    return Card(
      margin: const EdgeInsets.only(bottom: 14),
      elevation: 0,
      color: AppColors.surface,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(
          color: isLatest ? AppColors.primary.withValues(alpha: 0.3) : Colors.grey.shade200,
        ),
      ),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // HEADER PHIÊN BẢN
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Phiên bản ${item.phienBan}',
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 15,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      'Ngày nộp: ${AppFormatters.formatDateTime(item.ngayNop)}',
                      style: const TextStyle(
                        fontSize: 12,
                        color: AppColors.mutedText,
                      ),
                    ),
                  ],
                ),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                  decoration: BoxDecoration(
                    color: badgeColor.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(6),
                  ),
                  child: Text(
                    badgeText,
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: badgeColor,
                    ),
                  ),
                ),
              ],
            ),
            const Divider(height: 18),

            // MÔ TẢ
            const Text(
              'Mô tả hoàn thành:',
              style: TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 13,
                color: AppColors.heading,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              item.moTaBanGiao,
              style: const TextStyle(fontSize: 13, color: AppColors.bodyText),
            ),
            const SizedBox(height: 10),

            // SẢN PHẨM URL
            Row(
              children: [
                const Icon(Icons.link_rounded, size: 18, color: AppColors.primary),
                const SizedBox(width: 6),
                Expanded(
                  child: Text(
                    item.fileSanPhamUrl,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontSize: 13,
                      color: AppColors.primary,
                      decoration: TextDecoration.underline,
                    ),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.open_in_new_rounded, size: 18),
                  tooltip: 'Mở sản phẩm',
                  color: AppColors.primary,
                  onPressed: () => _moLink(item.fileSanPhamUrl),
                ),
                IconButton(
                  icon: const Icon(Icons.copy_rounded, size: 16),
                  tooltip: 'Sao chép link',
                  color: Colors.grey.shade600,
                  onPressed: () async {
                    await Clipboard.setData(
                        ClipboardData(text: item.fileSanPhamUrl));
                    _thongBao('Đã sao chép link sản phẩm.');
                  },
                ),
              ],
            ),

            // LINK DEMO (NẾU CÓ)
            if (item.linkDemo != null && item.linkDemo!.trim().isNotEmpty) ...[
              const SizedBox(height: 4),
              Row(
                children: [
                  const Icon(Icons.play_circle_outline_rounded,
                      size: 18, color: Colors.blue),
                  const SizedBox(width: 6),
                  Expanded(
                    child: Text(
                      item.linkDemo!,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontSize: 13,
                        color: Colors.blue,
                        decoration: TextDecoration.underline,
                      ),
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.open_in_new_rounded, size: 18),
                    tooltip: 'Mở link demo',
                    color: Colors.blue,
                    onPressed: () => _moLink(item.linkDemo!),
                  ),
                  IconButton(
                    icon: const Icon(Icons.copy_rounded, size: 16),
                    tooltip: 'Sao chép link',
                    color: Colors.grey.shade600,
                    onPressed: () async {
                      await Clipboard.setData(
                          ClipboardData(text: item.linkDemo!));
                      _thongBao('Đã sao chép link demo.');
                    },
                  ),
                ],
              ),
            ],

            // DANH SÁCH YÊU CẦU CHỈNH SỬA CHO BẢN NÀY
            if (changeRequests.isNotEmpty) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.orange.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.orange.withValues(alpha: 0.3)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Icon(Icons.warning_amber_rounded,
                            size: 18, color: Colors.orange.shade800),
                        const SizedBox(width: 6),
                        Text(
                          'Yêu cầu chỉnh sửa từ Nhà tuyển dụng (${changeRequests.length}):',
                          style: TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 13,
                            color: Colors.orange.shade900,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    ...changeRequests.map((yc) => Padding(
                          padding: const EdgeInsets.only(bottom: 6),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                '• ${yc.noiDungYeuCau}',
                                style: const TextStyle(
                                  fontSize: 13,
                                  color: AppColors.heading,
                                ),
                              ),
                              Padding(
                                padding: const EdgeInsets.only(left: 12, top: 2),
                                child: Text(
                                  AppFormatters.formatDateTime(yc.ngayYeuCau),
                                  style: TextStyle(
                                    fontSize: 11,
                                    color: Colors.grey.shade600,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        )),
                  ],
                ),
              ),
            ],

            // NÚT ĐÁNH GIÁ CỦA NTD
            if (canReview) ...[
              const SizedBox(height: 14),
              const Divider(height: 1),
              const SizedBox(height: 10),
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton.icon(
                      onPressed: isProcessing
                          ? null
                          : () => _moHopThoaiYeuCauChinhSua(item),
                      style: OutlinedButton.styleFrom(
                        foregroundColor: Colors.orange.shade800,
                        side: BorderSide(color: Colors.orange.shade700),
                        padding: const EdgeInsets.symmetric(vertical: 10),
                      ),
                      icon: const Icon(Icons.edit_note_rounded, size: 18),
                      label: const Text(
                        'Yêu cầu sửa',
                        style: TextStyle(fontWeight: FontWeight.bold),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: FilledButton.icon(
                      onPressed: isProcessing
                          ? null
                          : () => _xacNhanNghiemThu(item),
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        padding: const EdgeInsets.symmetric(vertical: 10),
                      ),
                      icon: const Icon(Icons.check_circle_rounded, size: 18),
                      label: const Text(
                        'Nghiệm thu đạt',
                        style: TextStyle(fontWeight: FontWeight.bold),
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }
}
