using GreenOxPOS.Models;
using GreenOxPOS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenOxPOS.Controllers
{
    public class ProductController : Controller
    {

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Product()
        {
            ValidatedUser();
            if (PopulateCache())
            {
                ModelState.Clear();
                List<Product> p = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];
                p = p.Where(f => f.IsActive == true).ToList();
                return View(p);
            }
            else return RedirectToAction("AddProduct", new { GUID = ViewBag.GUID });

        }

        public ActionResult ListProduct(int id)
        {

            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    ModelState.Clear();
                    List<Product> p = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];
                    // var pc = p.Where(f => f.ProductCategory.PCId == id).Select(s => new { s.ProductCategory.PCId,s.ProductCategory.Name}).ToArray();

                    // ViewBag.PC = pc;

                    return View(p.Where(f => f.ProductCategory.PCId == id).ToList<Product>());
                }
                else return RedirectToAction("AddProduct");//View("AddProduct");

            }
            else
            {
                ModelState.AddModelError("Denied", "Access Denied to View/Edit All Product");
                return View();
            }


        }

        public ActionResult AddProduct()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    ModelState.Clear();

                    List<ProductCategory> pc = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];
                    pc = pc.Where(f => f.IsActive == true).ToList();

                    foreach (var i in pc)
                    {
                        items.Add(new SelectListItem { Text = i.Name, Value = i.PCId.ToString() });
                    }
                    if (pc.Count() == 0) return RedirectToAction("AddProductCategory", new { id = 0, GUID = ViewBag.GUID });

                }
            }
            ViewBag.PC = items;


            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Product p, string PC)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ProductRepository pr = new ProductRepository();
                    int PCId = Formatting.ConvertNullToInt32(PC);
                    ProductCategory pc = null;
                    p.ProductCategory = new ProductCategory();
                    p.ProductCategory.PCId = PCId;

                    if (pr.Product(p, "ADDPRODUCT"))
                    {

                        List<Product> lp = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];
                        pc = null;
                        foreach (var item in lp)
                        {
                            if (item.ProductCategory.PCId == p.ProductCategory.PCId)
                            {
                                pc = item.ProductCategory;
                                break;
                            }
                        }

                        if (pc == null)
                        {

                            List<ProductCategory> lpc = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];
                            pc = lpc.Where(f => f.PCId == PCId).SingleOrDefault();
                        }

                        p.ProductCategory = pc;

                        lp.Add(p);
                    }
                    return RedirectToAction("ListProduct", new { id = PCId, GUID = Request["GUID"] });

                }
            }
            catch (Exception ex)
            {
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return View();
        }


        public ActionResult EditProduct(int id, Product p)
        {
            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    ModelState.Clear();

                    p.ProductCategory = new ProductCategory() { PCId = id };

                    List<ProductCategory> pc = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];

                    List<SelectListItem> items = new List<SelectListItem>();

                    foreach (var i in pc)
                    {
                        if (i.PCId == id)
                            items.Add(new SelectListItem { Text = i.Name, Value = i.PCId.ToString(), Selected = true });
                        else
                            items.Add(new SelectListItem { Text = i.Name, Value = i.PCId.ToString() });
                    }

                    ViewBag.PC = items;

                    return View(p);
                }
            }

            else
            {
                ModelState.AddModelError("Denied", "Access Denied to View/Edit Product");

            }
            return View(p);
        }
        [HttpPost]
        public ActionResult EditProduct(int id, string PC, Product p)
        {
            if (ModelState.IsValid)
            {
                ProductRepository pr = new ProductRepository();
                p.ProductCategory = new ProductCategory();
                p.ProductCategory.PCId = Formatting.ConvertNullToInt32(PC);
                if (pr.Product(p, "EDITPRODUCT"))
                {

                    List<Product> lp = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];

                    ProductCategory pc = null;

                    foreach (var item in lp)
                    {
                        if (item.ProductCategory.PCId == p.ProductCategory.PCId)
                        {
                            pc = item.ProductCategory;
                            break;
                        }
                    }

                    //cache value reset
                    foreach (var item in lp)
                    {
                        if (item.ProductId == p.ProductId)
                        {
                            item.Price = p.Price;
                            item.ProductName = p.ProductName;
                            item.IsActive = p.IsActive;
                            if (item.ProductCategory.PCId != p.ProductCategory.PCId)
                            {
                                item.ProductCategory = pc;
                            }
                            break;
                        }
                    }
                }
                //return RedirectToAction("EditProduct", new { id = p.ProductCategory.PCId, ProductId = p.ProductId, ProductName = p.ProductName, Price = p.Price, IsActive = p.IsActive, GUID = Request["GUID"] });
                return RedirectToAction("ListProduct", new { id = p.ProductCategory.PCId, GUID = Request["GUID"] });

            }
            return View();
        }

        public ActionResult DeleteProduct(int id, Product p)
        {
            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    ModelState.Clear();

                    p.ProductCategory = new ProductCategory() { PCId = id };

                    List<ProductCategory> pc = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];

                    List<SelectListItem> items = new List<SelectListItem>();

                    foreach (var i in pc)
                    {
                        if (i.PCId == id)
                        {
                            items.Add(new SelectListItem { Text = i.Name, Value = i.PCId.ToString(), Selected = true });
                            break;
                        }
                    }

                    ViewBag.PC = items;

                    return View(p);
                }
            }


            return View(p);
        }
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                ProductRepository pr = new ProductRepository();
                Product p = new Product();
                p.ProductId = id;
                p.ProductCategory = new ProductCategory();
                p.ProductCategory.PCId = id;
                if (pr.Product(p, "DELPRODUCT"))
                {
                    ViewBag.AlertMsg = "Product  deleted successfully";

                    List<Product> lp = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];

                    foreach (var item in lp)
                    {
                        if (item.ProductCategory.PCId == id)
                        {
                            lp.Remove(item);
                            break;
                        }
                    }

                }
                return RedirectToAction("Product", new { GUID = Request["GUID"] });


            }
            catch (Exception ex)
            {
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return RedirectToAction("Product", new { GUID = Request["GUID"] });
        }


        public ActionResult ProductCategory(int id)
        {
            ValidatedUser();
            if (PopulateCache())
            {
                ModelState.Clear();
                List<ProductCategory> p = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];
                p = p.Where(f => f.IsActive == true).ToList();
                ViewBag.Id = id;
                return View(p);
            }
            else return RedirectToAction("AddProductCategory", new { id = id });


        }

        public ActionResult AddProductCategory(int id)
        {
            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    ModelState.Clear();

                    ViewBag.Id = id;

                    return View();
                }
            }
            return RedirectToAction("ListProduct", new { id = id, GUID = Request["GUID"] });

        }
        [HttpPost]
        public ActionResult AddProductCategory(ProductCategory p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ProductRepository pr = new ProductRepository();
                    Product pro = new Product();
                    pro.ProductName = p.Name;
                    pro.ProductId = 0;
                    pro.ProductCategory = p;
                    if (pr.Product(pro, "ADDPRODUCTCAT"))
                    {
                        List<ProductCategory> pc = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];
                        p.PCId = pro.ProductId;
                        pc.Add(p);
                        return RedirectToAction("ProductCategory", new { id = pro.ProductId, GUID = Request["GUID"] });
                    }


                    return RedirectToAction("ProductCategory", new { id = p.PCId, GUID = Request["GUID"], Messages = pro.ProductName });

                }

            }
            catch (Exception ex)
            {
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return View();
        }


        public ActionResult EditProductCategory(ProductCategory p)
        {
            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    return View(p);
                }
            }
            return RedirectToAction("ListProduct", new { id = p.PCId });
        }
        [HttpPost]
        public ActionResult EditProductCategory(int id, ProductCategory p)
        {
            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    if (ModelState.IsValid)
                    {

                        ProductRepository pr = new ProductRepository();
                        Product pro = new Product();
                        pro.ProductName = p.Name;
                        pro.ProductId = p.PCId;
                        pro.ProductCategory = p;
                        pro.IsActive = p.IsActive;
                        if (pr.Product(pro, "EDITPRODUCTCAT"))
                        {

                            List<Product> lp = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];

                            ProductCategory pc = p;

                            foreach (var item in lp)
                            {
                                if (item.ProductCategory.PCId == p.PCId)
                                {
                                    item.ProductCategory.Name = p.Name;
                                    item.ProductCategory.IsActive = p.IsActive;

                                    break;

                                }
                            }

                            List<ProductCategory> lpc = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];

                            foreach (var item in lpc)
                            {
                                if (item.PCId == p.PCId)
                                {
                                    item.Name = p.Name;
                                    item.IsActive = p.IsActive;

                                    break;

                                }
                            }
                        }
                        return RedirectToAction("ProductCategory", new { id = p.PCId, GUID = Request["GUID"] });

                    }
                }
            }

            else
            {
                ModelState.AddModelError("Denied", "Access Denied to View/Edit Product");

            }
            return View();
        }

        public ActionResult DeleteProductCategory(ProductCategory p)
        {
            if (ValidatedUser())
            {
                if (PopulateCache())
                {
                    return View(p);
                }
            }
            return RedirectToAction("Product", new { Guid = ViewBag.GUID });
        }
        [HttpPost]
        public ActionResult DeleteProductCategory(int id)
        {
            try
            {
                ProductRepository pr = new ProductRepository();
                Product p = new Product();
                p.ProductId = id;
                p.ProductName = string.Empty;
                p.ProductCategory = new ProductCategory();
                if (pr.Product(p, "DELPRODUCTCAT"))
                {
                    ViewBag.AlertMsg = "Product Category deleted successfully";
                    List<Product> lp = (List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()];

                    foreach (var item in lp)
                    {
                        if (item.ProductCategory.PCId == id)
                        {
                            lp.Remove(item);

                        }
                    }

                    List<ProductCategory> lpc = (List<ProductCategory>)HttpRuntime.Cache[Enums.Cache.ProductCategory.ToString()];

                    foreach (var item in lpc)
                    {
                        if (item.PCId == id)
                        {
                            lpc.Remove(item);

                            break;

                        }
                    }

                }

                return RedirectToAction("ProductCategory", new { id = id, GUID = Request["GUID"] });

            }
            catch (Exception ex)
            {
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return View();
        }



        public ActionResult ResetPopulateCache()
        {
            if (ValidatedUser() && PopulateCache())
            {
                if (ResetCache())
                {
                    ModelState.AddModelError("Success", "Reset Successfull!");
                    return RedirectToAction("Product", new { GUID = Request["GUID"] });

                }
                else
                {
                    ModelState.AddModelError("Error", "Reset Failure!");

                }
            }
            return View();
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
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }

        public bool ResetCache()
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
                else if (((List<Product>)HttpRuntime.Cache[Enums.Cache.Product.ToString()]).Count() > 0)
                {
                    HttpRuntime.Cache.Remove(Enums.Cache.Product.ToString());
                    HttpRuntime.Cache.Remove(Enums.Cache.ProductCategory.ToString());

                    ProductRepository pr = new ProductRepository();
                    List<Product> p = pr.GetAllProduct("VIEWPRODUCT");
                    HttpRuntime.Cache.Insert(Enums.Cache.Product.ToString(), p, null, DateTime.MaxValue, TimeSpan.FromHours(6));

                    List<ProductCategory> pc = pr.GetAllProductCategory("VIEWPRODUCTCAT");
                    HttpRuntime.Cache.Insert(Enums.Cache.ProductCategory.ToString(), pc, null, DateTime.MaxValue, TimeSpan.FromHours(6));


                    return true;
                }
                else return false;

            }
            catch (Exception ex)
            {
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }
    }
}
