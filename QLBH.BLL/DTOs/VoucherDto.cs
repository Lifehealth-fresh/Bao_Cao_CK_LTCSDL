using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace QLBH.BLL.DTOs
{
    public class VoucherDto
    {
        public int VoucherID { get; set; }
        public string MaVoucher { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string LoaiGiam { get; set; } = string.Empty; // PhanTram, TienMat
        public decimal GiaTriGiam { get; set; }
        public decimal DonToiThieu { get; set; }
        public decimal? GiamToiDa { get; set; }
        public int SoLuong { get; set; }
        public int DaSuDung { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }
        public bool ConHieuLuc => TrangThai && SoLuong > DaSuDung && DateTime.Now >= NgayBatDau && DateTime.Now <= NgayKetThuc;
    }

    public class CreateVoucherDto
    {
        public string MaVoucher { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string LoaiGiam { get; set; } = string.Empty;
        public decimal GiaTriGiam { get; set; }
        public decimal DonToiThieu { get; set; }
        public decimal? GiamToiDa { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; } = true;
    }

    public class UpdateVoucherDto : CreateVoucherDto
    {
        public int VoucherID { get; set; }
        public int DaSuDung { get; set; }
    }
}