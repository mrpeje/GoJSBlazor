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
        public string EventsFiered { get; set; }
        public string ColorCode { get; set; } = "#FF0000";
        public bool isBlockCreationDisabled { get; set; }
        public string FromBlock { get; set; }
        public string ToBlock { get; set; }        
        public string FromPort { get; set; }
        public string ToPort { get; set; }
        public string NewBlockCategory { get; set; }

        public DiagramModel DiagramModel;

        public string SelectedCategory { get; set; }
        public List<string> AwalibleNodes { get; set; } = new List<string>();

        public string SelectedBlockId { get; set; }
        public string newXcoordinate { get; set; }
        public string newYcoordinate { get; set; }

        protected async void MoveBlock()
        {           
            var coordinates = new Point { X = Int32.Parse(newXcoordinate), Y = Int32.Parse(newYcoordinate) };
            await ExampleJsInterop.MoveBlock(coordinates, SelectedBlockId);
        }
        protected async void RemoveBlockPort()
        {
            await ExampleJsInterop.RemovePortFromBlock("right0", SelectedBlockId);
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
            await ExampleJsInterop.AddPortToBlock(port, SelectedBlockId);
            await ExampleJsInterop.AddPortToBlock(port2, SelectedBlockId);


        }
        protected async void AddNewBlock()
        {
            var newBlock = new Block
            {
                Name = "Name",
                Category = SelectedCategory,
                Description = $"New block {SelectedCategory} "                
            };
            
            await ExampleJsInterop.AddBlock(newBlock);
        }
        protected async void AddBlockToPalette()
        {
            var newBlock = new Block
            {
                Name = NewBlockCategory,
                Category = NewBlockCategory,
                Description = "Block",
                Color = ColorCode,
                Id = "0",
            };
            var port1 = new Port
            {
                Name = "left",
                Color = "black",
                Description = "portDescr",
                PortType = PortType.Input
            };             
            var port2 = new Port
            {
                Name = "right",
                Color = "red",
                Description = "portDescr2",
                PortType = PortType.Output
            };
            var list = new List<Port>
            {
                port1, port2
            };
            if (await ExampleJsInterop.AddPaletteBlock(newBlock, list))
                AwalibleNodes.Add(newBlock.Category);

            StateHasChanged();
        }
       
        protected async void AddLink()
        {
            var newLink = new Link
            {
                FromPort = $"right{FromPort}",
                ToPort = $"left{ToPort}",
                ToBlock = ToBlock,
                FromBlock = FromBlock
            };
            await ExampleJsInterop.AddLink(newLink);
        }
        protected async void RemoveLink()
        {
            var newLink = new Link
            {
                FromPort = $"right{FromPort}",
                ToPort = $"left{ToPort}",
                ToBlock = ToBlock,
                FromBlock = FromBlock
            };
            await ExampleJsInterop.RemoveLink(newLink);
        }

        protected async void RemoveBlock()
        {
            await ExampleJsInterop.RemoveBlock(SelectedBlockId);
        }          
        protected async void RemovePaletteBlock()
        {
            await ExampleJsInterop.RemovePaletteBlock("0");
        }
        protected void Save()
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

        
        private void HandleBlocksDeletedEvent(List<BlockModel> deletedBlocks)
        {
            EventsFiered += Environment.NewLine + deletedBlocks .Count+ " Blocks deleted";
            StateHasChanged();
        }
        private void HandleBlocksAddedEvent(List<BlockModel> deletedBlocks)
        {
            EventsFiered += Environment.NewLine + deletedBlocks.Count + " Blocks added";
            StateHasChanged();
        }
        private void HandleLinkEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Link added"; 
            StateHasChanged();
        }
        private void HandleLinkDeletedEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Link deleted";
            StateHasChanged();
        }
        private void HandleBlockPositionChangedEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Block position changed";
            StateHasChanged();
        }

        private void HandleNodeMouseHoverEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Node mouse hover";
            StateHasChanged();
        }
        private void HandleLinkMouseHoverEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Link mouse hover";
            StateHasChanged();
        }

        private void HandleSelectBlockEvent(string args)
        {
            SelectedBlockId = args;
            EventsFiered += Environment.NewLine + "selected block changed";
            StateHasChanged();
        }

        private void HandleUndoEvent()
        {
            EventsFiered += Environment.NewLine + "Undo event";
            StateHasChanged();
        }
        private void HandleRedoEvent()
        {
            EventsFiered += Environment.NewLine + "Redo event";
            StateHasChanged();
        }

        private void HandleContextHelpEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Context menu help";
            StateHasChanged();
        }
        private void HandleContextPropertiesEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Context menu properties";
            StateHasChanged();
        }
        private void HandleContextOpenoEvent(string args)
        {
            EventsFiered += Environment.NewLine + "Context menu open";
            StateHasChanged();
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                ExampleJsInterop.SelectionEventInterceptor.NodeSelectionChanged += HandleSelectBlockEvent;
                ExampleJsInterop.AddedEventInterceptor.BlockAddedEvent += HandleBlocksAddedEvent;
                ExampleJsInterop.AddedEventInterceptor.BlocksRemoveEvent += HandleBlocksDeletedEvent;
                ExampleJsInterop.AddedEventInterceptor.LinkAddedEvent += HandleLinkEvent;
                ExampleJsInterop.AddedEventInterceptor.LinkRemoveEvent += HandleLinkDeletedEvent;

                
                ExampleJsInterop.MouseHoverEventInterceptor.NodeMouseHover += HandleNodeMouseHoverEvent;
                ExampleJsInterop.MouseHoverEventInterceptor.LinkMouseHover += HandleLinkMouseHoverEvent;
                ExampleJsInterop.BlockPositionEventInterceptor.BlockPositionChanged += HandleBlockPositionChangedEvent;
                ExampleJsInterop.UndoRedoEventInterceptor.Undo += HandleUndoEvent;
                ExampleJsInterop.UndoRedoEventInterceptor.Redo += HandleRedoEvent;

                ExampleJsInterop.BlockContextMenuEventsInterceptor.ContextHelp += HandleContextHelpEvent;
                ExampleJsInterop.BlockContextMenuEventsInterceptor.ContextProperties += HandleContextPropertiesEvent;
                ExampleJsInterop.BlockContextMenuEventsInterceptor.ContextOpen += HandleContextOpenoEvent;



            }
        }


    }
}
