using QLCCCC.Models;

namespace QLCCCC.Repositories.Interfaces
{
    public interface IDichVuRepository
    {
        Task<IEnumerable<DichVu>> GetAllAsync();
        Task<DichVu?> GetByIdAsync(int id);
        Task AddAsync(DichVu dichVu);
        Task UpdateAsync(DichVu dichVu);
        Task DeleteAsync(int id);
    }
}
