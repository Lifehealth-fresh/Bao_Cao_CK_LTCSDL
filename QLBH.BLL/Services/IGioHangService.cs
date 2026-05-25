using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface IGioHangService
    {
        Task<GioHangDto?> GetCartByKhachHangIdAsync(int khachHangId);
        Task<bool> AddToCartAsync(AddToCartDto dto);
        Task<bool> UpdateQuantityAsync(UpdateCartItemDto dto);
        Task<bool> RemoveItemAsync(int chiTietId);
        Task<bool> ClearCartAsync(int khachHangId);
    }
}