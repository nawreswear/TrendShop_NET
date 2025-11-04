using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripePaymentController : ControllerBase
    {
        // Créer le paiement Stripe
        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] PaymentRequestDto request)
        {
            if (request.Amount <= 0)
                return BadRequest(new { error = "Le montant doit être supérieur à 0." });

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(request.Amount * 100), // montant en centimes
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = string.IsNullOrWhiteSpace(request.Description) ? "Commande TrendShop" : request.Description
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = request.RedirectUrl ?? "https://localhost:7033/Panier/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:7033/Order/Cancel"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new
            {
                id = session.Id,
                checkoutUrl = session.Url
            });
        }
    }

    // DTO pour la requête de paiement
    public class PaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? RedirectUrl { get; set; }
    }
}

