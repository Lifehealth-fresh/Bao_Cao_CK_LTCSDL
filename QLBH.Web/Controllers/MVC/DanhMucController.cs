using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class DanhMucController : Controller
    {
        private readonly IDanhMucService _danhMucService;
        private readonly ILogger<DanhMucController> _logger;

        public DanhMucController(IDanhMucService danhMucService, ILogger<DanhMucController> logger)
        {
            _danhMucService = danhMucService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _danhMucService.GetAllAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateDanhMucDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return View(dto);
                await _danhMucService.CreateAsync(dto);
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo danh mục");
                TempData["Error"] = "Thêm danh mục thất bại.";
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var danhMuc = await _danhMucService.GetByIdAsync(id);
            if (danhMuc == null) return NotFound();
            var updateDto = new UpdateDanhMucDto
            {
                DanhMucID = danhMuc.DanhMucID,
                TenDanhMuc = danhMuc.TenDanhMuc,
                MoTa = danhMuc.MoTa,
                TrangThai = danhMuc.TrangThai
            };
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateDanhMucDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return View(dto);
                await _danhMucService.UpdateAsync(dto);
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật danh mục");
                TempData["Error"] = "Cập nhật thất bại.";
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _danhMucService.DeleteAsync(id);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction(nameof(Index)); // hoặc Admin, tuỳ tên action của bạn
        }
    }
}