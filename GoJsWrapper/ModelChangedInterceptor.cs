using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper
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


    public class SelectionChangedEventInterceptor
    {
        public delegate void SelectionChangedHandler(string id);
        public event SelectionChangedHandler LinkSelectionChanged;
        public event SelectionChangedHandler NodeSelectionChanged;
        [JSInvokable]
        public void OnLinkSelectionChangedEvent(string args) => LinkSelectionChanged?.Invoke(args);        

        [JSInvokable]
        public void OnNodeSelectionChangedEvent(string args) => NodeSelectionChanged?.Invoke(args);
    }

}
