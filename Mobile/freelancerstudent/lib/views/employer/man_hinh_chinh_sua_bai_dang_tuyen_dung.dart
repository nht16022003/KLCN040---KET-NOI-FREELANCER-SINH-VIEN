import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_bai_dang_tuyen_dung.dart';
import '../../services/dich_vu_cong_viec.dart';
import '../../viewmodels/viewmodel_cong_viec.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';

/// Màn hình chỉnh sửa tin tuyển dụng của Nhà tuyển dụng
/// Kết nối trực tiếp API: `PUT api/JobPost/CapNhat`
class EditJobPostScreen extends StatefulWidget {
  final JobPostModel job;

  const EditJobPostScreen({super.key, required this.job});

  @override
  State<EditJobPostScreen> createState() => _EditJobPostScreenState();
}

class _EditJobPostScreenState extends State<EditJobPostScreen> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _titleController;
  late final TextEditingController _descriptionController;
  late final TextEditingController _salaryController;
  late final TextEditingController _slotsController;
  final TextEditingController _customSkillController = TextEditingController();

  final JobService _jobService = JobService();
  Timer? _debounceTimer;
  bool _isLoadingSuggestions = false;
  int _searchRequestId = 0;

  late DateTime _deadline;
  late String _status;
  final List<String> _selectedSkills = [];
  bool _isSubmitting = false;

  final List<String> _suggestedSkills = [
    'Photoshop',
    'Figma',
    'ASP.NET Core',
    'Flutter',
    'HTML/CSS',
    'SQL Server',
    'ReactJS',
    'Python',
    'C#',
    'Word / Excel',
  ];

  @override
  void initState() {
    super.initState();
    _titleController = TextEditingController(text: widget.job.tieude);
    _descriptionController = TextEditingController(text: widget.job.mota);
    _salaryController = TextEditingController(
      text: widget.job.thulao > 0 ? widget.job.thulao.toStringAsFixed(0) : '',
    );
    _slotsController = TextEditingController(
      text: widget.job.soluongtuyen > 0 ? widget.job.soluongtuyen.toString() : '1',
    );
    _deadline = widget.job.hanHoanThanh;
    _status = widget.job.status.isNotEmpty ? widget.job.status : 'DangTuyen';

    // Phân tách danh sách kỹ năng ban đầu
    if (widget.job.kynangyeucau.isNotEmpty) {
      final items = widget.job.kynangyeucau
          .split(RegExp(r'[,;]'))
          .map((s) => s.trim())
          .where((s) => s.isNotEmpty);
      for (final item in items) {
        if (!_selectedSkills.contains(item)) {
          _selectedSkills.add(item);
        }
      }
    }

    _titleController.addListener(_onTitleChanged);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_titleController.text.trim().isNotEmpty) {
        _onTitleChanged();
      }
    });
  }

  void _onTitleChanged() {
    _debounceTimer?.cancel();
    final text = _titleController.text.trim();
    if (text.isEmpty) return;

    _debounceTimer = Timer(const Duration(milliseconds: 400), () async {
      if (!mounted) return;
      final reqId = ++_searchRequestId;
      setState(() => _isLoadingSuggestions = true);

      try {
        final res = await _jobService.layGoiYKyNangTheoTieuDe(
          text,
          moTa: _descriptionController.text.trim(),
        );

        if (mounted && reqId == _searchRequestId && res.isSuccess && res.data != null && res.data!.isNotEmpty) {
          final newSkills = res.data!
              .map((item) => item['tenKyNang']?.toString().trim() ?? '')
              .where((s) => s.isNotEmpty)
              .toList();

          if (newSkills.isNotEmpty) {
            setState(() {
              _suggestedSkills.clear();
              _suggestedSkills.addAll(newSkills);
            });
          }
        }
      } catch (_) {
      } finally {
        if (mounted && reqId == _searchRequestId) {
          setState(() => _isLoadingSuggestions = false);
        }
      }
    });
  }

  @override
  void dispose() {
    _debounceTimer?.cancel();
    _titleController.removeListener(_onTitleChanged);
    _titleController.dispose();
    _descriptionController.dispose();
    _salaryController.dispose();
    _slotsController.dispose();
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

  Future<void> _pickDeadline() async {
    final now = DateTime.now();
    final initial = _deadline.isAfter(now) ? _deadline : now.add(const Duration(days: 7));
    final picked = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: now.subtract(const Duration(days: 1)),
      lastDate: now.add(const Duration(days: 365 * 2)),
    );
    if (picked != null) {
      setState(() => _deadline = picked);
    }
  }

  Future<void> _submitUpdate() async {
    if (!_formKey.currentState!.validate()) return;

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null || user.userId <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập lại để cập nhật tin.')),
      );
      return;
    }

    final salary = double.tryParse(_salaryController.text.replaceAll(RegExp(r'[^0-9]'), '')) ?? 0;
    if (salary < 50000) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Thù lao phải từ 50,000 VNĐ trở lên.')),
      );
      return;
    }

    setState(() => _isSubmitting = true);

    final payload = {
      'maJob': widget.job.maJob,
      'maUser': user.userId,
      'maNhaTuyenDung': user.userId,
      'tieude': _titleController.text.trim(),
      'mota': _descriptionController.text.trim(),
      'kynangyeucau': _selectedSkills.join(', '),
      'thulao': salary,
      'phiDangBai': widget.job.phiDangBai ?? 0,
      'thoigiandukienhoanthanh': _deadline.toIso8601String(),
      'soluongtuyen': int.tryParse(_slotsController.text.trim()) ?? 1,
      'status': _status,
      'fileDinhKem': '',
    };

    final response = await JobService().capNhatJob(payload);

    setState(() => _isSubmitting = false);

    if (!mounted) return;

    if (response.isSuccess) {
      // Làm mới danh sách công việc của tôi
      context.read<JobViewModel>().taiDanhSachJobCuaToi(user.userId);
      context.read<JobViewModel>().taiDanhSachJobPost(lamMoi: true);

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Cập nhật tin tuyển dụng thành công!'),
          backgroundColor: AppColors.success,
        ),
      );
      Navigator.pop(context, true);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            response.message.isNotEmpty
                ? response.message
                : 'Cập nhật tin thất bại, vui lòng thử lại.',
          ),
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
          'Chỉnh sửa bài tuyển dụng',
          style: TextStyle(
            fontWeight: FontWeight.bold,
            color: AppColors.heading,
            fontSize: 18,
          ),
        ),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: SafeArea(
        child: Form(
          key: _formKey,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              // Info Card
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.circular(14),
                  border: Border.all(color: AppColors.primary.withValues(alpha: 0.3)),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.edit_note, color: AppColors.primary, size: 24),
                    const SizedBox(width: 10),
                    Expanded(
                      child: Text(
                        'Đang chỉnh sửa mã tin: ${widget.job.maJob}',
                        style: const TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.w600,
                          color: AppColors.primaryDark,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),

              // PHẦN 1: NỘI DUNG CÔNG VIỆC
              Container(
                padding: const EdgeInsets.all(18),
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: AppColors.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Container(
                          width: 28,
                          height: 28,
                          decoration: BoxDecoration(
                            color: AppColors.primary.withValues(alpha: 0.15),
                            borderRadius: BorderRadius.circular(8),
                          ),
                          alignment: Alignment.center,
                          child: const Text(
                            '1',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              color: AppColors.primary,
                            ),
                          ),
                        ),
                        const SizedBox(width: 10),
                        const Text(
                          'Nội dung công việc',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: AppColors.heading,
                          ),
                        ),
                      ],
                    ),
                    const Divider(height: 24),

                    // Tiêu đề
                    const Text(
                      'Tiêu đề bài đăng *',
                      style: TextStyle(
                        fontWeight: FontWeight.w600,
                        fontSize: 14,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 6),
                    TextFormField(
                      controller: _titleController,
                      decoration: InputDecoration(
                        hintText: 'Ví dụ: Thiết kế giao diện ứng dụng di động Flutter',
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                      ),
                      validator: (v) => (v == null || v.trim().isEmpty) ? 'Vui lòng nhập tiêu đề' : null,
                    ),
                    const SizedBox(height: 16),

                    // Mô tả chi tiết
                    const Text(
                      'Mô tả chi tiết yêu cầu *',
                      style: TextStyle(
                        fontWeight: FontWeight.w600,
                        fontSize: 14,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 6),
                    TextFormField(
                      controller: _descriptionController,
                      maxLines: 4,
                      decoration: InputDecoration(
                        hintText: 'Mô tả chi tiết mục tiêu, yêu cầu kỹ thuật và sản phẩm cần bàn giao...',
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                      ),
                      validator: (v) => (v == null || v.trim().isEmpty) ? 'Vui lòng nhập mô tả công việc' : null,
                    ),
                    const SizedBox(height: 16),

                    // KHỐI KỸ NĂNG YÊU CẦU (THẺ / CARD RIÊNG BIỆT NHƯ WEB)
                    Container(
                      padding: const EdgeInsets.all(14),
                      decoration: BoxDecoration(
                        color: AppColors.background,
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: AppColors.border),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Row(
                            children: [
                              Icon(Icons.stars, color: AppColors.primary, size: 18),
                              SizedBox(width: 8),
                              Text(
                                'Kỹ năng yêu cầu',
                                style: TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 14,
                                  color: AppColors.heading,
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 10),

                          // Danh sách kỹ năng đã chọn
                          if (_selectedSkills.isEmpty)
                            const Padding(
                              padding: EdgeInsets.symmetric(vertical: 4),
                              child: Text(
                                'Chưa có kỹ năng nào. Chọn bên dưới hoặc nhập thêm.',
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
                          Row(
                            children: [
                              const Text(
                                'Gợi ý kỹ năng phù hợp:',
                                style: TextStyle(
                                  fontSize: 12,
                                  fontWeight: FontWeight.w600,
                                  color: AppColors.bodyText,
                                ),
                              ),
                              if (_isLoadingSuggestions) ...[
                                const SizedBox(width: 8),
                                const SizedBox(
                                  width: 12,
                                  height: 12,
                                  child: CircularProgressIndicator(
                                    strokeWidth: 2,
                                    color: AppColors.primary,
                                  ),
                                ),
                                const SizedBox(width: 4),
                                const Text(
                                  'Đang tìm...',
                                  style: TextStyle(
                                    fontSize: 11,
                                    color: AppColors.primary,
                                    fontStyle: FontStyle.italic,
                                  ),
                                ),
                              ],
                            ],
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
                                  daChon ? '✓ $skill' : '+ $skill',
                                  style: TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.w600,
                                    color: daChon ? Colors.white : AppColors.primary,
                                  ),
                                ),
                                backgroundColor: daChon ? AppColors.primary : AppColors.surface,
                                side: BorderSide(
                                  color: AppColors.primary.withValues(alpha: 0.3),
                                ),
                                onPressed: () {
                                  if (daChon) {
                                    _removeSkill(skill);
                                  } else {
                                    _addSkill(skill);
                                  }
                                },
                              );
                            }).toList(),
                          ),
                          const SizedBox(height: 10),

                          // Ô nhập kỹ năng khác
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
                  ],
                ),
              ),
              const SizedBox(height: 16),

              // PHẦN 2: NGÂN SÁCH & KẾ HOẠCH
              Container(
                padding: const EdgeInsets.all(18),
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: AppColors.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Container(
                          width: 28,
                          height: 28,
                          decoration: BoxDecoration(
                            color: AppColors.primary.withValues(alpha: 0.15),
                            borderRadius: BorderRadius.circular(8),
                          ),
                          alignment: Alignment.center,
                          child: const Text(
                            '2',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              color: AppColors.primary,
                            ),
                          ),
                        ),
                        const SizedBox(width: 10),
                        const Text(
                          'Ngân sách & Kế hoạch hoàn thành',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: AppColors.heading,
                          ),
                        ),
                      ],
                    ),
                    const Divider(height: 24),

                    // Thù lao & Số lượng
                    Row(
                      children: [
                        Expanded(
                          flex: 3,
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const Text(
                                'Mức thù lao (VNĐ) *',
                                style: TextStyle(
                                  fontWeight: FontWeight.w600,
                                  fontSize: 14,
                                  color: AppColors.heading,
                                ),
                              ),
                              const SizedBox(height: 6),
                              TextFormField(
                                controller: _salaryController,
                                keyboardType: TextInputType.number,
                                decoration: InputDecoration(
                                  hintText: 'Ví dụ: 1500000',
                                  suffixText: 'VNĐ',
                                  suffixStyle: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    color: AppColors.primary,
                                  ),
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                                ),
                                validator: (v) => (v == null || v.trim().isEmpty) ? 'Nhập thù lao' : null,
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          flex: 2,
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const Text(
                                'Số lượng *',
                                style: TextStyle(
                                  fontWeight: FontWeight.w600,
                                  fontSize: 14,
                                  color: AppColors.heading,
                                ),
                              ),
                              const SizedBox(height: 6),
                              TextFormField(
                                controller: _slotsController,
                                keyboardType: TextInputType.number,
                                decoration: InputDecoration(
                                  hintText: '1',
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                                ),
                                validator: (v) => (v == null || v.trim().isEmpty) ? 'Nhập số lượng' : null,
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),

                    // Hạn hoàn thành
                    const Text(
                      'Hạn hoàn thành dự kiến *',
                      style: TextStyle(
                        fontWeight: FontWeight.w600,
                        fontSize: 14,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 6),
                    InkWell(
                      onTap: _pickDeadline,
                      borderRadius: BorderRadius.circular(10),
                      child: Container(
                        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
                        decoration: BoxDecoration(
                          border: Border.all(color: Colors.grey.shade400),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Row(
                          children: [
                            const Icon(Icons.calendar_today_outlined, color: AppColors.primary, size: 20),
                            const SizedBox(width: 10),
                            Expanded(
                              child: Text(
                                'Hạn chót: ${_deadline.day}/${_deadline.month}/${_deadline.year}',
                                style: const TextStyle(
                                  fontSize: 14,
                                  fontWeight: FontWeight.w600,
                                  color: AppColors.heading,
                                ),
                              ),
                            ),
                            const Text(
                              'Đổi ngày',
                              style: TextStyle(
                                color: AppColors.primary,
                                fontWeight: FontWeight.bold,
                                fontSize: 13,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),

                    // Trạng thái bài đăng
                    const Text(
                      'Trạng thái bài đăng',
                      style: TextStyle(
                        fontWeight: FontWeight.w600,
                        fontSize: 14,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 6),
                    DropdownButtonFormField<String>(
                      initialValue: _status,
                      decoration: InputDecoration(
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                      ),
                      items: const [
                        DropdownMenuItem(
                          value: 'DangTuyen',
                          child: Text('Đang tuyển dụng (Mở)'),
                        ),
                        DropdownMenuItem(
                          value: 'DaDong',
                          child: Text('Đã đóng (Tạm dừng nhận ứng viên)'),
                        ),
                      ],
                      onChanged: (val) {
                        if (val != null) setState(() => _status = val);
                      },
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 24),

              // CÁC NÚT THAO TÁC
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => Navigator.pop(context),
                      style: OutlinedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 14),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                      child: const Text('Hủy bỏ'),
                    ),
                  ),
                  const SizedBox(width: 14),
                  Expanded(
                    flex: 2,
                    child: FilledButton(
                      onPressed: _isSubmitting ? null : _submitUpdate,
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        padding: const EdgeInsets.symmetric(vertical: 14),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                      child: Text(
                        _isSubmitting ? 'Đang cập nhật...' : 'Lưu thay đổi bài đăng',
                        style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 20),
            ],
          ),
        ),
      ),
    );
  }
}
