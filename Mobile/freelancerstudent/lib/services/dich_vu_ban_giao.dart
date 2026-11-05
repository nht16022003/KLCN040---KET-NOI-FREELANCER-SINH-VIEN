import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_ban_giao.dart';

class BanGiaoService {
  final ApiClient _apiClient = ApiClient();

  /// Lấy danh sách các phiên bản bàn giao của hợp đồng (GET api/BanGiaoSanPham/hopdong/{maHD})
  Future<ApiResponse<List<BanGiaoModel>>> layDanhSachBanGiao(String maHD, {int? maUser}) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.banGiaoSanPhamHopDong}/$maHD',
        queryParameters: maUser != null && maUser > 0 ? {'maUser': maUser} : null,
      );

      final resData = response.data;
      if (resData == null) {
        return const ApiResponse(isSuccess: false, message: 'Không có dữ liệu trả về.');
      }

      final rawList = resData['data'] as List<dynamic>? ?? [];
      final items = rawList
          .map((item) => BanGiaoModel.fromJson(item as Map<String, dynamic>))
          .toList();

      return ApiResponse<List<BanGiaoModel>>(
        isSuccess: resData['success'] ?? true,
        data: items,
        message: resData['message'] ?? '',
      );
    } catch (e) {
      return ApiResponse<List<BanGiaoModel>>(
        isSuccess: false,
        message: 'Lỗi khi tải lịch sử bàn giao: ${e.toString()}',
      );
    }
  }

  /// Lấy danh sách yêu cầu chỉnh sửa của hợp đồng (GET api/BanGiaoSanPham/yeucauchinhsua/{maHD})
  Future<ApiResponse<List<YeuCauChinhSuaModel>>> layDanhSachYeuCau(String maHD, {int? maUser}) async {
    try {
      final response = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.yeuCauChinhSua}/$maHD',
        queryParameters: maUser != null && maUser > 0 ? {'maUser': maUser} : null,
      );

      final resData = response.data;
      if (resData == null) {
        return const ApiResponse(isSuccess: false, message: 'Không có dữ liệu trả về.');
      }

      final rawList = resData['data'] as List<dynamic>? ?? [];
      final items = rawList
          .map((item) => YeuCauChinhSuaModel.fromJson(item as Map<String, dynamic>))
          .toList();

      return ApiResponse<List<YeuCauChinhSuaModel>>(
        isSuccess: resData['success'] ?? true,
        data: items,
        message: resData['message'] ?? '',
      );
    } catch (e) {
      return ApiResponse<List<YeuCauChinhSuaModel>>(
        isSuccess: false,
        message: 'Lỗi khi tải yêu cầu chỉnh sửa: ${e.toString()}',
      );
    }
  }

  /// Freelancer nộp bàn giao sản phẩm (POST api/BanGiaoSanPham)
  Future<ApiResponse<BanGiaoModel>> taoBanGiao({
    required String maHD,
    required String fileSanPhamUrl,
    String? linkDemo,
    required String moTaBanGiao,
    int? maUser,
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.banGiaoSanPham,
        {
          'maHD': maHD,
          'fileSanPhamUrl': fileSanPhamUrl,
          if (linkDemo != null && linkDemo.isNotEmpty) 'linkDemo': linkDemo,
          'moTaBanGiao': moTaBanGiao,
        },
        queryParameters: maUser != null && maUser > 0 ? {'maUser': maUser} : null,
      );

      final resData = response.data;
      if (resData == null) {
        return const ApiResponse(isSuccess: false, message: 'Không có dữ liệu trả về.');
      }

      final isSuccess = resData['success'] ?? false;
      final rawData = resData['data'] as Map<String, dynamic>?;

      return ApiResponse<BanGiaoModel>(
        isSuccess: isSuccess,
        data: rawData != null ? BanGiaoModel.fromJson(rawData) : null,
        message: resData['message'] ?? '',
      );
    } catch (e) {
      return ApiResponse<BanGiaoModel>(
        isSuccess: false,
        message: 'Lỗi khi gửi bàn giao sản phẩm: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng gửi yêu cầu chỉnh sửa (POST api/BanGiaoSanPham/yeucauchinhsua)
  Future<ApiResponse<dynamic>> guiYeuCauChinhSua({
    required int maBanGiao,
    required String noiDungYeuCau,
    int? maUser,
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.yeuCauChinhSua,
        {
          'maBanGiao': maBanGiao,
          'noiDungYeuCau': noiDungYeuCau,
        },
        queryParameters: maUser != null && maUser > 0 ? {'maUser': maUser} : null,
      );

      final resData = response.data;
      return ApiResponse<dynamic>(
        isSuccess: resData?['success'] ?? false,
        message: resData?['message'] ?? '',
        data: resData?['data'],
      );
    } catch (e) {
      return ApiResponse<dynamic>(
        isSuccess: false,
        message: 'Lỗi khi gửi yêu cầu chỉnh sửa: ${e.toString()}',
      );
    }
  }

  /// Nhà tuyển dụng nghiệm thu sản phẩm (PUT api/BanGiaoSanPham/nghiemthu/{maBanGiao})
  Future<ApiResponse<dynamic>> nghiemThu(int maBanGiao, {int? maUser}) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        '${ApiEndpoints.nghiemThuBanGiao}/$maBanGiao',
        {},
        queryParameters: maUser != null && maUser > 0 ? {'maUser': maUser} : null,
      );

      final resData = response.data;
      return ApiResponse<dynamic>(
        isSuccess: resData?['success'] ?? false,
        message: resData?['message'] ?? '',
        data: resData?['data'],
      );
    } catch (e) {
      return ApiResponse<dynamic>(
        isSuccess: false,
        message: 'Lỗi khi nghiệm thu sản phẩm: ${e.toString()}',
      );
    }
  }
}
