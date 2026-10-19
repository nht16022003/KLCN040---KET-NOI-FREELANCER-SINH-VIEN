import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_ung_tuyen.dart';
import '../../services/dich_vu_hop_dong.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../hop_dong/man_hinh_chi_tiet_hop_dong.dart';

/// Đại diện cho 1 mốc bàn giao trong form khởi tạo hợp đồng
class _MocBanGiaoItem {
  final TextEditingController tenController;
  final TextEditingController moTaController;
  final TextEditingController tyLeController;
  DateTime hanChot;

  _MocBanGiaoItem({
    required String ten,
    required String moTa,
    required double tyLe,
    required this.hanChot,
  })  : tenController = TextEditingController(text: ten),
        moTaController = TextEditingController(text: moTa),
        tyLeController = TextEditingController(text: tyLe.toStringAsFixed(0));

  void dispose() {
    tenController.dispose();
    moTaController.dispose();
    tyLeController.dispose();
  }
}

/// Màn hình Khởi tạo dự thảo hợp đồng từ ứng viên đã duyệt (dành cho Nhà tuyển dụng).
/// Tương ứng với Web `HopDongController.TaoHopDong` và API `POST api/HopDong/tao-hop-dong`.
class TaoHopDongScreen extends StatefulWidget {
  final ApplicationModel applicant;

  const TaoHopDongScreen({super.key, required this.applicant});

  @override
  State<TaoHopDongScreen> createState() => _TaoHopDongScreenState();
}

class _TaoHopDongScreenState extends State<TaoHopDongScreen> {
  final _formKey = GlobalKey<FormState>();
  final HopDongService _hopDongService = HopDongService();

  late TextEditingController _thuLaoController;
  late TextEditingController _dieuKhoanController;

  final List<_MocBanGiaoItem> _danhSachMoc = [];

  String _hinhThucLamViec = 'Remote';
  DateTime _ngayBatDau = DateTime.now();
  DateTime _ngayKetThuc = DateTime.now().add(const Duration(days: 14));
  bool _dangXuLy = false;

  static const String _dieuKhoanMacDinh = '''ĐIỀU KHOẢN HỢP ĐỒNG GIAO KẾT DỊCH VỤ FREELANCE

Điều 1. PHẠM VI VÀ YÊU CẦU CÔNG VIỆC:
- Freelancer cam kết hoàn thành đúng và đầy đủ các yêu cầu công việc được nêu trong bài đăng tuyển dụng và các mốc bàn giao thỏa thuận.
- Sản phẩm bàn giao phải đảm bảo chất lượng, tính ứng dụng thực tế và không vi phạm bản quyền hay sao chép trái phép.

Điều 2. QUYỀN VÀ NGHĨA VỤ CỦA KHÁCH HÀNG:
- Cung cấp kịp thời đầy đủ tài liệu, yêu cầu chi tiết và hỗ trợ giải đáp thắc mắc chuyên môn trong quá trình thực hiện.
- Nghiệm thu sản phẩm đúng hạn khi Freelancer bàn giao theo từng mốc giai đoạn hoặc bàn giao tổng kết.
- Giải ngân và thanh toán đúng số tiền thù lao đã ký kết khi nghiệm thu đạt yêu cầu.

Điều 3. QUYỀN VÀ NGHĨA VỤ CỦA FREELANCER:
- Bàn giao sản phẩm đầy đủ và đúng thời hạn cam kết (deadline).
- Chịu trách nhiệm sửa đổi, bảo hành hoặc khắc phục các lỗi phát sinh trong phạm vi thỏa thuận ban đầu.
- Giữ bí mật toàn bộ thông tin kinh doanh, dữ liệu kỹ thuật và mã nguồn của Khách hàng.

Điều 4. QUYỀN SỞ HỮU TRÍ TUỆ VÀ BẢO MẬT:
- Sau khi thanh toán toàn bộ thù lao, toàn bộ mã nguồn, tài liệu thiết kế và quyền sở hữu sản phẩm hoàn toàn thuộc về Khách hàng.
- Freelancer không được tự ý tái sử dụng hoặc chia sẻ sản phẩm này cho bất kỳ bên thứ ba nào khi chưa có sự đồng ý bằng văn bản của Khách hàng.''';

  @override
  void initState() {
    super.initState();
    final mucLuongGoc = widget.applicant.thuLaoDeXuat > 0
        ? widget.applicant.thuLaoDeXuat.toInt().toString()
        : '1000000';
    _thuLaoController = TextEditingController(text: mucLuongGoc);
    _dieuKhoanController = TextEditingController(text: _dieuKhoanMacDinh);

    // Mốc bàn giao mặc định ban đầu (tương tự Web)
    _danhSachMoc.add(
      _MocBanGiaoItem(
        ten: 'Giai đoạn 1: Bàn giao toàn bộ sản phẩm và nghiệm thu',
        moTa: 'Hoàn thành và bàn giao sản phẩm hoàn chỉnh theo đúng yêu cầu đã mô tả.',
        tyLe: 100,
        hanChot: _ngayKetThuc,
      ),
    );
  }

  @override
  void dispose() {
    _thuLaoController.dispose();
    _dieuKhoanController.dispose();
    for (final moc in _danhSachMoc) {
      moc.dispose();
    }
    super.dispose();
  }

  void _themMocBanGiao() {
    setState(() {
      final stt = _danhSachMoc.length + 1;
      _danhSachMoc.add(
        _MocBanGiaoItem(
          ten: 'Giai đoạn $stt: Bàn giao sản phẩm đợt $stt',
          moTa: 'Nghiệm thu các hạng mục theo tiến độ thỏa thuận đợt $stt.',
          tyLe: 0,
          hanChot: _ngayKetThuc,
        ),
      );
    });
  }

  void _xoaMocBanGiao(int index) {
    if (_danhSachMoc.length <= 1) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Hợp đồng cần tối thiểu 1 mốc bàn giao / nghiệm thu.')),
      );
      return;
    }
    setState(() {
      final removed = _danhSachMoc.removeAt(index);
      WidgetsBinding.instance.addPostFrameCallback((_) {
        removed.dispose();
      });
    });
  }

  Future<void> _chonHanChotMoc(int index) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _danhSachMoc[index].hanChot,
      firstDate: _ngayBatDau,
      lastDate: DateTime.now().add(const Duration(days: 730)),
    );
    if (picked != null) {
      setState(() {
        _danhSachMoc[index].hanChot = picked;
      });
    }
  }

  Future<void> _chonNgayBatDau() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _ngayBatDau,
      firstDate: DateTime.now().subtract(const Duration(days: 1)),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    if (picked != null) {
      setState(() {
        _ngayBatDau = picked;
        if (_ngayKetThuc.isBefore(_ngayBatDau)) {
          _ngayKetThuc = _ngayBatDau.add(const Duration(days: 7));
          for (final moc in _danhSachMoc) {
            moc.hanChot = _ngayKetThuc;
          }
        }
      });
    }
  }

  Future<void> _chonNgayKetThuc() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _ngayKetThuc,
      firstDate: _ngayBatDau,
      lastDate: DateTime.now().add(const Duration(days: 730)),
    );
    if (picked != null) {
      setState(() {
        _ngayKetThuc = picked;
        if (_danhSachMoc.length == 1) {
          _danhSachMoc[0].hanChot = picked;
        }
      });
    }
  }

  double get _tongTyLeGiaiNgan {
    return _danhSachMoc.fold(0.0, (sum, m) {
      return sum + (double.tryParse(m.tyLeController.text) ?? 0.0);
    });
  }

  double get _soTienThuLao {
    return double.tryParse(_thuLaoController.text.replaceAll(RegExp(r'[^0-9]'), '')) ?? 0;
  }

  Future<void> _taoHopDong() async {
    if (!_formKey.currentState!.validate()) return;

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null || user.userId <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập lại.')),
      );
      return;
    }

    final maFL = int.tryParse(widget.applicant.applicantId) ?? 0;
    final maUngTuyen = int.tryParse(widget.applicant.id);
    final soTien = _soTienThuLao;

    if (maFL <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không xác định được mã Freelancer.')),
      );
      return;
    }

    if (soTien <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Số tiền thù lao phải lớn hơn 0.')),
      );
      return;
    }

    if (_danhSachMoc.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng tạo ít nhất 1 mốc bàn giao.')),
      );
      return;
    }

    setState(() => _dangXuLy = true);

    // Chuẩn bị danh sách mốc bàn giao (giaiDoans) để lưu về database table GiaiDoan_HopDong
    final giaiDoansPayload = _danhSachMoc.map((m) {
      final tyLe = double.tryParse(m.tyLeController.text) ?? 0.0;
      final soTienMoc = tyLe > 0 ? (soTien * tyLe / 100).roundToDouble() : 0.0;
      return {
        'tenGiaiDoan': m.tenController.text.trim(),
        'moTa': m.moTaController.text.trim(),
        'hanChot': m.hanChot.toIso8601String(),
        'tyLeGiaiNgan': tyLe,
        'soTien': soTienMoc > 0 ? soTienMoc : soTien,
      };
    }).toList();

    final payload = {
      'maJob': widget.applicant.jobId,
      'maFreelancerStudent': maFL,
      'sotienkyquy': soTien,
      'ngayBatDau': _ngayBatDau.toIso8601String(),
      'ngayKetThuc': _ngayKetThuc.toIso8601String(),
      'hinhthuclamviec': _hinhThucLamViec,
      'dieuKhoan': _dieuKhoanController.text.trim(),
      'maUngTuyen': maUngTuyen,
      'giaiDoans': giaiDoansPayload,
    };

    final response = await _hopDongService.taoHopDong(payload, maUserNTD: user.userId);

    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (response.isSuccess && response.data != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Khởi tạo dự thảo hợp đồng ${response.data!.maHD} thành công!'),
          backgroundColor: AppColors.success,
        ),
      );
      // Chuyển sang màn hình chi tiết hợp đồng
      await Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (_) => HopDongDetailScreen(maHD: response.data!.maHD),
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(response.message.isNotEmpty ? response.message : 'Tạo hợp đồng thất bại.'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final tenUngVien = widget.applicant.tenUser?.trim().isNotEmpty == true
        ? widget.applicant.tenUser!
        : 'Freelancer #${widget.applicant.applicantId}';

    final tongTyLe = _tongTyLeGiaiNgan;
    final hopLeTyLe = (tongTyLe - 100.0).abs() < 0.01;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text(
          'Tạo hợp đồng giao kết',
          style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
        ),
        backgroundColor: AppColors.surface,
        elevation: 0.5,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Thông tin ứng viên & công việc
              Card(
                elevation: 0,
                color: AppColors.surface,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                  side: const BorderSide(color: AppColors.border),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          const CircleAvatar(
                            backgroundColor: AppColors.lightGreenSurface,
                            child: Icon(Icons.handshake_outlined, color: AppColors.primary),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  widget.applicant.tieude.isNotEmpty
                                      ? widget.applicant.tieude
                                      : 'Công việc #${widget.applicant.jobId}',
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 16,
                                    color: AppColors.heading,
                                  ),
                                ),
                                const SizedBox(height: 2),
                                Text(
                                  'Mã công việc: ${widget.applicant.jobId}',
                                  style: const TextStyle(fontSize: 12, color: AppColors.bodyText),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                      const Divider(height: 24),
                      _itemInfo('Ứng viên:', tenUngVien),
                      if ((widget.applicant.tenTruong ?? '').isNotEmpty)
                        _itemInfo('Trường đại học:', widget.applicant.tenTruong!),
                      if ((widget.applicant.tenChuyenNganh ?? '').isNotEmpty)
                        _itemInfo('Chuyên ngành:', widget.applicant.tenChuyenNganh!),
                      if (widget.applicant.thuLaoDeXuat > 0)
                        _itemInfo(
                          'Thù lao đề xuất:',
                          AppFormatters.formatCurrency(widget.applicant.thuLaoDeXuat),
                        ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),

              // Thù lao & Hình thức làm việc
              const Text(
                'Thỏa thuận thù lao & điều kiện',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 6),
              Card(
                elevation: 0,
                color: AppColors.surface,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                  side: const BorderSide(color: AppColors.border),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    children: [
                      TextFormField(
                        controller: _thuLaoController,
                        keyboardType: TextInputType.number,
                        decoration: const InputDecoration(
                          labelText: 'Số tiền thù lao thỏa thuận (VNĐ) *',
                          prefixIcon: Icon(Icons.payments_outlined, color: AppColors.primary),
                          border: OutlineInputBorder(),
                        ),
                        validator: (value) {
                          if (value == null || value.trim().isEmpty) {
                            return 'Vui lòng nhập số tiền thù lao';
                          }
                          final n = double.tryParse(value.replaceAll(RegExp(r'[^0-9]'), ''));
                          if (n == null || n <= 0) return 'Số tiền phải lớn hơn 0';
                          return null;
                        },
                        onChanged: (_) => setState(() {}),
                      ),
                      const SizedBox(height: 14),
                      DropdownButtonFormField<String>(
                        initialValue: _hinhThucLamViec,
                        decoration: const InputDecoration(
                          labelText: 'Hình thức làm việc',
                          prefixIcon: Icon(Icons.work_outline, color: AppColors.primary),
                          border: OutlineInputBorder(),
                        ),
                        items: const [
                          DropdownMenuItem(value: 'Remote', child: Text('Làm việc từ xa (Remote)')),
                          DropdownMenuItem(value: 'Onsite', child: Text('Làm việc trực tiếp (Onsite)')),
                          DropdownMenuItem(value: 'Hybrid', child: Text('Kết hợp (Hybrid)')),
                        ],
                        onChanged: (val) {
                          if (val != null) setState(() => _hinhThucLamViec = val);
                        },
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),

              // Thời hạn hợp đồng
              const Text(
                'Thời gian thực hiện tổng thể',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 6),
              Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('Ngày bắt đầu', style: TextStyle(fontSize: 12, color: AppColors.bodyText)),
                        const SizedBox(height: 4),
                        InkWell(
                          onTap: _chonNgayBatDau,
                          child: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 14),
                            decoration: BoxDecoration(
                              color: AppColors.surface,
                              borderRadius: BorderRadius.circular(12),
                              border: Border.all(color: AppColors.border),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Text('${_ngayBatDau.day}/${_ngayBatDau.month}/${_ngayBatDau.year}'),
                                const Icon(Icons.calendar_today, size: 18, color: AppColors.primary),
                              ],
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('Hạn chót hoàn thành', style: TextStyle(fontSize: 12, color: AppColors.bodyText)),
                        const SizedBox(height: 4),
                        InkWell(
                          onTap: _chonNgayKetThuc,
                          child: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 14),
                            decoration: BoxDecoration(
                              color: AppColors.surface,
                              borderRadius: BorderRadius.circular(12),
                              border: Border.all(color: AppColors.border),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Text('${_ngayKetThuc.day}/${_ngayKetThuc.month}/${_ngayKetThuc.year}'),
                                const Icon(Icons.calendar_today, size: 18, color: AppColors.primary),
                              ],
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 20),

              // ====================================================
              // KHU VỰC: CÁC MỐC BÀN GIAO & TỶ LỆ GIẢI NGÂN (Milestones)
              // ====================================================
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Các mốc bàn giao & giải ngân',
                          style: TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 15,
                            color: AppColors.heading,
                          ),
                        ),
                        SizedBox(height: 2),
                        Text(
                          'Lộ trình nghiệm thu sản phẩm bàn giao',
                          style: TextStyle(fontSize: 12, color: AppColors.bodyText),
                        ),
                      ],
                    ),
                  ),
                  OutlinedButton.icon(
                    onPressed: _themMocBanGiao,
                    icon: const Icon(Icons.add, size: 16),
                    label: const Text('Thêm mốc'),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppColors.primary,
                      side: const BorderSide(color: AppColors.primary),
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 10),

              // Thanh hiển thị tổng tỷ lệ giải ngân
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: hopLeTyLe ? AppColors.lightGreenSurface : Colors.amber.shade50,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(
                    color: hopLeTyLe ? AppColors.border : Colors.amber.shade300,
                  ),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text(
                          'Tổng tỷ lệ giải ngân các mốc:',
                          style: TextStyle(fontWeight: FontWeight.w600, fontSize: 13),
                        ),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                          decoration: BoxDecoration(
                            color: hopLeTyLe ? AppColors.primary : AppColors.warning,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Text(
                            '${tongTyLe.toStringAsFixed(0)}%',
                            style: const TextStyle(
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
                              fontSize: 12,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    ClipRRect(
                      borderRadius: BorderRadius.circular(6),
                      child: LinearProgressIndicator(
                        value: (tongTyLe / 100).clamp(0.0, 1.0),
                        backgroundColor: Colors.grey.shade300,
                        valueColor: AlwaysStoppedAnimation<Color>(
                          hopLeTyLe ? AppColors.primary : AppColors.warning,
                        ),
                        minHeight: 8,
                      ),
                    ),
                    if (!hopLeTyLe) ...[
                      const SizedBox(height: 6),
                      Text(
                        'Lưu ý: Tổng tỷ lệ giải ngân hiện tại là ${tongTyLe.toStringAsFixed(0)}% (nên đạt đúng 100% để phân bổ đầy đủ thù lao hợp đồng).',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.amber.shade900,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ],
                ),
              ),
              const SizedBox(height: 12),

              // Danh sách các thẻ mốc bàn giao
              ..._danhSachMoc.asMap().entries.map((entry) {
                final index = entry.key;
                final moc = entry.value;
                final tyLeMoc = double.tryParse(moc.tyLeController.text) ?? 0.0;
                final soTienUocTinh = tyLeMoc > 0 ? (_soTienThuLao * tyLeMoc / 100) : 0.0;

                return Card(
                  elevation: 0,
                  margin: const EdgeInsets.only(bottom: 12),
                  color: AppColors.surface,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(14),
                    side: const BorderSide(color: AppColors.border),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(14),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // Header mốc
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Row(
                              children: [
                                const Icon(Icons.flag, color: AppColors.primary, size: 18),
                                const SizedBox(width: 8),
                                Text(
                                  'Mốc bàn giao #${index + 1}',
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 14,
                                    color: AppColors.primaryDark,
                                  ),
                                ),
                              ],
                            ),
                            if (_danhSachMoc.length > 1)
                              IconButton(
                                icon: const Icon(Icons.delete_outline, color: AppColors.error, size: 20),
                                tooltip: 'Xóa mốc này',
                                onPressed: () => _xoaMocBanGiao(index),
                              ),
                          ],
                        ),
                        const Divider(height: 16),

                        // Tên mốc giai đoạn
                        TextFormField(
                          controller: moc.tenController,
                          decoration: const InputDecoration(
                            labelText: 'Tên giai đoạn / mốc bàn giao *',
                            border: OutlineInputBorder(),
                            contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                          ),
                          validator: (v) =>
                              (v == null || v.trim().isEmpty) ? 'Vui lòng nhập tên mốc bàn giao' : null,
                        ),
                        const SizedBox(height: 10),

                        // Hạn chót & Tỷ lệ giải ngân
                        Row(
                          children: [
                            Expanded(
                              flex: 3,
                              child: InkWell(
                                onTap: () => _chonHanChotMoc(index),
                                child: Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 12),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(4),
                                    border: Border.all(color: Colors.grey.shade400),
                                  ),
                                  child: Row(
                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                    children: [
                                      Text(
                                        'Hạn: ${moc.hanChot.day}/${moc.hanChot.month}/${moc.hanChot.year}',
                                        style: const TextStyle(fontSize: 12),
                                      ),
                                      const Icon(Icons.calendar_today, size: 16, color: AppColors.primary),
                                    ],
                                  ),
                                ),
                              ),
                            ),
                            const SizedBox(width: 8),
                            Expanded(
                              flex: 2,
                              child: TextFormField(
                                controller: moc.tyLeController,
                                keyboardType: TextInputType.number,
                                decoration: const InputDecoration(
                                  labelText: 'Giải ngân (%)',
                                  suffixText: '%',
                                  border: OutlineInputBorder(),
                                  contentPadding: EdgeInsets.symmetric(horizontal: 10, vertical: 12),
                                ),
                                onChanged: (_) => setState(() {}),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 10),

                        // Mô tả sản phẩm bàn giao
                        TextFormField(
                          controller: moc.moTaController,
                          maxLines: 2,
                          decoration: const InputDecoration(
                            labelText: 'Mô tả sản phẩm bàn giao & tiêu chuẩn nghiệm thu',
                            hintText: 'Yêu cầu sản phẩm, tài liệu, mã nguồn cần bàn giao...',
                            border: OutlineInputBorder(),
                            contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                          ),
                        ),
                        const SizedBox(height: 8),

                        // Số tiền ước tính cho mốc
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                          decoration: BoxDecoration(
                            color: AppColors.background,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text('Số tiền giải ngân ước tính:', style: TextStyle(fontSize: 12, color: AppColors.bodyText)),
                              Text(
                                AppFormatters.formatCurrency(soTienUocTinh),
                                style: const TextStyle(
                                  fontSize: 12,
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.primary,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              }),
              const SizedBox(height: 16),

              // Điều khoản hợp đồng
              const Text(
                'Điều khoản hợp đồng',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 6),
              TextFormField(
                controller: _dieuKhoanController,
                maxLines: 6,
                decoration: InputDecoration(
                  filled: true,
                  fillColor: AppColors.surface,
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                ),
              ),
              const SizedBox(height: 24),

              // Nút tạo dự thảo hợp đồng
              SizedBox(
                width: double.infinity,
                height: 52,
                child: FilledButton(
                  onPressed: _dangXuLy ? null : _taoHopDong,
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                  ),
                  child: _dangXuLy
                      ? const CircularProgressIndicator(color: Colors.white)
                      : const Text(
                          'Khởi tạo dự thảo hợp đồng',
                          style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                        ),
                ),
              ),
              const SizedBox(height: 24),
            ],
          ),
        ),
      ),
    );
  }

  Widget _itemInfo(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 130,
            child: Text(label, style: const TextStyle(color: Colors.black54, fontSize: 13)),
          ),
          Expanded(
            child: Text(value, style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
          ),
        ],
      ),
    );
  }
}
