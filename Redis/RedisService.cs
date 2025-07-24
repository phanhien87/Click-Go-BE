using StackExchange.Redis;

namespace Click_Go.Redis
{
    public class RedisService
    {
        private readonly IDatabase _db;
        private const string SortedSetKey = "hot_search_keywords"; // Đồng bộ với Kafka consumer

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        // Lấy top N từ khóa hot theo thứ tự giảm dần
        public async Task<List<string>> GetTopKeywordsAsync(int topN = 10)
        {
            try
            {
                var result = await _db.SortedSetRangeByRankAsync(SortedSetKey, 0, topN - 1, Order.Descending);
                return result.Select(x => x.ToString()).ToList();
            }
            catch (Exception)
            {
                // Trả về list rỗng nếu có lỗi
                return new List<string>();
            }
        }

        // Lấy top N từ khóa với điểm số
        public async Task<Dictionary<string, double>> GetTopKeywordsWithScoresAsync(int topN = 10)
        {
            try
            {
                var result = await _db.SortedSetRangeByRankWithScoresAsync(SortedSetKey, 0, topN - 1, Order.Descending);
                return result.ToDictionary(x => x.Element.ToString(), x => x.Score);
            }
            catch (Exception)
            {
                return new Dictionary<string, double>();
            }
        }

        // Đếm số từ khóa có lượt tìm kiếm >= minScore
        public async Task<long> CountKeywordsAboveScoreAsync(double minScore = 1)
        {
            try
            {
                return await _db.SortedSetLengthAsync(SortedSetKey, minScore, double.MaxValue);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // Lấy điểm số của một từ khóa cụ thể
        public async Task<double?> GetKeywordScoreAsync(string keyword)
        {
            try
            {
                return await _db.SortedSetScoreAsync(SortedSetKey, keyword);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Xóa từ khóa có điểm số thấp (cleanup)
        public async Task<long> RemoveKeywordsBelowScoreAsync(double maxScore)
        {
            try
            {
                return await _db.SortedSetRemoveRangeByScoreAsync(SortedSetKey, 0, maxScore);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // Lấy tổng số từ khóa
        public async Task<long> GetTotalKeywordsCountAsync()
        {
            try
            {
                return await _db.SortedSetLengthAsync(SortedSetKey);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // Thêm hoặc cập nhật từ khóa (cho test)
        public async Task<bool> IncrementKeywordAsync(string keyword, double increment = 1)
        {
            try
            {
                await _db.SortedSetIncrementAsync(SortedSetKey, keyword, increment);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Kiểm tra kết nối Redis
        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                await _db.PingAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}