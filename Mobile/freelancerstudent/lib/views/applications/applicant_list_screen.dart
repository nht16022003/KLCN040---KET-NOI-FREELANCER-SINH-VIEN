import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/api_endpoints.dart';
import '../../models/application_model.dart';
import '../../services/application_service.dart';
import '../../viewmodels/auth_viewmodel.dart';
import '../profile/profile_screen.dart';

class ApplicantListScreen extends StatefulWidget {
  const ApplicantListScreen({super.key});

  @override
  State<ApplicantListScreen> createState() => _ApplicantListScreenState();
}

class _ApplicantListScreenState extends State<ApplicantListScreen> {
  final ApplicationService _service = ApplicationService();
  bool _isLoading = false;
  String? _errorMessage;
  List<ApplicationModel> _applicants = <ApplicationModel>[];
  final Set<String> _updatingIds = <String>{};

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadApplicants();
    });
  }

  Future<void> _loadApplicants() async {
    final userId = context.read<AuthViewModel>().currentUser?.userId;
    if (userId == null || userId <= 0) {
      setState(() {
        _errorMessage = 'Vui lòng đăng nhập lại để xem danh sách ứng viên.';
      });
      return;
    }

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    final response = await _service.getMyApplications(userId: userId);

    if (!mounted) return;

    setState(() {
      _isLoading = false;
      if (response.isSuccess) {
        _applicants = response.data ?? <ApplicationModel>[];
        if (_applicants.isEmpty) {
          _errorMessage = response.message.isNotEmpty
              ? response.message
              : 'Chưa có ứng viên nào ứng tuyển.';
        }
      } else {
        _errorMessage = response.message;
      }
    });
  }

  String _statusLabel(String status) {
    final value = status.trim();
    if (value.isEmpty) return 'Chờ duyệt';
    final lower = value.toLowerCase();
    if (lower == 'chapnhan' ||
        lower.contains('approved') ||
        lower.contains('accept')) {
      return 'Phê duyệt';
    }
    if (lower == 'tuchoi' ||
        lower.contains('reject') ||
        lower.contains('denied')) {
      return 'Từ chối';
    }
    return 'Chờ duyệt';
  }

  bool _isPending(ApplicationModel applicant) {
    final status = _statusLabel(applicant.status);
    return status == 'Chờ duyệt';
  }

  Future<void> _updateStatus(
    ApplicationModel applicant,
    String nextStatus,
  ) async {
    if (_updatingIds.contains(applicant.id)) return;
    setState(() => _updatingIds.add(applicant.id));

    final response = await _service.updateApplicationStatus(
      applicant.id,
      nextStatus,
    );

    if (!mounted) return;
    setState(() => _updatingIds.remove(applicant.id));

    if (response.isSuccess) {
      final index = _applicants.indexWhere((item) => item.id == applicant.id);
      if (index == -1) return;
      setState(() {
        _applicants[index] = applicant.copyWith(status: nextStatus);
      });

      final label = nextStatus == 'ChapNhan'
          ? 'đã được phê duyệt'
          : 'bị từ chối';
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Đã gửi thông báo cho freelancer: $label.')),
      );
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          response.message.isEmpty
              ? 'Không thể cập nhật trạng thái ứng viên.'
              : response.message,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Danh sách ứng viên'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadApplicants,
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: _loadApplicants,
        child: Builder(
          builder: (_) {
            if (_isLoading && _applicants.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }

            if (_errorMessage != null && _applicants.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Text(
                    _errorMessage!,
                    textAlign: TextAlign.center,
                    style: const TextStyle(color: Colors.red),
                  ),
                ),
              );
            }

            return ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: _applicants.length,
              itemBuilder: (context, index) {
                final applicant = _applicants[index];
                final isPending = _isPending(applicant);
                final statusText = _statusLabel(applicant.status);
                final freelancerId = int.tryParse(applicant.applicantId);

                return Card(
                  margin: const EdgeInsets.only(bottom: 12),
                  child: Padding(
                    padding: const EdgeInsets.all(14),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            InkWell(
                              borderRadius: BorderRadius.circular(28),
                              onTap: freelancerId == null || freelancerId <= 0
                                  ? null
                                  : () => Navigator.push(
                                      context,
                                      MaterialPageRoute(
                                        builder: (_) => ProfileScreen(
                                          freelancerId: freelancerId,
                                          readOnly: true,
                                        ),
                                      ),
                                    ),
                              child: CircleAvatar(
                                backgroundColor: const Color(0xFFE9E9E9),
                                backgroundImage:
                                    applicant.avatar?.trim().isNotEmpty == true
                                    ? NetworkImage(
                                        ApiEndpoints.dinhDangUrlAnh(
                                          applicant.avatar,
                                          tenMacDinh:
                                              applicant.tenUser ?? 'Freelancer',
                                        ),
                                      )
                                    : null,
                                child:
                                    applicant.avatar?.trim().isNotEmpty == true
                                    ? null
                                    : const Icon(
                                        Icons.person,
                                        color: Colors.black87,
                                      ),
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: InkWell(
                                onTap: freelancerId == null || freelancerId <= 0
                                    ? null
                                    : () => Navigator.push(
                                        context,
                                        MaterialPageRoute(
                                          builder: (_) => ProfileScreen(
                                            freelancerId: freelancerId,
                                            readOnly: true,
                                          ),
                                        ),
                                      ),
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(
                                      applicant.tenUser?.trim().isNotEmpty ==
                                              true
                                          ? applicant.tenUser!
                                          : 'Freelancer #${applicant.applicantId}',
                                      style: const TextStyle(
                                        fontWeight: FontWeight.bold,
                                        fontSize: 17,
                                      ),
                                    ),
                                    const SizedBox(height: 4),
                                    Text(
                                      statusText,
                                      style: TextStyle(
                                        color: statusText == 'Phê duyệt'
                                            ? Colors.green
                                            : statusText == 'Từ chối'
                                            ? Colors.red
                                            : const Color(0xFFB88A00),
                                        fontWeight: FontWeight.w600,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 12),
                        Text(
                          applicant.tieude.isEmpty
                              ? 'Mã công việc: ${applicant.jobId}'
                              : 'Công việc: ${applicant.tieude}',
                          style: const TextStyle(fontWeight: FontWeight.w600),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          applicant.coverLetter.isNotEmpty
                              ? applicant.coverLetter
                              : 'Không có lời giới thiệu.',
                          style: const TextStyle(
                            color: Color(0xFF5C7567),
                            height: 1.5,
                          ),
                        ),
                        const SizedBox(height: 12),
                        if (isPending)
                          Row(
                            children: [
                              Expanded(
                                child: ElevatedButton.icon(
                                  onPressed: _updatingIds.contains(applicant.id)
                                      ? null
                                      : () => _updateStatus(
                                          applicant,
                                          'ChapNhan',
                                        ),
                                  icon: const Icon(Icons.check),
                                  label: const Text('Phê duyệt'),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: Colors.green,
                                    foregroundColor: Colors.white,
                                  ),
                                ),
                              ),
                              const SizedBox(width: 8),
                              Expanded(
                                child: ElevatedButton.icon(
                                  onPressed: _updatingIds.contains(applicant.id)
                                      ? null
                                      : () =>
                                            _updateStatus(applicant, 'TuChoi'),
                                  icon: const Icon(Icons.close),
                                  label: const Text('Từ chối'),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: Colors.red,
                                    foregroundColor: Colors.white,
                                  ),
                                ),
                              ),
                            ],
                          )
                        else
                          Align(
                            alignment: Alignment.centerRight,
                            child: Chip(
                              label: Text(statusText),
                              backgroundColor: statusText == 'Phê duyệt'
                                  ? Colors.green.shade100
                                  : Colors.red.shade100,
                            ),
                          ),
                      ],
                    ),
                  ),
                );
              },
            );
          },
        ),
      ),
    );
  }
}
