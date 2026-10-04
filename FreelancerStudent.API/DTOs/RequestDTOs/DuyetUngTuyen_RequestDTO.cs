namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class DuyetUngTuyen_RequestDTO
    {
        public int maUngTuyen { get; set; }
        public string trangThai { get; set; } = string.Empty; // "ChapNhan" hoặc "TuChoi"
    }
}
