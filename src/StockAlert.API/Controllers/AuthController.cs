using Microsoft.AspNetCore.Mvc;
using StockAlert.Application.Auth.UseCases;
using StockAlert.Communication.Requests.Auth;
using StockAlert.Communication.Responses.Auth;

namespace StockAlert.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginWithGoogleUseCase _loginUseCase;

    public AuthController(LoginWithGoogleUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost("google")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginWithGoogle([FromBody] LoginRequest request)
    {
        var response = await _loginUseCase.Execute(request);

        return Ok(response);
    }
}