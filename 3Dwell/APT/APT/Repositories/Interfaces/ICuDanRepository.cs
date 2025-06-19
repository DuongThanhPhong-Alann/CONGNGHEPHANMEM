using QLCCCC.Models;

namespace QLCCCC.Repositories.Interfaces
{
    public interface ICuDanRepository
    {
        Task<IEnumerable<CuDan>> GetAllAsync();
        Task<CuDan> GetByNguoiDungIdAsync(int nguoiDungId);
        Task<CuDan?> GetByIdAsync(int id);
        Task AddAsync(CuDan cuDan);
        Task UpdateAsync(CuDan cuDan);
        Task DeleteAsync(int id);
    }
}
