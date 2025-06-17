using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaneTicketShop.Core.Models;
using PlaneTicketShop.Infrastructure.Data;

namespace PlaneTicketShop.Web.Controllers
{
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Flight
        public async Task<IActionResult> Index()
        {
            var flights = await _context.Flights.ToListAsync();
            return View(flights);
        }

        // GET: Flight/Details/5
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

        // GET: Flight/Create
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flight/Create
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

        // GET: Flight/Book/5
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

            return View(new Booking { FlightId = flight.Id });
        }

        // POST: Flight/Book/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Book(int id, [Bind("NumberOfPassengers,PassengerName,PassengerEmail,PassengerPhone")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var flight = await _context.Flights.FindAsync(id);
                if (flight == null)
                {
                    return NotFound();
                }

                booking.FlightId = id;
                booking.UserId = int.Parse(User.Identity.Name);
                booking.BookingDate = DateTime.UtcNow;
                booking.Status = "Confirmed";
                booking.TotalPrice = flight.Price * booking.NumberOfPassengers;

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }
    }
} 