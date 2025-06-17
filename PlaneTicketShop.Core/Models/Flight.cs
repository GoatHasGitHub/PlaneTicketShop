using System;
using System.ComponentModel.DataAnnotations;

namespace PlaneTicketShop.Core.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string FlightNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartureCity { get; set; }

        [Required]
        [StringLength(100)]
        public string ArrivalCity { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableSeats { get; set; }

        [Required]
        [StringLength(100)]
        public string Airline { get; set; }

        [Required]
        [StringLength(50)]
        public string AircraftType { get; set; }
    }
} 