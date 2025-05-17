using Click_Go.DTOs;

namespace Click_Go.Services.Interfaces
{
    public interface IReactService
    {
        Task ReactComment(ReactDto dto, string userId);
       
    }
}
