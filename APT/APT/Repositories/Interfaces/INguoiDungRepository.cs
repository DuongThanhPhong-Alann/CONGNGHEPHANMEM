using QLCCCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLCCCC.Repositories.Interfaces
{
    public interface INguoiDungRepository
    {
        Task<List<NguoiDung>> GetAllAsync();
        Task<NguoiDung?> GetByIdAsync(int id);
        Task<NguoiDung?> GetByPhoneNumberAsync(string soDienThoai);
        Task<NguoiDung?> GetByEmailAsync(string email);
        Task AddAsync(NguoiDung nguoiDung);
        Task UpdateAsync(NguoiDung nguoiDung);
        Task DeleteAsync(int id);
    }
}
