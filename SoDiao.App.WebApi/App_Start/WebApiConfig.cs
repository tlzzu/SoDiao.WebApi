using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SoDiao.App.WebApi
{

   
    public static class WebApiConfig
    {
        

        public static void Register(HttpConfiguration config)
        {
            
            // Web API configuration and services
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",//让webapi根据action来选择相应的方法，而不是采用webapi自带的由HttpMethod
                defaults: new { id = RouteParameter.Optional }
            );
            //权限过滤
            config.MessageHandlers.Add(new AuthenticationHandler());
            //通用错误处理过滤器，保证错误信息都是以{HasError:1,ErrorMessage:""}的格式
            config.Filters.Add(new GeneralExceptionFilterAttribute());
            //转换POST传递过来的参数值
            config.ParameterBindingRules.Insert(0, SimplePostVariableParameterBinding.HookupParameterBinding);

            #region 格式化json日期
            System.Net.Http.Formatting.JsonMediaTypeFormatter jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            jsonFormatter.SerializerSettings.ContractResolver = new CancelNullResolver();//取消json空值
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;  //树结构无法格式化的问题
            #endregion

            ////解决RestSharp请求https匿名证书
            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            
            //让所有的接口都支持跨域
            GlobalConfiguration.Configuration.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            #region Ioc Autofac
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.Load(ConfigurationSettings.AppSettings["LoadAssembly"]))
                .Where(t => true)
                .AsImplementedInterfaces();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.EnsureInitialized();
            #endregion

            #region 系统日志
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(System.Web.HttpContext.Current.Server.MapPath("~") + @"\log4net.config"));
            #endregion
            
        }
    }
}
