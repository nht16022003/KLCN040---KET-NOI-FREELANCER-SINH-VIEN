import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_bai_dang_tuyen_dung.dart';
import '../../services/dich_vu_ung_tuyen.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';

class ApplyJobBottomSheet extends StatefulWidget {
  final JobPostModel job;

  const ApplyJobBottomSheet({super.key, required this.job});

  @override
  State<ApplyJobBottomSheet> createState() => _ApplyJobBottomSheetState();
}

class _ApplyJobBottomSheetState extends State<ApplyJobBottomSheet> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _coverLetterController;
  late final TextEditingController _thuLaoController;
  late final TextEditingController _thoiGianController;
  bool _isSubmitting = false;

  @override
  void initState() {
    super.initState();
    _coverLetterController = TextEditingController();
    final mucLuongMacDinh = widget.job.budget > 0
        ? widget.job.budget.toInt().toString()
        : '';
    _thuLaoController = TextEditingController(text: mucLuongMacDinh);
    _thoiGianController = TextEditingController(text: '7 ngày');
  }

  @override
  void dispose() {
    _coverLetterController.dispose();
    _thuLaoController.dispose();
    _thoiGianController.dispose();
    super.dispose();
  }

  Future<void> _submitApplication() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final coverLetter = _coverLetterController.text.trim();
    final user = context.read<AuthViewModel>().currentUser;

    if (user == null || user.userId <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng đăng nhập tài khoản sinh viên để ứng tuyển.'),
          backgroundColor: Colors.orange,
        ),
      );
      return;
    }

    final effectiveMaJob = widget.job.maJob.trim().isNotEmpty
        ? widget.job.maJob.trim()
        : widget.job.id.trim();

    if (effectiveMaJob.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Không xác định được mã công việc hợp lệ.'),
          backgroundColor: Colors.redAccent,
        ),
      );
      return;
    }

    final parsedThuLao = double.tryParse(
      _thuLaoController.text.replaceAll(RegExp(r'[^0-9]'), ''),
    );
    final thoiGian = _thoiGianController.text.trim();

    setState(() => _isSubmitting = true);

    final service = ApplicationService();
    final response = await service.nopDonUngTuyen(
      maJob: effectiveMaJob,
      maUser: user.userId,
      thuGioiThieu: coverLetter,
      thulaoDeXuat: parsedThuLao,
      thoiGianHoanThanhDeXuat: thoiGian.isNotEmpty ? thoiGian : null,
    );

    if (!mounted) return;
    setState(() => _isSubmitting = false);

    if (response.isSuccess) {
      final successMessage = response.message.isNotEmpty
          ? response.message
          : 'Nộp đơn ứng tuyển thành công! Nhà tuyển dụng sẽ sớm phản hồi.';

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(successMessage),
          backgroundColor: AppColors.success,
        ),
      );
      Navigator.pop(context, true);
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          response.message.isNotEmpty
              ? response.message
              : 'Ứng tuyển thất bại. Vui lòng thử lại sau.',
        ),
        backgroundColor: AppColors.error,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final tenNtd = widget.job.tenNhaTuyenDung ?? 'Doanh nghiệp tuyển dụng';
    final nganSach = widget.job.budget > 0
        ? AppFormatters.formatCurrency(widget.job.budget)
        : 'Thoả thuận';

    return Container(
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      padding: EdgeInsets.only(
        top: 14,
        left: 20,
        right: 20,
        bottom: MediaQuery.of(context).viewInsets.bottom + 20,
      ),
      child: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Thanh kéo nhỏ
              Center(
                child: Container(
                  width: 44,
                  height: 5,
                  decoration: BoxDecoration(
                    color: Colors.grey.shade300,
                    borderRadius: BorderRadius.circular(10),
                  ),
                ),
              ),
              const SizedBox(height: 14),

              // Tiêu đề
              const Text(
                'Nộp hồ sơ ứng tuyển',
                style: TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 6),

              // Thẻ tóm tắt công việc
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: AppColors.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.job.title,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: AppColors.primary,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        const Icon(
                          Icons.business_outlined,
                          size: 14,
                          color: AppColors.bodyText,
                        ),
                        const SizedBox(width: 4),
                        Expanded(
                          child: Text(
                            tenNtd,
                            style: const TextStyle(
                              fontSize: 12,
                              color: AppColors.bodyText,
                            ),
                          ),
                        ),
                        Text(
                          nganSach,
                          style: const TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                            color: AppColors.primary,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),

              // Thư giới thiệu (bắt buộc)
              const Text(
                'Thư giới thiệu / Đề xuất công việc *',
                style: TextStyle(
                  fontWeight: FontWeight.w600,
                  fontSize: 13,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 6),
              TextFormField(
                controller: _coverLetterController,
                maxLines: 4,
                decoration: InputDecoration(
                  hintText:
                      'Giới thiệu bản thân, kỹ năng và cách bạn sẽ hoàn thành công việc...',
                  hintStyle: const TextStyle(fontSize: 13, color: AppColors.bodyText),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: const BorderSide(color: AppColors.border),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: const BorderSide(color: AppColors.primary, width: 1.5),
                  ),
                  filled: true,
                  fillColor: AppColors.surface,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập thư giới thiệu.';
                  }
                  if (val.trim().length < 10) {
                    return 'Thư giới thiệu quá ngắn (tối thiểu 10 ký tự).';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 14),

              // Thù lao đề xuất & Thời gian hoàn thành
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'Thù lao đề xuất (VNĐ)',
                          style: TextStyle(
                            fontWeight: FontWeight.w600,
                            fontSize: 12,
                            color: AppColors.heading,
                          ),
                        ),
                        const SizedBox(height: 6),
                        TextFormField(
                          controller: _thuLaoController,
                          keyboardType: TextInputType.number,
                          decoration: InputDecoration(
                            hintText: 'VD: 5000000',
                            border: OutlineInputBorder(
                              borderRadius: BorderRadius.circular(10),
                              borderSide: const BorderSide(color: AppColors.border),
                            ),
                            contentPadding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 10,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'Thời gian hoàn thành',
                          style: TextStyle(
                            fontWeight: FontWeight.w600,
                            fontSize: 12,
                            color: AppColors.heading,
                          ),
                        ),
                        const SizedBox(height: 6),
                        TextFormField(
                          controller: _thoiGianController,
                          decoration: InputDecoration(
                            hintText: 'VD: 7 ngày',
                            border: OutlineInputBorder(
                              borderRadius: BorderRadius.circular(10),
                              borderSide: const BorderSide(color: AppColors.border),
                            ),
                            contentPadding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 10,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 20),

              // Nút hành động
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: _isSubmitting ? null : () => Navigator.pop(context),
                      style: OutlinedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 13),
                        side: const BorderSide(color: AppColors.border),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10),
                        ),
                      ),
                      child: const Text(
                        'Hủy',
                        style: TextStyle(color: AppColors.bodyText),
                      ),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    flex: 2,
                    child: ElevatedButton(
                      onPressed: _isSubmitting ? null : _submitApplication,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 13),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10),
                        ),
                      ),
                      child: _isSubmitting
                          ? const SizedBox(
                              width: 20,
                              height: 20,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: Colors.white,
                              ),
                            )
                          : const Text(
                              'Gửi hồ sơ ứng tuyển',
                              style: TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 14,
                              ),
                            ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
