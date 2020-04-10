using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Bondora.Api.Data;
using Bondora.Api.Models;
using Bondora.Api;
using Bondora2.Tools;
using Bondora.Api.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Bondora2.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Routing;

namespace Bondora2.Controllers
{
    [ResponseCache(Duration = 0)]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> logger;
        private IStringLocalizer<SharedResources> localizer;
        private IApiOrderRepository apiOrderRepository;
        private IApiCustomerRepository apiCustomerRepository;
        private IMemoryCache cache;

        public OrderController(IStringLocalizer<SharedResources> localizer,
            ILogger<OrderController> logger,
            IApiOrderRepository repository,
            IApiCustomerRepository apiCustomerRepository,
            IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.localizer = localizer;
            this.apiOrderRepository = repository;
            this.apiCustomerRepository = apiCustomerRepository;
            this.cache = memoryCache;
        }


        [HttpGet]
        public async Task<IActionResult> Order(string token)
        {
            string priceCur = ConstClass.priceCurrency;
            int count = 0, loyaltyPoints = 0;
            decimal total = 0;
            int customerId = 0;
            List<ModelCart> list = null;
            CalculateViewer calculateViewer = new CalculateViewer(new Calculate());
            StringBuilder strb = new StringBuilder();

            ModelParameters modelParameters = cache.Get<ModelParameters>("globalParams");
            if (modelParameters != null)
            {
                token = modelParameters.Token;
                customerId = modelParameters.CustomerId;
            }

            if (localizer != null)
            {
                ViewData["Title"] = localizer["Title"].Value;
                ViewBag.Name = localizer["Name"].Value;
                ViewBag.Type = localizer["Type"].Value;
                ViewBag.Duration = localizer["Duration"].Value;
                ViewBag.DateOrderLabel = localizer["DateOrder"].Value;
                ViewBag.ProductsCountLabel = localizer["ProductsCount"].Value;
                ViewBag.CustomerNameLabel = localizer["Customer"].Value;
                ViewBag.LoyaltyPointsLabel = localizer["LoyaltyPoints"].Value;
                ViewBag.StartNewOrder = localizer["StartNewOrder"].Value;
                ViewBag.Download = localizer["Download"].Value;
                ViewBag.Print = localizer["Print"].Value;
            }

            try
            {
                ApiOrderController orderController = new ApiOrderController(apiOrderRepository);
                Tuple<List<ModelCart>, BondoraOrder, BondoraCustomer> tuple = await orderController.Get(token);
                if (tuple != null)
                {
                    list = tuple.Item1;
                    BondoraOrder order = tuple.Item2;
                    BondoraCustomer customer = tuple.Item3;
                    if (logger != null) logger.LogInformation("ApiOrderController result: {0}", list);

                    if (list == null)
                    {
                        ViewBag.StatusMessage = localizer["NotFound"].Value;
                    }
                    else
                    {
                        ViewBag.ProductsCount = list.Count();
                        if (order != null)
                        {
                            ViewBag.OrderTitle = localizer["OrderNo"].Value + " " + order.OrderId;
                            ViewBag.DateOrder = order.DateOrder;
                        }
                        if (customer != null) ViewBag.CustomerName = customer.Username;


                        strb.Append("<br /><table width='100%'>");
                        strb.Append("<tr><th>" + localizer["Name"].Value + "</th><th>" + localizer["Type"].Value + "</th><th>" + localizer["Days"].Value + "</th><th>" + localizer["Price"].Value + "</th></tr>");
                        foreach (var item in list)
                        {
                            count += 1;
                            loyaltyPoints += calculateViewer.CalculatePoints(item.TypeId);
                            item.Price = calculateViewer.CalculatePrices(item.TypeId, item.Days);
                            item.PriceCur = priceCur;
                            total += item.Price;

                            strb.Append("<tr bgcolor='" + IsParity(2, count - 1) + "'><td>" + item.Name + "</td><td>" + item.TypeName + "</td><td>" + item.Days + "</td><td>" + item.Price + " " + item.PriceCur + "</td></tr>");
                        }
                        strb.Append("<tr><td colspan=2></td><td align=right>" + localizer["Total"].Value + ":</td><td class='price'>" + total + " " + priceCur + "</td></tr>");
                        strb.Append("</table>");
                        ViewBag.OrderProducts = strb.ToString();
                        ViewBag.LoyaltyPoints = loyaltyPoints;

                        ApiCustomerController apiCustomerController = new ApiCustomerController(apiCustomerRepository);
                        await apiCustomerController.updateLoyalty(customerId, loyaltyPoints);
                    }
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



        public static string IsParity(int param, int count)
        {
            string value = "";
            if (param == 1)
            {
                value = (count % 2 == 0) ? "#FFFFFF" : "#ffffcc";
            }
            if (param == 2)
            {
                value = (count % 2 == 0) ? "#FFFFFF" : "#eeeeee";
            }
            return value;
        }

        public IActionResult StartNewOrder()
        {
            //HttpContext.Session.SetString("token", "");
            //HttpContext.Session.Remove("token");
            HttpContext.Session.Clear();

            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie == ".AspNetCore.Session")
                    Response.Cookies.Delete(cookie);
            }

            cache.Remove("globalParams");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Download()
        {
            string file = "";
            string token = HttpContext.Session.GetString("token");
            ContentType mimeType = new ContentType("text/plain");
            mimeType.CharSet = "utf-8";
            
            string priceCur = ConstClass.priceCurrencyText;
            int count = 0, loyaltyPoints = 0;
            decimal total = 0;

            StringBuilder strb = new StringBuilder();
            List<ModelCart> list;
            CalculateViewer calculateViewer = new CalculateViewer(new Calculate());

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    ApiOrderController opderController = new ApiOrderController(apiOrderRepository);
                    Tuple<List<ModelCart>, BondoraOrder, BondoraCustomer> tuple = await opderController.Get(token);
                    BondoraOrder order = tuple.Item2;
                    
                    if (order == null)
                    {
                        strb.Append(localizer["NotFound"].Value);
                    }
                    else
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

                        if (logger != null) logger.LogInformation("ApiOrderController download list size: {0}", list.Count);
                        if (list != null)
                        {
                            strb.Append(localizer["OrderNo"].Value + " " + order.OrderId + Environment.NewLine + Environment.NewLine);
                            file = localizer["Order"].Value + order.OrderId + ".txt";
                            
                            foreach (var item in list)
                            {
                                count += 1;
                                loyaltyPoints += calculateViewer.CalculatePoints(item.TypeId);
                                item.Price = calculateViewer.CalculatePrices(item.TypeId, item.Days);
                                item.PriceCur = priceCur;
                                total += item.Price;

                                strb.Append(Environment.NewLine + item.Name + ": " + item.Price + " " + item.PriceCur + Environment.NewLine);
                            }
                            strb.Append(Environment.NewLine + Environment.NewLine + localizer["ProductsCount"].Value + ": " + count + Environment.NewLine);
                            strb.Append(Environment.NewLine + localizer["Total"].Value + ": " + total + " " + priceCur + Environment.NewLine);
                            strb.Append(Environment.NewLine + localizer["LoyaltyPoints"].Value + ": " + loyaltyPoints);
                        }
                    }

                    
                    ContentDisposition cd = new ContentDisposition
                    {
                        FileName = file,
                        Inline = false
                    };
                    Response.ContentType = "UTF-8";
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    

                    byte[] byteArray = Encoding.ASCII.GetBytes(strb.ToString());
                    MemoryStream stream = null;
                    var memory = new MemoryStream();
                    using (stream = new MemoryStream(byteArray))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, mimeType.ToString());
                }
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