namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class DanhGiaThongKe_ReponseDTO
    {
        public double diemTrungBinh { get; set; }
        public int tongLuotDanhGia { get; set; }
        public double trungBinhChatLuong { get; set; }
        public double trungBinhDungHan { get; set; }
        public double trungBinhGiaoTiep { get; set; }
        public List<int> phanBoSao { get; set; } = new();
        public double tyLeHaiLong { get; set; }
        public int trang { get; set; }
        public int kichThuocTrang { get; set; }
        public int tongSoTrang { get; set; }
        public List<DanhGia_ReponseDTO> danhSach { get; set; } = new();
    }
}
