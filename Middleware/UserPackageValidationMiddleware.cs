using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Middleware
{
    public class UserPackageValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserPackageValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IUserPackageRepository userPackageRepository, IPostService postService)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var userPackage = await userPackageRepository.CheckPackageByUserId(userId);

                    if (userPackage != null && userPackage.ExpireDate < DateTime.UtcNow && userPackage.Status != 0)
                    {
                        userPackage.Status = 0;
                        await userPackageRepository.UpdateAsync(userPackage);
                        await postService.LockPostAsync(userId,0);
                    }
                }

                
            }
            await _next(context);
        }
    }
}
