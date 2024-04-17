
using AcmeCorp.Shopper.UiWebApp.Models;

namespace AcmeCorp.Shopper.UiWebApp.APIClient
{
    public interface IOrdersClient
    {
        Task<Order> PostOrders(Order order);
    }
}
