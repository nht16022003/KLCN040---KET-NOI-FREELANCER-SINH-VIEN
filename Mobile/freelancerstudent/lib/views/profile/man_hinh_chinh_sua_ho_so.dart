import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/kiem_tra_du_lieu.dart';
import '../../models/mo_hinh_chinh_sua_ho_so_freelancer.dart';
import '../../services/dich_vu_minh_chung.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_ho_so_freelancer.dart';

class EditProfileScreen extends StatefulWidget {
  const EditProfileScreen({super.key});

  @override
  State<EditProfileScreen> createState() => _EditProfileScreenState();
}

class _EditProfileScreenState extends State<EditProfileScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _phoneController = TextEditingController();
  final _emailController = TextEditingController();
  final _majorCodeController = TextEditingController();
  final _majorNameController = TextEditingController();
  final _schoolController = TextEditingController();
  final _locationController = TextEditingController();
  final _yearController = TextEditingController();
  final _gpaController = TextEditingController();
  final _cohortController = TextEditingController();
  final _bioController = TextEditingController();
  final _basicSkillsController = TextEditingController();
  final _languagesController = TextEditingController();
  final _priceController = TextEditingController();
  final _newSkillController = TextEditingController();
  final List<String> _skills = [];
  bool _acceptingWork = true;
  bool _initialized = false;
  bool _requestedLoad = false;
  bool _isCardVerified = false;
  bool _checkedVerification = false;
  List<Map<String, dynamic>> _majors = [];
  bool _loadedMajors = false;
  String? _selectedMajorCode;
  bool _isCustomMajor = false;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
    if (userId > 0 && !_checkedVerification) {
      _checkedVerification = true;
      MinhChungService().kiemTraTrangThaiXacThuc(userId).then((res) {
        if (mounted && res.isSuccess && res.data != null) {
          setState(() {
            _isCardVerified = res.data!['daXacMinh'] == true;
          });
        }
      });
    }

    if (!_loadedMajors) {
      _loadedMajors = true;
      MinhChungService().layDanhSachChuyenNganh().then((res) {
        if (mounted && res.isSuccess && res.data != null) {
          setState(() {
            _majors = res.data!;
          });
        }
      });
    }

    if (_initialized) return;
    final profile = context.watch<FreelancerProfileViewModel>().profile;
    if (profile == null) {
      final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
      if (userId > 0 && !_requestedLoad) {
        _requestedLoad = true;
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (mounted) {
            context.read<FreelancerProfileViewModel>().loadProfile(userId);
          }
        });
      }
      return;
    }
    _initialized = true;
    final edit = FreelancerProfileEditModel.fromProfile(profile);
    _nameController.text = edit.tenUser;
    _phoneController.text = edit.sdt ?? '';
    _emailController.text = edit.email ?? '';
    _majorCodeController.text = edit.maChuyenNganh;
    _majorNameController.text = edit.tenChuyenNganh;
    _selectedMajorCode = edit.maChuyenNganh.isNotEmpty ? edit.maChuyenNganh : null;
    _schoolController.text = edit.tenTruong;
    _locationController.text = edit.diaDiemFreelancerStudent;
    _yearController.text = edit.namThu.toString();
    _gpaController.text = edit.gpa.toString();
    _cohortController.text = edit.nienKhoa;
    _bioController.text = edit.gioithieu ?? '';
    _basicSkillsController.text = edit.kyNangCoBan ?? '';
    _languagesController.text = edit.ngonNgu ?? '';
    _priceController.text = edit.chiPhiTu?.toString() ?? '';
    _acceptingWork = edit.trangthaiNhanViec;
    _skills.addAll(edit.danhSachKyNang);
  }

  @override
  void dispose() {
    _nameController.dispose();
    _phoneController.dispose();
    _emailController.dispose();
    _majorCodeController.dispose();
    _majorNameController.dispose();
    _schoolController.dispose();
    _locationController.dispose();
    _yearController.dispose();
    _gpaController.dispose();
    _cohortController.dispose();
    _bioController.dispose();
    _basicSkillsController.dispose();
    _languagesController.dispose();
    _priceController.dispose();
    _newSkillController.dispose();
    super.dispose();
  }

  void _addSkillsFromInput() {
    final newSkills = _newSkillController.text
        .split(',')
        .map((skill) => skill.trim())
        .where((skill) => skill.isNotEmpty);
    for (final skill in newSkills) {
      if (!_skills.any(
        (existing) => existing.toLowerCase() == skill.toLowerCase(),
      )) {
        setState(() => _skills.add(skill));
      }
    }
    _newSkillController.clear();
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;
    final profile = context.read<FreelancerProfileViewModel>().profile;
    final userId = context.read<AuthViewModel>().currentUser?.userId ?? 0;
    if (profile == null || profile.maFreelancerStudents <= 0 || userId <= 0) {
      _showMessage('Không tìm thấy hồ sơ để cập nhật. Vui lòng tải lại hồ sơ.');
      return;
    }

    final base = FreelancerProfileEditModel.fromProfile(profile);
    final updated = FreelancerProfileEditModel(
      maFreelancerStudents: base.maFreelancerStudents,
      maUser: base.maUser,
      tenUser: _nameController.text.trim(),
      sdt: _phoneController.text.trim(),
      email: base.email,
      avatar: base.avatar,
      maChuyenNganh: _majorCodeController.text.trim(),
      tenChuyenNganh: _majorNameController.text.trim(),
      tenTruong: _schoolController.text.trim(),
      diaDiemFreelancerStudent: _locationController.text.trim(),
      namThu: int.parse(_yearController.text.trim()),
      gpa: double.parse(_gpaController.text.trim()),
      nienKhoa: _cohortController.text.trim(),
      gioithieu: _bioController.text.trim(),
      kyNangCoBan: _basicSkillsController.text.trim(),
      ngonNgu: _languagesController.text.trim(),
      trangthaiNhanViec: _acceptingWork,
      chiPhiTu: _priceController.text.trim().isEmpty
          ? null
          : double.parse(_priceController.text.trim()),
      danhSachKyNang: List.unmodifiable(_skills),
    );

    final viewModel = context.read<FreelancerProfileViewModel>();
    final success = await viewModel.saveProfile(updated);
    if (!mounted) return;
    if (!success) {
      _showMessage(viewModel.errorMessage ?? 'Không thể cập nhật hồ sơ.');
      return;
    }
    Navigator.pop(context, true);
  }

  void _showMessage(String message) {
    ScaffoldMessenger.of(context)
        .showSnackBar(SnackBar(content: Text(message)));
  }

  String? _validateInteger(String? value, String label, int min, int max) {
    final number = int.tryParse(value?.trim() ?? '');
    if (number == null || number < min || number > max) {
      return '$label phải nằm trong khoảng $min-$max.';
    }
    return null;
  }

  String? _validateGpa(String? value) {
    final number = double.tryParse(value?.trim() ?? '');
    if (number == null || number < 0 || number > 4) {
      return 'GPA phải là số từ 0 đến 4.';
    }
    return null;
  }

  String? _validatePrice(String? value) {
    if (value == null || value.trim().isEmpty) return null;
    final number = double.tryParse(value.trim());
    if (number == null || number < 0) return 'Chi phí phải là số không âm.';
    return null;
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<FreelancerProfileViewModel>();
    if (viewModel.isLoading && viewModel.profile == null) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }
    if (viewModel.profile == null) {
      return Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          title: const Text('Chỉnh sửa hồ sơ năng lực'),
          backgroundColor: AppColors.surface,
          foregroundColor: AppColors.heading,
          elevation: 0.5,
        ),
        body: Center(
          child: Text(
            viewModel.errorMessage ?? 'Không thể tải hồ sơ freelancer.',
          ),
        ),
      );
    }

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Chỉnh sửa hồ sơ năng lực'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            if (_isCardVerified)
              Container(
                margin: const EdgeInsets.only(bottom: 16),
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: const Color(0xFFF0FDF4),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: const Color(0xFF86EFAC)),
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Icon(Icons.verified, color: AppColors.primary, size: 22),
                    const SizedBox(width: 10),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: const [
                          Text(
                            'Hồ sơ đã được xác minh thẻ sinh viên',
                            style: TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF166534),
                            ),
                          ),
                          SizedBox(height: 2),
                          Text(
                            'Các trường thông tin từ thẻ (Họ và tên, Trường học, Chuyên ngành, Năm học, Niên khóa) đã được khóa và không thể tự chỉnh sửa.',
                            style: TextStyle(
                              fontSize: 12.5,
                              color: Color(0xFF15803D),
                              height: 1.3,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            _sectionTitle('Thông tin cá nhân'),
            _field(
              _nameController,
              'Họ và tên',
              readOnly: _isCardVerified,
              validator: (value) =>
                  AppValidators.validateRequired(value, 'Họ và tên'),
            ),
            _field(
              _phoneController,
              'Số điện thoại',
              keyboardType: TextInputType.phone,
              validator: AppValidators.validatePhoneNumber,
            ),
            _field(_emailController, 'Email', readOnly: true),
            const SizedBox(height: 8),
            _sectionTitle('Học vấn'),
            Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: DropdownButtonFormField<String>(
                initialValue: (_majors.any((m) => (m['maChuyenNganh'] ?? m['MaChuyenNganh']) == _selectedMajorCode))
                    ? _selectedMajorCode
                    : (_isCustomMajor || (_majorNameController.text.isNotEmpty && _selectedMajorCode != null && _selectedMajorCode!.isNotEmpty))
                        ? '__NEW__'
                        : null,
                decoration: const InputDecoration(
                  labelText: 'Chuyên ngành',
                  border: OutlineInputBorder(),
                  helperText: 'Chọn chuyên ngành có sẵn hoặc chọn nhập mới',
                ),
                items: [
                  ..._majors.map((m) {
                    final code = (m['maChuyenNganh'] ?? m['MaChuyenNganh'] ?? '').toString();
                    final name = (m['tenChuyenNganh'] ?? m['TenChuyenNganh'] ?? '').toString();
                    return DropdownMenuItem<String>(
                      value: code,
                      child: Text('$name ($code)'),
                    );
                  }),
                  const DropdownMenuItem<String>(
                    value: '__NEW__',
                    child: Text('+ Nhập chuyên ngành khác...'),
                  ),
                ],
                onChanged: (val) {
                  setState(() {
                    if (val == '__NEW__') {
                      _isCustomMajor = true;
                      _selectedMajorCode = '__NEW__';
                      _majorCodeController.text = '';
                    } else if (val != null) {
                      _isCustomMajor = false;
                      _selectedMajorCode = val;
                      _majorCodeController.text = val;
                      final found = _majors.firstWhere(
                        (m) => (m['maChuyenNganh'] ?? m['MaChuyenNganh']) == val,
                        orElse: () => {},
                      );
                      _majorNameController.text = (found['tenChuyenNganh'] ?? found['TenChuyenNganh'] ?? '').toString();
                    }
                  });
                },
              ),
            ),
            if (_isCustomMajor || (_selectedMajorCode == '__NEW__') || (_majorNameController.text.isNotEmpty && !_majors.any((m) => (m['maChuyenNganh'] ?? m['MaChuyenNganh']) == _selectedMajorCode)))
              _field(
                _majorNameController,
                'Tên chuyên ngành mới (hệ thống tự tạo mã)',
                validator: (val) => AppValidators.validateRequired(val, 'Tên chuyên ngành'),
              ),
            _field(_schoolController, 'Trường', readOnly: _isCardVerified),
            _field(_locationController, 'Địa điểm'),
            _field(
              _yearController,
              'Năm học',
              readOnly: _isCardVerified,
              keyboardType: TextInputType.number,
              validator: (value) => _validateInteger(value, 'Năm học', 1, 8),
            ),
            _field(
              _gpaController,
              'GPA (thang điểm 4)',
              keyboardType: const TextInputType.numberWithOptions(
                decimal: true,
              ),
              validator: _validateGpa,
            ),
            _field(_cohortController, 'Niên khóa', readOnly: _isCardVerified),
            const SizedBox(height: 8),
            _sectionTitle('Năng lực và mong muốn'),
            _field(_bioController, 'Giới thiệu bản thân', maxLines: 4),
            _field(_basicSkillsController, 'Kỹ năng cơ bản', maxLines: 2),
            _field(_languagesController, 'Ngoại ngữ', maxLines: 2),
            _field(
              _priceController,
              'Chi phí tối thiểu (VNĐ)',
              keyboardType: const TextInputType.numberWithOptions(
                decimal: true,
              ),
              validator: _validatePrice,
            ),
            SwitchListTile(
              contentPadding: EdgeInsets.zero,
              title: const Text('Đang nhận việc'),
              value: _acceptingWork,
              onChanged: (value) => setState(() => _acceptingWork = value),
            ),
            const SizedBox(height: 8),
            _sectionTitle('Kỹ năng chuyên môn'),
            Wrap(
              spacing: 8,
              runSpacing: 4,
              children: _skills
                  .map(
                    (skill) => InputChip(
                      label: Text(skill),
                      onDeleted: () => setState(() => _skills.remove(skill)),
                    ),
                  )
                  .toList(),
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _newSkillController,
                    textInputAction: TextInputAction.done,
                    onSubmitted: (_) => _addSkillsFromInput(),
                    decoration: const InputDecoration(
                      labelText: 'Thêm kỹ năng',
                      hintText: 'Ví dụ: Flutter, UI/UX',
                      border: OutlineInputBorder(),
                    ),
                  ),
                ),
                IconButton(
                  onPressed: _addSkillsFromInput,
                  icon: const Icon(Icons.add_circle_outline),
                  tooltip: 'Thêm kỹ năng',
                ),
              ],
            ),
            const SizedBox(height: 20),
            FilledButton(
              onPressed: viewModel.isLoading ? null : _save,
              child: viewModel.isLoading
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Lưu thay đổi'),
            ),
          ],
        ),
      ),
    );
  }

  Widget _sectionTitle(String title) => Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: Text(
      title,
      style: const TextStyle(
        fontSize: 17,
        fontWeight: FontWeight.bold,
        color: AppColors.heading,
      ),
    ),
  );

  Widget _field(
    TextEditingController controller,
    String label, {
    int maxLines = 1,
    bool readOnly = false,
    TextInputType? keyboardType,
    String? Function(String?)? validator,
  }) => Padding(
    padding: const EdgeInsets.only(bottom: 12),
    child: TextFormField(
      controller: controller,
      readOnly: readOnly,
      maxLines: maxLines,
      keyboardType: keyboardType,
      validator: validator,
      decoration: InputDecoration(
        labelText: label,
        suffixIcon: readOnly ? const Icon(Icons.lock_outline, size: 18, color: Colors.grey) : null,
        filled: readOnly,
        fillColor: readOnly ? const Color(0xFFF1F5F9) : null,
        border: const OutlineInputBorder(),
      ),
    ),
  );
}
