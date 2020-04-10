using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bondora.Api.Data;
using Bondora.Api.Models;
using Bondora.Api.Tools;

namespace Bondora.Api.Controllers
{
    public class ApiOrderRepository : IApiOrderRepository
    {
        public ApiOrderRepository() { }

        public async Task<Tuple<List<ModelCart>, BondoraOrder, BondoraCustomer>> Get(string token)
        {
            List<ModelCart> list = null;
            BondoraOrder order = null;
            BondoraCustomer customer = null;
            CalculateViewer calculateViewer = new CalculateViewer(new Calculate());
            string PriceCur = "€";

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    order = await (from a in context.BondoraOrder
                                   where a.Token == token
                                   select a).FirstOrDefaultAsync();
                    if (order != null)
                    {
                        customer = await (from a in context.BondoraCustomer
                                          where a.CustomerId == order.CustomerId
                                          select a).FirstOrDefaultAsync();

                        list = await (from a in context.BondoraInventory
                                      join b in context.BondoraInventoryTypes on a.TypeId equals b.TypeId
                                      join c in context.BondoraCart on a.InventoryId equals c.InventoryId
                                      where c.Token == token
                                      select new ModelCart()
                                      {
                                          InventoryId = a.InventoryId,
                                          Name = a.Name,
                                          TypeId = a.TypeId,
                                          TypeName = b.TypeName,
                                          Days = c.Days,
                                          Price = 0
                                      }).ToListAsync();
                        foreach (var item in list)
                        {
                            item.Price = calculateViewer.CalculatePrices(item.TypeId, item.Days);
                            item.PriceCur = PriceCur;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new Tuple<List<ModelCart>, BondoraOrder, BondoraCustomer>(list, order, customer);
        }

    }
}
