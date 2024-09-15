using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using GoJsWrapper;
using GoJsWrapper.Models;
using Microsoft.AspNetCore.Components.Routing;
using Newtonsoft.Json;
using GoJsWrapper.Models.DTO;
using System.Drawing;
using System.Text;

namespace GoJSBlazor.Pages {
    public partial class Index : ComponentBase
    {
        [Inject] GoJsNetWrapper GoJsNet { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public string NodeId { get; set; }
        public string TriggeredEvents { get; set; }
        public string ColorCode { get; set; } = "#FF0000";
        public bool isBlockCreationDisabled { get; set; }
        public int FromBlock { get; set; }
        public int ToBlock { get; set; }        
        public string FromPort { get; set; }
        public string ToPort { get; set; }
        public string NewBlockCategory { get; set; }

        public string JsDiagramModel;

        public string SelectedCategory { get; set; }
        public List<string> AwalibleNodes { get; set; } = new List<string>();

        public int SelectedBlockId { get; set; }
        public string newXcoordinate { get; set; }
        public string newYcoordinate { get; set; }
        protected async void MoveBlock()
        {
            if (newXcoordinate != null && newYcoordinate != null)
            {
                var coordinates = new Point { X = Int32.Parse(newXcoordinate), Y = Int32.Parse(newYcoordinate) };
                await GoJsNet.MoveBlock(coordinates, SelectedBlockId);
            }
        }
        protected async void RemoveBlockPort()
        {
            await GoJsNet.RemovePortFromBlock("right0", SelectedBlockId);
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
            await GoJsNet.AddPortToBlock(port, SelectedBlockId);
            await GoJsNet.AddPortToBlock(port2, SelectedBlockId);
        }
        protected async void AddNewBlock()
        {
            var newBlock = new Block
            {
                Name = "Name",
                Category = SelectedCategory,
                Description = $"New block {SelectedCategory} ",
                Id = 0
            };
            
            await GoJsNet.AddBlock(newBlock);
        }
        protected async void AddBlockToPalette()
        {
            var newBlock = new Block
            {
                Name = NewBlockCategory,
                Category = NewBlockCategory,
                Description = "Block",
                Color = ColorCode,
                Id = 0,
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
            if (await GoJsNet.AddPaletteBlock(newBlock, list))
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
            await GoJsNet.AddLink(newLink);
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
            await GoJsNet.RemoveLink(newLink);
        }

        protected async void RemoveBlock()
        {
            await GoJsNet.RemoveBlock(SelectedBlockId);
        }          
        protected async void RemovePaletteBlock()
        {
            Block block = GoJsNet.GetPaletteBlockById(SelectedBlockId);
            if (block != null)
            {
                AwalibleNodes.Remove(block.Category);
                await GoJsNet.RemovePaletteBlock(SelectedBlockId);
                StateHasChanged();
            }
        }
        protected async Task Save()
        {
            JsDiagramModel = GoJsNet.GetJsDiagramModel();
            var bytes = System.Text.Encoding.UTF8.GetBytes(JsDiagramModel);
            await FileUtil.SaveAs(JSRuntime, "Diagram.txt", bytes);
           
        }
        protected async void Load()
        {
            byte[] fileBytes =  await FileUtil.LoadFile(JSRuntime);
            var JsonModel =  System.Text.Encoding.UTF8.GetString(fileBytes);
            await GoJsNet.LoadDiagram(JsonModel);
        }

        
        private void HandleBlocksDeletedEvent(List<BlockModel> deletedBlocks)
        {
            TriggeredEvents += Environment.NewLine + deletedBlocks .Count+ " Blocks deleted";
            StateHasChanged();
        }
        private void HandleBlocksAddedEvent(List<BlockModel> deletedBlocks)
        {
            TriggeredEvents += Environment.NewLine + deletedBlocks.Count + " Blocks added";
            StateHasChanged();
        }
        private void HandleLinkEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Link added"; 
            StateHasChanged();
        }
        private void HandleLinkDeletedEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Link deleted";
            StateHasChanged();
        }
        private void HandleBlockPositionChangedEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Block position changed";
            StateHasChanged();
        }

        private void HandleNodeMouseHoverEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Node mouse hover";
            StateHasChanged();
        }
        private void HandleLinkMouseHoverEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Link mouse hover";
            StateHasChanged();
        }

        private void HandleSelectBlockEvent(string args)
        {
            Int32.TryParse(args, out var id);
            if (id != null)
            {
                SelectedBlockId = id;
                TriggeredEvents += Environment.NewLine + "selected block changed";
                StateHasChanged();
            }
        }

        private void HandleUndoEvent()
        {
            TriggeredEvents += Environment.NewLine + "Undo event";
            StateHasChanged();
        }
        private void HandleRedoEvent()
        {
            TriggeredEvents += Environment.NewLine + "Redo event";
            StateHasChanged();
        }

        private void HandleContextHelpEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Context menu help";
            StateHasChanged();
        }
        private void HandleContextPropertiesEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Context menu properties";
            StateHasChanged();
        }
        private void HandleContextOpenoEvent(string args)
        {
            TriggeredEvents += Environment.NewLine + "Context menu open";
            StateHasChanged();
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                GoJsNet.SelectionEventInterceptor.NodeSelectionChanged += HandleSelectBlockEvent;
                GoJsNet.AddedEventInterceptor.BlockAddedEvent += HandleBlocksAddedEvent;
                GoJsNet.AddedEventInterceptor.BlocksRemoveEvent += HandleBlocksDeletedEvent;
                GoJsNet.AddedEventInterceptor.LinkAddedEvent += HandleLinkEvent;
                GoJsNet.AddedEventInterceptor.LinkRemoveEvent += HandleLinkDeletedEvent;

                
                GoJsNet.MouseHoverEventInterceptor.NodeMouseHover += HandleNodeMouseHoverEvent;
                GoJsNet.MouseHoverEventInterceptor.LinkMouseHover += HandleLinkMouseHoverEvent;
                GoJsNet.BlockPositionEventInterceptor.BlockPositionChanged += HandleBlockPositionChangedEvent;
                GoJsNet.UndoRedoEventInterceptor.Undo += HandleUndoEvent;
                GoJsNet.UndoRedoEventInterceptor.Redo += HandleRedoEvent;

                GoJsNet.BlockContextMenuEventsInterceptor.ContextHelp += HandleContextHelpEvent;
                GoJsNet.BlockContextMenuEventsInterceptor.ContextProperties += HandleContextPropertiesEvent;
                GoJsNet.BlockContextMenuEventsInterceptor.ContextOpen += HandleContextOpenoEvent;



            }
        }


    }
}
