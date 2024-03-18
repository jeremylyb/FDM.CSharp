using Assignment_ADO.NETWebAPI_MVCApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment_ADO.NETWebAPI_MVCApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly ProductsAcmeContext _context;

        public ProductController(ProductsAcmeContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> ReadAllProducts()
        {
            return this.Ok(await _context.Products.ToListAsync());    
        }


        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>>? ReadProductByProductId(int productId)
        {
            var foundProduct = await _context.Products.FindAsync(productId);
            if (object.ReferenceEquals(foundProduct, null))
            {
                return this.NotFound();
            }
            return this.Ok(foundProduct);
        }

        [HttpGet("productName/{productName}")]
        public async Task<ActionResult<IEnumerable<Product>>>? ReadAllProductsByProductName(string productName)
        {

            return this.Ok(await _context.Products.Where(p => p.ProductName == productName).ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return this.CreatedAtAction("ReadProductByProductId", new { productId = product.ProductId }, product);      // Gives us successful 201 status
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (object.ReferenceEquals(existingProduct, null))
            {
                return this.NotFound();
            }
            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductPrice = product.ProductPrice;
            await _context.SaveChangesAsync();
            return this.Ok(existingProduct);

        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult<Product>> DeleteBook(int productId)
        {
            var foundProduct = await _context.Products.FindAsync(productId);
            if (object.ReferenceEquals(foundProduct, null))
            {
                // With this having ActionResult return type, we can determine the status we want to return
                return this.NotFound();

            }
            _context.Products.Remove(foundProduct);
            await _context.SaveChangesAsync();
            return this.Ok(foundProduct);
        }
    }
}

