using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Bondora.Api.Data;
using Bondora.Api.Models;
using Bondora.Api.Tools;

namespace Bondora.Api.Controllers
{
    [Route("home/[controller]")]
    public class ApiCartController : Controller
    {
        private IStringLocalizer<SharedResources> localizer;

        public ApiCartController(IStringLocalizer<SharedResources> localizer)
        {
            this.localizer = localizer;
        }

        [HttpGet]
        public async Task<List<ModelCart>> Get(string token)
        {
            List<ModelCart> list = null;
            string priceCur = ConstClass.priceCurrency;
            CalculateViewer calculateViewer = new CalculateViewer(new Calculate());

            try
            {
                using (var context = new ApplicationDbContext())
                {
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
                        item.PriceCur = priceCur;
                        item.Price = calculateViewer.CalculatePrices(item.TypeId, item.Days);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return list;
        }


        [HttpPost]
        [Consumes("application/json")]
        public async Task<ModelApiResponse> Post([FromBody] ModelOrder param)
        {
            ModelApiResponse modelApiResponse = new ModelApiResponse();
            try
            {
                if (param != null)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var newItem = new BondoraOrder();
                        newItem.CustomerId = param.CustomerId;
                        newItem.DateOrder = DateTime.Now;
                        newItem.Token = param.Token;
                        context.Entry(newItem).State = EntityState.Added;
                        await context.SaveChangesAsync();

                        modelApiResponse.Success = true;
                        modelApiResponse.Message = "";
                        modelApiResponse.CountOrdered = 0;
                    }
                } else
                {
                    modelApiResponse.Success = false;
                    modelApiResponse.Message = localizer["NoItemsToSubmit"].Value;
                    modelApiResponse.CountOrdered = 0;
                }
            }
            catch (Exception ex)
            {
                modelApiResponse.Success = false;
                modelApiResponse.Message = ex.Message + "<br />" + ex.InnerException + "<br />" + ex.StackTrace;
                modelApiResponse.CountOrdered = 0;
            }
            return modelApiResponse;
        }


        [HttpDelete]
        public async Task<ModelApiResponse> Delete([FromQuery] int? id, string token)
        {
            ModelApiResponse modelApiResponse = new ModelApiResponse();
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var product = await (from a in context.BondoraCart
                                        where a.Token == token && a.InventoryId == id
                                        select a).FirstOrDefaultAsync();
                    if (product !=null)
                    {
                        context.BondoraCart.Remove(product);
                        await context.SaveChangesAsync();

                        modelApiResponse.Success = true;
                    } else
                    {
                        modelApiResponse.Success = false;
                        modelApiResponse.Message = localizer["NotFound"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                modelApiResponse.Success = false;
                modelApiResponse.Message = ex.Message + "<br />" + ex.InnerException + "<br />" + ex.StackTrace;
            }
            return modelApiResponse;
        }
        
    }
}