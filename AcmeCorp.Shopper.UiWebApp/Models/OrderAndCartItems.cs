namespace AcmeCorp.Shopper.UiWebApp.Models
{
    internal class OrderAndCartItems
    {
        public OrderAndCartItems()
        {
        }

        public Order Order { get; set; }
        public CartDetailsDTO? CartDetailsDTO { get; internal set; }
    }
}