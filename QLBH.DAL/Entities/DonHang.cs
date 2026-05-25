using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonHangID { get; set; }

        [ForeignKey(nameof(KhachHang))]
        public int KhachHangID { get; set; }
        public virtual KhachHang KhachHang { get; set; } = null!;

        [ForeignKey(nameof(Voucher))]
        public int? VoucherID { get; set; }
        public virtual Voucher? Voucher { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;
        public DateTime HanThanhToan { get; set; } = DateTime.Now.AddMinutes(15);

        [Column(TypeName = "decimal(18,2)")]
        public decimal TamTinh { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiamGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [MaxLength(20)]
        public string TrangThai { get; set; } = "ChoXuLy"; // 'ChoXuLy', 'DaThanhToan', 'DaHuy'

        [MaxLength(100)]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [MaxLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [MaxLength(300)]
        public string DiaChi { get; set; } = string.Empty;

        [MaxLength(500)]
        public string GhiChu { get; set; } = string.Empty;
        [MaxLength(50)]
        public string PhuongThucThanhToan { get; set; } = "COD";

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
    }
}