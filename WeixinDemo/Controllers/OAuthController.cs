using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using  Senparc.Weixin.MP.Helpers;

namespace WeixinDemo.Controllers
{
    public class OAuthController : Controller
    {
        private string _appId = "wxe613d113ee8f60f4";
        private string _appsecret = "ff999e5a1d8c7b4a5a5737c1eb817284";
        private string _domain = "http://wx.creazy.shop";

        public ActionResult Login(string requestUrl)
        {
            //生成一个回调url，供微信授权后，返回给我们信息的接受地址
            var redirectUrl = $"{_domain}{Url.Action("CallBack", new {requestUrl})}";

            //生产一个验证码
            var state = "wx" + DateTime.Now.Millisecond;
            Session["state"] = state;
            //生成微信授权的压面url
            var url = OAuthApi.GetAuthorizeUrl(_appId,redirectUrl,state,OAuthScope.snsapi_base);
            return Redirect(url);
        }

        /// <summary>
        /// 这个是微信授权完成所执行的回调方法
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public ActionResult CallBack(string code, string state, string requestUrl)
        {
            if (state != (string)Session["state"])
            {
                return Content("非法访问");
            }
            //判断获取成功
            if (string.IsNullOrEmpty(code))
            {
                return Content("授权失败");
            }
            var oAuthAccessToken = OAuthApi.GetAccessToken(_appId, _appsecret, code);
            if (oAuthAccessToken.errcode != ReturnCode.请求成功)
            {
                return Content("授权失败！");
            }
            //获取令牌成功，就说明我们已经登录
            Session["AccessToken"] = oAuthAccessToken;
            //尝试获取用户信息，如果能获取到，就说明我们这个令牌是有权限的，如果没有获取到，令牌就没有权限
            //但是不管令牌是否有权限，openid都是一样的

            try
            {
                //生成一个回调url，供微信授权完成后，返回给我们的接受地址
                var userInfo = OAuthApi.GetUserInfo(oAuthAccessToken.access_token, oAuthAccessToken.openid);
                //如果不为空 就说明获取成功 同事也说明 我们的令牌是有权限的 
                Session["userinfo"] = userInfo;
                //重定向到用户一开始请求的页面地址
                return Redirect(requestUrl);
            }
            catch (Exception)
            {
                //生城一个回调url 供微信完成授权后 返回给我们信息的接受地址
                var redirectUrl = $"{_domain}{Url.Action("CallBack", new {requestUrl})}";
                //生成微信用户授权页面url
                var url = OAuthApi.GetAuthorizeUrl(_appId, requestUrl, state, OAuthScope.snsapi_userinfo);
                return Redirect(url);
            }

            

        }

        public ActionResult JsSdkConfig()
        {
            //生成需要注册的完整的url，包含前缀和域名
            var url = _domain + Request.RawUrl;
            //获取js sdk的配置信息
            var config = JSSDKHelper.GetJsSdkUiPackage(_appId, _appsecret, url);
            return PartialView(config);
        }
    }
}