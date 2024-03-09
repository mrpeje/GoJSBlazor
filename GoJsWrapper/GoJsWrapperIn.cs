using GoJsWrapper.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Newtonsoft;
using Newtonsoft.Json;
using System.Text.Json;


namespace GoJsWrapper
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class GoJsWrapperIn : IAsyncDisposable
    {
        public DiagramModel Model {  get; private set; }
        private DotNetObjectReference<Helper> ModelHelperRef;
        private Helper ModelHelper;
        public LinkSelectionChangedEventHelper LinkSelectionHelper;
        public NodeSelectionChangedEventHelper NodeSelectionHelper;
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly IJSRuntime _jsRuntime;
        private string JsonModel { get; set; }
        List<UnitCategoryModel> Blocks { get; set; }

        private DotNetObjectReference<CustomEventHelper> Reference;
        private DotNetObjectReference<NodeSelectionChangedEventHelper> ReferenceNodeSelectionChanged;
        private DotNetObjectReference<LinkSelectionChangedEventHelper> ReferenceLinkSelectionChanged;


        public GoJsWrapperIn(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            ModelHelper = new Helper();
            LinkSelectionHelper = new LinkSelectionChangedEventHelper();
            NodeSelectionHelper = new NodeSelectionChangedEventHelper();
            ModelHelper.ModelChanged += UpdateDiagramModel;

        }
        public async Task AddLink(LinkModel newlink) 
        {
            try
            {
                Model.ValidateNewLink(newlink);
                var newlinkJson = JsonConvert.SerializeObject(newlink);
                await _jsRuntime.InvokeAsync<string>("addLink", newlinkJson);
            }
            catch( BlockNotFoundException exeptionBlockNotFound) 
            {

            }
            catch (PortNotFoundException exeptionPortNotFound)
            {

            }
        }
        public async Task MoveBlock(string blockId, string newCoordinates)
        {
            var block = Model.MoveBlock(blockId, newCoordinates);
            var blockJson = JsonConvert.SerializeObject(block);
            await _jsRuntime.InvokeAsync<string>("updateBlock", blockJson);
        }
        public async Task UpdateBlock(UnitModel block)
        {
            var validatedBlock = Model.ValidateNewBlock(block);
            var foundLinkJson = JsonConvert.SerializeObject(validatedBlock);
            await _jsRuntime.InvokeAsync<string>("updateBlock", foundLinkJson);
        }
        public async Task RemoveLink(LinkModel link)
        {
            var foundLink = Model.GetLink(link);
            if (foundLink != null)
            {
                var foundLinkJson = JsonConvert.SerializeObject(foundLink);
                await _jsRuntime.InvokeAsync<string>("deleteLink", foundLinkJson);
            }
        }
        public async Task AddBlock(UnitModel newBlock)
        {
            var validatedBlock = Model.ValidateNewBlock(newBlock);
            var validatedBlockJson = JsonConvert.SerializeObject(validatedBlock); 
            await _jsRuntime.InvokeAsync<string>("addNewBlock", validatedBlockJson);
        }
        public async Task RemoveBlock(string id)
        {
            var foundedBlock = Model.GetBlock(id);
            if(foundedBlock != null)
            {
                var foundedBlockJson = JsonConvert.SerializeObject(foundedBlock);
                await _jsRuntime.InvokeAsync<string>("removeBlock", foundedBlockJson);
            }
            
        }
        public void UpdateDiagramModel(string model) 
        {
            try
            {
                var parsedModel = JsonConvert.DeserializeObject<DiagramModel>(model);
                if (parsedModel != null)
                {
                    Model = parsedModel;
                }
            }
            catch (Exception ex) 
            { 

            }
              
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
            SubscribeModelChangedEven();
            SetupLinkSelectionChangedEvent();
            SetupNodeSelectionChangedEvent();
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
        public void SubscribeModelChangedEven()
        {
            ModelHelperRef = DotNetObjectReference.Create(ModelHelper);
            _jsRuntime.InvokeVoidAsync("subscribeModelChangedEvent", ModelHelperRef);
        }

        public ValueTask<string> SetupCustomEventCallback(Func<string, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new CustomEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addCustomEventListener", Reference);
        }
        public void SetupNodeSelectionChangedEvent()
        {
            ReferenceNodeSelectionChanged = DotNetObjectReference.Create(NodeSelectionHelper);
            _jsRuntime.InvokeVoidAsync("subscribeNodeSelectionChangedEventListener", ReferenceNodeSelectionChanged);
        }
        public void SetupLinkSelectionChangedEvent()
        {
            ReferenceLinkSelectionChanged = DotNetObjectReference.Create(LinkSelectionHelper);
            _jsRuntime.InvokeVoidAsync("subscribeLinkSelectionChangedEventListener", ReferenceLinkSelectionChanged);
        }
    }
}
