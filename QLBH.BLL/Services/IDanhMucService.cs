using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QLBH.BLL.DTOs;

namespace QLBH.BLL.Services
{
    public interface IDanhMucService
    {
        Task<IEnumerable<DanhMucDto>> GetAllAsync();
        Task<DanhMucDto?> GetByIdAsync(int id);
        Task<DanhMucDto> CreateAsync(CreateDanhMucDto dto);
        Task<DanhMucDto?> UpdateAsync(UpdateDanhMucDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        
    }
}
