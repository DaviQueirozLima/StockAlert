using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StockAlert.API.Filters;
using StockAlert.Application.Auth.UseCases;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;
using StockAlert.Domain.Services;
using StockAlert.Infrastructure.Data;
using StockAlert.Infrastructure.ExternalServices.Brapi;
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
    // 1. Define o esquema de segurança
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Digite seu token JWT: **Bearer {seu_token}**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    // 2. Registra a definição com o ID "Bearer"
    options.AddSecurityDefinition("Bearer", securityScheme);

    // 3. Aplica a segurança globalmente (Sintaxe Funcional do Swashbuckle 10)
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
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

builder.Services.AddScoped<FluentValidation.IValidator<StockAlert.Communication.Requests
    .Stock.RegisterStockRequest>, StockAlert.Application.Stock.Validators.RegisterStockValidator>();
builder.Services.AddScoped<StockAlert.Application.Stock.UseCases.RegisterStockUseCase>();
builder.Services.AddScoped<StockAlert.Domain.Repositories.IStockRepository, StockAlert.Infrastructure.Repositories.StockRepository>();

builder.Services.AddScoped<FluentValidation.IValidator<StockAlert.Communication.Requests
    .AlertRule.RegisterAlertRuleRequest>, StockAlert.Application.AlertRule.Validator.RegisterAlertRuleValidator>();
builder.Services.AddScoped<StockAlert.Domain.Repositories.IAlertRuleRepository, StockAlert.Infrastructure.Repositories.AlertRuleRepository>();
builder.Services.AddScoped<StockAlert.Application.AlertRule.UseCases.RegisterAlertRuleUseCase>();
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<ILoggedUserAccessor, LoggedUserAccessor>();

builder.Services.AddScoped<IBrapiService, BrapiService>();
builder.Services.AddHttpClient<IBrapiService, BrapiService>();

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

//  Ordem correta
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();