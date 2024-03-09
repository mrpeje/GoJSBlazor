using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.EventInterceptors
{
    public class BlockPositionChangedEventInterceptor
    {
        public delegate void BlockPositionChangedHandler(string id);
        public event BlockPositionChangedHandler BlockPositionChanged;

        [JSInvokable]
        public void OnBlockPositionChangedEvent(string args) => BlockPositionChanged?.Invoke(args);


    }
}
