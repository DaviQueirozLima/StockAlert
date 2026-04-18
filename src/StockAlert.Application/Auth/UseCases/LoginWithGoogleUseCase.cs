using Google.Apis.Auth;
using StockAlert.Communication.Requests.Auth;
using StockAlert.Communication.Responses.Auth;
using StockAlert.Domain.Entities;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;
using StockAlert.Exception;
using StockAlert.Exception.ExceptionBase;
using Microsoft.Extensions.Configuration;

namespace StockAlert.Application.Auth.UseCases;

public class LoginWithGoogleUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public LoginWithGoogleUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            var clientId = _configuration["Google:ClientId"];

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            //  Validação segura do token
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch (InvalidJwtException)
        {
            throw new UnauthorizedException("Invalid Google token.");
        }
        catch (System.Exception)
        {
            throw new StockAlertException("Error validating Google token.");
        }

        //  Buscar usuário
        var user = await _userRepository.GetByGoogleIdAsync(payload.Subject);

        //  Criar usuário automaticamente
        if (user == null)
        {
            user = new User
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name,
                IsActive = true
            };

            await _userRepository.Add(user);
        }

        //  Gerar JWT
        var token = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            User = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }
}