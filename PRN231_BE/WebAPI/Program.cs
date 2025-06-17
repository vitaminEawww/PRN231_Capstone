using DataAccess.Common;
using DataAccess.Data;
using DataAccess.MappingConfigs;
using DataAccess.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.IRepositories;
using Repositories.Repository;
using Services.IServices;
using System.Text;
using Services.Services;


namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            //*<=====Set up policy=====>
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("corspolicy",
                    build => { build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); });
            });

            //* Swagger
            builder.Services.AddSwaggerGen(option =>
           {
               option.SwaggerDoc("v1", new OpenApiInfo { Title = "Smoking Free API", Version = "v1" });
               option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
               {
                   In = ParameterLocation.Header,
                   Description = "Please enter a valid token",
                   Name = "Authorization",
                   Type = SecuritySchemeType.Http,
                   BearerFormat = "JWT",
                   Scheme = "Bearer"
               });
               option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            //*<=====Add Database=====>
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(opts => opts.UseSqlServer(connectionString,
                options => { options.MigrationsAssembly("DataAccess"); }));

            //* Jwt
            var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddAuthorization();


            MappingRegistration.RegisterMappings();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddScoped<IDataSeederService, DataSeederService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ISmokingRecordService, SmokingRecordService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            var app = builder.Build();

            //* Seed data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dataSeederService = services.GetRequiredService<IDataSeederService>();
                    dataSeederService.SeedDefaultDataAsync().Wait();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    c.RoutePrefix = "";
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("corspolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}