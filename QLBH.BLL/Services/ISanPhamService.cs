using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface ISanPhamService
    {
        Task<IEnumerable<SanPhamDto>> GetAllAsync(string? keyword = null, int? danhMucId = null, decimal? minPrice = null, decimal? maxPrice = null, bool chiLaySanPhamHoatDong = false);
        Task<SanPhamDto?> GetByIdAsync(int id);
        Task<SanPhamDto> CreateAsync(CreateSanPhamDto dto);
        Task<SanPhamDto?> UpdateAsync(UpdateSanPhamDto dto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<BienTheSanPhamDto>> GetAllBienTheAsync(int sanPhamId);
        Task<BienTheSanPhamDto?> GetBienTheByIdAsync(int bienTheId);
        Task<BienTheSanPhamDto> CreateBienTheAsync(CreateBienTheSanPhamDto dto);
        Task<BienTheSanPhamDto?> UpdateBienTheAsync(UpdateBienTheSanPhamDto dto);
        Task<bool> DeleteBienTheAsync(int bienTheId);
    }
}