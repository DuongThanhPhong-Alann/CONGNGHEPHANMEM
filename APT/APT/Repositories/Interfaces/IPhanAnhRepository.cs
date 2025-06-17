using QLCCCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLCCCC.Repositories.Interfaces
{
    public interface IPhanAnhRepository
    {
        Task<IEnumerable<PhanAnh>> GetAllAsync();
        Task<PhanAnh?> GetByIdAsync(int id);
        Task AddAsync(PhanAnh phanAnh);
        Task UpdateAsync(PhanAnh phanAnh);
        Task DeleteAsync(int id);
    }
}
