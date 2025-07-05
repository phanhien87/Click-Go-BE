using System.Text;
using System.Text.Json.Serialization;
using Click_Go.Data;
using Click_Go.Helper;
using Click_Go.Hubs;
using Click_Go.Middleware;
using Click_Go.Models;
using Click_Go.Repositories;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

namespace Click_Go
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ListenAnyIP(8080);
                options.ListenAnyIP(443,
                    listenOptions => { listenOptions.UseHttps("/app/certificate.pfx", builder.Configuration["CertificatePassword"]); });
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.RequireHeaderSymmetry = false;
            });

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

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IReactService, ReactService>();
            builder.Services.AddScoped<IPackageService, PackageService>();

            builder.Services.AddScoped<IWishlistService, WishlistService>();

            builder.Services.AddScoped<IWishlistService, WishlistService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IVoucherService, VoucherService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();


            builder.Services.Configure<PayOSOptions>(builder.Configuration.GetSection("PayOS"));
            builder.Services.AddScoped<PayOSService>();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, MailKitEmailService>();


            builder.Services.AddScoped<SaveImage>();
            builder.Services.AddScoped<UnitOfWork>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));

            builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            //Add identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Domain = ".clickgo.dev";
            });

            builder.Services.ConfigureExternalCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

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
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // Nếu request đến Hub thì lấy token từ query string
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hubs/notification"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddFacebook(options =>
                {
                    options.AppId = builder.Configuration["Facebook:AppId"];
                    options.AppSecret = builder.Configuration["Facebook:AppSecret"];
                }).AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
                    options.CallbackPath = builder.Configuration["Google:CallbackPath"];
                    
                    options.CorrelationCookie.Path = "/";
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.CorrelationCookie.HttpOnly = true;
                    options.CorrelationCookie.IsEssential = true;

		    options.CorrelationCookie.Domain = ".clickgo.dev";
                });

            builder.Services.AddSignalR().AddHubOptions<NotificationHub>(options =>
            {
                options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    builder => builder
                        .WithOrigins("http://clickgo_frontend:3000",
                            "https://clickgo.dev",
                            "http://clickgo.dev",
                            "http://localhost:3001",
                            "http://localhost:3000") // Chỉ cho phép frontend này gọi
                        .AllowAnyMethod() // Cho phép mọi method (GET, POST, PUT, DELETE, ...)
                        .AllowAnyHeader() // Cho phép mọi header (ví dụ Authorization)
                        .AllowCredentials() // nếu dùng cookies/token
                );
            });


            builder.Services.AddAuthorization();


            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseForwardedHeaders();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("AllowReactApp");

            // Cho phép truy cập thư mục UploadedFiles như một static folder
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "data", "UploadedFiles")),
            //    RequestPath = "/data/UploadedFiles"
            //});

            app.UseStaticFiles(); // Duy nhất dòng này là đủ

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<BanCheckMiddleware>();
            app.UseMiddleware<UserPackageValidationMiddleware>();

            app.MapHub<NotificationHub>("/hubs/notification");
            app.MapHub<VoucherHub>("/voucherHub");

            app.MapControllers();

            // Apply migrations and seed data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                try
                {
                    // wait for database to be ready
                    for (int i = 0; i < 30; i++)
                    {
                        if (dbContext.Database.CanConnect())
                        {
                            // apply migrations
                            await dbContext.Database.MigrateAsync();
                            await SeedData.SeedAdminAsync(services);
                            break;
                        }

                        Console.WriteLine("waiting for database to be ready...");
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database initialization failed: {ex.Message}");
                    throw;
                }
            }

            await app.RunAsync();
        }
    }
}
