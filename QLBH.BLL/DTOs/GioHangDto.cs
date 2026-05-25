using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBH.BLL.DTOs
{
    public class GioHangDto
    {
        public int GioHangID { get; set; }
        public int KhachHangID { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public List<ChiTietGioHangDto> ChiTietGioHangs { get; set; } = new();
        public decimal TongTien => ChiTietGioHangs.Sum(x => x.ThanhTien);
    }

    public class ChiTietGioHangDto
    {
        public int ChiTietID { get; set; }
        public int BienTheID { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string MauSac { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }

    public class AddToCartDto
    {
        public int KhachHangID { get; set; }
        public int BienTheID { get; set; }
        public int SoLuong { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int ChiTietID { get; set; }
        public int SoLuong { get; set; }
    }
}