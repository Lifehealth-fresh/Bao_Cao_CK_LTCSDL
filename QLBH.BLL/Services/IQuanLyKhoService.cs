using QLBH.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLBH.BLL.Services
{
    public interface IQuanLyKhoService
    {
        Task<PagedResult<PhieuNhapDto>> GetPhieuNhapAsync(DateTime? tuNgay, DateTime? denNgay, int pageIndex, int pageSize);
        Task<PhieuNhapDto?> GetPhieuNhapByIdAsync(int id);
        Task<List<ChiTietPhieuNhapDto>> GetChiTietPhieuNhapAsync(int phieuNhapId);
        Task<int> TaoPhieuNhapAsync(TaoPhieuNhapDto model, int nhanVienId);
    }
}