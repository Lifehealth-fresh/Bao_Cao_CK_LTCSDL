using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface IVoucherService
    {
        Task<IEnumerable<VoucherDto>> GetAllAsync();
        Task<VoucherDto?> GetByIdAsync(int id);
        Task<VoucherDto> CreateAsync(CreateVoucherDto dto);
        Task<VoucherDto?> UpdateAsync(UpdateVoucherDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<VoucherDto>> GetAvailableVouchersAsync(decimal tongTamTinh);
        Task<VoucherDto?> ValidateVoucherAsync(string maVoucher, decimal tongTamTinh);
    }
}