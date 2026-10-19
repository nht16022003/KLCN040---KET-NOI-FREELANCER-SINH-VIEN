import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../services/job_service.dart';
import '../../viewmodels/auth_viewmodel.dart';
import '../../viewmodels/job_viewmodel.dart';

class CreateJobPostScreen extends StatefulWidget {
  const CreateJobPostScreen({super.key});

  @override
  State<CreateJobPostScreen> createState() => _CreateJobPostScreenState();
}

class _CreateJobPostScreenState extends State<CreateJobPostScreen> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _skillsController = TextEditingController();
  final _salaryController = TextEditingController();
  final _slotsController = TextEditingController(text: '1');
  DateTime? _deadline;
  bool _isSubmitting = false;

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
    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập lại để đăng tin.')),
      );
      return;
    }

    if (user.userId <= 0) {
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

    setState(() => _isSubmitting = true);

    final response = await JobService().createJob({
      'maUser': user.userId,
      'maNhaTuyenDung': user.userId,
      'tieude': _titleController.text.trim(),
      'mota': _descriptionController.text.trim(),
      'kynangyeucau': _skillsController.text.trim(),
      'thulao': double.tryParse(_salaryController.text.trim()) ?? 0,
      'phiDangBai': 0,
      'thoigiandangtuyen': DateTime.now().toIso8601String(),
      'thoigiandukienhoanthanh': _deadline!.toIso8601String(),
      'soluongtuyen': int.tryParse(_slotsController.text.trim()) ?? 1,
      'status': 'DangTuyen',
      'fileDinhKem': '',
    });

    setState(() => _isSubmitting = false);

    if (!mounted) return;

    if (response.isSuccess) {
      context.read<JobViewModel>().fetchJobs();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Đăng bài tuyển dụng thành công.')),
      );
      Navigator.pop(context);
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          response.message.isEmpty ? 'Đăng bài thất bại.' : response.message,
        ),
      ),
    );
  }

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    _skillsController.dispose();
    _salaryController.dispose();
    _slotsController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Đăng bài tuyển dụng')),
      body: SafeArea(
        child: Form(
          key: _formKey,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              TextFormField(
                controller: _titleController,
                decoration: const InputDecoration(
                  labelText: 'Tiêu đề công việc',
                  border: OutlineInputBorder(),
                ),
                validator: (value) => (value == null || value.trim().isEmpty)
                    ? 'Vui lòng nhập tiêu đề'
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _descriptionController,
                maxLines: 5,
                decoration: const InputDecoration(
                  labelText: 'Mô tả công việc',
                  border: OutlineInputBorder(),
                ),
                validator: (value) => (value == null || value.trim().isEmpty)
                    ? 'Vui lòng nhập mô tả'
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _skillsController,
                decoration: const InputDecoration(
                  labelText: 'Kỹ năng yêu cầu',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _salaryController,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(
                        labelText: 'Thù lao (VNĐ)',
                        border: OutlineInputBorder(),
                      ),
                      validator: (value) =>
                          (value == null || value.trim().isEmpty)
                          ? 'Nhập mức thù lao'
                          : null,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: TextFormField(
                      controller: _slotsController,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(
                        labelText: 'Số lượng',
                        border: OutlineInputBorder(),
                      ),
                      validator: (value) =>
                          (value == null || value.trim().isEmpty)
                          ? 'Nhập số lượng'
                          : null,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              InkWell(
                onTap: _pickDeadline,
                child: Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 16,
                  ),
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.grey.shade400),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    children: [
                      const Icon(Icons.date_range_outlined),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Text(
                          _deadline == null
                              ? 'Chọn thời gian dự kiến hoàn thành'
                              : 'Dự kiến hoàn thành: ${_deadline!.day}/${_deadline!.month}/${_deadline!.year}',
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 20),
              FilledButton(
                onPressed: _isSubmitting ? null : _submitJob,
                child: Text(_isSubmitting ? 'Đang đăng...' : 'Đăng bài'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
