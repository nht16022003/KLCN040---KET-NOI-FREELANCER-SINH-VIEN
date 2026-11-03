import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../models/mo_hinh_chinh_sua_ho_so_freelancer.dart';
import '../models/mo_hinh_ho_so_freelancer.dart';

class FreelancerProfileService {
  final ApiClient _apiClient = ApiClient();

  Future<ApiResponse<FreelancerProfileModel>> getFreelancerProfile(
    int maUser,
  ) async {
    try {
      final listResponse = await _apiClient.get<Map<String, dynamic>>(
        '/FreelancerStudent/DanhSachFreelancerStudent',
      );

      final listPayload = listResponse.data is Map<String, dynamic>
          ? Map<String, dynamic>.from(listResponse.data as Map<String, dynamic>)
          : <String, dynamic>{};

      final listData = listPayload['data'] is List
          ? listPayload['data'] as List
          : const <dynamic>[];

      final matchedFreelancer = listData.cast<dynamic>().firstWhereOrNull(
        (item) =>
            item is Map &&
            int.tryParse(item['maUser']?.toString() ?? '') == maUser,
      );

      final freelancerId = matchedFreelancer is Map
          ? (matchedFreelancer['maFreelancerStudents'] is num
                ? matchedFreelancer['maFreelancerStudents'] as num
                : int.tryParse(
                    matchedFreelancer['maFreelancerStudents']?.toString() ?? '',
                  ))
          : null;

      if (freelancerId == null) {
        return ApiResponse<FreelancerProfileModel>(
          isSuccess: false,
          message: 'Không tìm thấy hồ sơ freelancer cho tài khoản này.',
        );
      }

      return await getFreelancerProfileById(freelancerId.toInt());
    } catch (e) {
      return ApiResponse<FreelancerProfileModel>(
        isSuccess: false,
        message: 'Không thể tải hồ sơ người dùng: $e',
      );
    }
  }

  Future<ApiResponse<FreelancerProfileModel>> getFreelancerProfileById(
    int freelancerId,
  ) async {
    if (freelancerId <= 0) {
      return ApiResponse<FreelancerProfileModel>(
        isSuccess: false,
        message: 'Mã hồ sơ freelancer không hợp lệ.',
      );
    }

    try {
      final detailResponse = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.chiTietHoSoFreelancer}/$freelancerId',
      );
      final detailPayload = _readMap(detailResponse.data);
      final detail = _readMap(detailPayload['data']);
      if (detailPayload['success'] != true || detail.isEmpty) {
        return ApiResponse<FreelancerProfileModel>(
          isSuccess: false,
          message:
              detailPayload['message']?.toString() ??
              'Không thể tải thông tin chi tiết hồ sơ.',
        );
      }

      final profileResponse = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.profileFreelancer}/$freelancerId',
      );
      final profilePayload = _readMap(profileResponse.data);
      final profile = _readMap(profilePayload['data']);
      if (profilePayload['success'] != true || profile.isEmpty) {
        return ApiResponse<FreelancerProfileModel>(
          isSuccess: false,
          message:
              profilePayload['message']?.toString() ??
              'Không thể tải portfolio freelancer.',
        );
      }

      // Gọi thêm GET api/Portfolio/{maFreelancerStudents} để đồng bộ đầy đủ maDA, laDuAnNoiBat, moTaBanThan, url_video
      Map<String, dynamic> portfolioExtra = <String, dynamic>{};
      try {
        final pfRes = await _apiClient.get<Map<String, dynamic>>(
          '${ApiEndpoints.portfolioFreelancer}/$freelancerId',
        );
        final pfPayload = _readMap(pfRes.data);
        if (pfPayload['success'] == true && pfPayload['data'] is Map) {
          final pfData = _readMap(pfPayload['data']);
          portfolioExtra = {
            if (pfData['moTaBanThan'] != null)
              'moTaBanThan': pfData['moTaBanThan'],
            if (pfData['url_video'] != null) 'url_video': pfData['url_video'],
            if (pfData['projects'] is List) 'projects': pfData['projects'],
          };
        }
      } catch (_) {
        // Nếu sinh viên chưa khởi tạo Portfolio riêng thì giữ dữ liệu từ Profile
      }

      return ApiResponse<FreelancerProfileModel>(
        isSuccess: true,
        data: FreelancerProfileModel.fromJson({
          ...profile,
          ...detail,
          ...portfolioExtra,
        }),
        message:
            profilePayload['message']?.toString() ??
            'Tải thông tin freelancer thành công.',
      );
    } catch (e) {
      return ApiResponse<FreelancerProfileModel>(
        isSuccess: false,
        message: 'Không thể tải hồ sơ freelancer: $e',
      );
    }
  }

  Future<ApiResponse<bool>> updateFreelancerProfile(
    FreelancerProfileEditModel profile,
  ) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.capNhatHoSoFreelancer,
        profile.toJson(),
      );
      final payload = _readMap(response.data);
      return ApiResponse<bool>(
        isSuccess: payload['success'] == true,
        data: payload['success'] == true,
        message:
            payload['message']?.toString() ??
            'Không thể cập nhật hồ sơ freelancer.',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi cập nhật hồ sơ freelancer: $e',
      );
    }
  }

  /// Cập nhật giới thiệu và link video Portfolio (PUT api/Portfolio)
  Future<ApiResponse<bool>> updatePortfolioInfo({
    required int freelancerId,
    String? moTaBanThan,
    String? urlVideo,
  }) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.portfolioFreelancer,
        {
          'maFreelancerStudents': freelancerId,
          'moTaBanThan': moTaBanThan,
          'url_video': urlVideo,
        },
      );
      final payload = _readMap(response.data);
      return ApiResponse<bool>(
        isSuccess: payload['success'] == true,
        data: payload['success'] == true,
        message:
            payload['message']?.toString() ??
            'Cập nhật thông tin Portfolio thành công.',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi cập nhật thông tin Portfolio: $e',
      );
    }
  }

  /// Thêm dự án mới vào Portfolio (POST api/Portfolio/projects)
  Future<ApiResponse<bool>> addPortfolioProject({
    required int freelancerId,
    required String projectName,
    String? role,
    String? description,
    String? technologies,
    String? githubUrl,
    String? demoUrl,
    String? fileUrl,
  }) async {
    try {
      final response = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.duAnPortfolio,
        {
          'maFreelancerStudents': freelancerId,
          'tenDuAn': projectName,
          'vaiTro': role,
          'moTa': description,
          'congnghe': technologies,
          'linkGithub': githubUrl,
          'linkDemo': demoUrl,
          'link_file': fileUrl,
        },
      );
      final payload = _readMap(response.data);
      return ApiResponse<bool>(
        isSuccess: payload['success'] == true,
        data: payload['success'] == true,
        message:
            payload['message']?.toString() ??
            (payload['success'] == true
                ? 'Đã thêm dự án portfolio.'
                : 'Không thể thêm dự án portfolio.'),
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi thêm dự án portfolio: $e',
      );
    }
  }

  /// Chỉnh sửa dự án trong Portfolio (PUT api/Portfolio/projects)
  Future<ApiResponse<bool>> updatePortfolioProject({
    required String maDA,
    required int freelancerId,
    required String projectName,
    String? role,
    String? description,
    String? technologies,
    String? githubUrl,
    String? demoUrl,
    String? fileUrl,
  }) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.duAnPortfolio,
        {
          'maDA': maDA,
          'maFreelancerStudents': freelancerId,
          'tenDuAn': projectName,
          'vaiTro': role,
          'moTa': description,
          'congnghe': technologies,
          'linkGithub': githubUrl,
          'linkDemo': demoUrl,
          'link_file': fileUrl,
        },
      );
      final payload = _readMap(response.data);
      return ApiResponse<bool>(
        isSuccess: payload['success'] == true,
        data: payload['success'] == true,
        message:
            payload['message']?.toString() ??
            (payload['success'] == true
                ? 'Cập nhật dự án thành công.'
                : 'Không thể cập nhật dự án.'),
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi cập nhật dự án portfolio: $e',
      );
    }
  }

  /// Xóa dự án khỏi Portfolio (DELETE api/Portfolio/projects/{maDA}?maFreelancerStudents=...)
  Future<ApiResponse<bool>> deletePortfolioProject({
    required String maDA,
    required int freelancerId,
  }) async {
    try {
      final response = await _apiClient.delete<Map<String, dynamic>>(
        '${ApiEndpoints.duAnPortfolio}/$maDA',
        queryParameters: {'maFreelancerStudents': freelancerId},
      );
      final payload = _readMap(response.data);
      return ApiResponse<bool>(
        isSuccess: payload['success'] == true,
        data: payload['success'] == true,
        message:
            payload['message']?.toString() ??
            (payload['success'] == true
                ? 'Đã xóa dự án khỏi portfolio.'
                : 'Không thể xóa dự án.'),
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi xóa dự án portfolio: $e',
      );
    }
  }

  /// Chọn dự án nổi bật (PUT api/Portfolio/featured - Tối đa 2 dự án)
  Future<ApiResponse<bool>> updateFeaturedProjects({
    required int freelancerId,
    required List<String> maDAs,
  }) async {
    try {
      final response = await _apiClient.put<Map<String, dynamic>>(
        ApiEndpoints.duAnNoiBatPortfolio,
        {
          'maFreelancerStudents': freelancerId,
          'maDAs': maDAs,
        },
      );
      final payload = _readMap(response.data);
      return ApiResponse<bool>(
        isSuccess: payload['success'] == true,
        data: payload['success'] == true,
        message:
            payload['message']?.toString() ??
            (payload['success'] == true
                ? 'Đã cập nhật dự án nổi bật.'
                : 'Không thể cập nhật dự án nổi bật.'),
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: 'Lỗi khi cập nhật dự án nổi bật: $e',
      );
    }
  }

  Map<String, dynamic> _readMap(dynamic value) {
    if (value is Map<String, dynamic>) return value;
    if (value is Map) return Map<String, dynamic>.from(value);
    return <String, dynamic>{};
  }
}

extension _ListExt<T> on Iterable<T> {
  T? firstWhereOrNull(bool Function(T element) test) {
    for (final element in this) {
      if (test(element)) {
        return element;
      }
    }
    return null;
  }
}
