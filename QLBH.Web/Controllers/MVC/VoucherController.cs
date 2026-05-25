using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;
        private readonly ILogger<VoucherController> _logger;

        public VoucherController(IVoucherService voucherService, ILogger<VoucherController> logger)
        {
            _voucherService = voucherService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _voucherService.GetAllAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hiển thị danh sách voucher");
                TempData["Error"] = "Không thể tải danh sách voucher.";
                return View(Enumerable.Empty<VoucherDto>());
            }
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateVoucherDto
        {
            NgayBatDau = DateTime.Today,
            NgayKetThuc = DateTime.Today.AddMonths(1),
            TrangThai = true
        });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVoucherDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return View(dto);
                await _voucherService.CreateAsync(dto);
                TempData["Success"] = "Thêm voucher thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo voucher mới");
                TempData["Error"] = "Thêm voucher thất bại. Mã voucher có thể đã tồn tại.";
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null) return NotFound();

                var dto = new UpdateVoucherDto
                {
                    VoucherID = voucher.VoucherID,
                    MaVoucher = voucher.MaVoucher,
                    MoTa = voucher.MoTa,
                    LoaiGiam = voucher.LoaiGiam,
                    GiaTriGiam = voucher.GiaTriGiam,
                    DonToiThieu = voucher.DonToiThieu,
                    GiamToiDa = voucher.GiamToiDa,
                    SoLuong = voucher.SoLuong,
                    DaSuDung = voucher.DaSuDung,
                    NgayBatDau = voucher.NgayBatDau,
                    NgayKetThuc = voucher.NgayKetThuc,
                    TrangThai = voucher.TrangThai
                };
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hiển thị form sửa voucher {VoucherId}", id);
                TempData["Error"] = "Không thể tải thông tin voucher.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateVoucherDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return View(dto);
                await _voucherService.UpdateAsync(dto);
                TempData["Success"] = "Cập nhật voucher thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật voucher {VoucherId}", dto.VoucherID);
                TempData["Error"] = "Cập nhật voucher thất bại.";
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _voucherService.DeleteAsync(id);
                TempData["Success"] = "Xóa voucher thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa voucher {VoucherId}", id);
                TempData["Error"] = "Xóa voucher thất bại.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
