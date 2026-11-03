import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_danh_gia.dart';

class DanhGiaService {
  final ApiClient _apiClient = ApiClient();

  /// Lấy thông tin form đánh giá theo hợp đồng (GET api/DanhGia/hop-dong/{maHD}?maUser=...)
  Future<ApiResponse<FormDanhGiaModel>> layFormDanhGia({
    required String maHD,
    required int maUser,
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhGiaHopDong}/$maHD',
        queryParameters: {'maUser': maUser},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final data = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<FormDanhGiaModel>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: FormDanhGiaModel.fromJson(data),
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<FormDanhGiaModel>(
        isSuccess: false,
        message: 'Lỗi tải form đánh giá: ${e.toString()}',
      );
    }
  }

  /// Gửi đánh giá mới (POST api/DanhGia?maUser=...)
  Future<ApiResponse<bool>> guiDanhGia({
    required int maUser,
    required String maHD,
    required int soSaoTongThe,
    int? soSaoChatLuong,
    int? soSaoDungHan,
    int? soSaoGiaoTiep,
    String? nhanXet,
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.danhGia,
        {
          'maHD': maHD,
          'soSaoTongThe': soSaoTongThe,
          'soSaoChatLuong': soSaoChatLuong,
          'soSaoDungHan': soSaoDungHan,
          'soSaoGiaoTiep': soSaoGiaoTiep,
          'nhanXet': nhanXet,
        },
        queryParameters: {'maUser': maUser},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? 'Gửi đánh giá thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi gửi đánh giá: ${e.toString()}',
      );
    }
  }

  /// Cập nhật đánh giá đã gửi trong vòng 7 ngày (PUT api/DanhGia/{maDanhGia}?maUser=...)
  Future<ApiResponse<bool>> capNhatDanhGia({
    required int maDanhGia,
    required int maUser,
    required String maHD,
    required int soSaoTongThe,
    int? soSaoChatLuong,
    int? soSaoDungHan,
    int? soSaoGiaoTiep,
    String? nhanXet,
  }) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        '${ApiEndpoints.danhGia}/$maDanhGia',
        {
          'maHD': maHD,
          'soSaoTongThe': soSaoTongThe,
          'soSaoChatLuong': soSaoChatLuong,
          'soSaoDungHan': soSaoDungHan,
          'soSaoGiaoTiep': soSaoGiaoTiep,
          'nhanXet': nhanXet,
        },
        queryParameters: {'maUser': maUser},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};

      return ApiResponse<bool>(
        isSuccess: body['success'] == true || response.statusCode == 200,
        data: true,
        message: body['message']?.toString() ?? 'Cập nhật đánh giá thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi cập nhật đánh giá: ${e.toString()}',
      );
    }
  }

  /// Lấy danh sách hợp đồng đã hoàn thành để đánh giá (GET api/DanhGia/cong-viec-da-thue?maUser=...)
  Future<ApiResponse<List<HopDongDanhGiaModel>>> layCongViecDaThue(int maUser) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        ApiEndpoints.congViecDaThueDanhGia,
        queryParameters: {'maUser': maUser},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final rawList = body['data'] is List ? body['data'] as List : [];

      final list = rawList
          .whereType<Map<String, dynamic>>()
          .map((e) => HopDongDanhGiaModel.fromJson(e))
          .toList();

      return ApiResponse<List<HopDongDanhGiaModel>>(
        isSuccess: true,
        data: list,
        message: body['message']?.toString() ?? '',
      );
    } catch (e) {
      return ApiResponse<List<HopDongDanhGiaModel>>(
        isSuccess: false,
        message: 'Lỗi tải danh sách hợp đồng đã thuê: ${e.toString()}',
      );
    }
  }

  /// Lấy thống kê và danh sách đánh giá của Freelancer (GET api/DanhGia/freelancer/{maFreelancerStudent})
  Future<ApiResponse<ThongKeDanhGiaModel>> layDanhGiaFreelancer({
    required int maFreelancerStudent,
    int trang = 1,
    int kichThuocTrang = 10,
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhGiaFreelancer}/$maFreelancerStudent',
        queryParameters: {'trang': trang, 'kichThuocTrang': kichThuocTrang},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final data = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<ThongKeDanhGiaModel>(
        isSuccess: true,
        data: ThongKeDanhGiaModel.fromJson(data),
      );
    } catch (e) {
      return ApiResponse<ThongKeDanhGiaModel>(
        isSuccess: false,
        message: 'Lỗi tải đánh giá Freelancer: ${e.toString()}',
      );
    }
  }

  /// Lấy thống kê và danh sách đánh giá của Nhà tuyển dụng (GET api/DanhGia/nha-tuyen-dung/{maNhaTuyenDung})
  Future<ApiResponse<ThongKeDanhGiaModel>> layDanhGiaNhaTuyenDung({
    required int maNhaTuyenDung,
    int trang = 1,
    int kichThuocTrang = 10,
  }) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhGiaNhaTuyenDung}/$maNhaTuyenDung',
        queryParameters: {'trang': trang, 'kichThuocTrang': kichThuocTrang},
      );
      final body = response.data is Map<String, dynamic>
          ? response.data as Map<String, dynamic>
          : <String, dynamic>{};
      final data = body['data'] is Map<String, dynamic>
          ? body['data'] as Map<String, dynamic>
          : body;

      return ApiResponse<ThongKeDanhGiaModel>(
        isSuccess: true,
        data: ThongKeDanhGiaModel.fromJson(data),
      );
    } catch (e) {
      return ApiResponse<ThongKeDanhGiaModel>(
        isSuccess: false,
        message: 'Lỗi tải đánh giá Nhà tuyển dụng: ${e.toString()}',
      );
    }
  }
}
