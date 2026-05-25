using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBH.DAL.Entities
{
    [Table("DanhMuc")]
    public class DanhMuc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DanhMucID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenDanhMuc { get; set; } = string.Empty;

        [MaxLength(500)]
        public string MoTa { get; set; } = string.Empty;

        public bool TrangThai { get; set; } = true;

        public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}