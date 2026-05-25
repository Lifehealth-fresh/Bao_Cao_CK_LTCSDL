using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface IBienTheSanPhamService
    {
        Task<IEnumerable<BienTheSanPhamDto>> GetAllAsync();
        Task<BienTheSanPhamDto?> GetByIdAsync(int id);
        Task<IEnumerable<BienTheSanPhamDto>> GetBySanPhamIdAsync(int sanPhamId);
        Task<BienTheSanPhamDto> CreateAsync(CreateBienTheSanPhamDto dto);
        Task<BienTheSanPhamDto?> UpdateAsync(UpdateBienTheSanPhamDto dto);
        Task<bool> DeleteAsync(int id);
    }
}