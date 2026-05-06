using Shop.Models;
using Shop.ViewModels;

namespace Shop.Database
{
    public static class CustomMapper
    {
        public static RentApartViewModel ToViewModel(DataRentApart user)
        {
            if (user == null)
                return null;
            decimal value;
            return new RentApartViewModel
            {
                Id = user.id,
                Name = user.name,
                URL = user.url,
                requirement = user.requirement, //.ToString("dd.MM.yyyy")
                Price = Decimal.TryParse(user.price,out value) ? value : 0,
            };
        }

        public static List<RentApartViewModel> ToViewModelList(List<DataRentApart> data)
        {
            if (data == null)
                return new List<RentApartViewModel>();

            return data
                .Select(ToViewModel)
                .ToList();
        }
    }
}
