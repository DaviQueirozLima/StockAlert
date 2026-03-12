namespace StockAlert.Exception
{
    public class ValidationException : ExceptionBase.StockAlertException
    {
        public List<string> Errors { get; set; }

        public ValidationException(List<string> errors)
        {
            Errors = errors;
        }
    }
}
