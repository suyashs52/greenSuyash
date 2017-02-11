using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenOxPOS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ValidatedUser();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ValidatedUser();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ValidatedUser();
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

            }
            return IsValidUser;
        }
    }
}