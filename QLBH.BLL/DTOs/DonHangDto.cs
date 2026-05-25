using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBH.BLL.DTOs
{
    public class DonHangDto
    {
        public int DonHangID { get; set; }
        public int KhachHangID { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public int? VoucherID { get; set; }
        public string MaVoucher { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
        public DateTime HanThanhToan { get; set; }
        public decimal TamTinh { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = string.Empty; // ChoXuLy, DaThanhToan, DaHuy
        public string PhuongThucThanhToan { get; set; } = string.Empty; // COD, ChuyenKhoan
        public string TenNguoiNhan { get; set; } = string.Empty;
        public string SoDienThoaiNhan { get; set; } = string.Empty;
        public string DiaChiNhanHang { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public List<ChiTietDonHangDto> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangDto>();
    }

    public class ChiTietDonHangDto
    {
        public int ChiTietID { get; set; }
        public int DonHangID { get; set; }
        public int BienTheID { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string ThongTin { get; set; } = string.Empty; // Ví dụ: "Size M - Đen"
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class CreateDonHangDto
    {
        public int KhachHangID { get; set; }

        [Required(ErrorMessage = "Tên người nhận không được để trống")]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        public string DiaChi { get; set; } = string.Empty;

        public string? MaVoucher { get; set; }

        public string? GhiChu { get; set; }  // ← Không có [Required]

        public string PhuongThucThanhToan { get; set; } = "COD";
    }
}