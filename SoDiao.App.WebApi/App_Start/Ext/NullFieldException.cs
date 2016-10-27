using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoDiao.App.WebApi
{
    /// <summary>
    /// 字段空异常--这是我们不愿发生的错误！
    /// </summary>
    public class NullFieldException:System.Exception,Models.IBaseSoDiaoException
    {
        //public NullFieldException() { }
        public NullFieldException(string message)  {
            this.ErrorModel = new Models.BaseModel() { HasError = 1, ErrorMessage = message };
        }

        public Models.IBaseModel ErrorModel { get; set; }
    }
}