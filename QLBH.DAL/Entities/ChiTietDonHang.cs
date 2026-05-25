using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QLBH.DAL.Entities
{
    [Table("ChiTietDonHang")]
    public class ChiTietDonHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChiTietID { get; set; }

        [ForeignKey(nameof(DonHang))]
        public int DonHangID { get; set; }
        public virtual DonHang DonHang { get; set; } = null!;

        [ForeignKey(nameof(BienTheSanPham))]
        public int BienTheID { get; set; }
        public virtual BienTheSanPham BienTheSanPham { get; set; } = null!;

        [MaxLength(200)]
        public string TenSanPham { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ThongTin { get; set; } = string.Empty; // "Size M - Đen"

        [Range(1, int.MaxValue)]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }
    }
}