using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bondora.Api.Models;

namespace Bondora.Api.Models
{
    public interface IApiOrderRepository
    {
        Task<Tuple<List<ModelCart>, BondoraOrder, BondoraCustomer>> Get(string token);
    }
}