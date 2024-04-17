
namespace AcmeCorp.Shopper.UiWebApp.APIClient;
using AcmeCorp.Shopper.UiWebApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class CartsClient : ICartsClient
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public CartsClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;


        }

    public async Task<CartItem?> AddProductToCart(int cartId, int productId)
    {
        var cartClient = _httpClientFactory.CreateClient("CartsApiClient");
        var productClient = _httpClientFactory.CreateClient("ProductsApiClient");
        Console.WriteLine(cartId.ToString() + ", " + productId);
        HttpResponseMessage getResponse = await productClient.GetAsync($"/api/Product/{productId}");        // GetAsync returns a response statuse
        Console.WriteLine($"status code: {getResponse.StatusCode}");
        if (getResponse.IsSuccessStatusCode)
        {
            var getResponseBody = await getResponse.Content.ReadAsStreamAsync();
            var productJsonDeserialize = await JsonSerializer.DeserializeAsync<Product>(getResponseBody);
            string productJson = JsonSerializer.Serialize(productJsonDeserialize, typeof(Product));
            StringContent content = new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage putResponse = await cartClient.PutAsync($"/api/Carts/{cartId}/add-product/{productId}", content);
            //string responseContent = await putResponse.Content.ReadAsStringAsync();
            //Console.WriteLine($"Response Content: {responseContent}");
            Console.WriteLine($"status code: {putResponse.StatusCode}");
            if (putResponse.IsSuccessStatusCode)
            {
                var putResponseBody = await putResponse.Content.ReadAsStreamAsync();
                //return await JsonSerializer.DeserializeAsync<CartItem>(putResponseBody);
                return await JsonSerializer.DeserializeAsync<CartItem>(putResponseBody);
            }
        }


        return null;
    }

    public async Task<List<CartItem>?> DeleteProductFromCart(int cartId, int productId)
    {
        var cartClient = _httpClientFactory.CreateClient("CartsApiClient");
        var productClient = _httpClientFactory.CreateClient("ProductsApiClient");
        Console.WriteLine(cartId.ToString() +", "+ productId);
        HttpResponseMessage getResponse = await productClient.GetAsync($"/api/Product/{productId}");        // GetAsync returns a response statuse
        Console.WriteLine($"status code: {getResponse.StatusCode}");
        if (getResponse.IsSuccessStatusCode)
        {
            var getResponseBody = await getResponse.Content.ReadAsStreamAsync();
            var productJsonDeserialize = await JsonSerializer.DeserializeAsync<Product>(getResponseBody);
            string productJson = JsonSerializer.Serialize(productJsonDeserialize, typeof(Product));
            StringContent content = new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage putResponse = await cartClient.PutAsync($"/api/Carts/{cartId}/remove-product/{productId}", content);
            //string responseContent = await putResponse.Content.ReadAsStringAsync();
            //Console.WriteLine($"Response Content: {responseContent}");
            Console.WriteLine($"status code: {putResponse.StatusCode}");
            if (putResponse.IsSuccessStatusCode)
            {
                var putResponseBody = await putResponse.Content.ReadAsStreamAsync();
                //return await JsonSerializer.DeserializeAsync<CartItem>(putResponseBody);
                return await JsonSerializer.DeserializeAsync<List<CartItem>>(putResponseBody);
            }
        }


        return null;

    }

    public async Task<CartDetailsDTO?> GetAllCartItemsForCartAsync(int? cartId)
    {
        var client = _httpClientFactory.CreateClient("CartsApiClient");
        HttpResponseMessage response = await client.GetAsync($"/api/Carts/{cartId}");
        Console.WriteLine($"response statuscode from CartClient: {response.StatusCode}, {cartId}");
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStreamAsync();
            //var cartDetailsDTO = await JsonSerializer.DeserializeAsync<CartDetailsDTO>(responseBody);

            //// Print out the CartDetailsDTO
            //Console.WriteLine($"Cart Details DTO: {JsonSerializer.Serialize(cartDetailsDTO)}");

            return await JsonSerializer.DeserializeAsync<CartDetailsDTO>(responseBody);
        }
        return null;

    }

    public async Task<Cart?> PostNewCart()
    {
        var client = _httpClientFactory.CreateClient("CartsApiClient");
        var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync("/api/Carts", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var cart = JsonSerializer.Deserialize<Cart>(responseBody);

        if (cart != null)
        {
            await Console.Out.WriteLineAsync("Deserialized Cart:");
            await Console.Out.WriteLineAsync($"CartId: {cart.CartId}");
            Console.WriteLine($"CartPrice: {cart.CartPrice}");
            // Print other properties as needed
        }
        else
        {
            Console.WriteLine("cart is null");
        }
        //return await JsonSerializer.DeserializeAsync<Cart>(responseBody);
        return JsonSerializer.Deserialize<Cart>(responseBody);
    }

    public async Task<List<CartItem>?> DeleteAllProductFromCart(int cartId)
    {
        var cartClient = _httpClientFactory.CreateClient("CartsApiClient");
        HttpResponseMessage response = await cartClient.GetAsync($"/api/Carts/{cartId}");     
        Console.WriteLine($"status code: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            var getResponseBody = await response.Content.ReadAsStreamAsync();
            var cartJsonDeserialize = await JsonSerializer.DeserializeAsync<Cart>(getResponseBody);
            string cartJson = JsonSerializer.Serialize(cartJsonDeserialize, typeof(Cart));
            StringContent content = new StringContent(cartJson, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage putResponse = await cartClient.PutAsync($"/api/Carts/clear-items/{cartId}", content);
            Console.WriteLine($"status code: {putResponse.StatusCode}");
            if (putResponse.IsSuccessStatusCode)
            {
                var putResponseBody = await putResponse.Content.ReadAsStreamAsync();
                //return await JsonSerializer.DeserializeAsync<CartItem>(putResponseBody);
                return await JsonSerializer.DeserializeAsync<List<CartItem>>(putResponseBody);
            }
        }
        return null;
    }


}
