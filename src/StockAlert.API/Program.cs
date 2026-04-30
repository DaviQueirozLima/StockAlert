using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Polly;
using Polly.Extensions.Http;
using StockAlert.API.Configurations;
using StockAlert.API.Filters;
using StockAlert.API.Workers;
using StockAlert.Application.AlertRule.UseCases;
using StockAlert.Application.Auth.UseCases;
using StockAlert.Application.Services;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;
using StockAlert.Domain.Services;
using StockAlert.Infrastructure.Data;
using StockAlert.Infrastructure.ExternalServices.Brapi;
using StockAlert.Infrastructure.Repositories;
using StockAlert.Infrastructure.Security;
using StockAlert.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Controllers & Filters

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Digite seu token JWT: **Bearer {seu_token}**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

#endregion

#region Database

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

#endregion

#region DI - Repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IAlertRuleRepository, AlertRuleRepository>();
builder.Services.AddScoped<INotificationHistoryRepository, NotificationHistoryRepository>();

#endregion

#region DI - Security

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenService, JwtTokenGenerator>();
builder.Services.AddScoped<ILoggedUserAccessor, LoggedUserAccessor>();

#endregion

#region DI - UseCases

builder.Services.AddScoped<LoginWithGoogleUseCase>();
builder.Services.AddScoped<StockAlert.Application.Stock.UseCases.RegisterStockUseCase>();
builder.Services.AddScoped<RegisterAlertRuleUseCase>();
builder.Services.AddScoped<UpdateAlertRuleUseCase>();
builder.Services.AddScoped<DeleteAlertRuleUseCase>();

#endregion

#region DI - Validators

builder.Services.AddScoped<
    FluentValidation.IValidator<StockAlert.Communication.Requests.Stock.RegisterStockRequest>,
    StockAlert.Application.Stock.Validators.RegisterStockValidator>();

builder.Services.AddScoped<
    FluentValidation.IValidator<StockAlert.Communication.Requests.AlertRule.RegisterAlertRuleRequest>,
    StockAlert.Application.AlertRule.Validator.RegisterAlertRuleValidator>();

#endregion

#region DI - Application Services

builder.Services.AddScoped<IAlertConditionChecker, AlertConditionChecker>();

#endregion

#region DI - External Services

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddHttpClient<IBrapiService, BrapiService>()
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

#endregion

#region Worker Configuration

builder.Services.Configure<WorkerSettings>(
    builder.Configuration.GetSection("WorkerSettings")
);

builder.Services.AddHostedService<StockMonitorWorker>();

#endregion

#region Authentication & Authorization

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

#endregion

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();