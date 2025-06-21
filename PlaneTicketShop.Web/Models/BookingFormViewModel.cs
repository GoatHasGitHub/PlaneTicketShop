using System.ComponentModel.DataAnnotations;

namespace PlaneTicketShop.Web.Models
{
    public class BookingFormViewModel
    {
        public int FlightId { get; set; }
        public int NumberOfPassengers { get; set; }
        [Required(ErrorMessage = "Passenger name is required")]
        public string PassengerName { get; set; }
        [Required(ErrorMessage = "Passenger email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string PassengerEmail { get; set; }
        public string PassengerPhone { get; set; }
    }
} 