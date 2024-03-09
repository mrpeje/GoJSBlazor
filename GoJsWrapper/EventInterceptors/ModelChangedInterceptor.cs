using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.EventInterceptors
{
    internal class ModelChangedInterceptor
    {
        public delegate void ModelChangedHandler(string model);
        public event ModelChangedHandler DiagramModelChanged;
        public event ModelChangedHandler PaletteModelChanged;

        [JSInvokable]
        public void OnDiagramModelChangedEvent(string model)
        {
            DiagramModelChanged?.Invoke(model);
        }
        [JSInvokable]
        public void OnPaletteModelChangedEvent(string model)
        {
            PaletteModelChanged?.Invoke(model);
        }
    }
}
