using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoDiao.App.WebApi.Controllers
{
    
    public class DemoController : ApiController
    {
        Interface.ITest _test;
        public DemoController(Interface.ITest test) {
            this._test = test;
        }
        /// <summary>
        /// 根据姓名和年龄获取所有用户名称
        /// </summary>
        /// <param name="Name">姓名</param>
        /// <param name="Age">年龄</param>
        /// <returns>所有用户名称</returns>
        [HttpPost]
        
        [Authorize]//权限认证
        public IEnumerable<string> GetPeople(string Name, int Age)
        {
            var aa=_test.Add(1, 1);
            throw new NullFieldException("test错误！");
            return new string[] { "tongl", "tongling" };
        }

    }
}
