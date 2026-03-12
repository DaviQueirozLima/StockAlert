namespace StockAlert.Exception
{
    public class UnauthorizedException : ExceptionBase.StockAlertException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
