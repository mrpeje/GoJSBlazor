using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using GoJsWrapper;
using GoJsWrapper.Models;
using Microsoft.AspNetCore.Components.Routing;
using Newtonsoft.Json;
using GoJsWrapper.Models.DTO;
using System.Drawing;

namespace GoJSBlazor.Pages {
    public partial class Index : ComponentBase
    {
        [Inject] GoJsNetWrapper ExampleJsInterop { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public string NodeId { get; set; }
        public string Text2 { get; set; }
        public DiagramModel DiagramModel;

        //protected async void Save(string jsonModel = null)
        //{

        //    AddNewBlocksToPalette();
        //}
        //protected async void SelectBlock()
        //{
        //    await ExampleJsInterop.SelectBlockById(ExampleJsInterop.Diagram.Blocks.FirstOrDefault().Id);
        //}
        protected async void MoveBlock()
        {           
            var coordinates = new Point { X = 50, Y = 50 };
            await ExampleJsInterop.MoveBlock(coordinates, "0");
        }
        private void HandleCustomEvent(List<BlockModel> blocks)
        {
            NodeId = blocks.Count.ToString();
            StateHasChanged();
        }
        private void HandleCustomEvent2(/*string args*/)
        {
            Text2 = "Redo".ToString();
            StateHasChanged();
        }
        protected async void RemoveBlockPort()
        {
            await ExampleJsInterop.RemovePortFromBlock("right2", "0");
        }
        protected async void UpdateBlockPorts()
        {

            var port = new Port
            {
                Name = "Port1",
                Color = "#F92C00",
                Description = "red port right1",
                PortType = PortType.Output
            };             
            var port2 = new Port
            {
                Name = "Port2",
                Color = "blue",
                Description = "blue port left2",
                PortType = PortType.Input
            };
            await ExampleJsInterop.AddPortToBlock(port, "0");
            await ExampleJsInterop.AddPortToBlock(port2, "0");


        }
        protected async void AddNewBlock()
        {
            var newBlock = new Block
            {
                Name = "Name",
                Category = "Category",
                Description = "New block Category2",
                Color = "yello"                
            };
            
            await ExampleJsInterop.AddBlock(newBlock);
        }
        protected async void AddNewBlocksToPalette()
        {

            var newBlock = new Block
            {
                Name = "Red",
                Category = "Category",
                Description = "Red block Category",
                Color = "Red",
                Id = "0",
            };
            var port1 = new Port
            {
                Name = "le",
                Color = "black",
                Description = "portDescr",
                PortType = PortType.Input
            };             
            var port2 = new Port
            {
                Name = "le",
                Color = "red",
                Description = "portDescr2",
                PortType = PortType.Output
            };
            var list = new List<Port>
            {
                port1, port2
            };
            await ExampleJsInterop.AddPaletteBlock(newBlock, list);

        }
        protected async void AddLink()
        {
            var newLink = new Link
            {
                FromPort = "right0",
                ToPort = "left0",
                ToBlock = "0",
                FromBlock = "02"
            };
            await ExampleJsInterop.AddLink(newLink);
        }
        protected async void RemoveLink()
        {
            var newLink = new Link
            {
                FromPort = "right0",
                ToPort = "left0",
                ToBlock = "0",
                FromBlock = "02"
            };
            await ExampleJsInterop.RemoveLink(newLink);
        }

        protected async void RemoveBlock()
        {
            await ExampleJsInterop.RemoveBlock(Text2);
        }          
        protected async void RemovePaletteBlock()
        {
            await ExampleJsInterop.RemovePaletteBlock("0");
        }
        protected async void Save()
        {
            DiagramModel = ExampleJsInterop.SaveDiagramModel();
        }
        protected async void Load()
        {
            var a = @"[{""name"":""Red"",""category"":""Category"",""key"":""0"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""black""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""black""}],""loc"":null,""color"":""red"",""description"":""Category1""},{""name"":""Green"",""category"":""Category2"",""key"":""02"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""blue""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""blue""}],""loc"":""0 70"",""color"":""green"",""description"":""Category2""},{""name"":""Yellow"",""category"":""Category3"",""key"":""03"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""orange""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""orange""}],""loc"":""0 140"",""color"":""yellow"",""description"":""Category3""}]";
            var b = @"[{""name"":""Unit One"",""category"":""Cat1"",""key"":""1"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":""asdasd"",""portColor"":""#fae3d7""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""#eaeef8""},{""portId"":""right1"",""name"":null,""description"":null,""portColor"":""#fadfe5""}],""loc"":""101 204"",""color"":""#66d6d1"",""description"":""asd""},{""name"":""Yellow"",""category"":""Category3"",""key"":""03"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""orange""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""orange""}],""loc"":""-100 300.40625"",""color"":""yellow"",""description"":""Category3""},{""name"":""Green"",""category"":""Category2"",""key"":""02"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""blue""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""blue""}],""loc"":""262 302.40625"",""color"":""green"",""description"":""Category2""},{""name"":""Red"",""category"":""Category"",""key"":""0"",""leftArray"":[{""portId"":""left0"",""name"":null,""description"":null,""portColor"":""black""}],""rightArray"":[{""portId"":""right0"",""name"":null,""description"":null,""portColor"":""black""}],""loc"":""86 104.40625"",""color"":""red"",""description"":""Category1""}]";
            var c = @"[]";
            var Blocks = JsonConvert.DeserializeObject<List<BlockModel>>(b);
            var Palette = JsonConvert.DeserializeObject<List<BlockModel>>(a);
            var Links = JsonConvert.DeserializeObject<List<LinkModel>>(c);

            await ExampleJsInterop.LoadDiagram(DiagramModel);
        }
        protected async override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                //ExampleJsInterop = new ExampleJsInterop(JSRuntime);
                // This calls the script in gojs-scripts.js

                //
                //ExampleJsInterop.SelectionEventInterceptor.LinkSelectionChanged += HandleCustomEvent2;
                //ExampleJsInterop.SelectionEventInterceptor.NodeSelectionChanged += HandleCustomEvent;

                //ExampleJsInterop.AddedEventInterceptor.BlockAddedEvent += HandleCustomEvent;
                //ExampleJsInterop.UndoRedoEventInterceptor.Redo += HandleCustomEvent2;
            }
        }


    }
}
