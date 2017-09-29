using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeixinDemo.Filters;

namespace WeixinDemo.Filters
{
    public class OAuthFilterAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //判断是否登录授权，AccessToken标识着用户登录，这里面存放的是用户的令牌
            if (filterContext.HttpContext.Session["AccessToken"]!=null) return;

            //开始授权阶段
            //requestUrl代表用户访问的请求地址，需要保存，以便于授权完成后跳到这个地址来
            var requestUrl = filterContext.HttpContext.Request.RawUrl;

            //未登录，所以要跳转到登录页

            var urlHelper=new UrlHelper(filterContext.RequestContext);

            filterContext.Result=new RedirectResult(urlHelper.Action("Login", "OAuth", new { requestUrl }));
        }
    }
}