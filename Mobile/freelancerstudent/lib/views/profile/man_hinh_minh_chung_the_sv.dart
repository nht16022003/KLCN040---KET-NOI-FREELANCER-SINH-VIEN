import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../services/dich_vu_minh_chung.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';

class ManHinhMinhChungTheSV extends StatefulWidget {
  const ManHinhMinhChungTheSV({super.key});

  @override
  State<ManHinhMinhChungTheSV> createState() => _ManHinhMinhChungTheSVState();
}

class _ManHinhMinhChungTheSVState extends State<ManHinhMinhChungTheSV> {
  final MinhChungService _minhChungService = MinhChungService();
  final ImagePicker _picker = ImagePicker();

  File? _selectedImage;
  bool _isLoading = false;
  bool _isScanning = false;
  bool _isSaving = false;
  String? _errorMessage;

  // Dữ liệu hồ sơ hiện tại và danh mục
  Map<String, dynamic>? _thongTinHienTai;
  List<Map<String, dynamic>> _danhSachChuyenNganh = [];
  List<Map<String, dynamic>> _lichSuMinhChung = [];

  // Dữ liệu OCR nhận diện được
  Map<String, dynamic>? _ocrData;
  double _doTinCay = 0.0;
  String _fileMinhChungUrl = '';

  // Controllers cho form xác nhận
  final TextEditingController _hoTenCtrl = TextEditingController();
  final TextEditingController _mssvCtrl = TextEditingController();
  final TextEditingController _tenTruongCtrl = TextEditingController();
  final TextEditingController _nienKhoaCtrl = TextEditingController();
  String? _selectedMaChuyenNganh;
  int? _selectedNamThu;
  DateTime? _selectedNgaySinh;
  bool _dongYCapNhat = true;

  @override
  void initState() {
    super.initState();
    _loadInitialData();
  }

  @override
  void dispose() {
    _hoTenCtrl.dispose();
    _mssvCtrl.dispose();
    _tenTruongCtrl.dispose();
    _nienKhoaCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadInitialData() async {
    final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
    if (userId <= 0) return;

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final futures = await Future.wait([
        _minhChungService.layThongTinHienTai(userId),
        _minhChungService.layDanhSachChuyenNganh(),
        _minhChungService.layLichSuMinhChung(userId),
      ]);

      if (!mounted) return;

      final resThongTin = futures[0] as dynamic;
      final resChuyenNganh = futures[1] as dynamic;
      final resLichSu = futures[2] as dynamic;

      setState(() {
        if (resThongTin.isSuccess && resThongTin.data != null) {
          _thongTinHienTai = resThongTin.data;
        }
        if (resChuyenNganh.isSuccess && resChuyenNganh.data != null) {
          _danhSachChuyenNganh = List<Map<String, dynamic>>.from(resChuyenNganh.data);
        }
        if (resLichSu.isSuccess && resLichSu.data != null) {
          _lichSuMinhChung = List<Map<String, dynamic>>.from(resLichSu.data);
        }
        _isLoading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _errorMessage = 'Lỗi tải thông tin: $e';
      });
    }
  }

  Future<void> _pickImage(ImageSource source) async {
    try {
      final XFile? file = await _picker.pickImage(
        source: source,
        maxWidth: 1920,
        maxHeight: 1080,
        imageQuality: 90,
      );

      if (file == null) return;

      setState(() {
        _selectedImage = File(file.path);
        _ocrData = null;
        _errorMessage = null;
      });
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Không thể chọn ảnh: $e'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  Future<void> _startOcrScan() async {
    if (_selectedImage == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng chọn hoặc chụp ảnh thẻ sinh viên.'),
          backgroundColor: AppColors.warning,
        ),
      );
      return;
    }

    final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
    if (userId <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Không xác định được người dùng đăng nhập.'),
          backgroundColor: AppColors.error,
        ),
      );
      return;
    }

    setState(() {
      _isScanning = true;
      _errorMessage = null;
      _ocrData = null;
    });

    final res = await _minhChungService.quetTheSinhVien(_selectedImage!, userId);

    if (!mounted) return;

    setState(() {
      _isScanning = false;
    });

    if (res.isSuccess && res.data != null) {
      final data = res.data!;
      setState(() {
        _ocrData = data;
        _doTinCay = (data['doTinCay'] as num?)?.toDouble() ?? 0.0;
        _fileMinhChungUrl = data['fileMinhChungUrl']?.toString() ?? '';

        // Điền dữ liệu nhận diện vào các form controller
        _hoTenCtrl.text = data['hoVaTen']?.toString() ?? _thongTinHienTai?['hoTenHienTai']?.toString() ?? '';
        _mssvCtrl.text = data['mssv']?.toString() ?? 'Không tìm thấy MSSV';
        _tenTruongCtrl.text = data['tenTruong']?.toString() ?? _thongTinHienTai?['tenTruong']?.toString() ?? '';
        _nienKhoaCtrl.text = data['nienKhoa']?.toString() ?? '';

        final namThuRaw = data['namThu'];
        if (namThuRaw != null && namThuRaw is int && namThuRaw >= 1 && namThuRaw <= 6) {
          _selectedNamThu = namThuRaw;
        }

        final maChuyenNganhGoiY = data['maChuyenNganhGoiY']?.toString();
        if (maChuyenNganhGoiY != null && maChuyenNganhGoiY.isNotEmpty) {
          if (!_danhSachChuyenNganh.any((c) => c['maChuyenNganh'] == maChuyenNganhGoiY)) {
            _danhSachChuyenNganh.add({
              'maChuyenNganh': maChuyenNganhGoiY,
              'tenChuyenNganh': data['tenChuyenNganhGoiY']?.toString() ?? maChuyenNganhGoiY,
            });
          }
          _selectedMaChuyenNganh = maChuyenNganhGoiY;
        }

        final ngaySinhRaw = data['ngaySinh']?.toString();
        if (ngaySinhRaw != null && ngaySinhRaw.isNotEmpty) {
          _selectedNgaySinh = DateTime.tryParse(ngaySinhRaw);
        }
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message.isNotEmpty ? res.message : 'Nhận diện thẻ sinh viên thành công!'),
          backgroundColor: AppColors.success,
        ),
      );
    } else {
      setState(() {
        _errorMessage = res.message.isNotEmpty ? res.message : 'Không thể đọc thông tin trên thẻ sinh viên. Vui lòng chụp rõ lại.';
      });
    }
  }

  Future<void> _submitConfirmation() async {
    final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
    if (userId <= 0) return;

    if (!_dongYCapNhat) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng tích chọn đồng ý cập nhật hồ sơ.'),
          backgroundColor: AppColors.warning,
        ),
      );
      return;
    }

    final hoTen = _hoTenCtrl.text.trim();
    final tenTruong = _tenTruongCtrl.text.trim();

    if (hoTen.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Họ và tên sinh viên không được để trống.'),
          backgroundColor: AppColors.warning,
        ),
      );
      return;
    }

    if (tenTruong.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Tên trường học không được để trống.'),
          backgroundColor: AppColors.warning,
        ),
      );
      return;
    }

    setState(() {
      _isSaving = true;
    });

    final payload = {
      'maUser': userId,
      'mssv': _mssvCtrl.text.trim().isNotEmpty ? _mssvCtrl.text.trim() : null,
      'hoVaTen': hoTen,
      'tenTruong': tenTruong,
      'maChuyenNganh': _selectedMaChuyenNganh,
      'nienKhoa': _nienKhoaCtrl.text.trim().isNotEmpty ? _nienKhoaCtrl.text.trim() : null,
      'namThu': _selectedNamThu,
      'ngaySinh': _selectedNgaySinh != null ? DateFormat('yyyy-MM-dd').format(_selectedNgaySinh!) : null,
      'fileMinhChungUrl': _fileMinhChungUrl,
      'dongYCapNhatHoSo': true,
    };

    final res = await _minhChungService.xacNhanTheSinhVien(payload);

    if (!mounted) return;

    setState(() {
      _isSaving = false;
    });

    if (res.isSuccess) {
      await showDialog(
        context: context,
        builder: (ctx) => AlertDialog(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          title: const Row(
            children: [
              Icon(Icons.check_circle, color: AppColors.primary),
              SizedBox(width: 8),
              Text('Gửi minh chứng thành công'),
            ],
          ),
          content: const Text(
            'Minh chứng Thẻ sinh viên đã được gửi và chuyển sang trạng thái "Đang gửi" (chờ duyệt). Khi Ban quản trị phê duyệt, hồ sơ của bạn sẽ tự động cập nhật và các chức năng sẽ được mở khóa!',
          ),
          actions: [
            FilledButton(
              onPressed: () => Navigator.pop(ctx),
              style: FilledButton.styleFrom(backgroundColor: AppColors.primary),
              child: const Text('Đóng'),
            ),
          ],
        ),
      );

      // Tải lại dữ liệu
      _loadInitialData();
      setState(() {
        _ocrData = null;
        _selectedImage = null;
      });
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message.isNotEmpty ? res.message : 'Cập nhật thất bại.'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text(
          'Minh chứng Thẻ sinh viên',
          style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
        ),
        backgroundColor: AppColors.surface,
        elevation: 0.5,
        iconTheme: const IconThemeData(color: AppColors.heading),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  _buildNoticeBanner(),
                  const SizedBox(height: 16),
                  _buildCurrentProfileCard(),
                  const SizedBox(height: 16),
                  _buildImagePickerCard(),
                  if (_ocrData != null) ...[
                    const SizedBox(height: 16),
                    _buildVerificationForm(),
                  ],
                  const SizedBox(height: 24),
                  _buildEvidenceHistoryCard(),
                  const SizedBox(height: 40),
                ],
              ),
            ),
    );
  }

  Widget _buildNoticeBanner() {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.lightGreenSurface,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.border),
      ),
      child: const Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(Icons.shield_outlined, color: AppColors.primary, size: 24),
          SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Quy chuẩn Thẻ Sinh Viên chính quy',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    color: AppColors.heading,
                    fontSize: 13,
                  ),
                ),
                SizedBox(height: 4),
                Text(
                  'Chỉ dùng thẻ sinh viên do trường cấp (không dùng CCCD). Nhận diện OCR cục bộ an toàn, bạn có quyền kiểm tra và chỉnh sửa trước khi xác nhận lưu.',
                  style: TextStyle(fontSize: 12, color: AppColors.bodyText),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCurrentProfileCard() {
    final hoTen = _thongTinHienTai?['hoTenHienTai']?.toString() ?? 'Chưa cập nhật';
    final tenTruong = _thongTinHienTai?['tenTruong']?.toString() ?? 'Chưa cập nhật';
    final chuyenNganh = _thongTinHienTai?['chuyenNganh']?.toString() ?? 'Chưa cập nhật';

    return Card(
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
            const Row(
              children: [
                Icon(Icons.person_pin_rounded, color: AppColors.primary),
                SizedBox(width: 8),
                Text(
                  'Hồ sơ Freelancer hiện tại',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 14,
                    color: AppColors.heading,
                  ),
                ),
              ],
            ),
            const Divider(height: 20),
            _buildProfileRow('Họ và tên:', hoTen),
            _buildProfileRow('Trường ĐH/CĐ:', tenTruong),
            _buildProfileRow('Chuyên ngành:', chuyenNganh),
          ],
        ),
      ),
    );
  }

  Widget _buildProfileRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 110,
            child: Text(
              label,
              style: const TextStyle(fontSize: 12, color: AppColors.mutedText),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w600,
                color: AppColors.heading,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildImagePickerCard() {
    return Card(
      elevation: 0,
      color: AppColors.surface,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: const BorderSide(color: AppColors.border),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            const Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  '1. Tải ảnh Thẻ sinh viên',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 14,
                    color: AppColors.heading,
                  ),
                ),
                Text(
                  'JPG, PNG, WEBP',
                  style: TextStyle(fontSize: 11, color: AppColors.mutedText),
                ),
              ],
            ),
            const SizedBox(height: 12),
            if (_selectedImage == null) ...[
              Container(
                height: 140,
                decoration: BoxDecoration(
                  color: AppColors.background,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: AppColors.border, style: BorderStyle.solid),
                ),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.badge_outlined, size: 40, color: AppColors.primary),
                    const SizedBox(height: 8),
                    const Text(
                      'Chụp hoặc chọn ảnh thẻ sinh viên',
                      style: TextStyle(fontSize: 13, color: AppColors.bodyText),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        OutlinedButton.icon(
                          onPressed: () => _pickImage(ImageSource.camera),
                          icon: const Icon(Icons.camera_alt, size: 16),
                          label: const Text('Camera'),
                          style: OutlinedButton.styleFrom(
                            foregroundColor: AppColors.primary,
                            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
                          ),
                        ),
                        const SizedBox(width: 8),
                        OutlinedButton.icon(
                          onPressed: () => _pickImage(ImageSource.gallery),
                          icon: const Icon(Icons.photo_library, size: 16),
                          label: const Text('Thư viện'),
                          style: OutlinedButton.styleFrom(
                            foregroundColor: AppColors.primary,
                            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ] else ...[
              Stack(
                children: [
                  ClipRRect(
                    borderRadius: BorderRadius.circular(12),
                    child: Image.file(
                      _selectedImage!,
                      height: 190,
                      width: double.infinity,
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    top: 8,
                    right: 8,
                    child: CircleAvatar(
                      backgroundColor: Colors.black54,
                      radius: 16,
                      child: IconButton(
                        icon: const Icon(Icons.close, size: 16, color: Colors.white),
                        onPressed: () {
                          setState(() {
                            _selectedImage = null;
                            _ocrData = null;
                          });
                        },
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              FilledButton.icon(
                onPressed: _isScanning ? null : _startOcrScan,
                icon: _isScanning
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                      )
                    : const Icon(Icons.document_scanner),
                label: Text(_isScanning ? 'Đang trích xuất OCR...' : 'Bắt đầu quét & Nhận diện thẻ'),
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                ),
              ),
            ],
            if (_errorMessage != null) ...[
              const SizedBox(height: 10),
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: AppColors.error.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.error_outline, color: AppColors.error, size: 18),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        _errorMessage!,
                        style: const TextStyle(color: AppColors.error, fontSize: 12),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildVerificationForm() {
    return Card(
      elevation: 0,
      color: AppColors.surface,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: const BorderSide(color: AppColors.primary, width: 1.5),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  '2. Kiểm tra & Xác nhận thông tin',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 14,
                    color: AppColors.heading,
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                  decoration: BoxDecoration(
                    color: _doTinCay >= 80 ? AppColors.lightGreenSurface : Colors.amber.withValues(alpha: 0.2),
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Text(
                    'Độ tin cậy: ${_doTinCay.toStringAsFixed(0)}%',
                    style: TextStyle(
                      fontSize: 11,
                      fontWeight: FontWeight.bold,
                      color: _doTinCay >= 80 ? AppColors.primary : Colors.amber.shade900,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 14),

            // Họ và tên
            TextFormField(
              controller: _hoTenCtrl,
              decoration: const InputDecoration(
                labelText: 'Họ và tên sinh viên *',
                prefixIcon: Icon(Icons.person, size: 18),
                border: OutlineInputBorder(),
                isDense: true,
              ),
            ),
            const SizedBox(height: 10),

            // MSSV (Chỉ hiển thị đối chiếu)
            TextFormField(
              controller: _mssvCtrl,
              readOnly: true,
              decoration: const InputDecoration(
                labelText: 'Mã số sinh viên (Đối chiếu)',
                prefixIcon: Icon(Icons.qr_code, size: 18),
                border: OutlineInputBorder(),
                isDense: true,
                filled: true,
                fillColor: Color(0xFFF1F5F2),
              ),
            ),
            const SizedBox(height: 10),

            // Tên trường
            TextFormField(
              controller: _tenTruongCtrl,
              decoration: const InputDecoration(
                labelText: 'Trường Đại học / Cao đẳng *',
                prefixIcon: Icon(Icons.school, size: 18),
                border: OutlineInputBorder(),
                isDense: true,
              ),
            ),
            const SizedBox(height: 10),

            // Chuyên ngành đào tạo
            DropdownButtonFormField<String>(
              key: ValueKey('chuyen_nganh_$_selectedMaChuyenNganh'),
              initialValue: _selectedMaChuyenNganh,
              isExpanded: true,
              decoration: const InputDecoration(
                labelText: 'Chuyên ngành đào tạo *',
                prefixIcon: Icon(Icons.book, size: 18),
                border: OutlineInputBorder(),
                isDense: true,
              ),
              items: _danhSachChuyenNganh.map((cn) {
                return DropdownMenuItem<String>(
                  value: cn['maChuyenNganh']?.toString(),
                  child: Text(
                    '${cn['tenChuyenNganh']} (${cn['maChuyenNganh']})',
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(fontSize: 13),
                  ),
                );
              }).toList(),
              onChanged: (val) {
                setState(() {
                  _selectedMaChuyenNganh = val;
                });
              },
            ),
            const SizedBox(height: 10),

            // Niên khóa & Năm thứ
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _nienKhoaCtrl,
                    decoration: const InputDecoration(
                      labelText: 'Niên khóa (VD: 2021-2025)',
                      border: OutlineInputBorder(),
                      isDense: true,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: DropdownButtonFormField<int>(
                    key: ValueKey('nam_thu_$_selectedNamThu'),
                    initialValue: _selectedNamThu,
                    decoration: const InputDecoration(
                      labelText: 'Năm thứ',
                      border: OutlineInputBorder(),
                      isDense: true,
                    ),
                    items: [1, 2, 3, 4, 5, 6].map((nam) {
                      return DropdownMenuItem<int>(
                        value: nam,
                        child: Text('Năm $nam'),
                      );
                    }).toList(),
                    onChanged: (val) {
                      setState(() {
                        _selectedNamThu = val;
                      });
                    },
                  ),
                ),
              ],
            ),
            const SizedBox(height: 10),

            // Ngày sinh
            ListTile(
              contentPadding: EdgeInsets.zero,
              dense: true,
              title: const Text('Ngày sinh', style: TextStyle(fontSize: 13)),
              subtitle: Text(
                _selectedNgaySinh != null
                    ? DateFormat('dd/MM/yyyy').format(_selectedNgaySinh!)
                    : 'Chưa chọn',
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              trailing: IconButton(
                icon: const Icon(Icons.calendar_month, color: AppColors.primary),
                onPressed: () async {
                  final picked = await showDatePicker(
                    context: context,
                    initialDate: _selectedNgaySinh ?? DateTime(2003, 1, 1),
                    firstDate: DateTime(1980),
                    lastDate: DateTime.now(),
                  );
                  if (picked != null) {
                    setState(() {
                      _selectedNgaySinh = picked;
                    });
                  }
                },
              ),
            ),

            // Checkbox đồng ý
            CheckboxListTile(
              contentPadding: EdgeInsets.zero,
              dense: true,
              value: _dongYCapNhat,
              onChanged: (val) {
                setState(() {
                  _dongYCapNhat = val ?? false;
                });
              },
              title: const Text(
                'Tôi xác nhận thông tin thẻ sinh viên là chính xác và đồng ý lưu minh chứng kèm cập nhật hồ sơ.',
                style: TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
              ),
            ),
            const SizedBox(height: 12),

            // Nút Lưu và Xác nhận
            FilledButton.icon(
              onPressed: _isSaving ? null : _submitConfirmation,
              icon: _isSaving
                  ? const SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                    )
                  : const Icon(Icons.check_circle),
              label: Text(_isSaving ? 'Đang cập nhật hồ sơ...' : 'Xác nhận & Cập nhật hồ sơ'),
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                padding: const EdgeInsets.symmetric(vertical: 13),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildEvidenceHistoryCard() {
    return Card(
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
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Row(
                  children: [
                    Icon(Icons.history, color: AppColors.primary),
                    SizedBox(width: 8),
                    Text(
                      'Lịch sử minh chứng đã gửi',
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 14,
                        color: AppColors.heading,
                      ),
                    ),
                  ],
                ),
                Text(
                  '${_lichSuMinhChung.length} bản ghi',
                  style: const TextStyle(fontSize: 12, color: AppColors.mutedText),
                ),
              ],
            ),
            const Divider(height: 20),
            if (_lichSuMinhChung.isEmpty)
              const Padding(
                padding: EdgeInsets.symmetric(vertical: 12),
                child: Center(
                  child: Text(
                    'Chưa có lịch sử minh chứng nào.',
                    style: TextStyle(fontSize: 12, color: AppColors.mutedText),
                  ),
                ),
              )
            else
              ..._lichSuMinhChung.map((item) {
                final loai = item['loaiMinhChung']?.toString() ?? 'Thẻ sinh viên';
                final trangThai = item['trangThaiGuiMinhChung']?.toString() ?? 'Đang gửi';
                final ngayNopRaw = item['ngayNop']?.toString();
                final ngayNop = ngayNopRaw != null ? DateTime.tryParse(ngayNopRaw) : null;
                final ngayNopStr = ngayNop != null ? DateFormat('dd/MM/yyyy HH:mm').format(ngayNop) : '';

                Color statusColor = AppColors.warning;
                if (trangThai == 'Đã xác minh') statusColor = AppColors.success;
                if (trangThai == 'Đã từ chối') statusColor = AppColors.error;

                return Container(
                  margin: const EdgeInsets.only(bottom: 8),
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: AppColors.background,
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: AppColors.border),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            loai,
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 13,
                              color: AppColors.heading,
                            ),
                          ),
                          if (ngayNopStr.isNotEmpty)
                            Text(
                              'Nộp ngày: $ngayNopStr',
                              style: const TextStyle(fontSize: 11, color: AppColors.mutedText),
                            ),
                        ],
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(
                          color: statusColor.withValues(alpha: 0.12),
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Text(
                          trangThai,
                          style: TextStyle(
                            fontSize: 11,
                            fontWeight: FontWeight.bold,
                            color: statusColor,
                          ),
                        ),
                      ),
                    ],
                  ),
                );
              }),
          ],
        ),
      ),
    );
  }
}
