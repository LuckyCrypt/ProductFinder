using System.Threading.Channels;

namespace Shop.Services.Scraping
{
    /// <summary>
    /// Задание на сбор цен. <see cref="ProductId"/> == null — обновить все цели,
    /// иначе только офферы одного товара.
    /// </summary>
    public record ScrapeJob(int? ProductId);

    /// <summary>
    /// Очередь заданий парсинга. Кнопки в админке кладут задание сюда и сразу
    /// возвращают ответ; фоновый воркер выгребает задания и выполняет их
    /// последовательно (Playwright нельзя гонять параллельно).
    /// </summary>
    public interface IScrapeQueue
    {
        ValueTask EnqueueAsync(ScrapeJob job, CancellationToken ct = default);
        ValueTask<ScrapeJob> DequeueAsync(CancellationToken ct);
    }

    public sealed class ScrapeQueue : IScrapeQueue
    {
        private readonly Channel<ScrapeJob> _channel =
            Channel.CreateUnbounded<ScrapeJob>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });

        public ValueTask EnqueueAsync(ScrapeJob job, CancellationToken ct = default)
            => _channel.Writer.WriteAsync(job, ct);

        public ValueTask<ScrapeJob> DequeueAsync(CancellationToken ct)
            => _channel.Reader.ReadAsync(ct);
    }
}
