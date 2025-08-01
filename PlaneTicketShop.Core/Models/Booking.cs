using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace PlaneTicketShop.Core.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int UserId { get; set; }
        public int NumberOfPassengers { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        [Required(ErrorMessage = "Passenger name is required")]
        public string PassengerName { get; set; }
        [Required(ErrorMessage = "Passenger email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string PassengerEmail { get; set; }
        [BindNever]
        public Flight Flight { get; set; }
        [BindNever]
        public string Status { get; set; }
        public string PassengerPhone { get; set; }
    }
} 