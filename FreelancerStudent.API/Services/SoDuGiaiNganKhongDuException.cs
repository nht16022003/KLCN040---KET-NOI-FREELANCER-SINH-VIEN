namespace FreelancerStudent.API.Services
{
    public class SoDuGiaiNganKhongDuException : InvalidOperationException
    {
        public SoDuGiaiNganKhongDuException(string message = "Số dư đóng băng (ký quỹ) của nhà tuyển dụng không đủ để giải ngân hợp đồng.")
            : base(message)
        {
        }
    }
}
