namespace Shop.Domain.Entities
{
    /// <summary>
    /// Предложение конкретного магазина по конкретному товару — ядро сравнения цен.
    /// На один <see cref="Product"/> приходится много офферов (по одному на магазин).
    /// </summary>
    public class Offer : Entity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int StoreId { get; set; }
        public Store? Store { get; set; }

        public decimal Price { get; set; }

        /// <summary>Ссылка на страницу товара в этом магазине.</summary>
        public string? ProductUrl { get; set; }

        public bool InStock { get; set; } = true;

        /// <summary>Когда цена последний раз обновлялась (парсером или вручную).</summary>
        public DateTime LastCheckedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Итог последнего сбора парсером: Ok | Captcha | Blocked | Error | NotFound.</summary>
        public string? LastStatus { get; set; }

        /// <summary>Текст ошибки последнего сбора (если был), для диагностики в админке.</summary>
        public string? LastError { get; set; }
    }
}
