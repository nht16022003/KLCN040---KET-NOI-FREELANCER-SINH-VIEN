import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/utils/validators.dart';
import '../../models/freelancer_profile_edit_model.dart';
import '../../viewmodels/auth_viewmodel.dart';
import '../../viewmodels/freelancer_profile_viewmodel.dart';

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

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
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
        appBar: AppBar(title: const Text('Chỉnh sửa hồ sơ năng lực')),
        body: Center(
          child: Text(
            viewModel.errorMessage ?? 'Không thể tải hồ sơ freelancer.',
          ),
        ),
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Chỉnh sửa hồ sơ năng lực')),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            _sectionTitle('Thông tin cá nhân'),
            _field(
              _nameController,
              'Họ và tên',
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
            _field(_majorCodeController, 'Mã chuyên ngành'),
            _field(_majorNameController, 'Tên chuyên ngành'),
            _field(_schoolController, 'Trường'),
            _field(_locationController, 'Địa điểm'),
            _field(
              _yearController,
              'Năm học',
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
            _field(_cohortController, 'Niên khóa'),
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
        color: Color(0xFF1D5D3B),
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
        border: const OutlineInputBorder(),
      ),
    ),
  );
}
