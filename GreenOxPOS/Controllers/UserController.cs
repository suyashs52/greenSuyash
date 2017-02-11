using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenOxPOS.Models;
using GreenOxPOS.Repository;
namespace GreenOxPOS.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Login(User u)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    u.UserName = u.UserName.Trim();

                    UserRepository ur = new UserRepository();
                    Tuple<bool, bool> t = ur.Credential(u);
                    if (t.Item2 == false) {
                        
                        ModelState.AddModelError("", "Invalid login attempt.");
                     }
                    else
                    {
                        ViewBag.Message = "";
                        if (HttpContext.Cache[u.UserName + "User"] != null)
                            HttpContext.Cache.Remove(u.UserName + "User");

                        string sGUID = System.Guid.NewGuid().ToString();

                        HttpCookie ckGUID = new HttpCookie(sGUID + "User");
                        ckGUID.Value = u.UserName;
                        ckGUID.Expires = DateTime.Now.AddHours(6);
                        HttpContext.Response.Cookies.Add(ckGUID);

                        HttpCookie UserAdmin = new HttpCookie(sGUID + "UserAdmin");
                        UserAdmin.Value = t.Item1.ToString();
                        UserAdmin.Expires = DateTime.Now.AddHours(6);
                        HttpContext.Response.Cookies.Add(UserAdmin);

                       
                        HttpContext.Cache[u.UserName + "User"] = u.UserName;

                        return RedirectToAction("Product", "Product", new { GUID=sGUID});
                    }
                }
                
            }
            catch (Exception ex)
            {
                UserRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return View();

        }
         
        // GET: User/Create
        [HttpPost]
        public ActionResult LogOff()
        {

            if (Request["GUID"] == null) { }
            else
            {
                string guid = Request["GUID"];
                HttpCookie ckGUID = new HttpCookie(guid + "User");

                HttpRuntime.Cache.Remove(ckGUID.Value + "User");

                ckGUID.Expires = DateTime.Now.AddMinutes(1);
                HttpContext.Response.Cookies.Add(ckGUID);

            }
            return RedirectToAction("Product", "Product");
        }
 
    }
}
