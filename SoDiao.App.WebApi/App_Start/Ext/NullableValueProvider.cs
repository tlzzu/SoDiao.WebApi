using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SoDiao.App.WebApi
{
    /// <summary>
    /// 处理string空值
    /// </summary>
    public class NullableValueProvider : Newtonsoft.Json.Serialization.IValueProvider
    {
        private readonly object _defaultValue;
        private readonly Newtonsoft.Json.Serialization.IValueProvider _underlyingValueProvider;
        public NullableValueProvider(MemberInfo memberInfo, Type underlyingType)
        {
            _underlyingValueProvider = new DynamicValueProvider(memberInfo);
            //if (underlyingType == typeof(DateTime))
            //    _defaultValue = string.Empty;
            //else 
            _defaultValue = Activator.CreateInstance(underlyingType);
        }
        public object GetValue(object target)
        {
            return _underlyingValueProvider.GetValue(target) ?? _defaultValue;//todo DateTime
        }

        public void SetValue(object target, object value)
        {
            _underlyingValueProvider.SetValue(target, value);
        }
    }
}