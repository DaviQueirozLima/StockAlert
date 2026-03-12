namespace StockAlert.Exception.ExceptionBase
{
    public class StockAlertException : System.Exception
    {
        public StockAlertException() { }
        public StockAlertException(string message) : base(message) { }

    }
}
