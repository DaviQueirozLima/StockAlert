namespace StockAlert.Communication.Responses.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = default!;
        public UserResponse User { get; set; } = default!;
    }
}
