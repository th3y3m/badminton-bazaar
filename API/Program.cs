using API.Middlewares;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Repositories.Interfaces;
using Services;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;
using System.Text;
using Nest;
using System.Threading.RateLimiting;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.CircuitBreaker;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Google;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;

            // Configure DbContext
            builder.Services.AddDbContext<Repositories.DbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("BadmintonBazaarDb"));
            });

            // Hangfire
            builder.Services.AddHangfire(config =>
                config.UseSqlServerStorage(configuration.GetConnectionString("BadmintonBazaarDb")));
            builder.Services.AddHangfireServer();

            // Add services to the container
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();

            // Identity Configuration
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true; // Require email confirmation
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<Repositories.DbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(24));

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
                };

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 2
                        }));
            });

            // JWT Authentication Configuration
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Google:ClientId"];
                options.ClientSecret = configuration["Google:ClientSecret"];
            })
            .AddFacebook(options =>
            {
                options.AppId = configuration["Facebook:ClientId"];
                options.AppSecret = configuration["Facebook:ClientSecret"];
            });

            // Swagger Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // Dependency Injection for Repositories and Services
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IColorRepository, ColorRepository>();
            builder.Services.AddScoped<INewsRepository, NewsRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<ISizeRepository, SizeRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IUserDetailRepository, UserDetailRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFreightPriceRepository, FreightPriceRepository>();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IColorService, ColorService>();
            builder.Services.AddScoped<INewsService, NewsService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<ISizeService, SizeService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IUserDetailService, UserDetailService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IFreightPriceService, FreightPriceService>();
            builder.Services.AddScoped<IRedisLock, RedisLock>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<IAuthService, AuthenticationService>();
            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddHttpClient<IChatService, ChatService>();

            builder.Services.AddScoped<IVnpayService, VnpayService>();
            builder.Services.AddScoped<IMoMoService, MoMoService>();

            // Mail Settings Configuration
            builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            builder.Services.Configure<MoMoSettings>(configuration.GetSection("MoMoSettings"));

            // Register FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginModelValidator>();

            // Configure Redis with Polly
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configurationOptions = new ConfigurationOptions
                {
                    EndPoints = { "localhost:6379" },
                    ConnectTimeout = 5000,
                    AbortOnConnectFail = false
                };

                var retryPolicy = Polly.Policy
                    .Handle<RedisConnectionException>()
                    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(retryAttempt), (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retrying Redis connection. Attempt {retryCount}. Error: {exception.Message}");
                    });

                return retryPolicy.Execute(() => ConnectionMultiplexer.Connect(configurationOptions));
            });

            builder.Services.AddSingleton<IElasticClient>(sp =>
            {
                var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                    .DefaultIndex("products");

                var retryPolicy = Polly.Policy
                    .Handle<Exception>() // Handle any exception, or you can be more specific
                    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            Console.WriteLine($"Attempt {retryCount} to connect to Elasticsearch failed. Error: {exception.Message}");
                        });

                IElasticClient client = null;
                try
                {
                    retryPolicy.ExecuteAsync(async () =>
                    {
                        client = new ElasticClient(settings);
                        var pingResponse = await client.PingAsync();
                        if (!pingResponse.IsValid)
                        {
                            throw new Exception("Elasticsearch ping failed.");
                        }
                    }).Wait();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Elasticsearch connection failed after retries: {ex.Message}");
                }

                return client ?? new ElasticClient(settings); // Return client even if failed
            });
            builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();

            //var settings = new ConnectionSettings(new Uri("http://localhost:9200")) 
            //    .DefaultIndex("products");
            //builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));

            //var client = new ElasticClient(settings);

            //var indexHelper = new ElasticsearchService(client);
            //await indexHelper.CreateIndexAsync("products");

            // CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins(
                            "https://localhost:3000",
                            "http://localhost:3000"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            // Register SignalR
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseRateLimiter();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowSpecificOrigin");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RequestResponseMiddleware>();

            app.UseHangfireDashboard();

            // Map SignalR hubs
            app.MapHub<ProductHub>("/productHub");

            app.MapControllers();
            app.Run();
        }
    }
}
