using GoJsWrapper.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Newtonsoft;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json;


namespace GoJsWrapper
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class GoJsNetWrapper : IAsyncDisposable
    {
        public DiagramModel Model;
        private DotNetObjectReference<ModelChangedInterceptor> ModelHelperRef;
        private ModelChangedInterceptor ModelHelper;
        public SelectionChangedEventInterceptor SelectionEventInterceptor;
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly IJSRuntime _jsRuntime;
        private string JsonModel { get; set; }
        List<UnitCategoryModel> Blocks { get; set; }

        private DotNetObjectReference<CustomEventHelper> Reference;
        private DotNetObjectReference<SelectionChangedEventInterceptor> ReferenceSelectionChangedInterceptor;


        public GoJsNetWrapper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Model = new DiagramModel(_jsRuntime);
            ModelHelper = new ModelChangedInterceptor();
            SelectionEventInterceptor = new SelectionChangedEventInterceptor();
            ModelHelper.ModelChanged += Model.UpdateDiagramModel;

        }        
        

        public async Task AddBlockToPallete(List<UnitCategoryModel> BlockTypes)
        {
            var BlockTypesJson = JsonConvert.SerializeObject(BlockTypes);
            await _jsRuntime.InvokeAsync<string>("initPallete", BlockTypesJson);
        }

        public async ValueTask InitGoJS(List<UnitCategoryModel> BlockTypes = null)
        {
            var BlockTypesJson = JsonConvert.SerializeObject(BlockTypes);
            await _jsRuntime.InvokeAsync<string>("initDiagram", BlockTypesJson);
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
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
        public void SetupModelChangedEvent()
        {
            ModelHelperRef = DotNetObjectReference.Create(ModelHelper);
            _jsRuntime.InvokeVoidAsync("subscribeModelChangedEvent", ModelHelperRef);
        }


        public void SetupSelectionChangedEvent()
        {
            ReferenceSelectionChangedInterceptor = DotNetObjectReference.Create(SelectionEventInterceptor);
            _jsRuntime.InvokeVoidAsync("subscribeSelectionChangedEventListener", ReferenceSelectionChangedInterceptor);
        }
    }
}
