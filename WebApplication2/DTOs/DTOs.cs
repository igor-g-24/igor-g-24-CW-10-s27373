using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs
{
    public class PaginatedTripsResponseDto
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int AllPages { get; set; }
        public List<TripDto> Trips { get; set; }
    }

    public class TripDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }
        public List<CountryDto> Countries { get; set; }
        public List<ClientBasicDto> Clients { get; set; }
    }

    public class CountryDto
    {
        public string Name { get; set; }
    }

    public class ClientBasicDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AssignClientToTripRequestDto
    {
        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(120)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(120)]
        public string Email { get; set; }

        [Required]
        [MaxLength(120)]
        public string Telephone { get; set; }

        [Required]
        [MaxLength(120)]
        public string Pesel { get; set; }

        [Required]
        [MaxLength(120)]
        public string TripName { get; set; } 

        public string? PaymentDate { get; set; }
    }
}