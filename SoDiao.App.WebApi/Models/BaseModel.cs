using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoDiao.App.WebApi.Models
{
    public class BaseModel:IBaseModel
    {
        public int HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}