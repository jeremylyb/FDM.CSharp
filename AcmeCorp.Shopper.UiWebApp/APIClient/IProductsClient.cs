
using AcmeCorp.Shopper.UiWebApp.Models;
namespace AcmeCorp.Shopper.UiWebApp.APIClient
{
    public interface IProductsClient
    {
        Task<List<Product>?> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(string id);
    }
}