using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task AddAsync(Rating rating);
    }
}
