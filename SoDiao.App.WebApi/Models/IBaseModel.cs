using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoDiao.App.WebApi.Models
{
    public interface IBaseModel
    {
        /// <summary>
        /// 是否有错误，大于0就是有错误，等于0就是没错误
        /// </summary>
        int HasError { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrorMessage { get; set; }
    }
}