using Microsoft.JSInterop;
using System.Text.Json.Nodes;
using WrapperTest2.Models;

namespace WrapperTest2
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class ExampleJsInterop : IAsyncDisposable
    {
        private DotNetObjectReference<Helper> ModelHelperRef;
        private Helper ModelHelper;
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly IJSRuntime _jsRuntime;
        private string JsonModel {  get; set; }
        List<UnitModel> Blocks { get; set; }
        public ExampleJsInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            ModelHelper = new Helper();

        }
        public async ValueTask InitGoJS()
        {
            await _jsRuntime.InvokeAsync<string>("initDiagram");
            SubscribeModelChangedEven();
        }
        
        public async Task<string> GetModelJson()
        {
            JsonModel = await _jsRuntime.InvokeAsync<string>("getModelJson");
            return JsonModel;
        }
        public async Task<string> SaveDiagram(string jsonModel)
        {

            JsonModel =  await _jsRuntime.InvokeAsync<string>("save");
            return JsonModel;
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
        public void SubscribeModelChangedEven()
        {
            ModelHelperRef = DotNetObjectReference.Create(ModelHelper);
            _jsRuntime.InvokeVoidAsync("subscribeModelChangedEvent", ModelHelperRef);
        }


    }

}
