using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bondora.Api.Models;

namespace Bondora.Api.Controllers
{
    [Route("home/[controller]")]
    public class ApiOrderController : Controller
    {
        private IApiOrderRepository apiOrderRepository;

        public ApiOrderController(IApiOrderRepository repository)
        {
            this.apiOrderRepository = repository;
        }

        [HttpGet]
        public async Task<Tuple<List<ModelCart>, BondoraOrder, BondoraCustomer>> Get(string token)
        {
            var result = await apiOrderRepository.Get(token);
            return (result);
        }

    }
}