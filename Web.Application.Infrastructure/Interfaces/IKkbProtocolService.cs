using System.Threading.Tasks;

namespace Web.Application.Infrastructure.Interfaces
{
    public interface IKkbProtocolService
    {
        bool IsValid { get; }
        Task<bool> Verify(string forVerify, string sign);
        Task<string> OrderPay(string orderId, decimal amount, string email);
        string Build64Sync(string idOrder, decimal amount);
        string Status(string orderId);
        string Approve(string orderId, string reference, string approval, string amount, string currency);
    }
}
