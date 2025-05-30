using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Client
    {
        [Key]
        public int IdClient { get; set; }
        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(120)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(120)]
        public string Email { get; set; }
        [Required]
        [MaxLength(120)]
        public string Telephone { get; set; }
        [Required]
        [MaxLength(120)]
        public string Pesel { get; set; }

        public virtual ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();
    }

    public class Country
    {
        [Key]
        public int IdCountry { get; set; }
        [Required]
        [MaxLength(120)]
        public string Name { get; set; }
        public virtual ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();
    }

    public class Trip
    {
        [Key]
        public int IdTrip { get; set; }
        [Required]
        [MaxLength(120)]
        public string Name { get; set; }
        [Required]
        [MaxLength(220)]
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }

        public virtual ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();
        public virtual ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();
    }

    public class ClientTrip
    {
        public int IdClient { get; set; }
        public int IdTrip { get; set; }
        public int RegisteredAt { get; set; }
        public int? PaymentDate { get; set; }

        [ForeignKey("IdClient")]
        public virtual Client Client { get; set; }
        [ForeignKey("IdTrip")]
        public virtual Trip Trip { get; set; }
    }

    public class CountryTrip
    {
        public int IdCountry { get; set; }
        public int IdTrip { get; set; }

        [ForeignKey("IdCountry")]
        public virtual Country Country { get; set; }
        [ForeignKey("IdTrip")]
        public virtual Trip Trip { get; set; }
    }
}