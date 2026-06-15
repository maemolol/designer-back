using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Dtos;
using Models;
using Stripe.Forwarding;

namespace Controllers
{
    [ApiController]
    [Route("pay")]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> create([FromBody] CheckoutRequest request)
        {
            Console.WriteLine(request.Email);

            foreach (var id in request.PaintingIds)
            {
                Console.WriteLine(id);
            }

            var paintings = await _context.Paintings
                .Where(p => request.PaintingIds.Contains(p.Id) && !p.Sold)
                .ToListAsync();
            
            if (paintings.Count == 0)
            {
                return BadRequest("Basket is empty or items unavailable.");
            }

            if (paintings.Count != request.PaintingIds.Count)
                return BadRequest("Unavailable items.");

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"{request.SuccessUrlBase}/success",
                CancelUrl = $"{request.SuccessUrlBase}/basket",
                CustomerEmail = request.Email,
                LineItems = paintings.Select(p => new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmountDecimal = (decimal?) p.Price * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = p.Name
                        }
                    }
                }).ToList()
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new
            {
                url = session.Url
            });
        }
    }
}