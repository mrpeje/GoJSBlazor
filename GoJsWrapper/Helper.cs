using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper
{
    public delegate void LinkSelectionChangedEvent(string model);
    internal class Helper
    {       
        public event LinkSelectionChangedEvent ModelChanged;

        [JSInvokable]
        public void UpdateModel(string model)
        {
            ModelChanged?.Invoke(model);
        }
    }

    public class CustomEventHelper
    {
        private readonly Func<string, Task> _callback;
        public CustomEventHelper(Func<string, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnCustomEvent(string args) => _callback(args);
    }

    public class NodeSelectionChangedEventHelper
    {
        public delegate void SelectionChangedHandler(string id);
        public event SelectionChangedHandler NodeSelectionChanged;

        [JSInvokable]
        public void OnNodeSelectionChangedEvent(string args) => NodeSelectionChanged?.Invoke(args);
    }
    public class LinkSelectionChangedEventHelper
    {
        public delegate void SelectionChangedHandler(string id);
        public event LinkSelectionChangedEvent LinkSelectionChanged;

        [JSInvokable]
        public void OnLinkSelectionChangedEvent(string args) => LinkSelectionChanged?.Invoke(args);
    }

}
