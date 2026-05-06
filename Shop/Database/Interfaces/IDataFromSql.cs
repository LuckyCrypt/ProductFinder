using Shop.Models;
using Shop.ViewModels;

namespace Shop.Domain.Interfaces
{
    public interface IDataFromSql
    {
        Task<List<RentApartViewModel>> GetRentAparts();
    }
}
