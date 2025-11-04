using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public interface IStripeService
    {
        Task<Session> CreateCheckoutSessionAsync(List<CartItem> cartItems, string customerEmail, string successUrl, string cancelUrl);
        Task<Session> GetSessionAsync(string sessionId);
    }

    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            // Set Stripe API key from configuration
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public async Task<Session> CreateCheckoutSessionAsync(List<CartItem> cartItems, string customerEmail, string successUrl, string cancelUrl)
        {
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in cartItems)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (long)(item.Price * 100), // Stripe uses cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName,
                            Description = $"Quantity: {item.Quantity}"
                        },
                    },
                    Quantity = item.Quantity,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                CustomerEmail = customerEmail,
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session;
        }

        public async Task<Session> GetSessionAsync(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);
            return session;
        }
    }

    public class CartItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
