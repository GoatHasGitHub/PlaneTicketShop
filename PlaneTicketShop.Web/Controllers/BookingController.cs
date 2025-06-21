using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaneTicketShop.Core.Models;
using PlaneTicketShop.Infrastructure.Data;

namespace PlaneTicketShop.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
    }
} 