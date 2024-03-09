using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperTest2
{
    internal class ModelChangedEventHandler : IJsEventHandler
    {
        private readonly ExampleJsInterop interop;
        public ModelChangedEventHandler(ExampleJsInterop interop)
        {
            this.interop = interop;
        }

        public object Handaler()
        {
            throw new NotImplementedException();
        }
    }
}
