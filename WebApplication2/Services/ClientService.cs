using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;
using System;

namespace WebApplication2.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteClientAsync(int idClient)
        {
            var client = await _context.Clients.FindAsync(idClient);
            if (client == null)
            {
                throw new ArgumentException("Client not found.");
            }

            bool isAssignedToTrip = await _context.Client_Trip.AnyAsync(ct => ct.IdClient == idClient);
            if (isAssignedToTrip)
            {
                throw new InvalidOperationException("Client cannot be deleted as they are assigned to one or more trips.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}