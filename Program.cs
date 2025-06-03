using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Services.Interfaces;
using Click_Go.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Click_Go.Helper;
using Click_Go.Repositories.Interfaces;
using Click_Go.Repositories;
using Click_Go.Middleware;
using Microsoft.Extensions.FileProviders;
using System.IO;


namespace Click_Go
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Register DI
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            builder.Services.AddScoped<IReactRepository, ReactRepository>();
            builder.Services.AddScoped<IUserPackageRepository, UserPackageRepository>();
            builder.Services.AddScoped<IPackageRepository, PackageRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();


            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IReactService, ReactService>();
            builder.Services.AddScoped<IPackageService, PackageService>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();

            builder.Services.Configure<PayOSOptions>(builder.Configuration.GetSection("PayOS"));
            builder.Services.AddScoped<PayOSService>();


            builder.Services.AddScoped<SaveImage>();
            builder.Services.AddScoped<UnitOfWork>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Add identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
            
            //Add Jwt
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidIssuer = jwtSettings["Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddFacebook(options =>
            {
                options.AppId = builder.Configuration["Facebook:AppId"];
                options.AppSecret = builder.Configuration["Facebook:AppSecret"];
            }).AddGoogle(options =>
            {
                options.ClientId = "826336473893-hivarf6lubp1g3qn4ccvlgoskp3v7qta.apps.googleusercontent.com";
                options.ClientSecret = "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";
                options.CallbackPath = "/signin-google";
            });
             
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    builder => builder
                        .WithOrigins("http://localhost:3000")  // Chỉ cho phép frontend này gọi
                        .AllowAnyMethod()                      // Cho phép mọi method (GET, POST, PUT, DELETE, ...)
                        .AllowAnyHeader()                // Cho phép mọi header (ví dụ Authorization)
                        .AllowCredentials()                 // nếu dùng cookies/token
                        ); 
            });


            builder.Services.AddAuthorization();


            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<UserPackageValidationMiddleware>();

            app.UseCors("AllowReactApp");

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            
            app.UseSession();

            app.UseAuthentication();
            
            app.UseAuthorization();

            // Cho phép truy cập thư mục UploadedFiles như một static folder
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "UploadedFiles")),
                RequestPath = "/UploadedFiles"
            });


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.SeedAdminAsync(services);
            }
            


            await app.RunAsync();
        }
    }
}
