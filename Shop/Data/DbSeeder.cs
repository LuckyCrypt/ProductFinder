using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Domain;
using Shop.Domain.Entities;

namespace Shop.Data
{
    /// <summary>
    /// Идемпотентное первичное наполнение БД: роли, учётка администратора,
    /// категории, магазины и демо-товары с офферами (цены в грн).
    /// Реальные данные позже поставляет парсер.
    /// </summary>
    public static class DbSeeder
    {
        public const string AdminRole = "Admin";
        public const string ClientRole = "Client";

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var context = sp.GetRequiredService<DBContext>();
            await context.Database.MigrateAsync();

            await SeedRolesAndAdminAsync(sp);
            await SeedCatalogAsync(context);
        }

        private static async Task SeedRolesAndAdminAsync(IServiceProvider sp)
        {
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in new[] { AdminRole, ClientRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            const string adminEmail = "admin@productfinder.local";
            if (await userManager.FindByNameAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Администратор"
                };
                // Демо-пароль для локальной разработки. В проде задаётся отдельно.
                var result = await userManager.CreateAsync(admin, "Admin123$");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }

        private static async Task SeedCatalogAsync(DBContext context)
        {
            if (await context.Categories.AnyAsync())
                return; // каталог уже наполнен

            // --- Категории ---
            var computers = new Category { Name = "Компьютеры", Slug = "computers", IconOrImage = "💻" };
            var gadgets = new Category { Name = "Гаджеты", Slug = "gadgets", IconOrImage = "📱" };
            var laptops = new Category { Name = "Ноутбуки", Slug = "noutbuki", IconOrImage = "💻", Parent = computers };
            var phones = new Category { Name = "Мобильные телефоны", Slug = "phones", IconOrImage = "📱", Parent = gadgets };
            context.Categories.AddRange(computers, gadgets, laptops, phones);

            // --- Магазины ---
            var rozetka = new Store { Name = "Rozetka", SiteUrl = "https://rozetka.com.ua", LogoUrl = "🛍️" };
            var comfy = new Store { Name = "Comfy", SiteUrl = "https://comfy.ua", LogoUrl = "🏬" };
            var foxtrot = new Store { Name = "Foxtrot", SiteUrl = "https://foxtrot.com.ua", LogoUrl = "🦊" };
            var allo = new Store { Name = "Allo", SiteUrl = "https://allo.ua", LogoUrl = "📦" };
            var stores = new[] { rozetka, comfy, foxtrot, allo };
            context.Stores.AddRange(stores);

            var rnd = new Random(20260718);

            // Демо-товары: (категория, имя, бренд, год, теги, базовая цена, спеки)
            var seed = new (Category cat, string name, string brand, int year, string tags, decimal basePrice, (string group, string name, string val)[] specs)[]
            {
                (laptops, "Asus TUF Gaming A16", "Asus", 2025, "игровой,165 Гц,RTX 5060,DDR5", 55949m, new[]
                {
                    ("Дисплей","Диагональ","16\""),("Дисплей","Разрешение","1920x1200"),
                    ("Процессор","CPU","AMD Ryzen 7"),("Видеокарта","GPU","RTX 5060"),
                    ("Память","ОЗУ","16 ГБ"),("Память","Накопитель","512 ГБ SSD"),
                }),
                (laptops, "Apple MacBook Air 15", "Apple", 2025, "ультрабук,автономный,MacOS", 45999m, new[]
                {
                    ("Дисплей","Диагональ","15,3\""),("Дисплей","Разрешение","2880x1864"),
                    ("Процессор","CPU","Apple M4"),("Память","ОЗУ","16 ГБ"),
                    ("Память","Накопитель","256 ГБ SSD"),
                }),
                (laptops, "Lenovo IdeaPad Slim 5", "Lenovo", 2024, "офисный,лёгкий", 28499m, new[]
                {
                    ("Дисплей","Диагональ","14\""),("Процессор","CPU","AMD Ryzen 5"),
                    ("Память","ОЗУ","16 ГБ"),("Память","Накопитель","512 ГБ SSD"),
                }),
                (laptops, "Acer Nitro V 15", "Acer", 2024, "игровой,RTX 4050", 33999m, new[]
                {
                    ("Дисплей","Диагональ","15,6\""),("Процессор","CPU","Intel Core i5"),
                    ("Видеокарта","GPU","RTX 4050"),("Память","ОЗУ","16 ГБ"),
                    ("Память","Накопитель","512 ГБ SSD"),
                }),
                (laptops, "HP Pavilion 15", "HP", 2024, "офисный", 24999m, new[]
                {
                    ("Дисплей","Диагональ","15,6\""),("Процессор","CPU","Intel Core i3"),
                    ("Память","ОЗУ","8 ГБ"),("Память","Накопитель","512 ГБ SSD"),
                }),
                (phones, "Apple iPhone 16 Pro", "Apple", 2024, "флагман,iOS,120 Гц", 52999m, new[]
                {
                    ("Экран","Диагональ","6,3\""),("Экран","Тип","OLED 120 Гц"),
                    ("Камера","Основная","48 Мп"),("Память","Встроенная","256 ГБ"),
                    ("Аккумулятор","Ёмкость","3582 мА·ч"),
                }),
                (phones, "Samsung Galaxy S24", "Samsung", 2024, "флагман,Android,120 Гц", 31999m, new[]
                {
                    ("Экран","Диагональ","6,2\""),("Экран","Тип","AMOLED 120 Гц"),
                    ("Камера","Основная","50 Мп"),("Память","ОЗУ","8 ГБ"),
                    ("Память","Встроенная","256 ГБ"),("Аккумулятор","Ёмкость","4000 мА·ч"),
                }),
                (phones, "Xiaomi Redmi Note 14 Pro", "Xiaomi", 2025, "средний,Android", 12999m, new[]
                {
                    ("Экран","Диагональ","6,67\""),("Камера","Основная","200 Мп"),
                    ("Память","ОЗУ","8 ГБ"),("Память","Встроенная","256 ГБ"),
                    ("Аккумулятор","Ёмкость","5500 мА·ч"),
                }),
                (phones, "Google Pixel 9", "Google", 2024, "камерофон,Android", 34999m, new[]
                {
                    ("Экран","Диагональ","6,3\""),("Камера","Основная","50 Мп"),
                    ("Память","ОЗУ","12 ГБ"),("Память","Встроенная","128 ГБ"),
                }),
                (phones, "Motorola Edge 50", "Motorola", 2024, "средний,Android", 14499m, new[]
                {
                    ("Экран","Диагональ","6,67\""),("Камера","Основная","50 Мп"),
                    ("Память","ОЗУ","8 ГБ"),("Память","Встроенная","256 ГБ"),
                }),
            };

            foreach (var item in seed)
            {
                var product = new Product
                {
                    Name = item.name,
                    Brand = item.brand,
                    Category = item.cat,
                    Year = item.year,
                    Tags = item.tags,
                    Description = $"{item.name} — {item.brand}. Сравнение цен из разных магазинов.",
                };

                foreach (var (group, name, val) in item.specs)
                    product.Specifications.Add(new ProductSpecification { Group = group, Name = name, Value = val });

                // 2–4 оффера от случайных магазинов с разбросом цен ±12%.
                var chosen = stores.OrderBy(_ => rnd.Next()).Take(rnd.Next(2, 5)).ToList();
                foreach (var store in chosen)
                {
                    var factor = 1m + (decimal)(rnd.NextDouble() * 0.24 - 0.12);
                    var price = Math.Round(item.basePrice * factor, 0);
                    product.Offers.Add(new Offer
                    {
                        Store = store,
                        Price = price,
                        InStock = rnd.Next(0, 10) > 1,
                        ProductUrl = store.SiteUrl,
                        LastCheckedAt = DateTime.UtcNow,
                    });
                }

                product.RecomputePriceRange();
                context.Products.Add(product);
            }

            await context.SaveChangesAsync();
        }
    }
}
