using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Xml;

namespace SoDiao.App.WebApi
{
    /// <summary>
    /// 权限过滤
    /// appid secret timestamp
    /// </summary>
    public class AuthenticationHandler : System.Net.Http.DelegatingHandler
    {
        /// <summary>
        /// 获取所有AppIDSecret
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IDictionary<string, string> GetAllAppIDSecret(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode root = doc.DocumentElement;
            //获取节点列表
            XmlNodeList items = root.ChildNodes;
            IDictionary<string, string> dic = new Dictionary<string, string>();
            foreach (XmlNode item in items)
            {
                var AppID = item.Attributes["AppID"].InnerText;
                var Secret = item.Attributes["Secret"].InnerText;
                if (!string.IsNullOrWhiteSpace(AppID) && !string.IsNullOrWhiteSpace(Secret) && !dic.ContainsKey(AppID))
                    dic.Add(AppID, Secret);
            }
            dic.Add("", "");//从数据库中读取
            return dic;
        }

        /// <summary>
        /// appkey和secret的数据字典
        /// </summary>
        public static IDictionary<string, string> _AppIDSecret
        {
            get
            {
                IDictionary<string, string> dic = FileCacheHelper.GetCache<IDictionary<string, string>>("_AppIDSecret");
                if (dic == null)
                {
                    try
                    {
                        var path = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath), "App_Data\\AppIDSecret.xml");
                        dic = GetAllAppIDSecret(path);
                        if (dic == null)
                            dic = new Dictionary<string, string>();
                        FileCacheHelper.SetCache("_AppIDSecret", dic, path);
                    }
                    catch (Exception ex)
                    {
                        Logs.Error(ex);
                        dic = new Dictionary<string, string>();
                    }
                }
                return dic;
            }
        }



        public string FindSecret(string appID)
        {
            string sec = string.Empty;
            _AppIDSecret.TryGetValue(appID, out sec);
            return sec;
        }


        ///// <summary>
        ///// 是否不过滤
        ///// </summary>
        ///// <param name="dic"></param>
        ///// <returns></returns>
        //private bool IsUnAuthentication(IDictionary<string, object> dic)
        //{
        //    object controller;
        //    if (!dic.TryGetValue("controller", out controller))
        //        return false;
        //    object action;
        //    if (!dic.TryGetValue("action", out action))
        //        return false;
        //    IList<string> unActions;
        //    if (!_auth.UnFilter.TryGetValue(controller.ToString(), out unActions))
        //        return false;
        //    if (unActions == null || unActions.Count <= 0)
        //        return false;
        //    return unActions.Contains(action.ToString());
        //}
        ///// <summary>
        ///// 无权查看信息
        ///// </summary>
        ///// <returns></returns>
        //public static System.Threading.Tasks.Task<HttpResponseMessage> ReturnError()
        //{
        //    var response = new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new StringContent("资格不够，拒绝应答！")
        //    };
        //    var tsc = new TaskCompletionSource<HttpResponseMessage>();
        //    tsc.SetResult(response);
        //    return tsc.Task;
        //}



        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            //var routeData = request.GetRouteData().Values;
            //if (IsUnAuthentication(routeData))//如果不用过滤
            //    return base.SendAsync(request, cancellationToken);
            try
            {
                IEnumerable<string> AppIDs;
                if (!request.Headers.TryGetValues("appid", out AppIDs))
                    return base.SendAsync(request, cancellationToken);
                IEnumerable<string> Secrets;
                if (!request.Headers.TryGetValues("secret", out Secrets))
                    return base.SendAsync(request, cancellationToken);
                IEnumerable<string> Timestamps;
                if (!request.Headers.TryGetValues("timestamp", out Timestamps))//20150528093456
                    return base.SendAsync(request, cancellationToken);
                if (AppIDs == null || Secrets == null || Timestamps == null || string.IsNullOrWhiteSpace(AppIDs.FirstOrDefault()) || string.IsNullOrWhiteSpace(Secrets.FirstOrDefault()) || string.IsNullOrWhiteSpace(Timestamps.FirstOrDefault()))
                    return base.SendAsync(request, cancellationToken);
                DateTime dt;
                if (!DateTime.TryParseExact(Timestamps.FirstOrDefault(), "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dt))
                    return base.SendAsync(request, cancellationToken);
                var ExpiredTime = System.Configuration.ConfigurationSettings.AppSettings["ExpiredTime"].ToString();//权限过期时间
                if (Math.Abs((DateTime.Now.ToUniversalTime() - dt).TotalMinutes) > Convert.ToInt32(ExpiredTime))//前后时间相差不能超过15分钟
                    return base.SendAsync(request, cancellationToken);
                string appid = AppIDs.FirstOrDefault();
                string sec = FindSecret(appid);//获取明文secret
                if (string.IsNullOrWhiteSpace(sec))
                    return base.SendAsync(request, cancellationToken);
                string secret = string.Format("{0}{1}{2}", appid, sec, Timestamps.FirstOrDefault());
                secret = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(secret, "MD5");//md5加密（大写）
                if (secret != Secrets.FirstOrDefault())
                    return base.SendAsync(request, cancellationToken);

                //var appid = "123";

                if (!string.IsNullOrWhiteSpace(appid))//set user
                {
                    Thread.CurrentPrincipal = HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(appid), null);
                }
                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
                return base.SendAsync(request, cancellationToken);
            }
        }

    }
}