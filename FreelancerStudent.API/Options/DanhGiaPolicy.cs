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
            if (hopDong.trangThai is not ("DaThanhToan" or "DaHoanThanh" or "DaHuyHoanTien"))
                return false;
            var closingDate = hopDong.ngayDongHopDong ?? hopDong.ngayHoanThanh ?? hopDong.ngayKetThuc;
            if (!closingDate.HasValue)
                return true;
            return DateTime.Now <= closingDate.Value.AddDays(_options.SoNgayChoDanhGia);
        }

        public DateTime? HanDanhGia(HopDong hopDong)
            => (hopDong.ngayDongHopDong ?? hopDong.ngayHoanThanh ?? hopDong.ngayKetThuc)?.AddDays(_options.SoNgayChoDanhGia);

        public DateTime HanChinhSua(DateTime ngayDanhGia)
            => ngayDanhGia.AddDays(_options.SoNgayChinhSuaDanhGia);
    }
}
