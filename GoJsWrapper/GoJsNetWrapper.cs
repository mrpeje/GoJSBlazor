using GoJsWrapper.EventInterceptors;
using GoJsWrapper.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Newtonsoft;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json;


namespace GoJsWrapper
{
    public class GoJsNetWrapper : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private string JsonModel { get; set; }
        private DotNetObjectReference<SelectionChangedEventInterceptor> ReferenceSelectionChangedInterceptor;
        private DotNetObjectReference<ModelChangedInterceptor> ReferenceModelInterceptor;
        private ModelChangedInterceptor ModelInterceptor;

        public SelectionChangedEventInterceptor SelectionEventInterceptor;
        public Diagram Diagram;
        public Palette Palette;

        public GoJsNetWrapper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Diagram = new Diagram(_jsRuntime);
            Palette = new Palette(_jsRuntime);
            ModelInterceptor = new ModelChangedInterceptor();
            SelectionEventInterceptor = new SelectionChangedEventInterceptor();
            ModelInterceptor.DiagramModelChanged += Diagram.UpdateDiagramModel;
            ModelInterceptor.PaletteModelChanged += Palette.UpdatePaletteModel;
        }               

        public async ValueTask InitGoJS()
        {
            await _jsRuntime.InvokeAsync<string>("initDiagram");
            SetupModelChangedEvent();
            SetupSelectionChangedEvent();
        }

        public async Task<string> GetModelJson()
        {
            JsonModel = await _jsRuntime.InvokeAsync<string>("getModelJson");
            return JsonModel;
        }
        public async Task SaveDiagram()
        {

        }
        public async Task<string> SelectBlockById(string id)
        {
            return await _jsRuntime.InvokeAsync<string>("SetupDiagram");
        }

        public async Task<string> LoadDiagram()
        {

            return await _jsRuntime.InvokeAsync<string>("loadDiagram");
        }
        public async ValueTask DisposeAsync()
        {

        }
        public void SetupModelChangedEvent()
        {
            ReferenceModelInterceptor = DotNetObjectReference.Create(ModelInterceptor);
            _jsRuntime.InvokeVoidAsync("subscribeModelChangedEvent", ReferenceModelInterceptor);
        }

        public void SetupSelectionChangedEvent()
        {
            ReferenceSelectionChangedInterceptor = DotNetObjectReference.Create(SelectionEventInterceptor);
            _jsRuntime.InvokeVoidAsync("subscribeSelectionChangedEventListener", ReferenceSelectionChangedInterceptor);
        }
    }
}
