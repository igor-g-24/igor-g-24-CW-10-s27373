using System.Threading.Tasks;
using WebApplication2.DTOs;

namespace WebApplication2.Services
{
    public interface ITripService
    {
        Task<PaginatedTripsResponseDto> GetTripsAsync(int page, int pageSize);
        Task<bool> AssignClientToTripAsync(int idTrip, AssignClientToTripRequestDto request);
    }
}