using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("BienTheSanPham")]
    public class BienTheSanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BienTheID { get; set; }

        [ForeignKey(nameof(SanPham))]
        public int SanPhamID { get; set; }
        public virtual SanPham SanPham { get; set; } = null!;

        [MaxLength(10)]
        public string Size { get; set; } = string.Empty;

        [MaxLength(50)]
        public string MauSac { get; set; } = string.Empty;

        [MaxLength(10)]
        public string MaMau { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        public int SoLuongTon { get; set; }

        public bool TrangThai { get; set; } = true;

        // Navigation collections
        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    }
}
