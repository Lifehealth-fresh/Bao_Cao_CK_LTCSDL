using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.DAL.Data;
using System.Security.Claims;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanApiController : ControllerBase
    {
        private readonly IThanhToanService _thanhToanService;
        private readonly AppDbContext _context;

        public ThanhToanApiController(IThanhToanService thanhToanService, AppDbContext context)
        {
            _thanhToanService = thanhToanService;
            _context = context;
        }

        private int? GetKhachHangIdFromToken()
        {
            var tkClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(tkClaim, out var taiKhoanId)) return null;
            return _context.KhachHangs.Where(x => x.TaiKhoanID == taiKhoanId).Select(x => (int?)x.KhachHangID).FirstOrDefault();
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPayments()
        {
            var list = await _thanhToanService.GetPendingPaymentsAsync();
            return Ok(list);
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ConfirmPaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _thanhToanService.ProcessPaymentAsync(dto);
            if (!result) return BadRequest(new { message = "Xử lý thanh toán thất bại" });
            return Ok(new { message = "Đã xử lý thanh toán" });
        }

        [Authorize(Roles = "KhachHang")]
        [HttpGet("history")]
        public async Task<IActionResult> GetMyHistory()
        {
            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });

            var list = await _thanhToanService.GetHistoryByKhachHangIdAsync(khachHangId.Value);
            return Ok(list);
        }
    }
}
