using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Click_Go.Kafka
{
    public class SearchKeywordConsumer : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<SearchKeywordConsumer> _logger;

        public SearchKeywordConsumer(IConnectionMultiplexer redis, ILogger<SearchKeywordConsumer> logger)
        {
            _redis = redis;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IDatabase db = null;
            IConsumer<Ignore, string> consumer = null;

            try
            {
                // Kiểm tra Redis connection
                db = _redis.GetDatabase();
                await db.PingAsync(); // Test connection
                _logger.LogInformation("Redis connected successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis connection failed.");
                return;
            }

            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "search-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    SessionTimeoutMs = 6000,
                    HeartbeatIntervalMs = 3000,
                    MaxPollIntervalMs = 300000,
                    FetchWaitMaxMs = 500 // Giảm thời gian chờ
                };

                consumer = new ConsumerBuilder<Ignore, string>(config)
                    .SetErrorHandler((_, e) => _logger.LogError($"Kafka error: {e.Reason}"))
                    .Build();

                consumer.Subscribe("search-queries");
                _logger.LogInformation("Kafka consumer subscribed successfully");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Sử dụng timeout để tránh block vô hạn
                        var cr = consumer.Consume(TimeSpan.FromMilliseconds(1000));

                        if (cr != null && !string.IsNullOrEmpty(cr.Message?.Value))
                        {
                            var keyword = cr.Message.Value;

                            // Thêm timeout cho Redis operation
                            using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                            cts.CancelAfter(TimeSpan.FromSeconds(5));

                            await db.SortedSetIncrementAsync("hot_search_keywords", keyword, 1);
                            _logger.LogInformation($"[Kafka->Redis] Keyword processed: {keyword}");
                        }
                        else
                        {
                            // Không có message, nghỉ ngắn
                            await Task.Delay(100, stoppingToken);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Kafka consume error");
                        await Task.Delay(5000, stoppingToken); // Nghỉ 5s trước khi retry
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Consumer operation cancelled");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                        await Task.Delay(1000, stoppingToken); // Nghỉ 1s trước khi retry
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in Kafka consumer");
            }
            finally
            {
                try
                {
                    consumer?.Close();
                    consumer?.Dispose();
                    _logger.LogInformation("Kafka consumer disposed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing consumer");
                }
            }
        }
    }

}
