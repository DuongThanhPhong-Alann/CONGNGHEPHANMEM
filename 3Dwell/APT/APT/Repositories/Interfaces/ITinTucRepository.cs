using QLCCCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLCCCC.Repositories
{
    public interface ITinTucRepository
    {
        Task<IEnumerable<TinTuc>> GetAllAsync();
        Task<TinTuc?> GetByIdAsync(int id);
        Task AddAsync(TinTuc tinTuc);
        Task UpdateAsync(TinTuc tinTuc);
        Task DeleteAsync(int id);
    }
}