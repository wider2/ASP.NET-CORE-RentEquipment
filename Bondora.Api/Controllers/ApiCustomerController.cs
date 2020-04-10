using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bondora.Api.Models;

namespace Bondora.Api.Controllers
{
    [Route("home/[controller]")]
    public class ApiCustomerController : Controller
    {
        private IApiCustomerRepository apiCustomerRepository;

        public ApiCustomerController(IApiCustomerRepository repository)
        {
            this.apiCustomerRepository = repository;
        }

        [HttpGet]
        public async Task<BondoraCustomer> RegisterOrModify(string username)
        {
            var result = await apiCustomerRepository.InsertOrUpdate(username);
            return (result);
        }
        
        [HttpGet]
        public async Task<BondoraCustomer> updateLoyalty(int customerId, int points)
        {
            var result = await apiCustomerRepository.updateLoyalty(customerId, points);
            return (result);
        }

    }
}