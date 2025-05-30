using System.Threading.Tasks;

namespace WebApplication2.Services
{
    public interface IClientService
    {
        Task<bool> DeleteClientAsync(int idClient);
    }
}