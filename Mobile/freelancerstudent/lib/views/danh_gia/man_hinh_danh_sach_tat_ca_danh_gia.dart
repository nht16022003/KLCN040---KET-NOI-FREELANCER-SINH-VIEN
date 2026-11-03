import 'package:flutter/material.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_danh_gia.dart';
import '../../services/dich_vu_danh_gia.dart';

class ManHinhDanhSachTatCaDanhGia extends StatefulWidget {
  final int? maFreelancerStudent;
  final int? maNhaTuyenDung;
  final String tenDoiTuong;
  final bool laNhaTuyenDung;

  const ManHinhDanhSachTatCaDanhGia({
    super.key,
    this.maFreelancerStudent,
    this.maNhaTuyenDung,
    required this.tenDoiTuong,
    this.laNhaTuyenDung = false,
  }) : assert(
          maFreelancerStudent != null || maNhaTuyenDung != null,
          'Cần cung cấp ít nhất mã Freelancer hoặc mã Nhà tuyển dụng',
        );

  @override
  State<ManHinhDanhSachTatCaDanhGia> createState() =>
      _ManHinhDanhSachTatCaDanhGiaState();
}

class _ManHinhDanhSachTatCaDanhGiaState
    extends State<ManHinhDanhSachTatCaDanhGia> {
  final DanhGiaService _service = DanhGiaService();
  final ScrollController _scrollController = ScrollController();

  bool _dangTai = true;
  bool _dangTaiThem = false;
  String? _thongBaoLoi;
  ThongKeDanhGiaModel? _thongKe;
  final List<DanhGiaModel> _danhSach = [];
  int _trangHienTai = 1;
  int _tongSoTrang = 1;

  @override
  void initState() {
    super.initState();
    _taiDuLieu(trang: 1);
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
            _scrollController.position.maxScrollExtent - 200 &&
        !_dangTai &&
        !_dangTaiThem &&
        _trangHienTai < _tongSoTrang) {
      _taiDuLieu(trang: _trangHienTai + 1, laTaiThem: true);
    }
  }

  Future<void> _taiDuLieu({int trang = 1, bool laTaiThem = false}) async {
    if (laTaiThem) {
      setState(() => _dangTaiThem = true);
    } else {
      setState(() {
        _dangTai = true;
        _thongBaoLoi = null;
      });
    }

    try {
      final res = widget.laNhaTuyenDung
          ? await _service.layDanhGiaNhaTuyenDung(
              maNhaTuyenDung: widget.maNhaTuyenDung!,
              trang: trang,
              kichThuocTrang: 10,
            )
          : await _service.layDanhGiaFreelancer(
              maFreelancerStudent: widget.maFreelancerStudent!,
              trang: trang,
              kichThuocTrang: 10,
            );

      if (!mounted) return;

      if (res.isSuccess && res.data != null) {
        setState(() {
          _thongKe = res.data;
          _trangHienTai = res.data!.trang;
          _tongSoTrang = res.data!.tongSoTrang;
          if (laTaiThem) {
            _danhSach.addAll(res.data!.danhSach);
          } else {
            _danhSach
              ..clear()
              ..addAll(res.data!.danhSach);
          }
          _thongBaoLoi = null;
        });
      } else {
        setState(() {
          _thongBaoLoi = res.message.isNotEmpty
              ? res.message
              : 'Không thể tải danh sách đánh giá.';
        });
      }
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _thongBaoLoi = 'Lỗi kết nối: ${e.toString()}';
      });
    } finally {
      if (mounted) {
        setState(() {
          _dangTai = false;
          _dangTaiThem = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final titleText = widget.laNhaTuyenDung
        ? 'Đánh giá Nhà tuyển dụng'
        : 'Đánh giá Freelancer';

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              titleText,
              style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold),
            ),
            Text(
              widget.tenDoiTuong,
              style: const TextStyle(fontSize: 12, color: AppColors.mutedText),
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_dangTai && _danhSach.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_thongBaoLoi != null && _danhSach.isEmpty) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.error_outline, size: 48, color: AppColors.error),
              const SizedBox(height: 12),
              Text(
                _thongBaoLoi!,
                textAlign: TextAlign.center,
                style: const TextStyle(color: AppColors.bodyText),
              ),
              const SizedBox(height: 16),
              FilledButton.icon(
                onPressed: () => _taiDuLieu(trang: 1),
                icon: const Icon(Icons.refresh),
                label: const Text('Thử lại'),
                style: FilledButton.styleFrom(backgroundColor: AppColors.primary),
              ),
            ],
          ),
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: () => _taiDuLieu(trang: 1),
      child: ListView(
        controller: _scrollController,
        padding: const EdgeInsets.all(16),
        children: [
          // Thẻ thống kê tổng quan
          if (_thongKe != null) _buildOverviewCard(_thongKe!),
          const SizedBox(height: 16),

          // Tiêu đề danh sách
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Tất cả nhận xét (${_thongKe?.tongLuotDanhGia ?? _danhSach.length})',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const Text(
                'Mới nhất trước',
                style: TextStyle(fontSize: 12, color: AppColors.mutedText),
              ),
            ],
          ),
          const SizedBox(height: 12),

          if (_danhSach.isEmpty)
            Container(
              padding: const EdgeInsets.symmetric(vertical: 40, horizontal: 20),
              alignment: Alignment.center,
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: AppColors.border),
              ),
              child: Column(
                children: [
                  Icon(Icons.rate_review_outlined,
                      size: 48, color: Colors.grey.shade400),
                  const SizedBox(height: 12),
                  const Text(
                    'Chưa có đánh giá nào.',
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.w600,
                      color: AppColors.heading,
                    ),
                  ),
                  const SizedBox(height: 4),
                  const Text(
                    'Đánh giá sẽ hiển thị khi các hợp đồng dịch vụ được hoàn tất.',
                    textAlign: TextAlign.center,
                    style: TextStyle(fontSize: 12, color: AppColors.mutedText),
                  ),
                ],
              ),
            )
          else ...[
            ..._danhSach.map((item) => _buildReviewItemCard(item)),
            if (_dangTaiThem)
              const Padding(
                padding: EdgeInsets.symmetric(vertical: 16),
                child: Center(child: CircularProgressIndicator()),
              ),
          ],
        ],
      ),
    );
  }

  Widget _buildOverviewCard(ThongKeDanhGiaModel stats) {
    return Card(
      elevation: 0,
      color: AppColors.surface,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: Colors.grey.shade200),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.baseline,
                      textBaseline: TextBaseline.alphabetic,
                      children: [
                        Text(
                          stats.diemTrungBinh.toStringAsFixed(1),
                          style: const TextStyle(
                            fontSize: 32,
                            fontWeight: FontWeight.bold,
                            color: AppColors.heading,
                          ),
                        ),
                        const SizedBox(width: 4),
                        const Text(
                          '/ 5.0',
                          style: TextStyle(
                            fontSize: 14,
                            color: AppColors.mutedText,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    _buildStarRating(stats.diemTrungBinh),
                    const SizedBox(height: 4),
                    Text(
                      '${stats.tongLuotDanhGia} lượt đánh giá',
                      style: const TextStyle(
                        fontSize: 12,
                        color: AppColors.mutedText,
                      ),
                    ),
                  ],
                ),
                const SizedBox(width: 24),
                // Thanh phân bổ sao
                Expanded(
                  child: Column(
                    children: List.generate(5, (index) {
                      final star = 5 - index;
                      final count = stats.phanBoSao.length >= star
                          ? stats.phanBoSao[star - 1]
                          : 0;
                      final percent = stats.tongLuotDanhGia > 0
                          ? count / stats.tongLuotDanhGia
                          : 0.0;

                      return Padding(
                        padding: const EdgeInsets.symmetric(vertical: 2),
                        child: Row(
                          children: [
                            Text(
                              '$star',
                              style: const TextStyle(
                                fontSize: 11,
                                fontWeight: FontWeight.bold,
                                color: AppColors.heading,
                              ),
                            ),
                            const Icon(Icons.star,
                                size: 12, color: Colors.amber),
                            const SizedBox(width: 6),
                            Expanded(
                              child: ClipRRect(
                                borderRadius: BorderRadius.circular(4),
                                child: LinearProgressIndicator(
                                  value: percent,
                                  minHeight: 6,
                                  backgroundColor: Colors.grey.shade200,
                                  valueColor:
                                      const AlwaysStoppedAnimation<Color>(
                                    Colors.amber,
                                  ),
                                ),
                              ),
                            ),
                            const SizedBox(width: 6),
                            SizedBox(
                              width: 20,
                              child: Text(
                                '$count',
                                textAlign: TextAlign.right,
                                style: const TextStyle(
                                  fontSize: 11,
                                  color: AppColors.mutedText,
                                ),
                              ),
                            ),
                          ],
                        ),
                      );
                    }),
                  ),
                ),
              ],
            ),
            if (stats.trungBinhChatLuong > 0 ||
                stats.trungBinhDungHan > 0 ||
                stats.trungBinhGiaoTiep > 0) ...[
              const Divider(height: 24),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  if (stats.trungBinhChatLuong > 0)
                    _buildSubRatingItem(
                      widget.laNhaTuyenDung ? 'Rõ ràng yêu cầu' : 'Chất lượng',
                      stats.trungBinhChatLuong,
                    ),
                  if (stats.trungBinhDungHan > 0)
                    _buildSubRatingItem(
                      widget.laNhaTuyenDung ? 'Thanh toán' : 'Đúng hạn',
                      stats.trungBinhDungHan,
                    ),
                  if (stats.trungBinhGiaoTiep > 0)
                    _buildSubRatingItem(
                      'Giao tiếp',
                      stats.trungBinhGiaoTiep,
                    ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildSubRatingItem(String label, double score) {
    return Column(
      children: [
        Text(
          score.toStringAsFixed(1),
          style: const TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 15,
            color: AppColors.primaryDark,
          ),
        ),
        const SizedBox(height: 2),
        Text(
          label,
          style: const TextStyle(fontSize: 11, color: AppColors.mutedText),
        ),
      ],
    );
  }

  Widget _buildReviewItemCard(DanhGiaModel item) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      elevation: 0,
      color: AppColors.surface,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: Colors.grey.shade200),
      ),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                CircleAvatar(
                  radius: 18,
                  backgroundColor: AppColors.primary.withValues(alpha: 0.12),
                  child: Text(
                    item.tenNguoiDanhGia.isNotEmpty
                        ? item.tenNguoiDanhGia[0].toUpperCase()
                        : 'U',
                    style: const TextStyle(
                      color: AppColors.primary,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        item.tenNguoiDanhGia,
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 14,
                          color: AppColors.heading,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Row(
                        children: [
                          _buildStarRating(item.soSaoTongThe.toDouble(),
                              size: 14),
                          const SizedBox(width: 6),
                          Text(
                            AppFormatters.formatDateTime(item.ngayDanhGia),
                            style: const TextStyle(
                              fontSize: 11,
                              color: AppColors.mutedText,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
            if (item.tenCongViec.isNotEmpty) ...[
              const SizedBox(height: 8),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: Colors.grey.shade100,
                  borderRadius: BorderRadius.circular(4),
                ),
                child: Text(
                  'Dự án: ${item.tenCongViec}',
                  style: TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.w600,
                    color: Colors.grey.shade800,
                  ),
                ),
              ),
            ],
            if (item.nhanXet != null && item.nhanXet!.trim().isNotEmpty) ...[
              const SizedBox(height: 8),
              Text(
                item.nhanXet!.trim(),
                style: const TextStyle(
                  fontSize: 13,
                  color: AppColors.bodyText,
                  height: 1.4,
                ),
              ),
            ],
            if (item.soSaoChatLuong != null ||
                item.soSaoDungHan != null ||
                item.soSaoGiaoTiep != null) ...[
              const SizedBox(height: 10),
              Wrap(
                spacing: 12,
                runSpacing: 4,
                children: [
                  if (item.soSaoChatLuong != null)
                    _buildCriterionBadge('Chất lượng', item.soSaoChatLuong!),
                  if (item.soSaoDungHan != null)
                    _buildCriterionBadge('Đúng hạn', item.soSaoDungHan!),
                  if (item.soSaoGiaoTiep != null)
                    _buildCriterionBadge('Giao tiếp', item.soSaoGiaoTiep!),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildCriterionBadge(String label, int star) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          '$label: ',
          style: const TextStyle(fontSize: 11, color: AppColors.mutedText),
        ),
        Text(
          '$star ★',
          style: const TextStyle(
            fontSize: 11,
            fontWeight: FontWeight.bold,
            color: Colors.amber,
          ),
        ),
      ],
    );
  }

  Widget _buildStarRating(double rating, {double size = 16}) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: List.generate(5, (index) {
        if (index < rating.floor()) {
          return Icon(Icons.star, size: size, color: Colors.amber);
        } else if (index < rating) {
          return Icon(Icons.star_half, size: size, color: Colors.amber);
        } else {
          return Icon(Icons.star_border, size: size, color: Colors.amber);
        }
      }),
    );
  }
}
