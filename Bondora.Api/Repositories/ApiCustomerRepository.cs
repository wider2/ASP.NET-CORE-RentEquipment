using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bondora.Api.Data;
using Bondora.Api.Models;

namespace Bondora.Api.Controllers
{
    public class ApiCustomerRepository : IApiCustomerRepository
    {

        public async Task<BondoraCustomer> InsertOrUpdate(string username) {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var dataUnique = await (from a in context.BondoraCustomer
                                            where a.Username == username
                                            select a).FirstOrDefaultAsync();
                    if (dataUnique == null)
                    {
                        var newItem = new BondoraCustomer();
                        newItem.Username = username;
                        newItem.LoyaltyPoints = 0;
                        context.Entry(newItem).State = EntityState.Added;
                        await context.SaveChangesAsync();
                        
                        return newItem;
                    } else
                    {
                        dataUnique.Username = username;
                        context.Entry(dataUnique).State = EntityState.Modified;
                        await context.SaveChangesAsync();

                        return dataUnique;
                    }
                }
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BondoraCustomer> updateLoyalty(int customerId, int points)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var dataUnique = await (from a in context.BondoraCustomer
                                            where a.CustomerId == customerId
                                            select a).FirstOrDefaultAsync();
                    if (dataUnique == null)
                    {
                        throw new CustomerException("customer not found");
                    }
                    else
                    {
                        dataUnique.LoyaltyPoints += points;
                        context.Entry(dataUnique).State = EntityState.Modified;
                        await context.SaveChangesAsync();

                        return dataUnique;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
