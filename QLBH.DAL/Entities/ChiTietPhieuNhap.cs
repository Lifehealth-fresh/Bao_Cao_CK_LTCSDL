using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("ChiTietPhieuNhap")]
    public class ChiTietPhieuNhap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChiTietID { get; set; }

        [ForeignKey(nameof(PhieuNhap))]
        public int PhieuNhapID { get; set; }
        public virtual PhieuNhap PhieuNhap { get; set; } = null!;

        [ForeignKey(nameof(BienTheSanPham))]
        public int BienTheID { get; set; }
        public virtual BienTheSanPham BienTheSanPham { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }
    }
}