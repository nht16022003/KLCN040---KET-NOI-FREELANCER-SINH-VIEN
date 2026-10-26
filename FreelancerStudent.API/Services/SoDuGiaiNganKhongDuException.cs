namespace FreelancerStudent.API.Services
{
    public class SoDuGiaiNganKhongDuException : InvalidOperationException
    {
        public SoDuGiaiNganKhongDuException()
            : base("Số dư không đủ để giải ngân. Vui lòng nạp thêm tiền.")
        {
        }
    }
}
