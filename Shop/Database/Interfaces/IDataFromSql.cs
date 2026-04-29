using Shop.Models;
using Shop.ViewModels;

namespace Shop.Database.Interfaces
{
    public interface IDataFromSql
    {
        Task<List<RentApartViewModel>> GetRentAparts();
    }
}
