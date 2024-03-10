using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.EventInterceptors
{
    public class BlockContextMenuEventsInterceptor
    {
        public delegate void BlockPositionChangedHandler(string id);
        public event BlockPositionChangedHandler ContextHelp;
        public event BlockPositionChangedHandler ContextOpen;
        public event BlockPositionChangedHandler ContextProperties;

        [JSInvokable]
        public void OnContextHelpEvent(string args) => ContextHelp?.Invoke(args);
        [JSInvokable]
        public void OnContextOpenEvent(string args) => ContextOpen?.Invoke(args);
        [JSInvokable]
        public void OnContextPropertiesEvent(string args) => ContextProperties?.Invoke(args);


    }
}
