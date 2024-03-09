using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperTest2
{
    internal interface IJsEventHandler
    {
        [JSInvokable]
        public Object Handaler();
    }
}
