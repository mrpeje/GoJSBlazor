using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using GoJsWrapper;
using GoJsWrapper.Models;
using Microsoft.AspNetCore.Components.Routing;
using Newtonsoft.Json;

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
            await ExampleJsInterop.SelectBlockById(ExampleJsInterop.Diagram.Blocks.FirstOrDefault().Id);
        }
        protected async void MoveBlock()
        {
            var block = ExampleJsInterop.Diagram.Blocks.FirstOrDefault();
            block.Coordinates = "50 50";
            await ExampleJsInterop.Diagram.MoveBlock(block.Id, block.Coordinates);
        }
        private void HandleCustomEvent(/*string args*/)
        {
            NodeId = "DiagramLoaded".ToString();
            StateHasChanged();
        }
        private void HandleCustomEvent2(/*string args*/)
        {
            Text2 = "Redo".ToString();
            StateHasChanged();
        }
        protected async void UpdateBlock()
        {
            var block = ExampleJsInterop.Diagram.Blocks.FirstOrDefault();

            block.Name = "Name1";
            block.Category = "Category";
            block.Color = "#000000";
            block.OutputPorts.FirstOrDefault().Color = "#F92C00";
            block.InputPorts.FirstOrDefault().Color = "#F92C00";
            block.InputPorts.FirstOrDefault().Description = "red port";
            block.InputPorts.FirstOrDefault().Name = "red port";
            block.Description = "New description";
            await ExampleJsInterop.Diagram.UpdateBlock(block);
        }
        protected async void AddNewBlock()
        {
            var newBlock = new BlockModel
            {
                Name = "Name",
                Category = "Category",
                Id="1",
                Description ="",
                InputPorts = new List<PortModel> { new PortModel() { Id = "left0", Color = "red" } },
                OutputPorts = new List<PortModel> { new PortModel() { Id = "right0", Color = "red" } }
                
            };
            
            await ExampleJsInterop.Diagram.AddBlock(newBlock);
        }
        protected async void AddNewBlocksToPalette()
        {
            var newBlock = new BlockModel
            {
                Name = "Red",
                Category = "Category",
                Id = "0",
                Description = "Category1",
                InputPorts = new List<PortModel> { new PortModel() { Id = "left0", Color = "black" } },
                OutputPorts = new List<PortModel> { new PortModel() { Id = "right0", Color = "black" } },
                Color = "red"
            };

            await ExampleJsInterop.Palette.AddBlock(newBlock);
            var newBlock2 = new BlockModel
            {
                Name = "Green",
                Category = "Category2",
                Id = "0",
                Description = "Category2",
                InputPorts = new List<PortModel> { new PortModel() { Id = "left0", Color = "blue" } },
                OutputPorts = new List<PortModel> { new PortModel() { Id = "right0", Color = "blue" } },
                Color = "green"
            };

            await ExampleJsInterop.Palette.AddBlock(newBlock2);
            var newBlock3 = new BlockModel
            {
                Name = "Yellow",
                Category = "Category3",
                Id = "0",
                Description = "Category3",
                InputPorts = new List<PortModel> { new PortModel() { Id = "left0", Color = "orange" } },
                OutputPorts = new List<PortModel> { new PortModel() { Id = "right0", Color = "orange" } },
                Color = "yellow"
            };

            await ExampleJsInterop.Palette.AddBlock(newBlock3);
        }
        protected async void AddLink()
        {
            var fromBlock = ExampleJsInterop.Diagram.Blocks.FirstOrDefault();
            var toBlock = ExampleJsInterop.Diagram.Blocks.FirstOrDefault(e=>e.Id != fromBlock.Id);
            var newLink = new LinkModel
            {
                fromPort = "right0",
                toPort = "left0",
                To = toBlock.Id,
                From = fromBlock.Id
            };
            await ExampleJsInterop.Diagram.AddLink(newLink);
        }
        protected async void RemoveBlock()
        {

            await ExampleJsInterop.Diagram.RemoveBlock(NodeId);
        }
        protected async void RemovePaletteBlock()
        {
            await ExampleJsInterop.Palette.RemoveBlock(ExampleJsInterop.Palette.Model.FirstOrDefault().Id);
        }
        protected async void RemoveLink()
        {
            var linkId = "1";
            var link = ExampleJsInterop.Diagram.Links.FirstOrDefault(e => e.Id == linkId);
            if( link != null)
                await ExampleJsInterop.Diagram.RemoveLink(link.Id);
        }
        protected async void Load()
        {
            var a = @"[{""name"":""Red"",""category"":""Category"",""key"":""0"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""black""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""black""}],""loc"":null,""color"":""red"",""description"":""Category1""},{""name"":""Green"",""category"":""Category2"",""key"":""02"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""blue""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""blue""}],""loc"":""0 70"",""color"":""green"",""description"":""Category2""},{""name"":""Yellow"",""category"":""Category3"",""key"":""03"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""orange""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""orange""}],""loc"":""0 140"",""color"":""yellow"",""description"":""Category3""}]";
            var b = @"[{""name"":""Unit One"",""category"":""Cat1"",""key"":""1"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":""asdasd"",""portColor"":""#fae3d7""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""#eaeef8""},{""portId"":""right1"",""name"":null,""description"":null,""portColor"":""#fadfe5""}],""loc"":""101 204"",""color"":""#66d6d1"",""description"":""asd""},{""name"":""Yellow"",""category"":""Category3"",""key"":""03"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""orange""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""orange""}],""loc"":""-100 300.40625"",""color"":""yellow"",""description"":""Category3""},{""name"":""Green"",""category"":""Category2"",""key"":""02"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""blue""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""blue""}],""loc"":""262 302.40625"",""color"":""green"",""description"":""Category2""},{""name"":""Red"",""category"":""Category"",""key"":""0"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""black""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""black""}],""loc"":""86 104.40625"",""color"":""red"",""description"":""Category1""}]";
            var c = @"[]";
            var Blocks = JsonConvert.DeserializeObject<List<BlockModel>>(b);
            var Palette = JsonConvert.DeserializeObject<List<BlockModel>>(a);
            var Links = JsonConvert.DeserializeObject<List<LinkModel>>(c);

            await ExampleJsInterop.LoadDiagram(Blocks, Links, Palette);
        }
        protected async override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                //ExampleJsInterop = new ExampleJsInterop(JSRuntime);
                // This calls the script in gojs-scripts.js

                await ExampleJsInterop.InitGoJS();
                AddNewBlocksToPalette();
                //ExampleJsInterop.SelectionEventInterceptor.LinkSelectionChanged += HandleCustomEvent2;
                //ExampleJsInterop.SelectionEventInterceptor.NodeSelectionChanged += HandleCustomEvent;

                ExampleJsInterop.DiagramLoaded += HandleCustomEvent;
                ExampleJsInterop.UndoRedoEventInterceptor.Redo += HandleCustomEvent2;
            }
        }


    }
}
