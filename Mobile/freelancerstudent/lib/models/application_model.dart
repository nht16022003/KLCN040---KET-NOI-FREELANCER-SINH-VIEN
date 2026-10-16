class ApplicationModel {
  final String id;
  final String jobId;
  final String applicantId;
  final String coverLetter;
  final double proposedBudget;
  final String status;

  const ApplicationModel({
    required this.id,
    required this.jobId,
    required this.applicantId,
    required this.coverLetter,
    required this.proposedBudget,
    required this.status,
  });

  factory ApplicationModel.fromJson(Map<String, dynamic> json) {
    final rawId = json['maUngTuyen'] ?? json['id'] ?? '';
    final rawJobId = json['maJob'] ?? json['jobId'] ?? '';
    final rawApplicantId =
        json['maFreelancerStudents'] ?? json['applicantId'] ?? '';
    final rawCover = json['thuGioiThieu'] ?? json['coverLetter'] ?? '';
    final rawBudget = json['thulaoDeXuat'] ?? json['proposedBudget'] ?? 0;
    final rawStatus = json['trangThaiUngTuyen'] ?? json['status'] ?? 'Pending';

    return ApplicationModel(
      id: rawId.toString(),
      jobId: rawJobId.toString(),
      applicantId: rawApplicantId.toString(),
      coverLetter: rawCover.toString(),
      proposedBudget: rawBudget is num ? rawBudget.toDouble() : 0,
      status: rawStatus.toString(),
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'jobId': jobId,
    'applicantId': applicantId,
    'coverLetter': coverLetter,
    'proposedBudget': proposedBudget,
    'status': status,
  };
}
