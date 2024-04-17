using AcmeCorp.Shopper.OrdersRestAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorp.Shopper.OrdersRestAPI.Controllers
{
    [Route("api/[controller]")]                         
    [ApiController]
    public class OrdersController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly OrdersAcmeContext _context;
        // Readonly only can be set in constructor. Only assign it once and we have 
        // up till exit constructor to assign
        public OrdersController(OrdersAcmeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            // Persist the resource to database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return this.CreatedAtAction("ReadOrderByOrderId", new { orderId = order.OrderId }, order);      // Gives us successful 201 status
        }


        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>>? ReadOrderByOrderId(int orderId)
        {
            var foundOrder = await _context.Orders.FindAsync(orderId);
            if (object.ReferenceEquals(foundOrder, null))
            {
                return this.NotFound();

            }
            return this.Ok(foundOrder);

        }
    }
}
