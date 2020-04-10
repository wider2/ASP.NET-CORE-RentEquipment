using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Bondora2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Bondora.Api.Models;
using Bondora.Api.Tools;
using Bondora.Api;
using Bondora.Api.Controllers;
using Microsoft.Extensions.Caching.Memory;

namespace Bondora2.Controllers
{
    [ResponseCache(Duration = 5)]
    public class HomeController : Controller
    {
        private IMemoryCache cache;
        private readonly ILogger<HomeController> logger;
        private IStringLocalizer<SharedResources> localizer;
        private IApiCustomerRepository apiCustomerRepository;

        public HomeController(IStringLocalizer<SharedResources> localizer,
            ILogger<HomeController> logger,
            IMemoryCache memoryCache,
            IApiCustomerRepository apiCustomerRepository)
        {
            this.logger = logger;
            this.localizer = localizer;
            this.cache = memoryCache;
            this.apiCustomerRepository = apiCustomerRepository;
        }

        #region Private Methods
        private void cacheData(string token, int customerId)
        {
            int? expireTime = 60;
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddMinutes(expireTime.Value);
            options.SlidingExpiration = TimeSpan.FromMinutes(expireTime.Value);

            ModelParameters model = new ModelParameters();
            model.Token = token;
            model.CustomerId = customerId;
            cache.Set("globalParams", model, options);
        }
        #endregion


        [HttpGet, ResponseCache(CacheProfileName = "Base")]
        //[HttpGet]
        public async Task<IActionResult> Index()
        {
            int customerId = 0;
            string username = "Abrakadabra";  //just for this demo application
            List<ModelInventory> result = null;
            int countOrdered = 0;

            string culture = "en";
            var cultureInfo = new CultureInfo(culture);
            cultureInfo.NumberFormat.CurrencySymbol = "€";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);


            string token ="";
            if (HttpContext != null)
            {
                // actually it is a demo variant of registration
                ApiCustomerController apiCustomerController = new ApiCustomerController(apiCustomerRepository);
                var resultRegister = await apiCustomerController.RegisterOrModify(username);
                if (resultRegister == null)
                {
                    ViewBag.StatusMessage = localizer["BadRegistration"].Value;
                } else
                {
                    customerId = resultRegister.CustomerId;
                }
                                
                token = HttpContext.Session.GetString("token");
                if (string.IsNullOrEmpty(token))
                {
                    Token t = new Token();
                    token = t.CreateToken();
                    HttpContext.Session.SetString("token", token);

                    cacheData(token, customerId);
                }
                ViewBag.Token = token;
                HttpContext.Session.SetInt32("customerId", customerId);
                                
            }
            if (localizer != null)
            {
                ViewData["Title"] = localizer["Title"].Value;
                ViewBag.NotFound = localizer["NotFound"].Value;
                ViewBag.Name = localizer["Name"].Value;
                ViewBag.Type = localizer["Type"].Value;
                ViewBag.Duration = localizer["Duration"].Value;
                ViewBag.Days = localizer["Days"].Value;
                ViewBag.AddCart = localizer["AddCart"].Value;
                ViewBag.ErrorApiCall = localizer["ErrorApiCall"].Value;
                ViewBag.Cart = localizer["ShoppingCart"].Value;
                ViewBag.OpenCart = localizer["OpenCart"].Value;
                ViewBag.WrongInput = localizer["WrongInput"].Value;
            }

            try
            {
                ApiInventoryController apiInventoryController = new ApiInventoryController(localizer);
                Tuple<List<ModelInventory>, int> tuple = await apiInventoryController.Get(token);
                result = tuple.Item1;
                countOrdered = tuple.Item2;                                
                ViewBag.CountOrdered = countOrdered;

                return View(result);
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
