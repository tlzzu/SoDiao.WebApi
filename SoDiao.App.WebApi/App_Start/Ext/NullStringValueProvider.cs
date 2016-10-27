using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SoDiao.App.WebApi
{
    /// <summary>
    /// string类型的空值
    /// </summary>
    public class NullStringValueProvider : Newtonsoft.Json.Serialization.IValueProvider
    {
        // private readonly object _defaultValue;
        private readonly Newtonsoft.Json.Serialization.IValueProvider _underlyingValueProvider;
        public NullStringValueProvider(MemberInfo memberInfo)
        {
            _underlyingValueProvider = new DynamicValueProvider(memberInfo);
            //_defaultValue = Activator.CreateInstance(underlyingType);
        }
        public object GetValue(object target)
        {
            return _underlyingValueProvider.GetValue(target) ?? string.Empty;
        }

        public void SetValue(object target, object value)
        {
            _underlyingValueProvider.SetValue(target, value);
        }
    }
}