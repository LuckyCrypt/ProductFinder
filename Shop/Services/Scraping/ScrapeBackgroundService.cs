namespace Shop.Services.Scraping
{
    /// <summary>
    /// Фоновый воркер: выгребает задания из очереди и выполняет сбор цен последовательно.
    /// Отдельный DI-scope на каждое задание (scoped DBContext + PriceCollectorService).
    /// </summary>
    public sealed class ScrapeBackgroundService : BackgroundService
    {
        private readonly IScrapeQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScrapeBackgroundService> _logger;

        public ScrapeBackgroundService(
            IScrapeQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<ScrapeBackgroundService> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScrapeBackgroundService запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                ScrapeJob job;
                try
                {
                    job = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var collector = scope.ServiceProvider.GetRequiredService<PriceCollectorService>();
                    await collector.CollectAsync(job, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при выполнении задания сбора цен");
                }
            }

            _logger.LogInformation("ScrapeBackgroundService остановлен");
        }
    }
}
