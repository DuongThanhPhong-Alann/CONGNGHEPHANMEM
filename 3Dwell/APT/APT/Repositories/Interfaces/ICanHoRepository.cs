using System.Collections.Generic;
using System.Threading.Tasks;
using QLCCCC.Models;

namespace QLCCCC.Repositories.Interfaces
{
    public interface ICanHoRepository
    {
        Task<IEnumerable<CanHo>> GetAllAsync();
        Task<IEnumerable<CanHo>> GetAllWithDetailsAsync(); // Added method
        Task<CanHo> GetByIdAsync(int id);
        Task<CanHo> GetByIdWithDetailsAsync(int id);
        Task UpdateAsync(CanHo canHo);
        Task DeleteAsync(int id);
        Task DeleteImagesAsync(ICollection<HinhAnhCanHo> images);
    }
}