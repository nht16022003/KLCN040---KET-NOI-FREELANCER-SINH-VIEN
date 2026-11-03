import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_bai_dang_sinh_vien.dart';
import '../../viewmodels/viewmodel_bai_dang_sinh_vien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import 'man_hinh_chi_tiet_bai_dang_sinh_vien.dart';
import 'man_hinh_tao_bai_dang_sinh_vien.dart';

class QuanLyBaiDangSinhVienScreen extends StatefulWidget {
  const QuanLyBaiDangSinhVienScreen({super.key});

  @override
  State<QuanLyBaiDangSinhVienScreen> createState() => _QuanLyBaiDangSinhVienScreenState();
}

class _QuanLyBaiDangSinhVienScreenState extends State<QuanLyBaiDangSinhVienScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _taiDanhSach();
    });
  }

  void _taiDanhSach() {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      context.read<StudentPostViewModel>().taiDanhSachBaiDangCuaToi(user.userId);
    }
  }

  Future<void> _moFormBaiDang({StudentPostModel? baiDang}) async {
    final result = await Navigator.push<bool>(
      context,
      MaterialPageRoute(
        builder: (_) => CreateStudentPostScreen(baiDangCanSua: baiDang),
      ),
    );
    if (result == true && mounted) {
      _taiDanhSach();
    }
  }

  Future<void> _doiTrangThai(StudentPostModel post) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    final trangThaiMoi = post.trangThai == 'DaAn' ? 'HienThi' : 'DaAn';
    final vm = context.read<StudentPostViewModel>();
    final ok = await vm.doiTrangThaiBaiDang(post.maBaiDang, trangThaiMoi, user.userId);

    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            ok
                ? (trangThaiMoi == 'DaAn' ? 'Đã ẩn bài đăng.' : 'Đã hiển thị bài đăng.')
                : (vm.thongBaoLoi ?? 'Thao tác thất bại.'),
          ),
        ),
      );
    }
  }

  Future<void> _xoa(StudentPostModel post) async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    final xacNhan = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
        title: const Text('Xác nhận xóa'),
        content: Text('Bạn có chắc chắn muốn xóa bài đăng "${post.tieuDe}"?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(
              backgroundColor: AppColors.error,
              shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
            ),
            child: const Text('Xóa'),
          ),
        ],
      ),
    );

    if (xacNhan != true || !mounted) return;

    final vm = context.read<StudentPostViewModel>();
    final ok = await vm.xoaBaiDang(post.maBaiDang, user.userId);

    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            ok ? 'Đã xóa bài đăng thành công.' : (vm.thongBaoLoi ?? 'Xóa thất bại.'),
          ),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<StudentPostViewModel>();

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Quản lý bài tìm việc'),
        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
        actions: [
          TextButton(
            onPressed: () => _moFormBaiDang(),
            style: TextButton.styleFrom(
              foregroundColor: AppColors.primary,
              shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
            ),
            child: const Text('+ Tạo bài mới'),
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () async => _taiDanhSach(),
        child: Builder(
          builder: (context) {
            if (vm.dangTai && vm.danhSachBaiDangCuaToi.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }

            if (vm.thongBaoLoi != null && vm.danhSachBaiDangCuaToi.isEmpty) {
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
                        onPressed: _taiDanhSach,
                        style: OutlinedButton.styleFrom(
                          shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                        ),
                        child: const Text('Thử lại'),
                      ),
                    ],
                  ),
                ),
              );
            }

            if (vm.danhSachBaiDangCuaToi.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Text(
                        'Bạn chưa đăng bài tìm việc nào.',
                        style: TextStyle(color: AppColors.bodyText),
                      ),
                      const SizedBox(height: 12),
                      FilledButton(
                        onPressed: () => _moFormBaiDang(),
                        style: FilledButton.styleFrom(
                          backgroundColor: AppColors.primary,
                          shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                        ),
                        child: const Text('+ Đăng bài tìm việc ngay'),
                      ),
                    ],
                  ),
                ),
              );
            }

            return ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: vm.danhSachBaiDangCuaToi.length,
              separatorBuilder: (_, _) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final item = vm.danhSachBaiDangCuaToi[index];
                return _buildItemBaiDang(item);
              },
            );
          },
        ),
      ),
    );
  }

  Widget _buildItemBaiDang(StudentPostModel item) {
    final daAn = item.trangThai == 'DaAn';

    return InkWell(
      onTap: () async {
        await Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => StudentPostDetailScreen(post: item),
          ),
        );
        if (mounted) _taiDanhSach();
      },
      child: Container(
        decoration: BoxDecoration(
          color: AppColors.surface,
          border: Border.all(color: AppColors.cardBorder),
        ),
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    item.tieuDe,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: daAn ? Colors.grey.shade100 : Colors.green.shade50,
                    border: Border.all(
                      color: daAn ? Colors.grey.shade400 : Colors.green,
                    ),
                  ),
                  child: Text(
                    daAn ? 'Đang ẩn' : 'Hiển thị',
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                      color: daAn ? Colors.grey.shade700 : Colors.green.shade800,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            if (item.moTa.isNotEmpty) ...[
              Text(
                item.moTa,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(fontSize: 13, color: AppColors.bodyText),
              ),
              const SizedBox(height: 8),
            ],
            if (item.kyNangList.isNotEmpty) ...[
              Wrap(
                spacing: 6,
                runSpacing: 4,
                children: item.kyNangList
                    .map(
                      (k) => Container(
                        padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                        decoration: BoxDecoration(
                          color: Colors.blue.shade50,
                          border: Border.all(color: Colors.blue.shade200),
                        ),
                        child: Text(
                          k,
                          style: TextStyle(fontSize: 11, color: Colors.blue.shade900),
                        ),
                      ),
                    )
                    .toList(),
              ),
              const SizedBox(height: 8),
            ],
            if (item.mucGiaTu > 0)
              Text(
                'Mức giá từ: ${AppFormatters.formatCurrency(item.mucGiaTu)}',
                style: const TextStyle(
                  fontSize: 13,
                  fontWeight: FontWeight.w600,
                  color: AppColors.primary,
                ),
              ),
            const SizedBox(height: 12),
            const Divider(height: 1),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                TextButton(
                  onPressed: () => _doiTrangThai(item),
                  style: TextButton.styleFrom(
                    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                  ),
                  child: Text(daAn ? 'Hiện tin' : 'Ẩn tin'),
                ),
                const SizedBox(width: 8),
                OutlinedButton(
                  onPressed: () => _moFormBaiDang(baiDang: item),
                  style: OutlinedButton.styleFrom(
                    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                  ),
                  child: const Text('Chỉnh sửa'),
                ),
                const SizedBox(width: 8),
                OutlinedButton(
                  onPressed: () => _xoa(item),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppColors.error,
                    side: const BorderSide(color: AppColors.error),
                    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                  ),
                  child: const Text('Xóa'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class FormBaiDangTimViecWidget extends StatefulWidget {
  final StudentPostModel? baiDang;
  final VoidCallback onThanhCong;

  const FormBaiDangTimViecWidget({
    super.key,
    this.baiDang,
    required this.onThanhCong,
  });

  @override
  State<FormBaiDangTimViecWidget> createState() => _FormBaiDangTimViecWidgetState();
}

class _FormBaiDangTimViecWidgetState extends State<FormBaiDangTimViecWidget> {
  final _formKey = GlobalKey<FormState>();
  final _tieuDeCtrl = TextEditingController();
  final _moTaCtrl = TextEditingController();
  final _kyNangCtrl = TextEditingController();
  final _mucGiaCtrl = TextEditingController();

  bool _dangXuLy = false;
  String? _loi;

  @override
  void initState() {
    super.initState();
    if (widget.baiDang != null) {
      _tieuDeCtrl.text = widget.baiDang!.tieuDe;
      _moTaCtrl.text = widget.baiDang!.moTa;
      _kyNangCtrl.text = widget.baiDang!.kyNang;
      if (widget.baiDang!.mucGiaTu > 0) {
        _mucGiaCtrl.text = widget.baiDang!.mucGiaTu.toInt().toString();
      }
    }
  }

  @override
  void dispose() {
    _tieuDeCtrl.dispose();
    _moTaCtrl.dispose();
    _kyNangCtrl.dispose();
    _mucGiaCtrl.dispose();
    super.dispose();
  }

  Future<void> _luu() async {
    if (!_formKey.currentState!.validate()) return;

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      setState(() => _loi = 'Vui lòng đăng nhập.');
      return;
    }

    setState(() {
      _dangXuLy = true;
      _loi = null;
    });

    final vm = context.read<StudentPostViewModel>();
    final payload = {
      'maUser': user.userId,
      'tieude': _tieuDeCtrl.text.trim(),
      'mota': _moTaCtrl.text.trim(),
      'kynang': _kyNangCtrl.text.trim(),
      'mucGiaTu': double.tryParse(_mucGiaCtrl.text.trim()) ?? 0,
    };

    bool ok = false;
    if (widget.baiDang != null) {
      ok = await vm.capNhatBaiDang(widget.baiDang!.maBaiDang, payload, user.userId);
    } else {
      ok = await vm.taoBaiDang(payload);
    }

    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (ok) {
      widget.onThanhCong();
    } else {
      setState(() => _loi = vm.thongBaoLoi ?? 'Có lỗi xảy ra khi lưu bài đăng.');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      color: AppColors.surface,
      child: Form(
        key: _formKey,
        child: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    widget.baiDang != null ? 'Cập nhật bài tìm việc' : 'Đăng tin tìm việc mới',
                    style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  TextButton(
                    onPressed: () => Navigator.pop(context),
                    style: TextButton.styleFrom(
                      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                    ),
                    child: const Text('Đóng'),
                  ),
                ],
              ),
              const Divider(height: 20),
              TextFormField(
                controller: _tieuDeCtrl,
                decoration: const InputDecoration(
                  labelText: 'Tiêu đề bài đăng (*)',
                  hintText: 'VD: Lập trình viên Flutter kinh nghiệm 1 năm tìm việc part-time',
                  border: OutlineInputBorder(borderRadius: BorderRadius.zero),
                ),
                validator: (v) => (v == null || v.trim().isEmpty) ? 'Vui lòng nhập tiêu đề' : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _moTaCtrl,
                maxLines: 4,
                decoration: const InputDecoration(
                  labelText: 'Mô tả chi tiết khả năng & thời gian làm việc',
                  hintText: 'Mô tả chi tiết kinh nghiệm, định hướng công việc và thời gian có thể nhận dự án...',
                  border: OutlineInputBorder(borderRadius: BorderRadius.zero),
                ),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _kyNangCtrl,
                decoration: const InputDecoration(
                  labelText: 'Kỹ năng (cách nhau bởi dấu phẩy)',
                  hintText: 'VD: Flutter, Dart, Firebase, RESTful API',
                  border: OutlineInputBorder(borderRadius: BorderRadius.zero),
                ),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _mucGiaCtrl,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                  labelText: 'Mức thù lao mong muốn (VNĐ)',
                  hintText: 'VD: 3000000',
                  border: OutlineInputBorder(borderRadius: BorderRadius.zero),
                ),
              ),
              if (_loi != null) ...[
                const SizedBox(height: 10),
                Text(_loi!, style: const TextStyle(color: AppColors.error, fontSize: 13)),
              ],
              const SizedBox(height: 16),
              SizedBox(
                width: double.infinity,
                height: 44,
                child: FilledButton(
                  onPressed: _dangXuLy ? null : _luu,
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                  ),
                  child: Text(_dangXuLy ? 'Đang lưu...' : (widget.baiDang != null ? 'Cập nhật' : 'Đăng bài')),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
