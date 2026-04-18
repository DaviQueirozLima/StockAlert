using StockAlert.Exception.ExceptionBase;

namespace StockAlert.Exception
{
    public class ValidationException : StockAlertException
    {
        public List<string> Errors { get; set; }

        public ValidationException(List<string> errors) : base(string.Empty)
        {
            Errors = errors;
        }
    }
}
