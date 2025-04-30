namespace AngularApp1.Server.Interfaces
{
    public interface ISalesforceService
    {
        Task<string> GetSalesforceDataAsync();
    }
}
