import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../services/dich_vu_cong_viec.dart';
import '../../services/dich_vu_vi_tien.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_cong_viec.dart';
import '../wallet/man_hinh_nap_tien.dart';

class CreateJobPostScreen extends StatefulWidget {
  const CreateJobPostScreen({super.key});

  @override
  State<CreateJobPostScreen> createState() => _CreateJobPostScreenState();
}

class _CreateJobPostScreenState extends State<CreateJobPostScreen> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _salaryController = TextEditingController();
  final _slotsController = TextEditingController(text: '1');
  final _customSkillController = TextEditingController();

  final JobService _jobService = JobService();
  final WalletService _walletService = WalletService();
  Timer? _debounceTimer;
  bool _isLoadingSuggestions = false;
  int _searchRequestId = 0;

  DateTime? _deadline;
  final List<String> _selectedSkills = [];
  bool _isSubmitting = false;

  // Quản lý phí đăng bài và số dư ví (Đồng bộ chuẩn hệ thống và Website)
  double? _phiDangBai;
  bool _isLoadingFee = true;
  String? _feeError;
  double _soDuKhaDung = 0;
  bool _isLoadingWallet = true;

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
    _titleController.addListener(_onTitleChanged);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _taiThongTinPhiVaVi();
    });
  }

  Future<void> _taiThongTinPhiVaVi() async {
    if (!mounted) return;
    setState(() {
      _isLoadingFee = true;
      _isLoadingWallet = true;
      _feeError = null;
    });

    final user = context.read<AuthViewModel>().currentUser;

    // 1. Tải mức phí đăng bài hiện hành từ hệ thống (GET api/JobPost/PhiDangBaiHienTai)
    final feeRes = await _jobService.layPhiDangBaiHienTai();

    // 2. Tải số dư ví khả dụng của Nhà tuyển dụng
    double soDu = 0;
    if (user != null && user.userId > 0) {
      final viRes = await _walletService.layViTheoUser(user.userId);
      if (viRes.isSuccess && viRes.data != null) {
        soDu = viRes.data!.soDuKhaDung;
      }
    }

    if (!mounted) return;
    setState(() {
      _isLoadingFee = false;
      _isLoadingWallet = false;
      if (feeRes.isSuccess && feeRes.data != null) {
        _phiDangBai = feeRes.data;
        _feeError = null;
      } else {
        _phiDangBai = null;
        _feeError = feeRes.message.isNotEmpty
            ? feeRes.message
            : 'Không thể kết nối máy chủ để lấy thông tin phí đăng bài.';
      }
      _soDuKhaDung = soDu;
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
    final picked = await showDatePicker(
      context: context,
      initialDate: now.add(const Duration(days: 7)),
      firstDate: now,
      lastDate: now.add(const Duration(days: 365 * 2)),
    );
    if (picked != null) {
      setState(() => _deadline = picked);
    }
  }

  Future<void> _submitJob() async {
    if (!_formKey.currentState!.validate()) return;

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null || user.userId <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập lại để đăng tin.')),
      );
      return;
    }

    if (_deadline == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng chọn thời hạn dự kiến hoàn thành.'),
        ),
      );
      return;
    }

    final salary = double.tryParse(_salaryController.text.replaceAll(RegExp(r'[^0-9]'), '')) ?? 0;
    if (salary < 50000) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Mức thù lao phải từ 50,000 VNĐ trở lên.')),
      );
      return;
    }

    // Kiểm tra trạng thái tải phí đăng bài
    if (_isLoadingFee || _isLoadingWallet) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Hệ thống đang tải mức phí đăng bài, vui lòng đợi trong giây lát.')),
      );
      return;
    }

    if (_phiDangBai == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(_feeError ?? 'Không thể xác định mức phí đăng bài. Vui lòng bấm Thử lại.'),
          backgroundColor: AppColors.error,
          action: SnackBarAction(
            label: 'Thử lại',
            textColor: Colors.white,
            onPressed: _taiThongTinPhiVaVi,
          ),
        ),
      );
      return;
    }

    // Nghiệp vụ đối chiếu chuẩn Website: Nếu có phí và số dư không đủ -> Báo lỗi & hỗ trợ nạp tiền
    final double phiDangBaiApDung = _phiDangBai!;
    if (phiDangBaiApDung > 0 && _soDuKhaDung < phiDangBaiApDung) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Số dư khả dụng trong ví (${AppFormatters.formatCurrency(_soDuKhaDung)}) không đủ để thanh toán phí đăng bài (${AppFormatters.formatCurrency(phiDangBaiApDung)}). Vui lòng nạp thêm tiền vào ví trước khi đăng bài.',
          ),
          backgroundColor: AppColors.error,
          duration: const Duration(seconds: 4),
          action: SnackBarAction(
            label: 'Nạp ví ngay',
            textColor: Colors.white,
            onPressed: () async {
              await Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const DepositScreen()),
              );
              if (mounted) _taiThongTinPhiVaVi();
            },
          ),
        ),
      );
      return;
    }

    setState(() => _isSubmitting = true);

    final payload = {
      'maUser': user.userId,
      'maNhaTuyenDung': user.userId,
      'tieude': _titleController.text.trim(),
      'mota': _descriptionController.text.trim(),
      'kynangyeucau': _selectedSkills.join(', '),
      'thulao': salary,
      'phiDangBai': phiDangBaiApDung,
      'thoigiandangtuyen': DateTime.now().toIso8601String(),
      'thoigiandukienhoanthanh': _deadline!.toIso8601String(),
      'soluongtuyen': int.tryParse(_slotsController.text.trim()) ?? 1,
      'status': 'DangTuyen',
      'fileDinhKem': '',
    };

    final response = await JobService().createJob(payload);

    setState(() => _isSubmitting = false);

    if (!mounted) return;

    if (response.isSuccess) {
      context.read<JobViewModel>().fetchJobs();
      context.read<JobViewModel>().taiDanhSachJobCuaToi(user.userId);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Đăng bài tuyển dụng thành công!'),
          backgroundColor: AppColors.success,
        ),
      );
      Navigator.pop(context, true);
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          response.message.isEmpty ? 'Đăng bài thất bại.' : response.message,
        ),
        backgroundColor: AppColors.error,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text(
          'Đăng tin tuyển dụng',
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
              // Banner thông báo đăng tin
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.circular(14),
                  border: Border.all(color: AppColors.primary.withValues(alpha: 0.3)),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.campaign_outlined, color: AppColors.primary, size: 24),
                    const SizedBox(width: 10),
                    const Expanded(
                      child: Text(
                        'Đăng bài để tiếp cận hàng ngàn Freelancer Sinh viên có năng lực và kỹ năng phù hợp.',
                        style: TextStyle(
                          fontSize: 13,
                          color: AppColors.heading,
                          height: 1.3,
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
                        const Expanded(
                          child: Text(
                            'Nội dung công việc',
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: AppColors.heading,
                            ),
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
                        hintText: 'Ví dụ: Thiết kế giao diện website bán hàng bằng ASP.NET Core',
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                      ),
                      validator: (value) => (value == null || value.trim().isEmpty)
                          ? 'Vui lòng nhập tiêu đề bài đăng'
                          : null,
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
                        hintText: 'Mô tả phạm vi công việc, mục tiêu, yêu cầu kỹ thuật và sản phẩm cần bàn giao...',
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                      ),
                      validator: (value) => (value == null || value.trim().isEmpty)
                          ? 'Vui lòng nhập mô tả chi tiết công việc'
                          : null,
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
                                'Chưa có kỹ năng nào. Nhấn gợi ý bên dưới hoặc tự nhập thêm.',
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

              // PHẦN 2: NGÂN SÁCH THÙ LAO & KẾ HOẠCH HOÀN THÀNH
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
                        const Expanded(
                          child: Text(
                            'Ngân sách thù lao & Kế hoạch hoàn thành',
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: AppColors.heading,
                            ),
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
                                'Mức thù lao ngân sách (VNĐ) *',
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
                                validator: (value) => (value == null || value.trim().isEmpty)
                                    ? 'Nhập mức thù lao'
                                    : null,
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
                                validator: (value) => (value == null || value.trim().isEmpty)
                                    ? 'Nhập số lượng'
                                    : null,
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
                                _deadline == null
                                    ? 'Chọn thời gian dự kiến hoàn thành'
                                    : 'Dự kiến hoàn thành: ${_deadline!.day}/${_deadline!.month}/${_deadline!.year}',
                                style: TextStyle(
                                  fontSize: 14,
                                  fontWeight: _deadline == null ? FontWeight.normal : FontWeight.w600,
                                  color: _deadline == null ? AppColors.bodyText : AppColors.heading,
                                ),
                              ),
                            ),
                            Text(
                              _deadline == null ? 'Chọn' : 'Đổi ngày',
                              style: const TextStyle(
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

                    // Chính sách phí đăng bài (Fee Box đồng bộ với Web)
                    _buildFeeBox(),
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
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 14),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                      child: const Text(
                        'Hủy bỏ',
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    flex: 2,
                    child: FilledButton(
                      onPressed: _isSubmitting ? null : _submitJob,
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 14),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                      child: Text(
                        _isSubmitting ? 'Đang đăng tin...' : 'Đăng tin tuyển dụng ngay',
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
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

  /// Khung hiển thị thông tin phí đăng bài và trạng thái số dư ví đối chiếu chuẩn Website
  Widget _buildFeeBox() {
    // 1. Trạng thái đang tải dữ liệu phí hoặc ví
    if (_isLoadingFee || _isLoadingWallet) {
      return Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
        decoration: BoxDecoration(
          color: Colors.grey.shade100,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: Colors.grey.shade300),
        ),
        child: const Row(
          children: [
            SizedBox(
              width: 18,
              height: 18,
              child: CircularProgressIndicator(strokeWidth: 2, color: AppColors.primary),
            ),
            SizedBox(width: 12),
            Expanded(
              child: Text(
                'Đang kiểm tra mức phí đăng bài và số dư ví...',
                style: TextStyle(fontSize: 13, color: AppColors.bodyText),
              ),
            ),
          ],
        ),
      );
    }

    // 2. Trạng thái lỗi không tải được mức phí đăng bài
    if (_feeError != null && _phiDangBai == null) {
      return Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: const Color(0xFFFFF5F5),
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: AppColors.error.withValues(alpha: 0.4)),
        ),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Icon(Icons.error_outline, color: AppColors.error, size: 22),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Không thể tải phí đăng bài từ hệ thống',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 13,
                      color: AppColors.error,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    _feeError!,
                    style: const TextStyle(fontSize: 12, color: AppColors.bodyText),
                  ),
                  const SizedBox(height: 6),
                  InkWell(
                    onTap: _taiThongTinPhiVaVi,
                    child: const Text(
                      'Bấm để thử lại ↻',
                      style: TextStyle(
                        color: AppColors.primary,
                        fontWeight: FontWeight.bold,
                        fontSize: 12,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    // 3. Hiển thị thông tin phí thực tế & Số dư ví (Đồng bộ chuẩn nghiệp vụ Website)
    final double fee = _phiDangBai ?? 0;
    final bool khongDuTien = fee > 0 && _soDuKhaDung < fee;

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: khongDuTien ? const Color(0xFFFFF5F5) : AppColors.lightGreenSurface,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: khongDuTien ? AppColors.error : AppColors.primary.withValues(alpha: 0.25),
          width: khongDuTien ? 1.5 : 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 28,
                height: 28,
                decoration: BoxDecoration(
                  color: khongDuTien ? AppColors.error : AppColors.primary,
                  shape: BoxShape.circle,
                ),
                alignment: Alignment.center,
                child: Icon(
                  khongDuTien ? Icons.priority_high : Icons.check,
                  color: Colors.white,
                  size: 16,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        Expanded(
                          child: Text(
                            fee == 0
                                ? 'Miễn phí đăng tin (0 đ)'
                                : '${AppFormatters.formatCurrency(fee)} phí đăng bài',
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 13,
                              color: khongDuTien ? AppColors.error : AppColors.heading,
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Flexible(
                          child: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                            decoration: BoxDecoration(
                              color: khongDuTien ? AppColors.error : AppColors.primary,
                              borderRadius: BorderRadius.circular(6),
                            ),
                            child: Text(
                              'Ví: ${AppFormatters.formatCurrency(_soDuKhaDung)}',
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: const TextStyle(
                                color: Colors.white,
                                fontSize: 11,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    const Text(
                      'Phí đăng tin được trừ trực tiếp từ số dư khả dụng khi xuất bản bài đăng thành công (không ký quỹ).',
                      style: TextStyle(
                        fontSize: 11,
                        color: AppColors.bodyText,
                        height: 1.3,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          if (khongDuTien) ...[
            const SizedBox(height: 10),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: AppColors.error.withValues(alpha: 0.5)),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(
                      '⚠️ Thiếu: ${AppFormatters.formatCurrency(fee - _soDuKhaDung)}',
                      style: const TextStyle(
                        color: AppColors.error,
                        fontWeight: FontWeight.bold,
                        fontSize: 12,
                      ),
                    ),
                  ),
                  InkWell(
                    onTap: () async {
                      await Navigator.push(
                        context,
                        MaterialPageRoute(builder: (_) => const DepositScreen()),
                      );
                      if (mounted) _taiThongTinPhiVaVi();
                    },
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                      decoration: BoxDecoration(
                        color: AppColors.error,
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: const Text(
                        'Nạp ví ngay →',
                        style: TextStyle(
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                          fontSize: 11,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }
}
