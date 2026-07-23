using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParsElements.Scraping;
using Shop.Data;
using Shop.Domain;
using Shop.Domain.Entities;
using Shop.Services;
using Shop.Services.Scraping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<DBContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<DBContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account";
    options.AccessDeniedPath = "/account";
});

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// --- Парсер цен маркетплейсов ---
// Wildberries — через HttpClient (публичный JSON API).
builder.Services.AddHttpClient<IMarketplaceParser, WildberriesParser>();
// Ozon / Яндекс.Маркет — через Playwright (headful для обхода части анти-бот защиты).
builder.Services.AddSingleton<IMarketplaceParser>(_ => new OzonParser(headless: false));
builder.Services.AddSingleton<IMarketplaceParser>(_ => new YandexParser(headless: false));

builder.Services.AddScoped<PriceCollectorService>();
builder.Services.AddSingleton<IScrapeQueue, ScrapeQueue>();
builder.Services.AddHostedService<ScrapeBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Первичное наполнение БД (миграции + роли/админ + демо-каталог).
await DbSeeder.SeedAsync(app.Services);

app.Run();
