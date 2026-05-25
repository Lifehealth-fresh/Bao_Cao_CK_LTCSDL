using QLBH.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace QLBH.BLL.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public TaiKhoanDto TaiKhoan { get; set; } = new TaiKhoanDto();
    }
}