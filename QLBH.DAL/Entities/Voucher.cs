using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("Voucher")]
    public class Voucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VoucherID { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaVoucher { get; set; } = string.Empty;

        [MaxLength(500)]
        public string MoTa { get; set; } = string.Empty;

        [MaxLength(20)]
        public string LoaiGiam { get; set; } = string.Empty; // 'PhanTram', 'TienMat'

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTriGiam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonToiThieu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiamToiDa { get; set; }

        public int SoLuong { get; set; }
        public int DaSuDung { get; set; }

        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; } = true;

        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}
