using AcmeCorp.Shopper.UiWebApp.APIClient;
using AcmeCorp.Shopper.UiWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorp.Shopper.UiWebApp.Controllers
{
    public class LibraryController : Controller
    {

        private readonly IProductsClient _productsClient;
        public LibraryController(IProductsClient productsClient)
        {
            _productsClient = productsClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AllProducts(string searchStr)
        {
            var products = await _productsClient.GetAllProductsAsync();
            foreach (Product product in products)
            {
                Console.Out.WriteLine(product);
            }
            if (!string.IsNullOrWhiteSpace(searchStr))
            {

                var filteredProducts = products.Where(product => product.ProductName.Contains(searchStr)).OrderBy(product => product.ProductName);
                products = filteredProducts.ToList();
            }

            return View(products);
        }


        public async Task<IActionResult> OneProduct(string id)
        {
            var product = await _productsClient.GetProductByIdAsync(id);
            return View(product);

        }

    }
}
