using FreelancerStudent.API.Models;
using Microsoft.Extensions.Options;

namespace FreelancerStudent.API.Options
{
    public class DanhGiaPolicy
    {
        private readonly NghiepVuOptions _options;

        public DanhGiaPolicy(IOptions<NghiepVuOptions> options)
        {
            _options = options.Value;
        }

        public bool CoTheDanhGia(HopDong hopDong)
        {
            return hopDong.trangThai is "DaThanhToan" or "DaHuyHoanTien"
                && hopDong.ngayDongHopDong.HasValue
                && DateTime.Now <= hopDong.ngayDongHopDong.Value.AddDays(_options.SoNgayChoDanhGia);
        }

        public DateTime? HanDanhGia(HopDong hopDong)
            => hopDong.ngayDongHopDong?.AddDays(_options.SoNgayChoDanhGia);

        public DateTime HanChinhSua(DateTime ngayDanhGia)
            => ngayDanhGia.AddDays(_options.SoNgayChinhSuaDanhGia);
    }
}
