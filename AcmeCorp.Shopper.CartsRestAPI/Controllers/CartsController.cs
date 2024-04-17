//using AcmeCorp.Shopper.CartsRestAPI.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace AcmeCorp.Shopper.CartsRestAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CartsController : Controller
//    {
//        //public IActionResult Index()
//        //{
//        //    return View();
//        //}

//        private readonly CartsAcmeContext _context;

//        public CartsController(CartsAcmeContext context)
//        {
//            _context = context;
//        }

//        [HttpPost]
//        public async Task<ActionResult<Cart>> CreateCart([FromBody] Cart cart)
//        {
//            try
//            {
//                _context.Carts.Add(cart);
//                await _context.SaveChangesAsync();

//                return this.CreatedAtAction("ReadCartbyCarttId", new { cartId = cart.CartId }, cart);      // Gives us successful 201 status
//            }
//            catch (Exception ex)
//            {
//                // Handle any exceptions
//                return StatusCode(500, $"Error: {ex.Message}");
//            }
//        }


//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Cart>>> ReadAllCarts()
//        {
//            return this.Ok(await _context.Carts.ToListAsync());
//        }


//        [HttpGet("{cartId}")]
//        public async Task<ActionResult<Cart>>? ReadCartByCartId(int cartId)
//        {
//            var foundCart = await _context.Carts.FindAsync(cartId);
//            if (object.ReferenceEquals(foundCart, null))
//            {
//                return this.NotFound();
//            }
//            return this.Ok(foundCart);
//        }

//        //[HttpPut("{cartId}/add-product/{productId}")]
//        //public async Task<ActionResult<Cart>> UpdateCaart([FromBody] Cart cart)
//        //{
//        //    var existingProduct = await _context.Products.FindAsync(product.ProductId);
//        //    if (object.ReferenceEquals(existingProduct, null))
//        //    {
//        //        return this.NotFound();
//        //    }
//        //    existingProduct.ProductName = product.ProductName;
//        //    existingProduct.ProductPrice = product.ProductPrice;
//        //    await _context.SaveChangesAsync();
//        //    return this.Ok(existingProduct);

//        //}



//        [HttpDelete("{cartId}")]
//        public async Task<ActionResult<Cart>> DeleteCart(int cartId)
//        {
//            var foundCart = await _context.Carts.FindAsync(cartId);
//            if (object.ReferenceEquals(foundCart, null))
//            {
//                // With this having ActionResult return type, we can determine the status we want to return
//                return this.NotFound();

//            }
//            _context.Carts.Remove(foundCart);
//            await _context.SaveChangesAsync();
//            return this.Ok(foundCart);
//        }
//    }
//}
