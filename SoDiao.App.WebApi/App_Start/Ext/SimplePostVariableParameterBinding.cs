using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace SoDiao.App.WebApi
{
    /// <summary>
    /// 转换POST传递过来的参数值
    /// </summary>
    public class SimplePostVariableParameterBinding : HttpParameterBinding
    {
        private const string MultipleBodyParameters = "MultipleBodyParameters";

        public SimplePostVariableParameterBinding(HttpParameterDescriptor descriptor) : base(descriptor) { }

        /// <summary>
        /// Check for simple binding parameters in POST data. Bind POST
        /// data as well as query string data
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            string stringValue = null;
            try
            {
                NameValueCollection col = TryReadBody(actionContext.Request);
                if (col != null)
                    stringValue = col[Descriptor.ParameterName];
                // try reading query string if we have no POST/PUT match
                if (stringValue == null)
                {
                    var query = actionContext.Request.GetQueryNameValuePairs();
                    if (query != null)
                    {
                        var matches = query.Where(kv => kv.Key.ToLower() == Descriptor.ParameterName.ToLower());
                        if (matches.Count() > 0)
                            stringValue = matches.First().Value;
                    }
                }
                object value = StringToType(stringValue);

                // Set the binding result here 给字段挨个赋值
                SetValue(actionContext, value);

                // now, we can return a completed task with no result
                TaskCompletionSource<AsyncVoid> tcs = new TaskCompletionSource<AsyncVoid>();
                tcs.SetResult(default(AsyncVoid));
                return tcs.Task;
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
                return null;
            }
        }


        /// <summary>
        /// Method that implements parameter binding hookup to the global configuration object's
        /// ParameterBindingRules collection delegate.
        /// 
        /// This routine filters based on POST/PUT method status and simple parameter
        /// types.
        /// </summary>
        /// <example>
        /// GlobalConfiguration.Configuration.
        ///       .ParameterBindingRules
        ///       .Insert(0,SimplePostVariableParameterBinding.HookupParameterBinding);
        /// </example>    
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static HttpParameterBinding HookupParameterBinding(HttpParameterDescriptor descriptor)
        {
            try
            {
                var supportedMethods = descriptor.ActionDescriptor.SupportedHttpMethods;
                // Only apply this binder on POST operations
                if (supportedMethods.Contains(HttpMethod.Post))
                {
                    var supportedTypes = new Type[] { typeof(string), 
                                                typeof(int), 
                                                typeof(int?), 
                                                typeof(long),
                                                typeof(long?),
                                                typeof(decimal), 
                                                typeof(double), 
                                                typeof(decimal?), 
                                                typeof(double?), 
                                                typeof(bool),
                                                typeof(DateTime),
                                                typeof(DateTime?),
                                                typeof(byte[])
                                            };
                    if (supportedTypes.Where(typ => typ == descriptor.ParameterType).Count() > 0)
                        return new SimplePostVariableParameterBinding(descriptor);
                }
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
            }
            return null;
        }


        private object StringToType(string stringValue)
        {
            object value = null;
            try
            {
                //if (stringValue == null)
                //    value = null;
                //else 
                if (Descriptor.ParameterType == typeof(string))
                    value = stringValue??string.Empty;
                else if (Descriptor.ParameterType == typeof(int))
                    value =stringValue==null?0:int.Parse(stringValue, CultureInfo.CurrentCulture);
                else if (Descriptor.ParameterType == typeof(Int32))
                    value = stringValue==null?0:Int32.Parse(stringValue, CultureInfo.CurrentCulture);
                else if (Descriptor.ParameterType == typeof(Int64))
                    value = stringValue == null ? 0 : Int64.Parse(stringValue, CultureInfo.CurrentCulture);
                else if (Descriptor.ParameterType == typeof(decimal))
                    value = stringValue == null ? 0 : decimal.Parse(stringValue, CultureInfo.CurrentCulture);
                else if (Descriptor.ParameterType == typeof(double))
                    value = stringValue == null ? 0 : double.Parse(stringValue, CultureInfo.CurrentCulture);
                else if (Descriptor.ParameterType == typeof(DateTime))
                {
                    if (stringValue == null)
                        throw new NullFieldException(string.Format("类型为DateTime的{0}字段为null！", Descriptor.ParameterName));
                    value = DateTime.Parse(stringValue, CultureInfo.CurrentCulture);
                }
                else if (Descriptor.ParameterType == typeof(byte[]))
                    value = stringValue == null ? Descriptor .DefaultValue: byte.Parse(stringValue, CultureInfo.CurrentCulture);
                else if (Descriptor.ParameterType == typeof(bool))
                {
                    if (stringValue == null) {
                        throw new NullFieldException(string.Format("类型为DateTime的{0}字段为null！", Descriptor.ParameterName));
                    }
                    value = false;
                    if (stringValue == "true" || stringValue == "on" || stringValue == "1")
                        value = true;
                }
                else
                    value = stringValue;
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
            }
            return value;
        }

        /// <summary>
        /// Read and cache the request body
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private NameValueCollection TryReadBody(HttpRequestMessage request)
        {
            object result = null;
            try
            {
                if (!request.Properties.TryGetValue(MultipleBodyParameters, out result))
                {
                    var contentType = request.Content.Headers.ContentType.MediaType.ToLower();
                    if (contentType == null)
                    {
                        result = null;
                    }
                    else if (contentType.Contains("application/x-www-form-urlencoded"))
                    {
                        result = request.Content.ReadAsFormDataAsync().Result;
                    }
                    else if (contentType.Contains("application/json"))//解决json问题
                    {
                        var jsonStr = request.Content.ReadAsStringAsync().Result;//{"Name":"tongl","Age":22}
                        var json = JsonConvert.DeserializeObject<IDictionary<string, string>>(jsonStr);
                        if (json != null || json.Count > 0)
                        {
                            var nvc = new NameValueCollection();
                            foreach (var item in json)
                            {
                                nvc.Add(item.Key, item.Value);
                            }
                            result = nvc;
                        }
                    }
                    else
                    {
                        result = null;
                    }
                    request.Properties.Add(MultipleBodyParameters, result);
                }
            }
            catch (Exception ex)
            {
                Logs.Error(ex);
            }
            return result as NameValueCollection;
        }

        private struct AsyncVoid
        {
        }
    }
}