using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.Web.Extensions;

namespace QLBH.Web.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class BienTheSanPhamController : Controller
    {
        private readonly IBienTheSanPhamService _bienTheService;
        private readonly ISanPhamService _sanPhamService;
        private readonly ILogger<BienTheSanPhamController> _logger;

        public BienTheSanPhamController(IBienTheSanPhamService bienTheService, ISanPhamService sanPhamService, ILogger<BienTheSanPhamController> logger)
        {
            _bienTheService = bienTheService;
            _sanPhamService = sanPhamService;
            _logger = logger;
        }

        // Hiển thị danh sách biến thể (có thể lọc theo sản phẩm)
        public async Task<IActionResult> Index(int? sanPhamId)
        {
            IEnumerable<BienTheSanPhamDto> list;
            if (sanPhamId.HasValue)
            {
                list = await _bienTheService.GetBySanPhamIdAsync(sanPhamId.Value);
                ViewBag.SanPhamId = sanPhamId.Value;
            }
            else
            {
                list = await _bienTheService.GetAllAsync();
            }
            // Lấy danh sách sản phẩm để hiển thị dropdown lọc
            var sanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.SanPhams = sanPhams;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadSanPhamDropdown();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBienTheSanPhamDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadSanPhamDropdown();
                    return View(dto);
                }
                await _bienTheService.CreateAsync(dto);
                TempData["Success"] = "Thêm biến thể thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo biến thể");
                TempData["Error"] = "Thêm biến thể thất bại.";
                await LoadSanPhamDropdown();
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bienThe = await _bienTheService.GetByIdAsync(id);
            if (bienThe == null) return NotFound();
            var updateDto = new UpdateBienTheSanPhamDto
            {
                BienTheID = bienThe.BienTheID,
                SanPhamID = bienThe.SanPhamID,
                Size = bienThe.Size,
                MauSac = bienThe.MauSac,
                MaMau = bienThe.MaMau,
                Gia = bienThe.Gia,
                SoLuongTon = bienThe.SoLuongTon,
                TrangThai = bienThe.TrangThai
            };
            await LoadSanPhamDropdown();
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBienTheSanPhamDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadSanPhamDropdown();
                    return View(dto);
                }
                await _bienTheService.UpdateAsync(dto);
                TempData["Success"] = "Cập nhật biến thể thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật biến thể");
                TempData["Error"] = "Cập nhật thất bại.";
                await LoadSanPhamDropdown();
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _bienTheService.DeleteAsync(id);
                TempData["Success"] = "Xóa biến thể thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa biến thể {Id}", id);
                TempData["Error"] = "Xóa biến thể thất bại.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSanPhamDropdown()
        {
            var sanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.SanPhams = sanPhams.Select(sp => new SelectListItem
            {
                Value = sp.SanPhamID.ToString(),
                Text = sp.TenSanPham
            }).ToList();
        }
    }
}