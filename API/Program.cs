
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;

            // Configure DbContext
            builder.Services.AddDbContext<Repositories.DbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("BadmintonBazaarDb");
                options.UseSqlServer(connectionString);
            });
            // Add services to the container.

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<Repositories.DbContext>()
                .AddDefaultTokenProviders();
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
      options.TokenLifespan = TimeSpan.FromHours(24));


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
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
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Define security scheme for JWT
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

            builder.Services.AddScoped<ProductRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<ColorRepository>();
            builder.Services.AddScoped<NewsRepository>();
            builder.Services.AddScoped<OrderRepository>();
            builder.Services.AddScoped<OrderDetailRepository>();
            builder.Services.AddScoped<PaymentRepository>();
            builder.Services.AddScoped<ProductVariantRepository>();
            builder.Services.AddScoped<ReviewRepository>();
            builder.Services.AddScoped<SizeRepository>();
            builder.Services.AddScoped<SupplierRepository>();
            builder.Services.AddScoped<UserDetailRepository>();
            builder.Services.AddScoped<UserRepository>();

            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<ColorService>();
            builder.Services.AddScoped<NewsService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<OrderDetailService>();
            builder.Services.AddScoped<PaymentService>();
            builder.Services.AddScoped<ProductVariantService>();
            builder.Services.AddScoped<ReviewService>();
            builder.Services.AddScoped<SizeService>();
            builder.Services.AddScoped<SupplierService>();
            builder.Services.AddScoped<UserDetailService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<CartService>();

            //builder.Services.AddScoped<VnpayService>();

            //builder.Services.AddTransient<IMailService, MailService>();
            //builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseCors("AllowSpecificOrigin");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });

            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
