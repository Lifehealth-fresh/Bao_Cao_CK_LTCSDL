using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace QLBH.BLL.DTOs
{
    public class ThanhToanDto
    {
        public int ThanhToanID { get; set; }
        public int DonHangID { get; set; }
        public int? NhanVienID { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
        public string PhuongThuc { get; set; } = string.Empty; // COD, ChuyenKhoan
        public decimal SoTien { get; set; }
        public string TrangThai { get; set; } = string.Empty; // ChoDuyet, DaDuyet, TuChoi
        public DateTime? NgayXuLy { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    public class ConfirmPaymentDto
    {
        public int ThanhToanID { get; set; }
        public int NhanVienID { get; set; }
        public bool ChapNhan { get; set; } // true = duyệt, false = từ chối
        public string? GhiChu { get; set; }
    }
}