using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanApiController : ControllerBase
    {
        private readonly IThanhToanService _thanhToanService;

        public ThanhToanApiController(IThanhToanService thanhToanService)
        {
            _thanhToanService = thanhToanService;
        }

        [Authorize(Roles = "NhanVien")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPayments()
        {
            var list = await _thanhToanService.GetPendingPaymentsAsync();
            return Ok(list);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ConfirmPaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _thanhToanService.ProcessPaymentAsync(dto);
            if (!result) return BadRequest(new { message = "Xử lý thanh toán thất bại" });
            return Ok(new { message = "Đã xử lý thanh toán" });
        }

        [Authorize(Roles = "KhachHang")]
        [HttpGet("history/{khachHangId}")]
        public async Task<IActionResult> GetHistory(int khachHangId)
        {
            var list = await _thanhToanService.GetHistoryByKhachHangIdAsync(khachHangId);
            return Ok(list);
        }
    }
}