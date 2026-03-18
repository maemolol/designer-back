using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("basket")]
    public class BasketController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _email;

        public BasketController(AppDbContext dbContext, EmailService email)
        {
            _context = dbContext;
            _email = email;
        }

        public class ContactForm
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Message { get; set; } = "";
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] BasketRequest request)
{
            if (!ModelState.IsValid)
                return BadRequest();

            var paintings = await _context.Paintings
                .Where(p => request.PaintingIds.Contains(p.Id) && !p.Sold)
                .ToListAsync();

            if (paintings.Count != request.PaintingIds.Count)
                return BadRequest("One or more paintings already sold.");

            // Mark as sold
            foreach (var painting in paintings)
                painting.Sold = true;

            await _context.SaveChangesAsync();

            // Send email
            await _email.SendPurchaseEmail(request.Email, paintings);

            return Ok();
        }
    }
}
