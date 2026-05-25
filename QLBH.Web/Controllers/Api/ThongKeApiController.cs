using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "NhanVien")]
    public class ThongKeApiController : ControllerBase
    {
        private readonly IThongKeService _thongKeService;

        public ThongKeApiController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var summary = await _thongKeService.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        [HttpGet("doanhthu/ngay")]
        public async Task<IActionResult> GetDoanhThuTheoNgay([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
        {
            var result = await _thongKeService.GetDoanhThuTheoNgayAsync(tuNgay, denNgay);
            return Ok(result);
        }

        [HttpGet("doanhthu/thang")]
        public async Task<IActionResult> GetDoanhThuTheoThang([FromQuery] int? nam)
        {
            var result = await _thongKeService.GetDoanhThuTheoThangAsync(nam);
            return Ok(result);
        }

        [HttpGet("doanhthu/danhmuc")]
        public async Task<IActionResult> GetDoanhThuTheoDanhMuc()
        {
            var result = await _thongKeService.GetDoanhThuTheoDanhMucAsync();
            return Ok(result);
        }

      
    }
}