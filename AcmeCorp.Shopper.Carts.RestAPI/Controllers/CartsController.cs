using AcmeCorp.Shopper.CartsRestAPI.Models;
//using AcmeCorp.Shopper.UiWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AcmeCorp.Shopper.CartsRestAPI.Controllers
{

    [Route("api/[controller]")]         
    [ApiController]
    public class CartsController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly CartsAcmeContext _cartContext;

        private readonly ProductsAcmeContext _productContext;



        public CartsController(CartsAcmeContext cartContext, ProductsAcmeContext productContext)
        {
            _cartContext = cartContext;
            _productContext = productContext;
        }


        [HttpPost]  
        public async Task<ActionResult<Cart>> CreateCart()
        {
            Cart cart = new Cart() { CartItems =  new List<CartItem>(), CartPrice = 0.0m};
            _cartContext.Carts.Add(cart);
            await _cartContext.SaveChangesAsync();
            var result = this.CreatedAtAction("ReadCartByCartId", new { CartId = cart.CartId }, cart);      // Gives us successful 201 status
            await Console.Out.WriteLineAsync("printing from carts controller");
            if (cart != null)
            {
                await Console.Out.WriteLineAsync("Deserialized Cart:");
                await Console.Out.WriteLineAsync($"IDCcont: {cart.CartId}");
                await Console.Out.WriteLineAsync($"CartPriceCCont: {cart.CartPrice}");
                // Print other properties as needed
            }
            return this.Ok(cart);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> ReadAllCarts()
        {
            // Returning a ActionResult
            return this.Ok(await _cartContext.Carts.ToListAsync());      
        }




        [HttpGet("{cartId}")]
        public async Task<ActionResult<CartDetailsDTO>>? ReadCartByCartId(int cartId)
        {
            Console.WriteLine($"printing form CartController {cartId}");
            var foundCart = await _cartContext.Carts.FindAsync(cartId);
            if (object.ReferenceEquals(foundCart, null))
            {
                return this.NotFound();

            }
            //var joinedData = await (
            //    from cartItem in _cartContext.CartItems
            //    join product in _productContext.Products
            //        on cartItem.ProductId equals product.ProductId
            //    where cartItem.FkCartId == cartId
            //    select new { CartItem = cartItem, Product = product }
            //).ToListAsync();

            //var cartItemsWithProducts = joinedData.Select(joined =>
            //    new CartItemWithProduct
            //    {
            //        CartItem = joined.CartItem,
            //        Product = joined.Product
            //    }).ToList();

            //var cartDetailsDTO = new CartDetailsDTO
            //{
            //    Cart = foundCart,
            //    CartItemsWithProducts = cartItemsWithProducts
            //};

            // Fetch cart items from CartContext
            // This list the cartItems that belongs to cartId
            var cartItems = await _cartContext.CartItems
                .Where(item => item.FkCartId == cartId)
                .ToListAsync();

            // Fetch products from ProductContext
            // Step1: Select all the productIds from the list of cartItems that we extracted earlier
            var productIds = cartItems.Select(item => item.ProductId);
            // Step2: For each product from all product list streaming from _productContext.Products,
            // we check if the productId list we derived earlier contains this product we looking at now
            // if contain => products
            var products = await _productContext.Products
                .Where(product => productIds.Contains(product.ProductId))
                .ToListAsync();

            // Combine cart items with products
            var cartItemsWithProducts = cartItems.Select(cartItem =>
            {
                var product = products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                return new CartItemWithProduct
                {
                    CartItem = cartItem,
                    Product = product
                };
            }).ToList();

            var cartDetailsDTO = new CartDetailsDTO
            {
                Cart = foundCart,
                CartItemsWithProducts = cartItemsWithProducts
            };

            return this.Ok(cartDetailsDTO);
        }



        [HttpPut("{cartId}/add-product/{productId}")]
        public async Task<ActionResult<Cart>> UpdateCartAddProduct(int cartId, int productId)
        {
            var existingProduct = await _productContext.Products.FindAsync(productId);
            if (object.ReferenceEquals(existingProduct, null))
            {
                return this.NotFound();
            }
            else
            {
                CartItem newCartItem = new CartItem() { ProductId = productId, FkCartId = cartId };
                _cartContext.CartItems.Add(newCartItem);

                var existingCart = await _cartContext.Carts.FindAsync(cartId);
                if (object.ReferenceEquals(existingCart, null))
                {
                    return this.NotFound();
                }
                else
                {
                    if (object.ReferenceEquals(existingCart.CartPrice, null))
                    {
                        existingCart.CartPrice = existingProduct.ProductPrice;
                        await _cartContext.SaveChangesAsync();
                        return this.Ok(existingCart);
                    }
                    else
                    {
                        existingCart.CartPrice += existingProduct.ProductPrice;
                        await _cartContext.SaveChangesAsync();
                        return this.Ok(existingCart);
                    }
                }

            }
        }

        [HttpPut("{cartId}/remove-product/{productId}")]
        public async Task<ActionResult<CartItem>> UpdateCartRemoveProduct(int cartId, int productId)
        {
            var existingCartItems = await _cartContext.CartItems
                                            .Where(item => item.FkCartId == cartId && item.ProductId == productId)
                                            .ToListAsync();
            if (object.ReferenceEquals(existingCartItems, null))
            {
                return this.NotFound();
            }
            else
            {
                foreach(var existingCartItem in existingCartItems)
                {
                    _cartContext.CartItems.Remove(existingCartItem);
                    var existingCart = await _cartContext.Carts.FindAsync(cartId);
                    var existingProduct = await _productContext.Products.FindAsync(productId);
                    existingCart.CartPrice -= existingProduct.ProductPrice;


                }
                await _cartContext.SaveChangesAsync();
                return this.Ok(existingCartItems);

            }

        }


        [HttpPut("clear-items/{cartId}")]
        public async Task<ActionResult<CartItem>> UpdateCartClearAllCartItems(int cartId)
        {
            var existingCartItems = await _cartContext.CartItems
                                            .Where(item => item.FkCartId == cartId)
                                            .ToListAsync();
            if (object.ReferenceEquals(existingCartItems, null))
            {
                return this.NotFound();
            }
            else
            {
                foreach (var existingCartItem in existingCartItems)
                {
                    _cartContext.CartItems.Remove(existingCartItem);
                    var existingCart = await _cartContext.Carts.FindAsync(cartId);
                    var existingProduct = await _productContext.Products.FindAsync(existingCartItem.ProductId);
                    existingCart.CartPrice -= existingProduct.ProductPrice;
                }
                await _cartContext.SaveChangesAsync();
                return this.Ok(existingCartItems);

            }

        }

        [HttpDelete("{cartId}")]
        public async Task<ActionResult<Cart>> DeleteCart(int cartId)
        {
            var foundCart = await _cartContext.Carts.FindAsync(cartId);
            if (object.ReferenceEquals(foundCart, null))
            { 
                return this.NotFound();

            }
            _cartContext.Carts.Remove(foundCart);
            await _cartContext.SaveChangesAsync();
            return this.Ok(foundCart);
        }

    }
}
