using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangApiController : ControllerBase
    {
        private readonly IDonHangService _donHangService;

        public DonHangApiController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }

        [Authorize(Roles = "KhachHang")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateDonHangDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _donHangService.CreateOrderAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var list = await _donHangService.GetAllAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpGet("{donHangId}")]
        public async Task<IActionResult> GetOrderById(int donHangId, [FromQuery] int? khachHangId)
        {
            var order = await _donHangService.GetByIdAsync(donHangId, khachHangId);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [Authorize(Roles = "KhachHang")]
        [HttpGet("history/{khachHangId}")]
        public async Task<IActionResult> GetHistory(int khachHangId)
        {
            var list = await _donHangService.GetHistoryByKhachHangIdAsync(khachHangId);
            return Ok(list);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? keyword, [FromQuery] string? trangThai, [FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
        {
            var list = await _donHangService.SearchAsync(keyword, trangThai, tuNgay, denNgay);
            return Ok(list);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPut("status/{donHangId}")]
        public async Task<IActionResult> UpdateStatus(int donHangId, [FromBody] string trangThaiMoi)
        {
            if (string.IsNullOrWhiteSpace(trangThaiMoi)) return BadRequest();
            var result = await _donHangService.UpdateTrangThaiAsync(donHangId, trangThaiMoi);
            if (!result) return NotFound();
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }
    }
}