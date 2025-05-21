using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Services
{
    public class ReactService : IReactService
    {
        private readonly IReactRepository _reactRepository;

        public ReactService(IReactRepository reactRepository)
        {
            _reactRepository = reactRepository;
        }

        public async Task ReactComment(ReactDto dto,string userId)
        {
        
            var existReactComment = await _reactRepository.GetByCommentIdAndUserId(dto.CommentId, userId);
            if (existReactComment == null)
            {
              var newReact = new CommentReaction
                {
                    CommentId = dto.CommentId,
                    UserId = userId,
                    CreatedDate = DateTime.Now,
                    CreatedUser = Guid.Parse(userId),
                    Status = 1,
                    IsLike = dto.IsLike
                };
                await _reactRepository.AddAsync(newReact);

            }
            else
            {
                if (dto.IsLike == null)
                {
                    await _reactRepository.RemoveAsync(existReactComment.Id);
                    return;
                }
               existReactComment.UpdatedDate = DateTime.Now;
               existReactComment.UpdatedUser = Guid.Parse(userId);
               existReactComment.IsLike = dto.IsLike;
                await _reactRepository.UpdateAsync(existReactComment);
            }
           
        }
    }
}
