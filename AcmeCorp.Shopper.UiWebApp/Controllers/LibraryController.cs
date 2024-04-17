using AcmeCorp.Shopper.UiWebApp.APIClient;
using AcmeCorp.Shopper.UiWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorp.Shopper.UiWebApp.Controllers
{
    public class LibraryController : Controller
    {

        private readonly IProductsClient _productsClient;

        private readonly ICartsClient _cartsClient;

        private readonly IOrdersClient _ordersClient;

        public LibraryController(IProductsClient productsClient, ICartsClient cartsClient, IOrdersClient orderClient)
        {
            _productsClient = productsClient;
            _cartsClient = cartsClient;
            _ordersClient = orderClient;
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
            
        public async Task<IActionResult> NewCart()
        {
            var cart = await _cartsClient.PostNewCart();
            HttpContext.Session.SetString("CartId", cart.CartId.ToString());
            return View(cart);

        }

        public async Task<IActionResult> ViewCart()
        {
            string cartIdString = HttpContext.Session.GetString("CartId");
            if (int.TryParse(cartIdString, out int cartId))
            {
                //Console.WriteLine($"print cartId from ViewCart Method: {cartId}");
                var cartDetailsDTO = await _cartsClient.GetAllCartItemsForCartAsync(cartId);
                //Console.WriteLine("Printout CartDetailsDTO from ViewCart");
                //Console.WriteLine(cartDetailsDTO.Cart.CartId);
                foreach(var cartItem in cartDetailsDTO.CartItemsWithProducts)
                {
                    Console.WriteLine(cartItem.CartItem.CartItemId);
                    Console.WriteLine(cartItem.Product.ProductId);
                    Console.WriteLine(cartItem.Product.ProductPrice);
                }
                //Console.WriteLine(cartDetailsDTO);
                return View(cartDetailsDTO);
            }
            else
            {
                // Handle parsing failure
                await Console.Out.WriteLineAsync("failed");
                return null;
            }

        }

        public async Task<IActionResult> DeleteProductFromCart(int productId)
        {
 

            string cartIdString = HttpContext.Session.GetString("CartId");
            if (int.TryParse(cartIdString, out int cartId))
            {
                //Console.WriteLine($"Product with ProductID={productId} to be deleted");
                await _cartsClient.DeleteProductFromCart(cartId, productId);
                return RedirectToAction(nameof(ViewCart));
            }
            else
            {
                // Handle parsing failure
                await Console.Out.WriteLineAsync("failed");
                return null;
            }

        }

        public async Task<IActionResult> AddProductToCart(int productId)
        {


            string cartIdString = HttpContext.Session.GetString("CartId");
            if (int.TryParse(cartIdString, out int cartId))
            {
                //Console.WriteLine($"Product with ProductID={productId} to be deleted");
                await _cartsClient.AddProductToCart(cartId, productId);
                return RedirectToAction(nameof(ViewCart));
            }
            else
            {
                // Handle parsing failure
                await Console.Out.WriteLineAsync("failed");
                return null;
            }

        }

        public async Task<IActionResult> ClearAllProducts()
        {


            string cartIdString = HttpContext.Session.GetString("CartId");
            if (int.TryParse(cartIdString, out int cartId))
            {
                //Console.WriteLine($"Product with ProductID={productId} to be deleted");
                await _cartsClient.DeleteAllProductFromCart(cartId);
                return RedirectToAction(nameof(ViewCart));
            }
            else
            {
                // Handle parsing failure
                await Console.Out.WriteLineAsync("failed");
                return null;
            }

        }
        public IActionResult NewOrderBefore()
        {
            return View();
        }

        public async Task<IActionResult> NewOrderAfter([Bind("CustomerName")] Order order)
        {
            string cartIdString = HttpContext.Session.GetString("CartId");
            if (int.TryParse(cartIdString, out int cartId))
            {
                //Console.WriteLine($"Product with ProductID={productId} to be deleted");
                order.CartId = cartId;

                order = await _ordersClient.PostOrders(order);
                var cartDetailsDTO = await _cartsClient.GetAllCartItemsForCartAsync(cartId);
                var orderAndCartItems = new OrderAndCartItems() { Order = order , CartDetailsDTO = cartDetailsDTO };

                return View(orderAndCartItems);
            }
            else
            {
                // Handle parsing failure
                await Console.Out.WriteLineAsync("failed");
                return null;            }

        }



    }
}
