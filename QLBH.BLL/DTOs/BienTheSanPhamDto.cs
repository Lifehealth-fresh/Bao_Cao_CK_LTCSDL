using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace QLBH.BLL.DTOs
{
    public class BienTheSanPhamDto
    {
        public int BienTheID { get; set; }
        public int SanPhamID { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string MauSac { get; set; } = string.Empty;
        public string MaMau { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public int SoLuongTon { get; set; }
        public bool TrangThai { get; set; }
        public string TenHienThi => $"{TenSanPham} - {Size} - {MauSac}";
    }

    public class CreateBienTheSanPhamDto
    {
        public int SanPhamID { get; set; }
        public string Size { get; set; } = string.Empty;
        public string MauSac { get; set; } = string.Empty;
        public string MaMau { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public int SoLuongTon { get; set; }
        public bool TrangThai { get; set; } = true;
    }

    public class UpdateBienTheSanPhamDto : CreateBienTheSanPhamDto
    {
        public int BienTheID { get; set; }
    }
}
