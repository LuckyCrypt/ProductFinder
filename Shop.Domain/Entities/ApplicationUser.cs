using Microsoft.AspNetCore.Identity;

namespace Shop.Domain.Entities
{
    /// <summary>
    /// Пользователь приложения на базе ASP.NET Core Identity.
    /// Хеширование паролей, роли и подтверждение email обеспечиваются Identity.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
