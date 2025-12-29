using Click_Go.Redis;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IProducer<Null, string> _producer;
        private readonly StackExchange.Redis.IDatabase _redis;
        private readonly RedisService _redisService;

        public SearchController(IProducer<Null, string> producer, IConnectionMultiplexer redis, RedisService redisService)
        {
            _producer = producer;
            _redis = redis.GetDatabase();
            _redisService = redisService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string keyword)
        {
            await _producer.ProduceAsync("search-queries", new Message<Null, string> { Value = keyword });
            return Ok(new { message = "Keyword sent to Kafka." });
        }

        [HttpGet("hot-keywords")]
        public async Task<IActionResult> GetHotKeywords(int top = 10)
        {
            var keywords = await _redisService.GetTopKeywordsWithScoresAsync(top);
            return Ok(keywords);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var total = await _redisService.GetTotalKeywordsCountAsync();
            var popular = await _redisService.CountKeywordsAboveScoreAsync(5); // Từ khóa có >= 5 lần tìm

            return Ok(new { TotalKeywords = total, PopularKeywords = popular });
        }

    }
}
