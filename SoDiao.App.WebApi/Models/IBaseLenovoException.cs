using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDiao.App.WebApi.Models
{
    interface IBaseSoDiaoException
    {
        IBaseModel ErrorModel { get; set; }
    }
}
