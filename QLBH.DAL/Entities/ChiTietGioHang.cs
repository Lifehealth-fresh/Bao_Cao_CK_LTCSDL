using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("ChiTietGioHang")]
    public class ChiTietGioHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChiTietID { get; set; }

        [ForeignKey(nameof(GioHang))]
        public int GioHangID { get; set; }
        public virtual GioHang GioHang { get; set; } = null!;

        [ForeignKey(nameof(BienTheSanPham))]
        public int BienTheID { get; set; }
        public virtual BienTheSanPham BienTheSanPham { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int SoLuong { get; set; }
    }
}
