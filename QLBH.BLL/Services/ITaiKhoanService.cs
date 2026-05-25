using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface ITaiKhoanService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<bool> DangKyAsync(DangKyRequestDto request);
        Task<TaiKhoanDto?> GetByIdAsync(int id);
        Task<ThongTinCaNhanDto?> GetThongTinCaNhanAsync(int taiKhoanId);
        Task<CapNhatThongTinDto?> GetCapNhatThongTinAsync(int taiKhoanId);
        Task<int?> GetNhanVienIdByTaiKhoanIdAsync(int taiKhoanId);
        Task<IEnumerable<TaiKhoanDto>> GetAllAsync();
        Task<bool> UpdateThongTinAsync(CapNhatThongTinDto dto);
        Task<bool> DoiMatKhauAsync(DoiMatKhauDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<NguoiDungDto>> GetNguoiDungListAsync();

        // Thêm 2 method mới
        Task<int> GetOrCreateKhachHangIdAsync(int taiKhoanId);
        Task<bool> CreateTaiKhoanAsync(TaoTaiKhoanDto dto);
    }
}