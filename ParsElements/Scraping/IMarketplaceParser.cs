namespace ParsElements.Scraping
{
    /// <summary>
    /// Результат сбора цены с карточки маркетплейса. Не содержит ничего из БД —
    /// адаптеры «чистые», персистом занимается слой оркестрации в веб-приложении.
    /// </summary>
    /// <param name="Price">Текущая цена в рублях, либо null если не удалось получить.</param>
    /// <param name="InStock">Есть ли товар в наличии.</param>
    /// <param name="Status">Итог: Ok | NotFound | Captcha | Blocked | Error.</param>
    /// <param name="Error">Текст ошибки для диагностики (если была).</param>
    public record PriceResult(decimal? Price, bool InStock, string Status, string? Error = null)
    {
        public static PriceResult Ok(decimal price, bool inStock) => new(price, inStock, ScrapeStatus.Ok);
        public static PriceResult Fail(string status, string? error = null) => new(null, false, status, error);
    }

    /// <summary>Стандартные значения <see cref="PriceResult.Status"/>.</summary>
    public static class ScrapeStatus
    {
        public const string Ok = "Ok";
        public const string NotFound = "NotFound";
        public const string Captcha = "Captcha";
        public const string Blocked = "Blocked";
        public const string Error = "Error";
    }

    /// <summary>
    /// Адаптер одного маркетплейса. Получает URL карточки, возвращает цену/наличие.
    /// Реализация НЕ должна бросать исключения наружу — любые сбои оборачиваются
    /// в <see cref="PriceResult"/> со статусом, чтобы сбой одного источника не ронял остальные.
    /// </summary>
    public interface IMarketplaceParser
    {
        /// <summary>Ключ адаптера, совпадает со Store.Code: "wb" | "ozon" | "yandex".</summary>
        string Code { get; }

        Task<PriceResult> FetchAsync(string url, CancellationToken ct = default);
    }
}
