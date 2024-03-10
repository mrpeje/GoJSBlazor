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
        private DotNetObjectReference<BlockPositionChangedEventInterceptor> ReferenceBlockPositionChangedInterceptor;
        private DotNetObjectReference<MouseHoverEventInterceptor> ReferenceMouseHoverEventInterceptor;
        private DotNetObjectReference<UndoRedoEventInterceptor> ReferenceUndoRedoEventInterceptor;
        private ModelChangedInterceptor ModelInterceptor;

        public MouseHoverEventInterceptor MouseHoverEventInterceptor;
        public SelectionChangedEventInterceptor SelectionEventInterceptor;
        public BlockPositionChangedEventInterceptor BlockPositionEventInterceptor;
        public UndoRedoEventInterceptor UndoRedoEventInterceptor;
        public Diagram Diagram;
        public Palette Palette;

        public delegate void DiagramLoadedHandler();
        public event DiagramLoadedHandler DiagramLoaded;

        public GoJsNetWrapper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Diagram = new Diagram(_jsRuntime);
            Palette = new Palette(_jsRuntime);
            ModelInterceptor = new ModelChangedInterceptor();
            SelectionEventInterceptor = new SelectionChangedEventInterceptor();
            BlockPositionEventInterceptor = new BlockPositionChangedEventInterceptor();
            MouseHoverEventInterceptor = new MouseHoverEventInterceptor();
            UndoRedoEventInterceptor = new UndoRedoEventInterceptor();
            ModelInterceptor.DiagramModelChanged += Diagram.UpdateDiagramModel;
            ModelInterceptor.PaletteModelChanged += Palette.UpdatePaletteModel;
        }               

        public async ValueTask InitGoJS()
        {
            SetupMouseHoverEvent();
            await _jsRuntime.InvokeAsync<string>("initDiagram", ReferenceMouseHoverEventInterceptor);
            SetupModelChangedEvent();
            SetupSelectionChangedEvent();
            SetupBlockPositionChangedEvent();
            SetupBlockUndoRedoEvent();
        }
        public async Task LoadDiagram(List<BlockModel> model, List<LinkModel> links, List<BlockModel> palette)
        {
            foreach (var modelBlock in model)
            {
                await Diagram.AddBlock(modelBlock);
            }

            foreach (var link in links)
            {
                await Diagram.AddLink(link);
            }

            foreach (var paletteBlock in palette)
            {
                await Palette.AddBlock(paletteBlock);
            }
            DiagramLoaded?.Invoke();
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
        public void SetupBlockPositionChangedEvent()
        {
            ReferenceBlockPositionChangedInterceptor = DotNetObjectReference.Create(BlockPositionEventInterceptor);
            _jsRuntime.InvokeVoidAsync("subscribeBlockMovedEvent", ReferenceBlockPositionChangedInterceptor);
        }
        public void SetupMouseHoverEvent()
        {
            ReferenceMouseHoverEventInterceptor = DotNetObjectReference.Create(MouseHoverEventInterceptor);

        }
        public void SetupBlockUndoRedoEvent()
        {
            ReferenceUndoRedoEventInterceptor = DotNetObjectReference.Create(UndoRedoEventInterceptor);
            _jsRuntime.InvokeVoidAsync("subscribeUndoRedoEvent", ReferenceUndoRedoEventInterceptor);
        }
        
    }
}
