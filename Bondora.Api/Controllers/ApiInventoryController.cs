using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Bondora.Api.Data;
using Bondora.Api.Models;

namespace Bondora.Api.Controllers
{
    [Route("home/[controller]")]
    public class ApiInventoryController : Controller
    {        
        private IStringLocalizer<SharedResources> localizer;

        public ApiInventoryController(IStringLocalizer<SharedResources> localizer)
        {
            this.localizer = localizer;
        }

        
        [HttpGet, ResponseCache(CacheProfileName = "Base")]
        public async Task<Tuple<List<ModelInventory>, int>> Get(string token)
        {
            List<ModelInventory> inventory = null;
            int countOrdered = 0;
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    inventory = await (
                        from a in context.BondoraInventory
                        join b in context.BondoraInventoryTypes on a.TypeId equals b.TypeId
                        select new ModelInventory()
                        {
                            InventoryId = a.InventoryId,
                            Name = a.Name,
                            TypeId = a.TypeId,
                            TypeName = b.TypeName
                        }).ToListAsync();

                    countOrdered = await (from a in context.BondoraCart
                                              where a.Token == token
                                              select a).CountAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new Tuple<List<ModelInventory>, int>(inventory, countOrdered);
        }
        


        [HttpPost]
        [Consumes("application/json")]
        public async Task<ModelApiResponse> Post([FromBody] ModelAddToCart param)
        {
            ModelApiResponse modelApiResponse = new ModelApiResponse();
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    if (param != null)
                    {
                        if (param.InventoryId != 0)
                        {
                            //if product has been already found in cart
                            var dataUnique = await (from a in context.BondoraCart
                                                     where a.InventoryId == param.InventoryId && a.Token == param.Token
                                                     select a).FirstOrDefaultAsync();
                            if (dataUnique == null)
                            {
                                //insert new one
                                var newItem = new BondoraCart();
                                newItem.Days = param.Numdays;
                                newItem.InventoryId = param.InventoryId;
                                newItem.Token = param.Token;
                                context.Entry(newItem).State = EntityState.Added;
                                await context.SaveChangesAsync();
                            } else
                            {
                                dataUnique.Days += param.Numdays;
                                context.Entry(dataUnique).State = EntityState.Modified;
                                await context.SaveChangesAsync();
                            }

                            var countOrdered = await (from a in context.BondoraCart
                                                      where a.Token == param.Token
                                                      select a).CountAsync();
                                                        
                            modelApiResponse.Success = true;
                            modelApiResponse.Message = (localizer != null) ? localizer["AddedToCard"].Value : "";
                            modelApiResponse.CountOrdered = countOrdered;
                        }
                        else
                        {
                            modelApiResponse.Success = false;
                            modelApiResponse.Message = localizer["AddedToCard"].Value;
                        }
                    } else
                    {
                        modelApiResponse.Success = false;
                        modelApiResponse.Message = localizer["NoItemsToSubmit"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                modelApiResponse.Success = false;
                modelApiResponse.Message = ex.Message + "<br />" + ex.InnerException + "<br />" + ex.StackTrace;
            }
            return (modelApiResponse);
        }

    }
}