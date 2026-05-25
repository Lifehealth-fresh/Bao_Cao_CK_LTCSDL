using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace QLBH.BLL.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TongDoanhThu { get; set; }
        public int TongDonHangThanhCong { get; set; }
        public int SoSanPhamSapHetHang { get; set; }
        public List<SanPhamBanChayDto> TopSanPhamBanChay { get; set; } = new List<SanPhamBanChayDto>();
        public List<KhachHangThongKeDto> TopKhachHang { get; set; } = new List<KhachHangThongKeDto>();
    }

    public class SanPhamBanChayDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int SoLuongDaBan { get; set; }
        public decimal DoanhThuMangLai { get; set; }
    }

    public class KhachHangThongKeDto
    {
        public string HoTen { get; set; } = string.Empty;
        public int SoDonDaMua { get; set; }
        public decimal TongChiTieu { get; set; }
    }

    public class DoanhThuTheoNgayDto
    {
        public DateTime Ngay { get; set; }
        public int SoDon { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DoanhThuTheoThangDto
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int SoDon { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DoanhThuTheoDanhMucDto
    {
        public string TenDanhMuc { get; set; } = string.Empty;
        public decimal DoanhThu { get; set; }
    }

    public class SanPhamSapHetDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
    }

    public class DonHangThanhCongDto
    {
        public int DonHangID { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public string KhachHang { get; set; } = string.Empty;
    }

}
