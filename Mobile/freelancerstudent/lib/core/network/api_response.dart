class ApiResponse<T> {
  final bool isSuccess;
  final T? data;
  final String message;
  final int? errorCode;

  const ApiResponse({
    required this.isSuccess,
    this.data,
    this.message = '',
    this.errorCode,
  });

  factory ApiResponse.fromJson(
    Map<String, dynamic> json,
    T Function(dynamic data)? fromJson,
  ) {
    final dynamic rawData = json['data'];
    final parsedData = fromJson != null && rawData != null
        ? fromJson(rawData)
        : null;

    final bool success = json['isSuccess'] ?? json['success'] ?? false;

    return ApiResponse<T>(
      isSuccess: success,
      data: parsedData,
      message: json['message'] ?? '',
      errorCode: json['errorCode'] as int?,
    );
  }
}
