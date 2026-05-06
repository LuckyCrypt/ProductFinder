using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain;
using Shop.Domain.Interfaces;
using Shop.Models;
using Shop.ViewModels;

namespace Shop.Repository
{
    public class DataFromSqlRepository : IDataFromSql
    {
        private readonly DBContext _appDbContext;


        public DataFromSqlRepository(DBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<List<RentApartViewModel>> GetRentAparts()
        {
            var result = await _appDbContext.apartmentsavito.ToListAsync();

            List<RentApartViewModel> viewModel = CustomMapper.ToViewModelList(result);
            return viewModel;
        }
    }
}


