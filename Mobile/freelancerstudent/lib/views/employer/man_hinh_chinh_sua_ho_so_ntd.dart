import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_nha_tuyen_dung.dart';
import '../../viewmodels/viewmodel_nha_tuyen_dung.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';

class ManHinhChinhSuaHoSoNTD extends StatefulWidget {
  final NhaTuyenDungProfileModel profile;

  const ManHinhChinhSuaHoSoNTD({super.key, required this.profile});

  @override
  State<ManHinhChinhSuaHoSoNTD> createState() =>
      _ManHinhChinhSuaHoSoNTDState();
}

class _ManHinhChinhSuaHoSoNTDState extends State<ManHinhChinhSuaHoSoNTD> {
  final _formKey = GlobalKey<FormState>();

  late TextEditingController _tenCongTyController;
  late TextEditingController _nguoiDaiDienController;
  late TextEditingController _sdtController;
  late TextEditingController _linhVucController;
  late TextEditingController _diaChiController;
  late TextEditingController _websiteController;
  late TextEditingController _gioiThieuController;

  bool _dangLuu = false;

  @override
  void initState() {
    super.initState();
    final p = widget.profile.nhaTuyenDung;
    _tenCongTyController = TextEditingController(text: p.tencongty ?? '');
    _nguoiDaiDienController = TextEditingController(text: p.hotenUser ?? '');
    _sdtController = TextEditingController(text: p.sdtUser ?? '');
    _linhVucController = TextEditingController(text: p.linhvuc ?? '');
    _diaChiController = TextEditingController(text: p.diachi ?? '');
    _websiteController = TextEditingController(text: p.link ?? '');
    _gioiThieuController = TextEditingController(text: p.gioithieu ?? '');
  }

  @override
  void dispose() {
    _tenCongTyController.dispose();
    _nguoiDaiDienController.dispose();
    _sdtController.dispose();
    _linhVucController.dispose();
    _diaChiController.dispose();
    _websiteController.dispose();
    _gioiThieuController.dispose();
    super.dispose();
  }

  Future<void> _luuHoSo() async {
    if (!_formKey.currentState!.validate()) return;

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) return;

    setState(() => _dangLuu = true);

    final vm = context.read<NhaTuyenDungViewModel>();

    // 1. Cập nhật thông tin cơ bản
    final okThongTin = await vm.capNhatThongTinNTD(
      maUser: user.userId,
      tencongty: _tenCongTyController.text.trim(),
      hotenNguoiDaiDien: _nguoiDaiDienController.text.trim(),
      sdtLienHe: _sdtController.text.trim(),
      linhvuc: _linhVucController.text.trim(),
      diachi: _diaChiController.text.trim(),
      link: _websiteController.text.trim(),
    );

    // 2. Cập nhật phần giới thiệu
    final okGioiThieu = await vm.capNhatGioiThieuNTD(
      maUser: user.userId,
      gioithieu: _gioiThieuController.text.trim(),
    );

    if (!mounted) return;
    setState(() => _dangLuu = false);

    if (okThongTin && okGioiThieu) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Cập nhật hồ sơ nhà tuyển dụng thành công!'),
          backgroundColor: AppColors.primary,
        ),
      );
      Navigator.pop(context, true);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            vm.thongBaoLoi ?? 'Có lỗi xảy ra khi lưu thông tin.',
          ),
          backgroundColor: Colors.red[700],
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFC),
      appBar: AppBar(
        title: const Text(
          'Chỉnh sửa hồ sơ công ty',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        backgroundColor: AppColors.primary,
        elevation: 0,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildSectionTitle('Thông tin doanh nghiệp'),
              const SizedBox(height: 12),
              _buildCard([
                TextFormField(
                  controller: _tenCongTyController,
                  decoration: const InputDecoration(
                    labelText: 'Tên công ty / Doanh nghiệp *',
                    hintText: 'Công ty Cổ phần Công nghệ ABC',
                    prefixIcon: Icon(Icons.business_outlined),
                    border: OutlineInputBorder(),
                  ),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Vui lòng nhập tên công ty';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: _linhVucController,
                  decoration: const InputDecoration(
                    labelText: 'Lĩnh vực hoạt động',
                    hintText: 'Ví dụ: Công nghệ thông tin, Thiết kế, Giáo dục...',
                    prefixIcon: Icon(Icons.category_outlined),
                    border: OutlineInputBorder(),
                  ),
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: _diaChiController,
                  decoration: const InputDecoration(
                    labelText: 'Địa chỉ trụ sở',
                    hintText: 'Quận 1, TP. Hồ Chí Minh',
                    prefixIcon: Icon(Icons.location_on_outlined),
                    border: OutlineInputBorder(),
                  ),
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: _websiteController,
                  decoration: const InputDecoration(
                    labelText: 'Đường dẫn Website',
                    hintText: 'https://congtyabc.vn',
                    prefixIcon: Icon(Icons.language_outlined),
                    border: OutlineInputBorder(),
                  ),
                ),
              ]),

              const SizedBox(height: 20),

              _buildSectionTitle('Thông tin người đại diện & Liên hệ'),
              const SizedBox(height: 12),
              _buildCard([
                TextFormField(
                  controller: _nguoiDaiDienController,
                  decoration: const InputDecoration(
                    labelText: 'Họ và tên người đại diện *',
                    hintText: 'Nguyễn Văn A',
                    prefixIcon: Icon(Icons.person_outline),
                    border: OutlineInputBorder(),
                  ),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Vui lòng nhập tên người đại diện';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: _sdtController,
                  keyboardType: TextInputType.phone,
                  decoration: const InputDecoration(
                    labelText: 'Số điện thoại liên hệ *',
                    hintText: '0901234567',
                    prefixIcon: Icon(Icons.phone_outlined),
                    border: OutlineInputBorder(),
                  ),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Vui lòng nhập số điện thoại';
                    }
                    return null;
                  },
                ),
              ]),

              const SizedBox(height: 20),

              _buildSectionTitle('Giới thiệu chi tiết doanh nghiệp'),
              const SizedBox(height: 12),
              _buildCard([
                TextFormField(
                  controller: _gioiThieuController,
                  maxLines: 5,
                  decoration: const InputDecoration(
                    labelText: 'Giới thiệu về văn hóa, sứ mệnh, môi trường làm việc...',
                    border: OutlineInputBorder(),
                    alignLabelWithHint: true,
                  ),
                ),
              ]),

              const SizedBox(height: 28),

              SizedBox(
                width: double.infinity,
                height: 48,
                child: ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    foregroundColor: Colors.white,
                    elevation: 0,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(6),
                    ),
                  ),
                  onPressed: _dangLuu ? null : _luuHoSo,
                  child: _dangLuu
                      ? const SizedBox(
                          width: 22,
                          height: 22,
                          child: CircularProgressIndicator(
                            strokeWidth: 2.5,
                            color: Colors.white,
                          ),
                        )
                      : const Text(
                          'Lưu thay đổi',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Text(
      title,
      style: const TextStyle(
        fontSize: 15,
        fontWeight: FontWeight.bold,
        color: AppColors.heading,
      ),
    );
  }

  Widget _buildCard(List<Widget> children) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: const Color(0xFFE2E8F0)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: children,
      ),
    );
  }
}

