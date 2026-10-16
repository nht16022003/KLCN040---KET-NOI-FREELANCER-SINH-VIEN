class StudentPostModel {
  final String id;
  final String title;
  final String summary;
  final double expectedSalary;
  final String skill;

  const StudentPostModel({
    required this.id,
    required this.title,
    required this.summary,
    required this.expectedSalary,
    required this.skill,
  });

  factory StudentPostModel.fromJson(Map<String, dynamic> json) {
    return StudentPostModel(
      id: json['id']?.toString() ?? '',
      title: json['title'] ?? '',
      summary: json['summary'] ?? '',
      expectedSalary: (json['expectedSalary'] as num?)?.toDouble() ?? 0,
      skill: json['skill'] ?? '',
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'title': title,
    'summary': summary,
    'expectedSalary': expectedSalary,
    'skill': skill,
  };
}
