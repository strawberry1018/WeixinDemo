using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using  WeixinDemo.Filters;

namespace WeixinDemo.Controllers
{
    [OAuthFilter]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var userinfo = Session["userinfo"] as OAuthUserInfo;
            return View(userinfo);
        }

        public ActionResult Share()
        {
            return View();
        }
    }
}