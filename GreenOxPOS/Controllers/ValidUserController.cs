using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenOxPOS.Repository;
using GreenOxPOS.Models;
namespace GreenOxPOS.Controllers
{
    public class ValidUserController : Controller
    {
        // GET: ValidUser
        public ActionResult Login()
        { 
            try
            {
                if (Request["GUID"] == null)
                {
                    ViewBag.IsAuthenticated = "false";
                }
                else
                {
                    string guid = Request["GUID"];
                    if (HttpContext.Request.Cookies[guid + "User"] == null)
                        ViewBag.IsAuthenticated = "false";
                    else
                    {
                        string ckGUID = HttpContext.Request.Cookies[guid + "User"].Value;
  
                        if (HttpRuntime.Cache[ckGUID + "User"] == null)
                        {
                            ViewBag.IsAuthenticated = "false";
                        }
                        else
                        {
                            ViewBag.IsAuthenticated = "true";
                            ViewBag.UserName = ckGUID;//(string)HttpRuntime.Cache[ckGUID.Value + "User"];
                        }

                    }


                }

            }

            catch (Exception ex)
            {
                ProductRepository.Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }


            return PartialView();// View();
        }

    }
}
