
using AcmeCorp.Shopper.UiWebApp.Models;
using System.Net.Http;
using System.Text.Json;

namespace AcmeCorp.Shopper.UiWebApp.APIClient
{
    public class OrdersClient : IOrdersClient
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;


        }

        public async Task<Order?> PostOrders(Order order)
        {
            var client = _httpClientFactory.CreateClient("OrdersApiClient");
            Console.WriteLine($"Before postiing: {order.OrderId}, {order.CartId}, {order.CustomerName}");
            string jsonOrder = JsonSerializer.Serialize(order, typeof(Order));
            StringContent content = new StringContent(jsonOrder, System.Text.Encoding.UTF8, "application/json");
            string contentString = await content.ReadAsStringAsync();
            Console.WriteLine($"StringContent: {contentString}");
            HttpResponseMessage response = await client.PostAsync("/api/Orders", content);
            Console.WriteLine($"Status code at OrdersClient: {response.StatusCode}");
            var responseBody = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Order>(responseBody);
        }
    }
}
