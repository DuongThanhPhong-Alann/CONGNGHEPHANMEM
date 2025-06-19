using QLCCCC.Models;

namespace QLCCCC.Repositories.Interfaces
{
    public interface IChungCuRepository
    {
        Task<IEnumerable<ChungCu>> GetAllAsync();
        Task<ChungCu?> GetByIdWithDetailsAsync(int id);
        Task<ChungCu?> GetByIdAsync(int id);
        Task AddAsync(ChungCu chungCu);
        Task UpdateAsync(ChungCu chungCu);
        Task DeleteAsync(int id);
    }
}
