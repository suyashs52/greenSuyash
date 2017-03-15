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
        public ActionResult ListOrder(string GUID)
        {
            if (ValidatedUser())
            { }
            return View();

        }
        public ActionResult ListOrderRecord(string From, string To, int size, int Offset, string GUID)
        {

            try
            {
                List<Order> o = new List<Order>();
                OrderRepository or = new OrderRepository();
                long count= or.ListOrder(DateTime.Parse(From), DateTime.Parse(To), size, Offset, ref o);
                if (o.Count() > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{\"1\":[");
                    foreach (Order obj in o)
                    {
                        sb.Append("{\"Id\":\"" + obj.id + "\"");
                        sb.Append(",\"Product\":\"" + obj.Product + "\"");
                        sb.Append(",\"Quantity\":\"" + obj.Quantity + "\"");
                        sb.Append(",\"ProductAmount\":\"" + obj.ProductAmount + "\"");
                        sb.Append(",\"DiscountAmount\":\"" + obj.DiscountAmount + "\"");
                        sb.Append(",\"Payment\":\"" + obj.Payment + "\"");
                        sb.Append(",\"CreatedOn\":\"" + obj.CreatedOn + "\"");
                        sb.Append(",\"CreatedBy\":\"" + obj.CreatedBy + "\"");
                        sb.Append(",\"AddressId\":\"" + obj.Address.Id + "\"");
                        sb.Append(",\"CustAddress\":\"" + obj.Address.CustAddress + "\"},");
                    }
                    sb.Append("],\"2\":[{\"count\":"+count+"}]}");
                    return Content(sb.ToString());

                }
                else
                {
                    return Content("{\"1\":[],\"2\":[{\"count\":" + count + "}]}");

                }
            }
            catch (Exception ex)
            {
                OrderRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return View();

        }


        [HttpPost]
        public ActionResult PlaceOrder(Order o, string name, string phoneno)
        {
            try
            {
                ValidatedUser();
                if (PopulateCache())
                {
                    if (ModelState.IsValid)
                    {
                        int[] arrProduct = o.ProductAmount.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                        int[] arrProductQantity = o.Quantity.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                        string UserName = string.Empty;
                        if (Request["GUID"] == null) { }
                        else
                        {
                            string guid = Request["GUID"];

                            if (HttpContext.Request.Cookies[guid + "User"] == null) { }
                            else
                            {
                                UserName = HttpContext.Request.Cookies[guid + "User"].Value;
                            }
                        }
                        o.Payment = 0;
                        for (int i = 0; i < arrProduct.Length; i++)
                        {
                            o.Payment += arrProduct[i] * arrProductQantity[i];
                        }
                        o.Payment = o.Payment - o.DiscountAmount;

                        OrderRepository or = new OrderRepository();
                        if (or.PlaceOrder(o, UserName, "CreateOrder"))
                        {
                            return RedirectToAction("PrintOrder", "Order", new { GUID = ViewBag.GUID, p = o.Product, q = o.Quantity, a = o.DiscountAmount + "," + o.Payment, o = o.id + "," + name + "," + phoneno });
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
        public ActionResult PrintOrder(string GUID, string p, string q, string a, string o)
        {
            ValidatedUser();
            if (PopulateCache())
            {
                try
                {
                    int[] arrProductId = p.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                    int[] arrProductQuantity = q.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                    int[] arrProductAmount = a.Split(',').Select(f => Formatting.ConvertNullToInt32(f)).ToArray();
                    string[] arrOrder = o.Split(',').Select(f => Formatting.ConvertNullToString(f)).ToArray();

                    List<Product> prod = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];

                    prod = prod.Where(f => arrProductId.Contains(f.ProductId)).ToList();

                    ViewBag.Product = prod;
                    ViewBag.Quantity = arrProductQuantity;
                    ViewBag.Amount = arrProductAmount;
                    ViewBag.Order = arrOrder;

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
