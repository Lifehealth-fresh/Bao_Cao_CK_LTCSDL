using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBH.BLL.DTOs
{
    public class DanhMucDto
    {
        public int DanhMucID { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; }
    }

    public class CreateDanhMucDto
    {
        public string TenDanhMuc { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }

    public class UpdateDanhMucDto : CreateDanhMucDto
    {
        public int DanhMucID { get; set; }
    }
}