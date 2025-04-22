using Click_Go.DTOs;
using Click_Go.Helper;
using Click_Go.Models;
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

        public async Task AddCommentAsync(CommentDto dto, string userId)
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


        }
        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId)
        {
            return await _commentRepo.GetByPostIdAsync(postId);
        }
    }



      
}

