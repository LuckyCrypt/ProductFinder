using Shop.Domain.Entities;
using Shop.Models;
using Shop.ViewModels;

namespace Shop.Domain.Interfaces
{
    public interface IDataFromSql
    {
        Task<List<RentApartViewModel>> GetRentAparts();
        Task<List<Shop.Domain.Entities.Phone>> GetPhones();
        Task AddPhone(Shop.Domain.Entities.Phone phone);
    }
}
