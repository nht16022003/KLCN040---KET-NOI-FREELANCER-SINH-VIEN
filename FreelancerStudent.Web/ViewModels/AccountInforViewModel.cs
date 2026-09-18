namespace FreelancerStudent.Web.ViewModels
{
    public class AccountInforViewModel
    {
        public string userID { get; set; } = string.Empty;
        public string userName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public string loaiUser { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }

        // Dữ liệu lấy từ bảng FreelancerSV
        public string free_ID { get; set; } = string.Empty;
        public DateTime? birth { get; set; }
        public string gender { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string university { get; set; } = string.Empty;
        public int? AcademicYearStart { get; set; }
        public int? AcademicYearEnd { get; set; }
        public string major { get; set; } = string.Empty;
        public string studentCardID { get; set; } = string.Empty;
        public double? GPA { get; set; }
    }
}
