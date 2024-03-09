using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperTest2
{
    internal class Helper
    {
        public string Model { get; set; }

        [JSInvokable]
        public void SayHello(string model)
        {
            Model = model;
        }
    }
}
