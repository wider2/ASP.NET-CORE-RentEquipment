using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Bondora.Api.Data;
using Bondora.Api.Models;
using Bondora.Api.Controllers;
using Bondora2.Models;
using Bondora.Api;
using Microsoft.Extensions.Caching.Memory;

namespace Bondora2.Controllers
{
    [ResponseCache(Duration = 0)]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> logger;
        private IStringLocalizer<SharedResources> localizer;
        private IMemoryCache cache;
        public CartController(IStringLocalizer<SharedResources> localizer, ILogger<CartController> logger, IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.localizer = localizer;
            this.cache = memoryCache;
        }

                
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            string priceCur = ConstClass.priceCurrency;
            decimal total = 0;
            List<ModelCart> list;
            string token = "";
            int? customerId = 0;

            ModelParameters modelParameters = cache.Get<ModelParameters>("globalParams");
            if (modelParameters != null)
            {
                token = modelParameters.Token;
                customerId = modelParameters.CustomerId;
            }

            ViewBag.Token = token;
            ViewBag.CustomerId = customerId;

            if (localizer != null)
            {
                ViewData["Title"] = localizer["Title"].Value;
                ViewBag.CartTitle = localizer["ShoppingCart"].Value;
                ViewBag.NotFound = localizer["NotFound"].Value;
                ViewBag.SubmitOrder = localizer["SubmitOrder"].Value;
                ViewBag.TotalLabel = localizer["Total"].Value;
                ViewBag.Name = localizer["Name"].Value;
                ViewBag.Type = localizer["Type"].Value;
                ViewBag.Days = localizer["Days"].Value;
                ViewBag.Duration = localizer["Duration"].Value;
                ViewBag.Price = localizer["Price"].Value;
                ViewBag.Delete = localizer["Delete"].Value;
            }

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    ApiCartController cartController = new ApiCartController(localizer); //_repository
                    list = await cartController.Get(token);
                    total = list.Sum(p => p.Price);
                    if (logger != null) logger.LogInformation("ApiCartController result: {0}", list);
                    ViewBag.Total = total + " " + priceCur;
                }
                return View(list);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + "; " + ex.InnerException);
                ViewBag.StatusMessage = ex.Message + "<br />" + ex.InnerException;
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> SubmitOrder()
        {
            string token = "";
            int? customerId = 0;
            
            ModelParameters modelParameters = this.cache.Get<ModelParameters>("globalParams");
            if (modelParameters != null)
            {
                token = modelParameters.Token;
                customerId = modelParameters.CustomerId;
            }

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var newItem = new BondoraOrder();
                    newItem.CustomerId = customerId;
                    newItem.DateOrder = DateTime.Now;                    
                    newItem.Token = token;
                    context.Entry(newItem).State = EntityState.Added;
                    await context.SaveChangesAsync();
                }
                if (logger != null) logger.LogInformation("Order created by customer: {0}", customerId);

                return RedirectToAction("Order", new RouteValueDictionary(
                    new { controller = "Order", action = "Order", token = token })
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + "; " + ex.InnerException);
                ViewBag.StatusMessage = ex.Message + "<br />" + ex.InnerException;
                return View();
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}