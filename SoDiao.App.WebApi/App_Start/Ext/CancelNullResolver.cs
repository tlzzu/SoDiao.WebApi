using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SoDiao.App.WebApi
{
    /// <summary>
    /// 取消Json.NET空值
    /// </summary>
    public class CancelNullResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override Newtonsoft.Json.Serialization.IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                var pi = (PropertyInfo)member;
                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return new NullableValueProvider(member, pi.PropertyType.GetGenericArguments().First());
                }
                else if (pi.PropertyType == typeof(string))
                {
                    return new NullStringValueProvider(member);
                }
                //else if(pi.PropertyType.GetGenericTypeDefinition()==typeof(Nullable<>)&&pi.PropertyType.is)
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                var fi = (FieldInfo)member;
                if (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return new NullableValueProvider(member, fi.FieldType.GetGenericArguments().First());
                else if (fi.FieldType == typeof(string))
                {
                    return new NullStringValueProvider(member);
                }
            }
            return base.CreateMemberValueProvider(member);
        }
    }


}