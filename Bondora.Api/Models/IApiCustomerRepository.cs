using System.Threading.Tasks;

namespace Bondora.Api.Models
{
    public interface IApiCustomerRepository
    {
        Task<BondoraCustomer> InsertOrUpdate(string Username);

        Task<BondoraCustomer> updateLoyalty(int customerId, int points);
    }
}