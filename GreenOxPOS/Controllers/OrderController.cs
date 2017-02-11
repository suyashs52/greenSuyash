using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenOxPOS.Models;
using GreenOxPOS.Repository;
using System.Text;
namespace GreenOxPOS.Controllers
{
    public class OrderController : Controller
    {


        // GET: Order/Details/5
        public ActionResult Details(string p, string q)
        {
            ValidatedUser();
            if (PopulateCache())
            {
                ModelState.Clear();
                try
                {
                    Order o = new Order();
                    o.Product = p;
                    o.Quantity = q;
                    o.DiscountAmount = 0;
                    o.Address = new Address();
                    o.Address.Id = 0;
                    o.ProductAmount = string.Empty;
                    o.Payment = 0;
                    int[] arrProductId = p.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                    int[] arrProductQuantity = q.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();

                    List<Product> prod = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];

                    prod = prod.Where(f => arrProductId.Contains(f.ProductId)).ToList();

                    foreach (var pr in prod)
                        o.ProductAmount += pr.Price + ",";
                    o.ProductAmount = o.ProductAmount.Substring(0, o.ProductAmount.Length - 1);

                    ViewBag.Product = prod;
                    ViewBag.Quantity = arrProductQuantity;
                    return View(o);
                }
                catch (Exception ex)
                {
                    OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
                return View(p);
            }
            else return RedirectToAction("AddProduct", new { GUID = ViewBag.GUID });
        }
        [HttpPost]
        public ActionResult PlaceOrder(Order o)
        {
            try
            {
                ValidatedUser();
                if (PopulateCache())
                {
                    if (ModelState.IsValid)
                    {
                        int[] arrProduct = o.ProductAmount.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                        foreach (int i in arrProduct)
                            o.Payment += i;

                        o.Payment = o.Payment - o.DiscountAmount;

                        OrderRepository or = new OrderRepository();
                        if (or.PlaceOrder(o, "CreateOrder"))
                        {
                            return RedirectToAction("PrintOrder", "Order", new { p = o.Product, q = o.Quantity, a = o.DiscountAmount + "," + o.Payment });
                            // PrintOrder(o);
                        }
                        else return View();
                    }
                }
            }
            catch (Exception ex)
            {
                OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            }
            return View();
        }
        public ActionResult PrintOrder(string p, string q, string a)
        {
            ValidatedUser();
            if (PopulateCache())
            {
                try
                {
                    int[] arrProductId = p.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                    int[] arrProductQuantity = q.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                    int[] arrProductAmount = a.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();

                    List<Product> prod = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];

                    prod = prod.Where(f => arrProductId.Contains(f.ProductId)).ToList();

                    ViewBag.Product = prod;
                    ViewBag.Quantity = arrProductQuantity;
                    ViewBag.Amount = arrProductAmount;
                    return View();
                }
                catch (Exception ex)
                {
                    OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            return View();
        }
        public ActionResult ShowCustomer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCustomer(Address address)
        {
            try
            {
                ValidatedUser();
                if (PopulateCache())
                {
                    if (ModelState.IsValid)
                    {

                        OrderRepository or = new OrderRepository();
                        if (or.AddEditCustomer(address, "CreateCustomer"))
                        {
                            return Content("{\"1\":[{\"Id\":\"" + address.Id + "\",\"Name\":\"" + address.Customer.Name + "\",\"PhoneNo\":\"" + address.Customer.PhoneNo + "\",\"Email\":\"" + address.Customer.Email + "\",\"Address\":\"" + address.CustAddress + "\"}]}");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            }
            return Content("{\"1\":[{\"Id\":\"" + 0 + "\",\"Name\":\"" + string.Empty + "\",\"PhoneNo\":\"" + string.Empty + "\",\"Email\":\"" + string.Empty + "\",\"Address\":\"" + string.Empty + "\"}]}");
        }
        [HttpPost]
        public ActionResult EditCustomer(Address address)
        {
            try
            {
                ValidatedUser();
                if (PopulateCache())
                {
                    if (ModelState.IsValid)
                    {

                        OrderRepository or = new OrderRepository();
                        if (or.AddEditCustomer(address, "UpdateCustomer"))
                        {
                            return Content("{\"1\":[{\"Id\":\"" + address.Id + "\",\"Name\":\"" + address.Customer.Name + "\",\"PhoneNo\":\"" + address.Customer.PhoneNo + "\",\"Email\":\"" + address.Customer.Email + "\",\"Address\":\"" + address.CustAddress + "\"}]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            }
            return Content("{\"1\":[{\"Id\":\"" + 0 + "\",\"Name\":\"" + string.Empty + "\",\"PhoneNo\":\"" + string.Empty + "\",\"Email\":\"" + string.Empty + "\",\"Address\":\"" + string.Empty + "\"}]}");
        }
        [HttpPost]
        public ActionResult SearchCustomer(string SearchTitle, string SearchText)
        {
            try
            {
                ValidatedUser();
                if (PopulateCache())
                {
                    if (ModelState.IsValid)
                    {

                        OrderRepository or = new OrderRepository();
                        List<Address> ad = new List<Address>();

                        if (or.SearchCustomer(SearchTitle, SearchText, ref ad))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{\"1\":[");

                            foreach (Address address in ad)
                                sb.Append("{\"Id\":\"" + address.Id + "\",\"Name\":\"" + address.Customer.Name + "\",\"PhoneNo\":\"" + address.Customer.PhoneNo + "\",\"Email\":\"" + address.Customer.Email + "\",\"Address\":\"" + address.CustAddress + "\"},");

                            sb.Append("]}");
                            return Content(sb.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            }
            return Content("{\"1\":[{\"Id\":\"" + 0 + "\",\"Name\":\"" + string.Empty + "\",\"PhoneNo\":\"" + string.Empty + "\",\"Email\":\"" + string.Empty + "\",\"Address\":\"" + string.Empty + "\"}]}");
        }
        public bool ValidatedUser()
        {
            bool IsValidUser = false;
            try
            {
                ViewBag.GUID = string.Empty;
                ViewBag.IsAdmin = string.Empty;

                if (Request["GUID"] == null) { }
                else
                {
                    string guid = Request["GUID"];

                    if (HttpContext.Request.Cookies[guid + "User"] == null) { }
                    else
                    {
                        string ckGUID = HttpContext.Request.Cookies[guid + "User"].Value;

                        if (HttpRuntime.Cache[ckGUID + "User"] == null) { }
                        else
                        {
                            IsValidUser = true;
                            ViewBag.GUID = guid;
                            ViewBag.IsAdmin = HttpContext.Request.Cookies[guid + "UserAdmin"].Value;

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return IsValidUser;
        }

        public bool PopulateCache()
        {
            try
            {
                if (HttpRuntime.Cache[Enums.Cache.Product.ToString()] == null)
                {
                    ProductRepository pr = new ProductRepository();
                    List<Product> p = pr.GetAllProduct("VIEWPRODUCT");
                    HttpRuntime.Cache.Insert(Enums.Cache.Product.ToString(), p, null, DateTime.MaxValue, TimeSpan.FromHours(6));

                    List<ProductCategory> pc = pr.GetAllProductCategory("VIEWPRODUCTCAT");
                    HttpRuntime.Cache.Insert(Enums.Cache.ProductCategory.ToString(), pc, null, DateTime.MaxValue, TimeSpan.FromHours(6));


                    return true;
                }
                else if (((List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()]).Count() > -1) return true;
                else return false;

            }
            catch (Exception ex)
            {
                OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }
    }
}
