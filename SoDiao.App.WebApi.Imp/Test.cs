using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDiao.App.WebApi.Imp
{
    public class Test : SoDiao.App.WebApi.Interface.ITest
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
