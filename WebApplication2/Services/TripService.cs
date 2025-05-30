using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.DTOs;
using WebApplication2.Models;
using System.Globalization;

namespace WebApplication2.Services
{
    public class TripService : ITripService
    {
        private readonly AppDbContext _context;

        public TripService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedTripsResponseDto> GetTripsAsync(int page, int pageSize)
        {
            var totalTrips = await _context.Trips.CountAsync();
            var totalPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

            var tripsQuery = _context.Trips
                .OrderByDescending(t => t.DateFrom)
                .Include(t => t.CountryTrips)
                    .ThenInclude(ct => ct.Country)
                .Include(t => t.ClientTrips)
                    .ThenInclude(ct => ct.Client)
                .Select(t => new TripDto
                {
                    Name = t.Name,
                    Description = t.Description,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    MaxPeople = t.MaxPeople,
                    Countries = t.CountryTrips.Select(ct => new CountryDto { Name = ct.Country.Name }).ToList(),
                    Clients = t.ClientTrips.Select(ct => new ClientBasicDto { FirstName = ct.Client.FirstName, LastName = ct.Client.LastName }).ToList()
                });

            var trips = await tripsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedTripsResponseDto
            {
                PageNum = page,
                PageSize = pageSize,
                AllPages = totalPages,
                Trips = trips
            };
        }

        public async Task<bool> AssignClientToTripAsync(int idTrip, AssignClientToTripRequestDto request)
        {
            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                throw new ArgumentException("Trip not found."); 
            }

            if (trip.DateFrom <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Cannot assign to a trip that has already started or occurred.");
            }
            if (request.TripName != trip.Name)
            {
                 throw new ArgumentException("Trip name mismatch with the provided IdTrip.");
            }

            var currentRegistrations = await _context.Client_Trip.CountAsync(ct => ct.IdTrip == idTrip);
            if (currentRegistrations >= trip.MaxPeople)
            {
                throw new InvalidOperationException("Trip is full.");
            }

            bool isClientRegisteredForThisTrip = await _context.Client_Trip
                .Include(ct => ct.Client)
                .AnyAsync(ct => ct.Client.Pesel == request.Pesel && ct.IdTrip == idTrip);

            if (isClientRegisteredForThisTrip)
            {
                throw new InvalidOperationException("Client with this PESEL is already registered for this trip.");
            }

            bool clientEntityExists = await _context.Clients.AnyAsync(c => c.Pesel == request.Pesel);
            if (clientEntityExists)
            {
                throw new InvalidOperationException("A client with this PESEL already exists in the system. This endpoint is for registering new clients only.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newClient = new Client
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Telephone = request.Telephone,
                    Pesel = request.Pesel
                };
                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync();

                int? paymentDateInt = null;
                if (!string.IsNullOrEmpty(request.PaymentDate))
                {
                    if (DateTime.TryParse(request.PaymentDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedPaymentDate))
                    {
                        if(parsedPaymentDate.Kind == DateTimeKind.Unspecified) {
                            parsedPaymentDate = DateTime.SpecifyKind(parsedPaymentDate, DateTimeKind.Utc);
                        } else if (parsedPaymentDate.Kind == DateTimeKind.Local) {
                            parsedPaymentDate = parsedPaymentDate.ToUniversalTime();
                        }
                        paymentDateInt = (int)new DateTimeOffset(parsedPaymentDate).ToUnixTimeSeconds();
                    }
                    else
                    {
                        throw new ArgumentException("Invalid PaymentDate format.");
                    }
                }

                var clientTrip = new ClientTrip
                {
                    IdClient = newClient.IdClient,
                    IdTrip = idTrip,
                    RegisteredAt = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    PaymentDate = paymentDateInt
                };
                _context.Client_Trip.Add(clientTrip);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}