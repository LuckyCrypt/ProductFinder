using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Database.Interfaces;
using Shop.Models;
using Shop.ViewModels;

namespace Shop.Repository
{
    public class DataFromSqlRepository : IDataFromSql
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;


        public DataFromSqlRepository(AppDbContext appDbContext, IConfiguration configuration, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<List<RentApartViewModel>> GetRentAparts()
        {
            var parent = await _appDbContext.apartmentsavito.ToListAsync();

            return _mapper.Map<List<RentApartViewModel>>(parent);
        }
    }
}


