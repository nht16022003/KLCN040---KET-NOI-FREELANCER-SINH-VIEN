import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_hop_dong.dart';
import '../../models/mo_hinh_task_cong_viec.dart';
import '../../viewmodels/viewmodel_hop_dong.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_task_cong_viec.dart';

const List<String> _cacTrangThaiTask = [
  'ChuaThucHien',
  'DangThucHien',
  'HoanThanh',
];

String _nhanTrangThaiTask(String t) {
  switch (t) {
    case 'ChuaThucHien':
      return 'Chưa thực hiện';
    case 'DangThucHien':
      return 'Đang thực hiện';
    case 'HoanThanh':
      return 'Hoàn thành';
    default:
      return t;
  }
}

String _ngay(DateTime d) =>
    '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';

/// Dữ liệu trả về từ hộp thoại thêm/sửa đầu việc
class _DuLieuDauViec {
  final String ten;
  final String moTa;
  final String? linkSanPham;
  final int? maGiaiDoan;
  final DateTime? hanHoanThanh;

  _DuLieuDauViec({
    required this.ten,
    required this.moTa,
    this.linkSanPham,
    this.maGiaiDoan,
    this.hanHoanThanh,
  });
}

/// Hộp thoại thêm / sửa đầu việc độc lập để quản lý lifecycle an toàn, tránh lỗi framework _dependents.isEmpty
class _HopThoaiDauViec extends StatefulWidget {
  final TaskCongViecModel? task;
  final List<GiaiDoanHopDongModel> danhSachGiaiDoan;

  const _HopThoaiDauViec({
    this.task,
    required this.danhSachGiaiDoan,
  });

  @override
  State<_HopThoaiDauViec> createState() => _HopThoaiDauViecState();
}

class _HopThoaiDauViecState extends State<_HopThoaiDauViec> {
  late final TextEditingController _tenController;
  late final TextEditingController _moTaController;
  late final TextEditingController _linkController;
  final _formKey = GlobalKey<FormState>();

  int? _maGiaiDoanChon;
  DateTime? _han;

  @override
  void initState() {
    super.initState();
    _tenController = TextEditingController(text: widget.task?.tenTask ?? '');
    _moTaController = TextEditingController(text: widget.task?.moTa ?? '');
    _linkController = TextEditingController(text: widget.task?.linkSanPham ?? '');

    _maGiaiDoanChon = widget.task?.maGiaiDoan;
    if (_maGiaiDoanChon != null &&
        !widget.danhSachGiaiDoan.any((g) => g.maGiaiDoan == _maGiaiDoanChon)) {
      _maGiaiDoanChon = null;
    }
    _han = widget.task?.hanHoanThanh;
  }

  @override
  void dispose() {
    _tenController.dispose();
    _moTaController.dispose();
    _linkController.dispose();
    super.dispose();
  }

  void _dongVaLuu() {
    if (!_formKey.currentState!.validate()) return;
    FocusScope.of(context).unfocus();

    final duLieu = _DuLieuDauViec(
      ten: _tenController.text.trim(),
      moTa: _moTaController.text.trim(),
      linkSanPham: _linkController.text.trim().isEmpty ? null : _linkController.text.trim(),
      maGiaiDoan: _maGiaiDoanChon,
      hanHoanThanh: _han,
    );
    Navigator.of(context).pop(duLieu);
  }

  void _dongVaHuy() {
    FocusScope.of(context).unfocus();
    Navigator.of(context).pop(null);
  }

  @override
  Widget build(BuildContext context) {
    final dsGiaiDoan = widget.danhSachGiaiDoan;
    final isSua = widget.task != null;

    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      title: Row(
        children: [
          Icon(
            isSua ? Icons.edit_note : Icons.add_task,
            color: AppColors.primary,
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              isSua ? 'Sửa đầu việc' : 'Thêm đầu việc',
              style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
            ),
          ),
          IconButton(
            icon: const Icon(Icons.close, color: Colors.grey),
            tooltip: 'Đóng',
            onPressed: _dongVaHuy,
          ),
        ],
      ),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (dsGiaiDoan.isNotEmpty) ...[
                DropdownButtonFormField<int?>(
                  initialValue: _maGiaiDoanChon,
                  isExpanded: true,
                  decoration: InputDecoration(
                    labelText: 'Giai đoạn trong hợp đồng',
                    prefixIcon: const Icon(Icons.layers_outlined, color: AppColors.primary),
                    filled: true,
                    fillColor: AppColors.lightGreenSurface.withValues(alpha: 0.5),
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                  ),
                  items: [
                    const DropdownMenuItem<int?>(
                      value: null,
                      child: Text(
                        '-- Không phân giai đoạn --',
                        overflow: TextOverflow.ellipsis,
                        style: TextStyle(fontStyle: FontStyle.italic, color: Colors.black54),
                      ),
                    ),
                    ...dsGiaiDoan.map(
                      (gd) => DropdownMenuItem<int?>(
                        value: gd.maGiaiDoan,
                        child: Text(
                          '${gd.tenGiaiDoan}${gd.hanChot != null ? " (Hạn: ${_ngay(gd.hanChot!)})" : ""}',
                          overflow: TextOverflow.ellipsis,
                          style: const TextStyle(fontWeight: FontWeight.w600),
                        ),
                      ),
                    ),
                  ],
                  onChanged: (v) {
                    setState(() {
                      _maGiaiDoanChon = v;
                      if (v != null) {
                        final gd = dsGiaiDoan.firstWhere((g) => g.maGiaiDoan == v);
                        if (_han == null && gd.hanChot != null) {
                          _han = gd.hanChot;
                        }
                        if (_tenController.text.trim().isEmpty) {
                          _tenController.text = gd.tenGiaiDoan;
                        }
                        if (_moTaController.text.trim().isEmpty && (gd.moTa ?? '').isNotEmpty) {
                          _moTaController.text = gd.moTa!;
                        }
                      }
                    });
                  },
                ),
                const SizedBox(height: 12),
              ],
              TextFormField(
                controller: _tenController,
                decoration: InputDecoration(
                  labelText: 'Tên đầu việc *',
                  hintText: 'Nhập tên công việc cụ thể',
                  prefixIcon: const Icon(Icons.assignment_outlined, color: AppColors.primary),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
                validator: (v) => (v == null || v.trim().isEmpty)
                    ? 'Vui lòng nhập tên đầu việc'
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _moTaController,
                maxLines: 3,
                decoration: InputDecoration(
                  labelText: 'Mô tả đầu việc *',
                  hintText: 'Mô tả chi tiết nội dung cần làm và kết quả',
                  prefixIcon: const Icon(Icons.description_outlined, color: AppColors.primary),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
                validator: (v) => (v == null || v.trim().isEmpty)
                    ? 'Vui lòng nhập mô tả đầu việc'
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _linkController,
                keyboardType: TextInputType.url,
                decoration: InputDecoration(
                  labelText: 'Link sản phẩm bàn giao (nếu có)',
                  hintText: 'URL Drive, GitHub, Figma, Demo...',
                  prefixIcon: const Icon(Icons.link, color: AppColors.primary),
                  suffixIcon: IconButton(
                    icon: const Icon(Icons.paste_outlined, size: 20),
                    tooltip: 'Dán từ bộ nhớ tạm',
                    onPressed: () async {
                      final data = await Clipboard.getData(Clipboard.kTextPlain);
                      if (data?.text != null && data!.text!.isNotEmpty) {
                        setState(() => _linkController.text = data.text!.trim());
                      }
                    },
                  ),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                  helperText: 'Dán link sản phẩm cần bàn giao cho Nhà tuyển dụng',
                  helperMaxLines: 2,
                ),
              ),
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                decoration: BoxDecoration(
                  color: Colors.grey.shade100,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: Colors.grey.shade300),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.calendar_today_outlined, size: 20, color: AppColors.primary),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        _han == null
                            ? 'Chưa đặt hạn hoàn thành'
                            : 'Hạn: ${_ngay(_han!)}',
                        style: TextStyle(
                          fontWeight: _han != null ? FontWeight.bold : FontWeight.normal,
                          color: _han != null ? AppColors.heading : Colors.grey.shade700,
                        ),
                      ),
                    ),
                    TextButton.icon(
                      onPressed: () async {
                        final chon = await showDatePicker(
                          context: context,
                          initialDate: _han ?? DateTime.now().add(const Duration(days: 3)),
                          firstDate: DateTime(2020),
                          lastDate: DateTime(2100),
                        );
                        if (chon != null) setState(() => _han = chon);
                      },
                      icon: const Icon(Icons.edit_calendar, size: 18),
                      label: const Text('Chọn ngày'),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
      actionsPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      actions: [
        OutlinedButton(
          onPressed: _dongVaHuy,
          style: OutlinedButton.styleFrom(
            foregroundColor: Colors.grey.shade700,
            side: BorderSide(color: Colors.grey.shade400),
          ),
          child: const Text('Hủy'),
        ),
        FilledButton.icon(
          onPressed: _dongVaLuu,
          style: FilledButton.styleFrom(
            backgroundColor: AppColors.primary,
            foregroundColor: Colors.white,
          ),
          icon: const Icon(Icons.save_outlined, size: 18),
          label: const Text('Lưu đầu việc'),
        ),
      ],
    );
  }
}

/// Cập nhật tiến độ dự án (Freelancer & Nhà tuyển dụng) qua `api/TaskCongViec`.
class TienDoScreen extends StatefulWidget {
  final String maHD;
  final String? tenCongViec;

  const TienDoScreen({super.key, required this.maHD, this.tenCongViec});

  @override
  State<TienDoScreen> createState() => _TienDoScreenState();
}

class _TienDoScreenState extends State<TienDoScreen> {
  int? get _maUser => context.read<AuthViewModel>().currentUser?.userId;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _tai());
  }

  Future<void> _tai() async {
    final u = _maUser;
    if (u == null) return;
    await context.read<TaskViewModel>().taiTienDo(widget.maHD, u);
  }

  void _thongBao(String s) =>
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(s)));

  /// Đánh giá từng giai đoạn của đầu việc dự án (Nhà tuyển dụng chấm Đạt / Chưa đạt cho từng giai đoạn)
  Future<void> _danhGiaGiaiDoan(
      GiaiDoanHopDongModel gd, String trangThaiMoi) async {
    final u = _maUser;
    if (u == null) return;

    if (gd.trangThai.toLowerCase() == trangThaiMoi.toLowerCase()) return;

    final dat = trangThaiMoi == 'Dat';
    final xacNhan = await showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: Row(
          children: [
            Icon(
              dat ? Icons.check_circle_outline : Icons.help_outline,
              color: dat ? AppColors.primary : Colors.orange.shade800,
            ),
            const SizedBox(width: 8),
            Text(dat ? 'Đánh giá: ĐẠT' : 'Đánh giá: CHƯA ĐẠT'),
          ],
        ),
        content: Text(
          dat
              ? 'Xác nhận mốc "${gd.tenGiaiDoan}" ĐÃ ĐẠT yêu cầu bàn giao?'
              : 'Đánh giá mốc "${gd.tenGiaiDoan}" CHƯA ĐẠT yêu cầu?',
        ),
        actions: [
          OutlinedButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            style: FilledButton.styleFrom(
              backgroundColor: dat ? AppColors.primary : Colors.orange.shade800,
            ),
            child: Text(dat ? 'Xác nhận Đạt' : 'Xác nhận Chưa đạt'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    await Future.delayed(const Duration(milliseconds: 120));
    if (!mounted) return;

    final vm = context.read<TaskViewModel>();
    final loi = await vm.danhGiaGiaiDoan(
      maHD: widget.maHD,
      maUser: u,
      maGiaiDoan: gd.maGiaiDoan,
      trangThai: trangThaiMoi,
    );
    if (!mounted) return;
    _thongBao(loi ??
        'Đã cập nhật mốc "${gd.tenGiaiDoan}" thành "${dat ? 'Đạt' : 'Chưa đạt'}".');
  }

  Future<void> _hienForm({
    TaskCongViecModel? task,
    List<GiaiDoanHopDongModel>? danhSachGiaiDoan,
  }) async {
    final u = _maUser;
    if (u == null) return;

    final vmHopDong = context.read<HopDongViewModel>();
    final dsGiaiDoan = (danhSachGiaiDoan != null && danhSachGiaiDoan.isNotEmpty)
        ? danhSachGiaiDoan
        : (context.read<TaskViewModel>().tienDo?.danhSachGiaiDoan.isNotEmpty == true
            ? context.read<TaskViewModel>().tienDo!.danhSachGiaiDoan
            : (vmHopDong.hopDongHienTai?.maHD == widget.maHD
                ? vmHopDong.hopDongHienTai!.giaiDoans
                : <GiaiDoanHopDongModel>[]));

    final duLieu = await showDialog<_DuLieuDauViec>(
      context: context,
      barrierDismissible: false,
      builder: (ctx) => _HopThoaiDauViec(
        task: task,
        danhSachGiaiDoan: dsGiaiDoan,
      ),
    );

    if (duLieu == null || !mounted) return;

    await Future.delayed(const Duration(milliseconds: 120));
    if (!mounted) return;

    final vm = context.read<TaskViewModel>();
    final loi = task == null
        ? await vm.themTask(
            maHD: widget.maHD,
            maUser: u,
            tenTask: duLieu.ten,
            moTa: duLieu.moTa,
            hanHoanThanh: duLieu.hanHoanThanh,
            maGiaiDoan: duLieu.maGiaiDoan,
            linkSanPham: duLieu.linkSanPham,
          )
        : await vm.suaTask(
            task: task,
            maUser: u,
            tenTask: duLieu.ten,
            moTa: duLieu.moTa,
            hanHoanThanh: duLieu.hanHoanThanh,
            maGiaiDoan: duLieu.maGiaiDoan,
            linkSanPham: duLieu.linkSanPham,
          );
    if (!mounted) return;
    _thongBao(loi ?? 'Đã lưu đầu việc thành công.');
  }

  Future<void> _doiTrangThai(TaskCongViecModel task, String moi) async {
    final u = _maUser;
    if (u == null || moi == task.trangThai) return;
    final loi = await context.read<TaskViewModel>().doiTrangThai(
          task: task,
          maUser: u,
          trangThai: moi,
        );
    if (!mounted) return;
    if (loi != null) {
      _thongBao(loi);
    } else {
      if (moi == 'HoanThanh') {
        _thongBao('Đã cập nhật đầu việc sang "Hoàn thành" (đã cộng vào tiến độ).');
      } else {
        _thongBao('Đã cập nhật trạng thái sang "${_nhanTrangThaiTask(moi)}".');
      }
    }
  }

  Future<void> _xoa(TaskCongViecModel task) async {
    final u = _maUser;
    if (u == null) return;
    final xacNhan = await showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Row(
          children: [
            Icon(Icons.warning_amber_rounded, color: Colors.redAccent),
            SizedBox(width: 8),
            Text('Xóa đầu việc'),
          ],
        ),
        content: Text('Bạn có chắc muốn xóa đầu việc "${task.tenTask}"?'),
        actions: [
          OutlinedButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            style: FilledButton.styleFrom(backgroundColor: Colors.redAccent),
            child: const Text('Xóa'),
          ),
        ],
      ),
    );
    if (xacNhan != true || !mounted) return;

    await Future.delayed(const Duration(milliseconds: 120));
    if (!mounted) return;

    final loi = await context.read<TaskViewModel>().xoaTask(
          task: task,
          maUser: u,
        );
    if (!mounted) return;
    _thongBao(loi ?? 'Đã xóa đầu việc.');
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<TaskViewModel>();
    final td = vm.tienDo?.maHD == widget.maHD ? vm.tienDo : null;
    final user = context.watch<AuthViewModel>().currentUser;
    final coQuyenCapNhat = td?.coQuyenCapNhat == true;
    final coQuyenDanhGia = td?.coQuyenDanhGia == true || user?.vaiTro == 'NhaTuyenDung';

    Widget noiDung;
    if (td == null) {
      noiDung = Center(
        child: vm.dangTai
            ? const CircularProgressIndicator()
            : Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      vm.thongBaoLoi ?? 'Không tải được tiến độ dự án.',
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 12),
                    FilledButton(onPressed: _tai, child: const Text('Thử lại')),
                  ],
                ),
              ),
      );
    } else {
      // Tính toán tỷ lệ phần trăm mức độ hoàn thành dựa trên số task trong hợp đồng
      final int tongSo = td.danhSachTask.isNotEmpty ? td.danhSachTask.length : td.tongSoTask;
      final int soHoanThanh = td.danhSachTask.isNotEmpty
          ? td.danhSachTask.where((t) => t.trangThai == 'HoanThanh').length
          : td.soTaskHoanThanh;
      final int soDangLam = td.danhSachTask.isNotEmpty
          ? td.danhSachTask.where((t) => t.trangThai == 'DangThucHien').length
          : td.soTaskDangThucHien;
      final int soChuaLam = td.danhSachTask.isNotEmpty
          ? td.danhSachTask.where((t) => t.trangThai == 'ChuaThucHien').length
          : td.soTaskChuaThucHien;

      final double tyLe = tongSo > 0 ? (soHoanThanh / tongSo * 100) : 0.0;
      final double tiLeDongGopMoiTask = tongSo > 0 ? (100.0 / tongSo) : 0.0;

      noiDung = RefreshIndicator(
        onRefresh: _tai,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // ==========================================
            // 1. CARD TỔNG QUAN TIẾN ĐỘ DỰ ÁN
            // ==========================================
            Card(
              elevation: 0,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(16),
                side: BorderSide(color: AppColors.primary.withValues(alpha: 0.2)),
              ),
              color: AppColors.surface,
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text(
                              'Mức độ hoàn thành',
                              style: TextStyle(
                                  fontSize: 13,
                                  color: Colors.black54,
                                  fontWeight: FontWeight.w500),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              '${tyLe.toStringAsFixed(tyLe % 1 == 0 ? 0 : 1)}%',
                              style: const TextStyle(
                                fontSize: 26,
                                fontWeight: FontWeight.bold,
                                color: AppColors.primary,
                              ),
                            ),
                          ],
                        ),
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 12, vertical: 6),
                          decoration: BoxDecoration(
                            color: tyLe == 100
                                ? Colors.green.shade100
                                : AppColors.lightGreenSurface,
                            borderRadius: BorderRadius.circular(20),
                            border: Border.all(
                              color: tyLe == 100
                                  ? AppColors.primary
                                  : AppColors.primary.withValues(alpha: 0.3),
                            ),
                          ),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Icon(
                                tyLe == 100
                                    ? Icons.check_circle
                                    : Icons.task_alt,
                                size: 16,
                                color: AppColors.primary,
                              ),
                              const SizedBox(width: 6),
                              Text(
                                '$soHoanThanh / $tongSo hoàn thành',
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 13,
                                  color: AppColors.primaryDark,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: LinearProgressIndicator(
                        value: tongSo > 0
                            ? (soHoanThanh / tongSo).clamp(0.0, 1.0)
                            : 0.0,
                        minHeight: 12,
                        color: AppColors.primary,
                        backgroundColor:
                            AppColors.primary.withValues(alpha: 0.15),
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      tongSo > 0
                          ? 'Xong 1 việc tính 1/$tongSo (+${tiLeDongGopMoiTask.toStringAsFixed(1)}%). Chỉ khi task chuyển sang "Hoàn thành" mới được tính %.'
                          : 'Chưa có đầu việc nào để tính tỷ lệ hoàn thành.',
                      style: TextStyle(
                        fontSize: 12,
                        color: Colors.grey.shade600,
                        fontStyle: FontStyle.italic,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Wrap(
                      spacing: 8,
                      runSpacing: 6,
                      children: [
                        Chip(
                          avatar: const Icon(Icons.list_alt, size: 16),
                          label: Text('Tổng: $tongSo'),
                          backgroundColor: Colors.grey.shade100,
                        ),
                        Chip(
                          avatar: const Icon(Icons.hourglass_empty,
                              size: 16, color: Colors.blueGrey),
                          label: Text('Chưa làm: $soChuaLam'),
                          backgroundColor: Colors.grey.shade100,
                        ),
                        Chip(
                          avatar: Icon(Icons.pending_actions,
                              size: 16, color: Colors.orange.shade800),
                          label: Text('Đang làm: $soDangLam'),
                          backgroundColor: Colors.orange.shade50,
                        ),
                        Chip(
                          avatar: const Icon(Icons.check_circle,
                              size: 16, color: AppColors.primary),
                          label: Text(
                            'Xong: $soHoanThanh',
                            style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                color: AppColors.primaryDark),
                          ),
                          backgroundColor: AppColors.lightGreenSurface,
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),

            // ==========================================
            // 2. KHỐI CÁC MỐC GIAI ĐOẠN & ĐÁNH GIÁ TỪNG GIAI ĐOẠN
            // ==========================================
            Card(
              elevation: 0,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(16),
                side: const BorderSide(color: AppColors.border),
              ),
              color: AppColors.surface,
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Header mốc giai đoạn
                    Row(
                      children: [
                        const Icon(Icons.layers, color: AppColors.primary, size: 20),
                        const SizedBox(width: 8),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                'Các giai đoạn dự án (${td.danhSachGiaiDoan.length})',
                                style: const TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.heading,
                                ),
                              ),
                              if (coQuyenDanhGia)
                                Text(
                                  'Nhà tuyển dụng đánh giá Đạt / Chưa đạt cho từng giai đoạn bên dưới:',
                                  style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
                                ),
                            ],
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),

                    // DANH SÁCH CHI TIẾT CÁC MỐC GIAI ĐOẠN TRONG HỢP ĐỒNG
                    if (td.danhSachGiaiDoan.isEmpty)
                      Padding(
                        padding: const EdgeInsets.symmetric(vertical: 12),
                        child: Center(
                          child: Text(
                            'Hợp đồng chưa thiết lập các mốc giai đoạn cụ thể.',
                            style: TextStyle(fontSize: 13, color: Colors.grey.shade600),
                          ),
                        ),
                      )
                    else
                      ...td.danhSachGiaiDoan.map((gd) {
                        final bool daDat = gd.trangThai.toLowerCase() == 'dat';
                        final tasksGiaiDoan = td.danhSachTask
                            .where((t) => t.maGiaiDoan == gd.maGiaiDoan)
                            .toList();
                        final soTaskXong = tasksGiaiDoan
                            .where((t) => t.trangThai == 'HoanThanh')
                            .length;
                        final taskCoLink = tasksGiaiDoan
                            .firstWhere((t) => (t.linkSanPham ?? '').isNotEmpty,
                                orElse: () => TaskCongViecModel(
                                    maTask: 0,
                                    maHD: '',
                                    tenTask: '',
                                    moTa: '',
                                    trangThai: ''));

                        return Container(
                          margin: const EdgeInsets.only(bottom: 12),
                          padding: const EdgeInsets.all(14),
                          decoration: BoxDecoration(
                            color: daDat
                                ? Colors.green.shade50.withValues(alpha: 0.5)
                                : Colors.grey.shade50,
                            borderRadius: BorderRadius.circular(12),
                            border: Border.all(
                              color: daDat ? Colors.green.shade300 : Colors.grey.shade300,
                              width: daDat ? 1.5 : 1.0,
                            ),
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Icon(
                                    daDat ? Icons.verified : Icons.flag_outlined,
                                    size: 20,
                                    color: daDat ? AppColors.primary : Colors.grey.shade700,
                                  ),
                                  const SizedBox(width: 8),
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          gd.tenGiaiDoan,
                                          style: TextStyle(
                                            fontWeight: FontWeight.bold,
                                            fontSize: 15,
                                            color: daDat ? AppColors.primaryDark : AppColors.heading,
                                          ),
                                        ),
                                        if (gd.hanChot != null)
                                          Padding(
                                            padding: const EdgeInsets.only(top: 2),
                                            child: Text(
                                              'Hạn chót: ${_ngay(gd.hanChot!)}',
                                              style: const TextStyle(fontSize: 12, color: Colors.black54),
                                            ),
                                          ),
                                      ],
                                    ),
                                  ),
                                  // Badge trạng thái hiện tại của giai đoạn
                                  Container(
                                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                                    decoration: BoxDecoration(
                                      color: daDat ? Colors.green.shade100 : Colors.orange.shade50,
                                      borderRadius: BorderRadius.circular(8),
                                      border: Border.all(
                                        color: daDat ? AppColors.primary : Colors.orange.shade400,
                                      ),
                                    ),
                                    child: Row(
                                      mainAxisSize: MainAxisSize.min,
                                      children: [
                                        Icon(
                                          daDat ? Icons.check_circle : Icons.schedule,
                                          size: 13,
                                          color: daDat ? AppColors.primary : Colors.orange.shade900,
                                        ),
                                        const SizedBox(width: 4),
                                        Text(
                                          daDat ? 'ĐÃ ĐẠT' : 'CHƯA ĐẠT',
                                          style: TextStyle(
                                            fontSize: 11,
                                            fontWeight: FontWeight.bold,
                                            color: daDat ? AppColors.primaryDark : Colors.orange.shade900,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ),
                                ],
                              ),
                              if ((gd.moTa ?? '').isNotEmpty) ...[
                                const SizedBox(height: 6),
                                Text(
                                  gd.moTa!,
                                  style: TextStyle(fontSize: 13, color: Colors.grey.shade700),
                                ),
                              ],
                              if (tasksGiaiDoan.isNotEmpty) ...[
                                const SizedBox(height: 6),
                                Row(
                                  children: [
                                    const Icon(Icons.task_alt, size: 14, color: Colors.blueGrey),
                                    const SizedBox(width: 4),
                                    Text(
                                      'Đầu việc thuộc giai đoạn: $soTaskXong/${tasksGiaiDoan.length} hoàn thành',
                                      style: const TextStyle(fontSize: 12, color: Colors.black87),
                                    ),
                                  ],
                                ),
                              ],
                              if ((taskCoLink.linkSanPham ?? '').isNotEmpty) ...[
                                const SizedBox(height: 6),
                                Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                  decoration: BoxDecoration(
                                    color: Colors.green.shade50,
                                    borderRadius: BorderRadius.circular(6),
                                    border: Border.all(color: Colors.green.shade200),
                                  ),
                                  child: Row(
                                    children: [
                                      const Icon(Icons.link, size: 14, color: AppColors.primary),
                                      const SizedBox(width: 6),
                                      Expanded(
                                        child: Text(
                                          'Sản phẩm: ${taskCoLink.linkSanPham!}',
                                          style: const TextStyle(
                                            fontSize: 11,
                                            color: Colors.blue,
                                            decoration: TextDecoration.underline,
                                          ),
                                          maxLines: 1,
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                      ),
                                      IconButton(
                                        padding: EdgeInsets.zero,
                                        constraints: const BoxConstraints(),
                                        icon: const Icon(Icons.copy, size: 14, color: AppColors.primary),
                                        tooltip: 'Sao chép link',
                                        onPressed: () {
                                          Clipboard.setData(ClipboardData(text: taskCoLink.linkSanPham!));
                                          _thongBao('Đã sao chép link bàn giao của giai đoạn.');
                                        },
                                      ),
                                    ],
                                  ),
                                ),
                              ],

                              // LỰA CHỌN ĐÁNH GIÁ ĐẠT / CHƯA ĐẠT CHO TỪNG GIAI ĐOẠN (NHÀ TUYỂN DỤNG)
                              if (coQuyenDanhGia) ...[
                                const SizedBox(height: 10),
                                Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 10),
                                  decoration: BoxDecoration(
                                    color: Colors.white,
                                    borderRadius: BorderRadius.circular(8),
                                    border: Border.all(color: Colors.grey.shade300),
                                  ),
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: [
                                      const Row(
                                        children: [
                                          Icon(Icons.rate_review_outlined, size: 15, color: AppColors.primary),
                                          SizedBox(width: 6),
                                          Text(
                                            'Đánh giá mốc giai đoạn này:',
                                            style: TextStyle(
                                              fontSize: 12,
                                              fontWeight: FontWeight.bold,
                                              color: Colors.black87,
                                            ),
                                          ),
                                        ],
                                      ),
                                      const SizedBox(height: 8),
                                      Row(
                                        children: [
                                          // Nút chọn ĐẠT cho giai đoạn này
                                          Expanded(
                                            child: InkWell(
                                              onTap: vm.dangGhi ? null : () => _danhGiaGiaiDoan(gd, 'Dat'),
                                              borderRadius: BorderRadius.circular(8),
                                              child: Container(
                                                padding: const EdgeInsets.symmetric(vertical: 8),
                                                decoration: BoxDecoration(
                                                  color: daDat ? AppColors.primary : Colors.grey.shade100,
                                                  borderRadius: BorderRadius.circular(8),
                                                  border: Border.all(
                                                    color: daDat ? AppColors.primary : Colors.grey.shade400,
                                                    width: daDat ? 1.5 : 1.0,
                                                  ),
                                                ),
                                                child: Row(
                                                  mainAxisAlignment: MainAxisAlignment.center,
                                                  children: [
                                                    Icon(
                                                      Icons.check_circle,
                                                      size: 15,
                                                      color: daDat ? Colors.white : Colors.grey.shade600,
                                                    ),
                                                    const SizedBox(width: 4),
                                                    Text(
                                                      'Đạt',
                                                      style: TextStyle(
                                                        fontSize: 12,
                                                        fontWeight: FontWeight.bold,
                                                        color: daDat ? Colors.white : Colors.grey.shade800,
                                                      ),
                                                    ),
                                                  ],
                                                ),
                                              ),
                                            ),
                                          ),
                                          const SizedBox(width: 8),
                                          // Nút chọn CHƯA ĐẠT cho giai đoạn này
                                          Expanded(
                                            child: InkWell(
                                              onTap: vm.dangGhi ? null : () => _danhGiaGiaiDoan(gd, 'ChuaDat'),
                                              borderRadius: BorderRadius.circular(8),
                                              child: Container(
                                                padding: const EdgeInsets.symmetric(vertical: 8),
                                                decoration: BoxDecoration(
                                                  color: !daDat ? Colors.orange.shade800 : Colors.grey.shade100,
                                                  borderRadius: BorderRadius.circular(8),
                                                  border: Border.all(
                                                    color: !daDat ? Colors.orange.shade800 : Colors.grey.shade400,
                                                    width: !daDat ? 1.5 : 1.0,
                                                  ),
                                                ),
                                                child: Row(
                                                  mainAxisAlignment: MainAxisAlignment.center,
                                                  children: [
                                                    Icon(
                                                      Icons.cancel,
                                                      size: 15,
                                                      color: !daDat ? Colors.white : Colors.grey.shade600,
                                                    ),
                                                    const SizedBox(width: 4),
                                                    Text(
                                                      'Chưa đạt',
                                                      style: TextStyle(
                                                        fontSize: 12,
                                                        fontWeight: FontWeight.bold,
                                                        color: !daDat ? Colors.white : Colors.grey.shade800,
                                                      ),
                                                    ),
                                                  ],
                                                ),
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
                        );
                      }),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),

            // ==========================================
            // 3. DANH SÁCH ĐẦU VIỆC (TASKS) CỦA DỰ ÁN
            // ==========================================
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    const Icon(Icons.assignment, color: AppColors.primary, size: 20),
                    const SizedBox(width: 8),
                    Text(
                      'Đầu việc dự án (${td.danhSachTask.length})',
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                  ],
                ),
                if (coQuyenCapNhat)
                  TextButton.icon(
                    onPressed: vm.dangGhi
                        ? null
                        : () => _hienForm(danhSachGiaiDoan: td.danhSachGiaiDoan),
                    icon: const Icon(Icons.add, size: 18),
                    label: const Text('Thêm việc'),
                  ),
              ],
            ),
            const SizedBox(height: 8),

            if (td.danhSachTask.isEmpty)
              Padding(
                padding: const EdgeInsets.all(24),
                child: Center(
                  child: Column(
                    children: [
                      Icon(Icons.task_alt, size: 48, color: Colors.grey.shade400),
                      const SizedBox(height: 8),
                      const Text(
                        'Chưa có đầu việc nào được thêm vào dự án.',
                        style: TextStyle(color: Colors.black54),
                      ),
                      if (coQuyenCapNhat) ...[
                        const SizedBox(height: 12),
                        FilledButton.icon(
                          onPressed: vm.dangGhi
                              ? null
                              : () => _hienForm(danhSachGiaiDoan: td.danhSachGiaiDoan),
                          icon: const Icon(Icons.add),
                          label: const Text('Thêm đầu việc đầu tiên'),
                        ),
                      ],
                    ],
                  ),
                ),
              ),
            ...td.danhSachTask.map(
              (t) => Card(
                margin: const EdgeInsets.only(bottom: 12),
                elevation: 1.5,
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12)),
                child: Padding(
                  padding: const EdgeInsets.all(14),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  t.tenTask,
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 16,
                                    color: AppColors.heading,
                                  ),
                                ),
                                if ((t.tenGiaiDoan ?? '').isNotEmpty) ...[
                                  const SizedBox(height: 4),
                                  Container(
                                    padding: const EdgeInsets.symmetric(
                                        horizontal: 8, vertical: 3),
                                    decoration: BoxDecoration(
                                      color: AppColors.lightGreenSurface,
                                      borderRadius: BorderRadius.circular(6),
                                      border: Border.all(
                                          color: AppColors.primary
                                              .withValues(alpha: 0.3)),
                                    ),
                                    child: Row(
                                      mainAxisSize: MainAxisSize.min,
                                      children: [
                                        const Icon(Icons.layers_outlined,
                                            size: 14, color: AppColors.primary),
                                        const SizedBox(width: 4),
                                        Flexible(
                                          child: Text(
                                            'Giai đoạn: ${t.tenGiaiDoan}',
                                            style: const TextStyle(
                                              fontSize: 12,
                                              fontWeight: FontWeight.w600,
                                              color: AppColors.primaryDark,
                                            ),
                                            overflow: TextOverflow.ellipsis,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ),
                                ],
                              ],
                            ),
                          ),
                          if (coQuyenCapNhat) ...[
                            IconButton(
                              tooltip: 'Sửa đầu việc',
                              icon: const Icon(Icons.edit_outlined,
                                  color: AppColors.primary),
                              onPressed: vm.dangGhi
                                  ? null
                                  : () => _hienForm(
                                        task: t,
                                        danhSachGiaiDoan: td.danhSachGiaiDoan,
                                      ),
                            ),
                            IconButton(
                              tooltip: 'Xóa đầu việc',
                              icon: const Icon(Icons.delete_outline,
                                  color: Colors.redAccent),
                              onPressed: vm.dangGhi ? null : () => _xoa(t),
                            ),
                          ],
                        ],
                      ),
                      if (t.moTa.isNotEmpty) ...[
                        const SizedBox(height: 8),
                        Text(
                          t.moTa,
                          style: const TextStyle(
                              color: Colors.black87, fontSize: 14),
                        ),
                      ],
                      if (t.hanHoanThanh != null) ...[
                        const SizedBox(height: 6),
                        Row(
                          children: [
                            const Icon(Icons.schedule,
                                size: 14, color: Colors.grey),
                            const SizedBox(width: 4),
                            Text(
                              'Hạn hoàn thành: ${_ngay(t.hanHoanThanh!)}',
                              style: const TextStyle(
                                  fontSize: 12, color: Colors.black54),
                            ),
                          ],
                        ),
                      ],
                      // Hiển thị Link sản phẩm bàn giao cho Nhà tuyển dụng
                      if ((t.linkSanPham ?? '').isNotEmpty) ...[
                        const SizedBox(height: 10),
                        Container(
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            color: Colors.green.shade50,
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(color: Colors.green.shade200),
                          ),
                          child: Row(
                            children: [
                              const Icon(Icons.cloud_done_outlined,
                                  color: AppColors.primary, size: 22),
                              const SizedBox(width: 8),
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    const Text(
                                      'Sản phẩm bàn giao cho Nhà tuyển dụng:',
                                      style: TextStyle(
                                        fontSize: 11,
                                        fontWeight: FontWeight.bold,
                                        color: AppColors.primaryDark,
                                      ),
                                    ),
                                    const SizedBox(height: 2),
                                    SelectableText(
                                      t.linkSanPham!,
                                      style: const TextStyle(
                                        fontSize: 13,
                                        color: Colors.blue,
                                        decoration: TextDecoration.underline,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                              IconButton(
                                icon: const Icon(Icons.copy,
                                    size: 18, color: AppColors.primary),
                                tooltip: 'Sao chép link',
                                onPressed: () {
                                  Clipboard.setData(
                                      ClipboardData(text: t.linkSanPham!));
                                  _thongBao(
                                      'Đã sao chép link bàn giao vào bộ nhớ tạm.');
                                },
                              ),
                            ],
                          ),
                        ),
                      ] else if (coQuyenCapNhat) ...[
                        const SizedBox(height: 8),
                        InkWell(
                          onTap: vm.dangGhi
                              ? null
                              : () => _hienForm(
                                    task: t,
                                    danhSachGiaiDoan: td.danhSachGiaiDoan,
                                  ),
                          borderRadius: BorderRadius.circular(6),
                          child: const Padding(
                            padding: EdgeInsets.symmetric(vertical: 4),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Icon(Icons.add_link,
                                    size: 16, color: AppColors.primary),
                                SizedBox(width: 4),
                                Text(
                                  'Đính kèm link sản phẩm bàn giao',
                                  style: TextStyle(
                                    fontSize: 12,
                                    color: AppColors.primary,
                                    fontWeight: FontWeight.w600,
                                    decoration: TextDecoration.underline,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ],
                      const SizedBox(height: 10),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text(
                            'Trạng thái:',
                            style: TextStyle(
                                fontSize: 13,
                                fontWeight: FontWeight.w500,
                                color: Colors.black54),
                          ),
                          if (coQuyenCapNhat)
                            Container(
                              padding:
                                  const EdgeInsets.symmetric(horizontal: 8),
                              decoration: BoxDecoration(
                                borderRadius: BorderRadius.circular(8),
                                border:
                                    Border.all(color: Colors.grey.shade300),
                              ),
                              child: DropdownButton<String>(
                                value: _cacTrangThaiTask.contains(t.trangThai)
                                    ? t.trangThai
                                    : null,
                                underline: const SizedBox(),
                                isDense: true,
                                items: _cacTrangThaiTask
                                    .map(
                                      (s) => DropdownMenuItem(
                                        value: s,
                                        child: Text(
                                          _nhanTrangThaiTask(s),
                                          style: TextStyle(
                                            fontWeight: FontWeight.w600,
                                            color: s == 'HoanThanh'
                                                ? AppColors.primary
                                                : (s == 'DangThucHien'
                                                    ? Colors.orange.shade800
                                                    : Colors.black87),
                                          ),
                                        ),
                                      ),
                                    )
                                    .toList(),
                                onChanged: vm.dangGhi
                                    ? null
                                    : (v) {
                                        if (v != null) _doiTrangThai(t, v);
                                      },
                              ),
                            )
                          else
                            Chip(
                              label: Text(_nhanTrangThaiTask(t.trangThai)),
                              backgroundColor: AppColors.lightGreenSurface,
                            ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
            ),
            const SizedBox(height: 70),
          ],
        ),
      );
    }

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Text(widget.tenCongViec ?? 'Tiến độ dự án'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: noiDung,
      floatingActionButton: coQuyenCapNhat
          ? FloatingActionButton.extended(
              onPressed: vm.dangGhi
                  ? null
                  : () => _hienForm(danhSachGiaiDoan: td?.danhSachGiaiDoan),
              icon: const Icon(Icons.add),
              label: const Text('Thêm đầu việc'),
            )
          : null,
    );
  }
}
