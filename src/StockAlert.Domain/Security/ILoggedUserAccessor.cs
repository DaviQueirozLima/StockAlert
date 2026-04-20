namespace StockAlert.Domain.Security
{
    public interface ILoggedUserAccessor
    {
        Guid GetUserId();
    }
}
