using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.EventInterceptors
{
    public class MouseHoverEventInterceptor
    {
        public delegate void MouseHoverHandler(string id);
        public event MouseHoverHandler NodeMouseHover;
        public event MouseHoverHandler LinkMouseHover;

        [JSInvokable]
        public void OnNodeMouseHoverEvent(string args) => NodeMouseHover?.Invoke(args);
        
        [JSInvokable]
        public void OnLinkMouseHoverEvent(string args) => LinkMouseHover?.Invoke(args);

    }
}
