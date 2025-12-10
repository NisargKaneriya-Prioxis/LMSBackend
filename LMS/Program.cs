using System.Text;
using EA.Services.RepositoryFactory;
using EvaluationAPI.Helper;
using LM.Model.Models.MyLMSDB;
using LM.Model.SpDbContext;
using LM.Services.Repositories.Implementation;
using LM.Services.Repositories.Interface;
using LM.Services.Token;
using LMS.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LMS;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy
                    .AllowAnyOrigin()   
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
        
        //Code for adding the log to the new file 
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();
        
        //code for adding the connectionString
        var connectionString = builder.Configuration.GetConnectionString("DBConnection");
        
        //DBContext
        builder.Services.AddDbContext<LMSDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {

            });
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableSensitiveDataLogging(true);
        }, ServiceLifetime.Transient);
        
        //SPCONTEXT CONFIGURATION
        builder.Services.AddDbContext<LMSSpContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {

                sqlOptions.EnableRetryOnFailure();

            });
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableSensitiveDataLogging(true);
        }, ServiceLifetime.Transient);

        
        // Register the UserService AND Unit OF Work
        UnitOfWorkServiceCollectionExtentions.AddUnitOfWork<LMSDbContext>(builder.Services);
        
        builder.Services.AddScoped<IBookRepository , BookRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAuthRepository,AuthRepository>();
        builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
        builder.Services.AddScoped<IBorrowedBookRepository,BorrowedBookRepository>();
        builder.Services.AddScoped<IRequestBookRepository,RequestBookRepository>();
        builder.Services.AddScoped<IReturnBookRepository,ReturnBookRepository>();
        builder.Services.AddScoped<TokenService>();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x =>
        {
            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, Id = "Bearer"
                        }
                    }, Array.Empty<string>()
                }
            });

        }
              
            );
        
        // Add JWT authentication
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
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    }
                
                };
                
            });
        
        

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
      
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.UseCors("AllowFrontend");
        
        app.UseAuthentication();
        app.UseAuthorization();

  
        app.MapControllers();

        app.Run();
    }
}