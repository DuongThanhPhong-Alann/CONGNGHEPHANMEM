using QLCCCC.Models;
using System.Collections.Generic;

namespace QLCCCC.Repositories.Interfaces
{
    public interface IHoaDonDichVuRepository
    {
        IEnumerable<HoaDonDichVu> GetAll();
        Task AddAsync(HoaDonDichVu hoaDon);
        HoaDonDichVu? GetById(int id);
        void Add(HoaDonDichVu hoaDon);
        void Update(HoaDonDichVu hoaDon);
        void Delete(int id);
        void Save();
    }
}