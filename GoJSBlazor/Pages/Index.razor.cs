using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using GoJsWrapper;
using GoJsWrapper.Models;
using Microsoft.AspNetCore.Components.Routing;

namespace GoJSBlazor.Pages {
    public partial class Index : ComponentBase
    {
        [Inject] GoJsNetWrapper ExampleJsInterop { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public string NodeId { get; set; }
        public string Text2 { get; set; }

        protected async void Save(string jsonModel = null)
        {

            //await ExampleJsInterop.SaveDiagram(jsonModel);
        }
        protected async void SelectBlock()
        {
            await ExampleJsInterop.SelectBlockById(ExampleJsInterop.Model.Blocks.FirstOrDefault().Id);
        }
        protected async void MoveBlock()
        {
            var block = ExampleJsInterop.Model.Blocks.FirstOrDefault();
            block.Coordinates = "50 50";
            await ExampleJsInterop.Model.MoveBlock(block.Id, block.Coordinates);
        }
        private void HandleCustomEvent(string args)
        {
            NodeId = args.ToString();
            StateHasChanged();
        }
        private void HandleCustomEvent2(string args)
        {
            Text2 = args.ToString();
            StateHasChanged();
        }
        protected async void UpdateBlock()
        {
            var block = ExampleJsInterop.Model.Blocks.FirstOrDefault();

            block.Name = "Name1";
            block.Category = "Category";
            block.Color = "#000000";
            block.OutputPorts.FirstOrDefault().Color = "#F92C00";
            block.InputPorts.FirstOrDefault().Color = "#F92C00";
            block.InputPorts.FirstOrDefault().Description = "red port";
            block.InputPorts.FirstOrDefault().Name = "red port";
            block.Description = "New description";
            await ExampleJsInterop.Model.UpdateBlock(block);
        }
        protected async void AddNewBlock()
        {
            var newBlock = new UnitModel
            {
                Name = "Name",
                Category = "Category",
                Id="1",
                Description ="",
                InputPorts = new List<Port> { new Port() { Id = "left0", Color = "red" } },
                OutputPorts = new List<Port> { new Port() { Id = "right0", Color = "red" } }
                
            };
            
            await ExampleJsInterop.Model.AddBlock(newBlock);
        }
        protected async void AddLink()
        {
            var fromBlock = ExampleJsInterop.Model.Blocks.FirstOrDefault();
            var toBlock = ExampleJsInterop.Model.Blocks.FirstOrDefault(e=>e.Id != fromBlock.Id);
            var newLink = new LinkModel
            {
                fromPort = "right0",
                toPort = "left0",
                To = toBlock.Id,
                From = fromBlock.Id
            };
            await ExampleJsInterop.Model.AddLink(newLink);
        }
        protected async void RemoveBlock()
        {

            await ExampleJsInterop.Model.RemoveBlock(NodeId);
        }
        protected async void RemoveLink()
        {
            var linkId = "1";
            var link = ExampleJsInterop.Model.Links.FirstOrDefault(e => e.Id == linkId);

            await ExampleJsInterop.Model.RemoveLink(link.Id);
        }
        protected async void Load()
        {
            await ExampleJsInterop.LoadDiagram();
        }
        protected async override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                //ExampleJsInterop = new ExampleJsInterop(JSRuntime);
                // This calls the script in gojs-scripts.js
                var listBlock = new List<UnitCategoryModel>
                {
                    new UnitCategoryModel
                    {
                        Id = 1,
                        Type ="Cat1",
                        Name = "Cat1",
                        NumberInputPorts = 1,
                        NumberOutputPorts = 2,
                    },
                    new UnitCategoryModel
                    {
                        Id = 1,
                        Type ="Cat2",
                        Name = "Cat2",
                        NumberInputPorts = 2,
                        NumberOutputPorts = 1,
                    },
                    new UnitCategoryModel
                    {
                        Id = 1,
                        Type ="Cat3",
                        Name = "Cat3",
                        NumberInputPorts = 2,
                        NumberOutputPorts = 2,
                    }
                };
                await ExampleJsInterop.InitGoJS(listBlock);

                ExampleJsInterop.SelectionEventInterceptor.LinkSelectionChanged += HandleCustomEvent2;
                ExampleJsInterop.SelectionEventInterceptor.NodeSelectionChanged += HandleCustomEvent;
            }
        }


    }
}
