using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SoDiao.App.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex == null)
                return;
            //if (!(Request is System.Exception) && Request.HttpMethod == "GET")
            //{
            //    if (!(ex is Models.IBaseSoDiaoException))//已知错误,已知错误不记录错误日志
            //    {
            //        Logs.Error(ex);
            //    }
            //    return;
            //}
            //System.Web.HttpContext.Current.Response.StatusCode = 200;
            //System.Web.HttpContext.Current.Response.ContentType = "application/json; charset=utf-8";
            //System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //System.Web.HttpContext.Current.Response.ClearContent();
            //if (ex is Models.IBaseSoDiaoException)//已知错误,已知错误不记录错误日志
            //{
            //    var model = ex as Models.IBaseSoDiaoException;
            //    System.Web.HttpContext.Current.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(model.ErrorModel));
            //}
            //else
            //{
            //    Logs.Error(ex);
            //    System.Web.HttpContext.Current.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new Models.BaseModel() { HasError = 1, ErrorMessage = ex.Message }));
            //}
            //System.Web.HttpContext.Current.Response.End();
            Logs.Error(ex);
        }
    }
}
