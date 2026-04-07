using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;

namespace Controllers
{
    [ApiController]
    [Route("pay")]
    public class PaymentController : Controller
    {
        private readonly StripeClient _stripe;

        public PaymentController(StripeClient stripe)
        {
            _stripe = stripe;
        }

        [HttpPost]
        public async Task<IActionResult> create(PaymentIntentCreateRequest intent)
        {
            var paymentIntent = _stripe.V1.PaymentIntents.Create(new PaymentIntentCreateOptions
            {
                Amount = CalculateOrderAmount(intent.Items),
                Currency = "eur",
                // In the latest version of the API, specifying the `automatic_payment_methods` parameter is optional because Stripe enables its functionality by default.
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                Enabled = true,
                },
            });

            return Json(new { clientSecret = paymentIntent.ClientSecret });
        }

        private long CalculateOrderAmount(Item[] items)
        {
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            long total = 0;
            foreach (Item item in items) {
                total += item.Amount;
            }
            return total;
        }

        public class Item
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("Amount")]
            public long Amount { get; set; }
        }

        public class PaymentIntentCreateRequest
        {
            [JsonProperty("items")]
            public Item[] Items { get; set; }
        }
    }
}