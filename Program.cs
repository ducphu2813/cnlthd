using System.IdentityModel.Tokens.Jwt;
using System.Text;
using APIApplication.Context;
using APIApplication.JWT;
using APIApplication.Mapper;
using APIApplication.Middleware;
using APIApplication.Repository;
using APIApplication.Repository.Interface;
using APIApplication.Service;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace APIApplication;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        //thêm logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //đăng ký database context
        builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseMySql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
            ));

        //đăng ký token provider
        builder.Services.AddSingleton<TokenProvider>();
        
        //đăng ký repository
        builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IinvoiceRepository, InvoiceRepository>();
        builder.Services.AddScoped<IInvoiceDetailRepository, InvoiceDetailRepository>();
        
        //đăng ký service
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<IInvoiceDetailService, InvoiceDetailService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        
        //dang ký AutoMapper
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        
        //thêm phần xác thực jwt
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)), //thực hiện xác thực secret key
                    ValidIssuer = builder.Configuration["Jwt:Issuer"], //thực hiện xác thực Issuer
                    ValidAudience = builder.Configuration["Jwt:Audience"], //thực hiện xác thực Audience
                    ClockSkew = TimeSpan.Zero, //thời gian hết hạn của token
                    RoleClaimType = "roles"
                };
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // Disable claim type mapping
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("vào on token validated");
                        // In ra các claim từ JWT sau khi xác thực thành công
                        var claims = context.Principal.Claims;
                        foreach (var claim in claims)
                        {
                            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("vào on authentication failed");
                        // khi jwt hết hạn, 
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            Console.WriteLine("JWT Token expired");
                    
                            // Trả về lỗi 401 Unauthorized khi JWT hết hạn
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new {
                                message = "Token expired"
                            }));
                        }
                        
                        // ghi ra console
                        Console.WriteLine("JWT Authentication Failed");
                        Console.WriteLine(context.Exception.ToString());
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new {
                            message = "Invalid token"
                        }));
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine("vào on challenge");
                        // khi ko có jwt 
                        if (context.AuthenticateFailure != null)
                        {
                            Console.WriteLine("No JWT token provided");
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                            {
                                message = "No token provided"
                            }));
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseMiddleware<ExceptionMiddleware>();
        
        //thêm cors ==> disable csrf
        app.UseCors("AllowAll");
        
        //thêm authentication
        app.UseAuthentication();

        app.UseMiddleware<JWTMiddleware>();
        
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}