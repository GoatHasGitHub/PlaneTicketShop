using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaneTicketShop.Core.Models;
using PlaneTicketShop.Infrastructure.Data;
using PlaneTicketShop.Web.Models;

namespace PlaneTicketShop.Web.Controllers
{
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var flights = await _context.Flights.ToListAsync();
            return View(flights);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Create([Bind("FlightNumber,DepartureCity,ArrivalCity,DepartureTime,ArrivalTime,Price,AvailableSeats,Airline,AircraftType")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        [Authorize]
        public async Task<IActionResult> Book(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(new BookingFormViewModel { FlightId = flight.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Book(int id, BookingFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var flight = await _context.Flights.FindAsync(id);
                if (flight == null)
                {
                    return NotFound();
                }
                var booking = new Booking
                {
                    FlightId = id,
                    NumberOfPassengers = model.NumberOfPassengers,
                    PassengerName = model.PassengerName,
                    PassengerEmail = model.PassengerEmail,
                    PassengerPhone = model.PassengerPhone,
                    UserId = int.Parse(User.Identity.Name),
                    BookingDate = DateTime.UtcNow,
                    Status = "Not successful",
                    TotalPrice = flight.Price * model.NumberOfPassengers
                };
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("Payment", new { bookingId = booking.Id });
            }
            model.FlightId = id;
            return View(model);
        }

        [Authorize]
        public IActionResult Payment(int bookingId)
        {
            ViewBag.BookingId = bookingId;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int bookingId, string stripeToken)
        {
            if (string.IsNullOrEmpty(stripeToken))
            {
                TempData["PaymentSuccess"] = "Payment failed: No payment token received.";
                return RedirectToAction("Index");
            }
            var booking = await _context.Bookings.Include(b => b.Flight).FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking != null)
            {
                booking.Status = "Successful";
                booking.Flight = await _context.Flights.FindAsync(booking.FlightId);
                await _context.SaveChangesAsync();
            }
            TempData["PaymentSuccess"] = "Payment successful! Your booking is confirmed.";
            return RedirectToAction("Index");
        }
    }
} 