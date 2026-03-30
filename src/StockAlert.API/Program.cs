using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StockAlert.API.Filters;
using StockAlert.Application.Auth.UseCases;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;
using StockAlert.Infrastructure.Data;
using StockAlert.Infrastructure.Repositories;
using StockAlert.Infrastructure.Security;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers + ExceptionFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Digite: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenGenerator>();
builder.Services.AddScoped<LoginWithGoogleUseCase>();

// JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY")
            ?? builder.Configuration["Settings:Jwt:SigningKey"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔐 Ordem correta
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();