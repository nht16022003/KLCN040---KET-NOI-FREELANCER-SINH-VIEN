import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/job_post_model.dart';
import '../../services/application_service.dart';
import '../../viewmodels/auth_viewmodel.dart';
import '../applications/my_applications_screen.dart';

class ApplyJobBottomSheet extends StatefulWidget {
  final JobPostModel job;

  const ApplyJobBottomSheet({super.key, required this.job});

  @override
  State<ApplyJobBottomSheet> createState() => _ApplyJobBottomSheetState();
}

class _ApplyJobBottomSheetState extends State<ApplyJobBottomSheet> {
  final TextEditingController _coverLetterController = TextEditingController();
  bool _isSubmitting = false;

  Future<void> _submitApplication() async {
    final coverLetter = _coverLetterController.text.trim();
    final user = context.read<AuthViewModel>().currentUser;

    if (coverLetter.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng nhập lời giới thiệu.')),
      );
      return;
    }

    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Bạn cần đăng nhập để ứng tuyển.')),
      );
      return;
    }

    final currentUser = user;

    setState(() => _isSubmitting = true);

    final service = ApplicationService();
    final response = await service.applyJob(widget.job.id, {
      'maJob': widget.job.id,
      'maFreelancerStudents': currentUser.userId,
      'thuGioiThieu': coverLetter,
      'status': 'Pending',
    });

    setState(() => _isSubmitting = false);

    if (!mounted) return;

    if (response.isSuccess) {
      final successMessage = response.message.isEmpty
          ? 'Ứng tuyển thành công!'
          : response.message;

      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text(successMessage)));
      if (Navigator.canPop(context)) {
        Navigator.pop(context);
      }
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(builder: (_) => const MyApplicationsScreen()),
      );
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          response.message.isEmpty ? 'Ứng tuyển thất bại.' : response.message,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Text(
            'Nộp đơn ứng tuyển',
            style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          TextField(
            controller: _coverLetterController,
            maxLines: 4,
            decoration: const InputDecoration(
              border: OutlineInputBorder(),
              labelText: 'Lời giới thiệu',
            ),
          ),
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            child: FilledButton(
              onPressed: _isSubmitting ? null : _submitApplication,
              child: Text(_isSubmitting ? 'Đang gửi...' : 'Gửi hồ sơ'),
            ),
          ),
        ],
      ),
    );
  }
}
