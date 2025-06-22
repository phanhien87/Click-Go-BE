using Click_Go.DTOs;
using Click_Go.Helper;
using Click_Go.Models;
using Click_Go.Repositories;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Services
{
    public class CommentService : ICommentService
    {

        private readonly ICommentRepository _commentRepo;
        private readonly IImageRepository _imageRepo;
        private readonly UnitOfWork _unitOfWork;

        public CommentService(ICommentRepository commentRepo, IImageRepository imageRepo, UnitOfWork unitOfWork)
        {
            _commentRepo = commentRepo;
            _imageRepo = imageRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetReplyByPostDto?> AddCommentAsync(CommentDto dto, string userId)
        {

            var comment = new Comment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                ParentId = dto.ParentId,
                UserId = userId,
                CreatedDate = DateTime.Now,
                CreatedUser = Guid.Parse(userId),
                Status = 1
            };

            await _commentRepo.AddAsync(comment);

            if (dto.Images != null && dto.Images.Any())
            {
                var images = new List<Image>();
                var tasks = new List<Task>();

                foreach (var file in dto.Images)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var path = Path.Combine("UploadedFiles/reviews", fileName);
                    var directory = Path.GetDirectoryName(path);

                    if (directory != null && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Copy file vào ổ cứng
                    var fileCopyTask = Task.Run(async () =>
                    {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    });

                    tasks.Add(fileCopyTask);

                    images.Add(new Image
                    {
                        ImagePath = $"UploadedFiles/reviews/{fileName}",
                        CommentId = comment.Id,
                        CreatedDate = DateTime.Now,
                        Status = 1,
                        CreatedUser = Guid.Parse(userId)
                    });
                }

                await Task.WhenAll(tasks); // Chờ tất cả ảnh lưu xong
                await _imageRepo.AddImagesAsync(images);
            }
            return new GetReplyByPostDto
            {
                CommentId = comment.Id,
                Content = comment.Content,
                UserName = comment.User?.UserName,
                UserId = userId,
                CreatedDate = comment.CreatedDate,
                LikeCount = comment.Reactions?.Count(r => r.IsLike == true) ?? 0,
                UnlikeCount = comment.Reactions?.Count(r => r.IsLike == false) ?? 0,
                ImagesUrl = comment.Images?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                Replies = new List<GetCommentByPostDto>()
            };
        }

        public async Task<List<GetCommentByPostDto>> GetCommentsByPostAsync(long postId, string? currentUserId = null)
        {
            var allComments = await _commentRepo.getCommentByPost(postId);
            return await BuildCommentTree(allComments, null, currentUserId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId, string? currentUserId = null)
        {
            return await _commentRepo.GetByPostIdAsync(postId);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId)
        {
            return await _commentRepo.GetByPostIdAsync(postId); 
        }

        public async Task<Comment> GetParentCmtById(long? id)
        {
           
            return await _commentRepo.GetByIdAsync(id);
        }

        private async Task<List<GetCommentByPostDto>> BuildCommentTree(List<Comment> allComments, long? parentId = null,
            string? currentUserId = null)
        {
            var comments = allComments.Where(c => c.ParentId == parentId)
                               .OrderBy(c => c.CreatedDate);

            var commentDtos = await Task.WhenAll(comments.Select(async c => new GetCommentByPostDto
            {
                CommentId = c.Id,
                Content = c.Content,
                UserName = c.User?.UserName ?? "",
                CreatedDate = c.CreatedDate,
                UpdateDate = c.UpdatedDate,
                ImagesUrl = c.Images?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                IsLike = currentUserId == null ? null : c.Reactions?.FirstOrDefault(r => r.UserId == currentUserId)?.IsLike,
                LikeCount = c.Reactions?.Count(r => r.IsLike == true) ?? 0,
                UnlikeCount = c.Reactions?.Count(r => r.IsLike == false) ?? 0,
                //Level = level,
                Replies = await BuildCommentTree(allComments, c.Id, currentUserId),
            }));

            return commentDtos.ToList();
        }

        public async Task<List<GetCommentByPostDto>> GetCommentsByPostAndParentAsync(long postId, long? parentId, string? currentUserId = null)
        {
            var comments = await _commentRepo.GetCommentsByPostAndParent(postId, parentId);

            var commentDtos = new List<GetCommentByPostDto>();

            foreach (var c in comments)
            {
                var dto = new GetCommentByPostDto
                {
                    CommentId = c.Id,
                    Content = c.Content,
                    UserName = c.User?.UserName ?? "",
                    CreatedDate = c.CreatedDate,
                    UpdateDate = c.UpdatedDate,
                    ImagesUrl = c.Images?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                    IsLike = currentUserId == null ? null : c.Reactions?.FirstOrDefault(r => r.UserId == currentUserId)?.IsLike,
                    LikeCount = c.Reactions?.Count(r => r.IsLike == true) ?? 0,
                    UnlikeCount = c.Reactions?.Count(r => r.IsLike == false) ?? 0,
                    ReplyCount = await _commentRepo.GetReplyCount(c.Id),
                    UserId = c.UserId
                };

                commentDtos.Add(dto);
            }

            return commentDtos;
        }

        public async Task<(bool Success, bool IsRootComment)> DeleteCommentAsync(long commentId, string userId)
        {
            return await _commentRepo.DeleteCommentAsync(commentId, userId);
        }

        public async Task<CommentAncestorPathResult> GetCommentAncestorPathAsync(long commentId)
        {
            try
            {
              
                // Lấy đường dẫn tổ tiên bằng CTE (Common Table Expression)
                var ancestorPath = await _commentRepo.GetAncestorPathWithCTE(commentId);

                return new CommentAncestorPathResult
                {
                    Exists = true,
                    AncestorPath = ancestorPath,
                    RootCommentId = ancestorPath.FirstOrDefault(),
                    Depth = ancestorPath.Count
                };
            }
            catch
            {
               
                throw new AppException("Error getting ancestor path for comment");
            }
        }
    }




}

