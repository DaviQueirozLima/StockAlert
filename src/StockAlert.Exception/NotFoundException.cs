namespace StockAlert.Exception
{
    public class NotFoundException : ExceptionBase.StockAlertException
    {
        public NotFoundException(string message) : base(message) 
        {
        }
    }
}
