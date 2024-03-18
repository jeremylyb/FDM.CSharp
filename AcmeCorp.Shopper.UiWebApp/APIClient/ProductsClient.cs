
using AcmeCorp.Shopper.UiWebApp.APIClient;
using AcmeCorp.Shopper.UiWebApp.Models;
using System.Text.Json;

namespace AcmeCorp.Shopper.UiWebApp
{
    public class ProductsClient : IProductsClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductsClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Product>?> GetAllProductsAsync() 
        { 
            var client = _httpClientFactory.CreateClient("ProductsApiClient");
            var productsStreamTask = client.GetStreamAsync("/api/Product");
            await Console.Out.WriteLineAsync("printing out before deserializing");
            using (var stream = await client.GetStreamAsync("/api/Product"))
            {
                // Read the content of the stream
                using (var reader = new StreamReader(stream))
                {
                    string content = await reader.ReadToEndAsync();
                    Console.WriteLine(content); // Print the content to the console
                }
            }

            return await JsonSerializer.DeserializeAsync<List<Product>>(await productsStreamTask);
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            var client = _httpClientFactory.CreateClient("ProductsApiClient");
            HttpResponseMessage response = await client.GetAsync($"/api/Product/{id}");        // GetAsync returns a response statuse
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<Product>(responseBody);
            }
            return null;
        }

    }
}