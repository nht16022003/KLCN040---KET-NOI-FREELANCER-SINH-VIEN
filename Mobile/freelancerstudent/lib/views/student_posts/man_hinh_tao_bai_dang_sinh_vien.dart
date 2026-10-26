import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../services/dich_vu_bai_dang_sinh_vien.dart';
import '../../services/dich_vu_minh_chung.dart';
import '../../viewmodels/viewmodel_bai_dang_sinh_vien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../profile/man_hinh_minh_chung_the_sv.dart';

class CreateStudentPostScreen extends StatefulWidget {
  const CreateStudentPostScreen({super.key});

  @override
  State<CreateStudentPostScreen> createState() => _CreateStudentPostScreenState();
}

class _CreateStudentPostScreenState extends State<CreateStudentPostScreen> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _tieuDeController = TextEditingController();
  final TextEditingController _kyNangController = TextEditingController();
  final TextEditingController _mucGiaController = TextEditingController();
  final TextEditingController _moTaController = TextEditingController();
  final TextEditingController _customSkillController = TextEditingController();

  final List<String> _selectedSkills = [];
  final List<String> _suggestedSkills = [
    'Flutter',
    'Dart',
    'Figma',
    'Photoshop',
    'ASP.NET Core',
    'SQL Server',
    'HTML/CSS',
    'Python',
    'ReactJS',
    'UI/UX Design',
    'Dịch thuật',
    'Content Writing',
  ];

  final StudentPostService _service = StudentPostService();
  bool _dangXuLy = false;

  @override
  void dispose() {
    _tieuDeController.dispose();
    _kyNangController.dispose();
    _mucGiaController.dispose();
    _moTaController.dispose();
    _customSkillController.dispose();
    super.dispose();
  }

  void _addSkill(String skill) {
    final cleanSkill = skill.trim();
    if (cleanSkill.isEmpty) return;
    if (!_selectedSkills.any((s) => s.toLowerCase() == cleanSkill.toLowerCase())) {
      setState(() {
        _selectedSkills.add(cleanSkill);
      });
    }
  }

  void _removeSkill(String skill) {
    setState(() {
      _selectedSkills.removeWhere((s) => s.toLowerCase() == skill.toLowerCase());
    });
  }

  Future<void> _guiBaiDang() async {
    if (!_formKey.currentState!.validate()) return;

    if (_selectedSkills.isEmpty && _kyNangController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn hoặc nhập ít nhất một kỹ năng.')),
      );
      return;
    }

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập để đăng tin tìm việc.')),
      );
      return;
    }

    setState(() => _dangXuLy = true);

    // Kiểm tra trạng thái xác thực minh chứng sinh viên
    final minhChungService = MinhChungService();
    final xacThucRes = await minhChungService.kiemTraTrangThaiXacThuc(user.userId);
    if (xacThucRes.isSuccess && xacThucRes.data != null && xacThucRes.data!['daXacMinh'] != true) {
      if (!mounted) return;
      setState(() => _dangXuLy = false);
      showDialog(
        context: context,
        builder: (ctx) => AlertDialog(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          title: const Row(
            children: [
              Icon(Icons.shield_outlined, color: Colors.orange),
              SizedBox(width: 8),
              Text('Yêu cầu xác thực', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
            ],
          ),
          content: Text(
            xacThucRes.data!['thongBao']?.toString() ??
                'Tài khoản sinh viên cần hoàn tất xác thực minh chứng Thẻ sinh viên (trạng thái "Đã xác minh") trước khi đăng bài tìm việc!',
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('Để sau'),
            ),
            ElevatedButton(
              style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
              onPressed: () {
                Navigator.pop(ctx);
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (_) => const ManHinhMinhChungTheSV()),
                );
              },
              child: const Text('Xác thực thẻ SV', style: TextStyle(color: Colors.white)),
            ),
          ],
        ),
      );
      return;
    }

    final rawGia = _mucGiaController.text.replaceAll(RegExp(r'[^0-9]'), '');
    final mucGia = double.tryParse(rawGia) ?? 0;

    final kyNang = _selectedSkills.isNotEmpty
        ? _selectedSkills.join(', ')
        : _kyNangController.text.trim();

    setState(() => _dangXuLy = true);

    final duLieu = {
      'maUser': user.userId,
      'tieude': _tieuDeController.text.trim(),
      'kynang': kyNang,
      'mucGiaTu': mucGia,
      'mota': _moTaController.text.trim(),
    };

    final res = await _service.taoBaiDangTimViec(duLieu);

    if (!mounted) return;
    setState(() => _dangXuLy = false);

    if (res.isSuccess) {
      await context.read<StudentPostViewModel>().taiDanhSachBaiDangTimViec();
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message.isNotEmpty ? res.message : 'Đăng tin tìm việc thành công!'),
          backgroundColor: AppColors.primary,
        ),
      );
      Navigator.pop(context);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(res.message.isNotEmpty ? res.message : 'Đăng bài thất bại, vui lòng thử lại.'),
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
        title: const Text('Tạo tin tìm việc'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Card(
                elevation: 0,
                color: AppColors.lightGreenSurface,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                  side: BorderSide(color: AppColors.primary.withAlpha(50)),
                ),
                child: const Padding(
                  padding: EdgeInsets.all(12),
                  child: Row(
                    children: [
                      Icon(Icons.info_outline, color: AppColors.primary, size: 20),
                      SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          'Đăng tin giúp các Nhà tuyển dụng dễ dàng tìm thấy và gửi lời mời hợp tác trực tiếp cho bạn.',
                          style: TextStyle(fontSize: 13, color: AppColors.heading),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),

              // Tiêu đề
              const Text(
                'Tiêu đề bài đăng',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _tieuDeController,
                decoration: InputDecoration(
                  hintText: 'Ví dụ: Lập trình viên Flutter Junior tìm việc part-time',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập tiêu đề bài đăng';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // KHỐI KỸ NĂNG CHUYÊN MÔN (THẺ / CARD)
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: AppColors.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Row(
                      children: [
                        Icon(Icons.stars_rounded, color: AppColors.primary, size: 20),
                        SizedBox(width: 8),
                        Text(
                          'Kỹ năng chuyên môn *',
                          style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
                        ),
                      ],
                    ),
                    const SizedBox(height: 10),

                    // Danh sách kỹ năng đã chọn
                    if (_selectedSkills.isEmpty)
                      const Padding(
                        padding: EdgeInsets.symmetric(vertical: 4),
                        child: Text(
                          'Chưa chọn kỹ năng nào. Nhấn gợi ý bên dưới hoặc tự nhập.',
                          style: TextStyle(fontSize: 12, color: AppColors.bodyText),
                        ),
                      )
                    else
                      Wrap(
                        spacing: 8,
                        runSpacing: 8,
                        children: _selectedSkills.map((skill) {
                          return Chip(
                            label: Text(
                              skill,
                              style: const TextStyle(
                                fontSize: 12,
                                fontWeight: FontWeight.w600,
                                color: AppColors.primaryDark,
                              ),
                            ),
                            backgroundColor: AppColors.lightGreenSurface,
                            deleteIcon: const Icon(Icons.close, size: 16, color: AppColors.primary),
                            onDeleted: () => _removeSkill(skill),
                            side: BorderSide(color: AppColors.primary.withValues(alpha: 0.3)),
                          );
                        }).toList(),
                      ),
                    const SizedBox(height: 12),

                    // Gợi ý kỹ năng cơ bản
                    const Text(
                      'Gợi ý kỹ năng phổ biến:',
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.w600,
                        color: AppColors.bodyText,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Wrap(
                      spacing: 6,
                      runSpacing: 6,
                      children: _suggestedSkills.map((skill) {
                        final daChon = _selectedSkills.any(
                          (s) => s.toLowerCase() == skill.toLowerCase(),
                        );
                        return ActionChip(
                          label: Text(
                            '+ $skill',
                            style: TextStyle(
                              fontSize: 12,
                              fontWeight: FontWeight.w600,
                              color: daChon ? AppColors.bodyText : AppColors.primary,
                            ),
                          ),
                          backgroundColor: daChon ? Colors.grey.shade200 : AppColors.surface,
                          side: BorderSide(
                            color: daChon ? Colors.grey.shade300 : AppColors.primary.withValues(alpha: 0.3),
                          ),
                          onPressed: daChon ? null : () => _addSkill(skill),
                        );
                      }).toList(),
                    ),
                    const SizedBox(height: 10),

                    // Ô nhập kỹ năng tự chọn
                    Row(
                      children: [
                        Expanded(
                          child: TextField(
                            controller: _customSkillController,
                            decoration: InputDecoration(
                              hintText: 'Nhập kỹ năng khác...',
                              hintStyle: const TextStyle(fontSize: 13),
                              border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                              contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                              isDense: true,
                            ),
                            onSubmitted: (val) {
                              _addSkill(val);
                              _customSkillController.clear();
                            },
                          ),
                        ),
                        const SizedBox(width: 8),
                        IconButton.filled(
                          tooltip: 'Thêm kỹ năng',
                          icon: const Icon(Icons.add, size: 20),
                          style: IconButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            foregroundColor: Colors.white,
                          ),
                          onPressed: () {
                            _addSkill(_customSkillController.text);
                            _customSkillController.clear();
                          },
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),

              // Mức thù lao mong muốn
              const Text(
                'Mức thù lao mong muốn (VND)',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _mucGiaController,
                keyboardType: TextInputType.number,
                decoration: InputDecoration(
                  hintText: 'Ví dụ: 3000000',
                  suffixText: 'VND',
                  prefixIcon: const Icon(Icons.attach_money),
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập mức thù lao mong muốn';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Mô tả chi tiết
              const Text(
                'Mô tả kinh nghiệm & nguyện vọng',
                style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.heading),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _moTaController,
                maxLines: 5,
                decoration: InputDecoration(
                  hintText: 'Mô tả chi tiết năng lực, kinh nghiệm qua các đồ án môn học, thời gian rảnh trong tuần...',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập mô tả chi tiết';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 24),

              // Nút đăng bài
              FilledButton(
                onPressed: _dangXuLy ? null : _guiBaiDang,
                style: FilledButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  backgroundColor: AppColors.primary,
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                ),
                child: _dangXuLy
                    ? const SizedBox(
                        height: 20,
                        width: 20,
                        child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                      )
                    : const Text(
                        'ĐĂNG BÀI TÌM VIỆC',
                        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
