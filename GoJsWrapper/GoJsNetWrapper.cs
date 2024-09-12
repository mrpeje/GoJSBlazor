using GoJsWrapper.EventInterceptors;
using GoJsWrapper.Interfaces;
using GoJsWrapper.Models;
using GoJsWrapper.Models.DTO;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Newtonsoft;
using Newtonsoft.Json;
using System.Drawing;
using System.Reflection;
using System.Text.Json;


namespace GoJsWrapper
{
    public class GoJsNetWrapper : IDiagram, IPalette
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<SelectionChangedEventInterceptor> ReferenceSelectionChangedInterceptor;
        private DotNetObjectReference<ModelChangedInterceptor> ReferenceModelInterceptor;
        private DotNetObjectReference<BlockPositionChangedEventInterceptor> ReferenceBlockPositionChangedInterceptor;
        private DotNetObjectReference<MouseHoverEventInterceptor> ReferenceMouseHoverEventInterceptor;
        private DotNetObjectReference<UndoRedoEventInterceptor> ReferenceUndoRedoEventInterceptor;
        private DotNetObjectReference<AddRemoveEventInterceptor> ReferenceAddedEventInterceptor;
        private DotNetObjectReference<BlockContextMenuEventsInterceptor> ReferenceBlockContextMenuEventsInterceptor;
        private ModelChangedInterceptor ModelInterceptor;

        public BlockContextMenuEventsInterceptor BlockContextMenuEventsInterceptor;
        public MouseHoverEventInterceptor MouseHoverEventInterceptor;
        public SelectionChangedEventInterceptor SelectionEventInterceptor;
        public BlockPositionChangedEventInterceptor BlockPositionEventInterceptor;
        public UndoRedoEventInterceptor UndoRedoEventInterceptor;
        public AddRemoveEventInterceptor AddedEventInterceptor;

        private Diagram Diagram;
        private Palette Palette;

        public delegate void DiagramLoadedHandler();
        public event DiagramLoadedHandler DiagramLoaded;
        public string GetJsDiagramModel()
        {
            return Diagram.JsModel;
        }
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
            AddedEventInterceptor = new AddRemoveEventInterceptor();
            BlockContextMenuEventsInterceptor = new BlockContextMenuEventsInterceptor();

            ModelInterceptor.DiagramModelChanged += Diagram.UpdateDiagramModel;
            ModelInterceptor.PaletteModelChanged += Palette.UpdatePaletteModel;
        }               

        public async Task InitGoJS()
        {
            ReferenceBlockContextMenuEventsInterceptor = DotNetObjectReference.Create(BlockContextMenuEventsInterceptor);
            ReferenceMouseHoverEventInterceptor = DotNetObjectReference.Create(MouseHoverEventInterceptor);
            ReferenceUndoRedoEventInterceptor = DotNetObjectReference.Create(UndoRedoEventInterceptor);
            await _jsRuntime.InvokeAsync<string>(
                "initDiagram", 
                ReferenceMouseHoverEventInterceptor, 
                ReferenceUndoRedoEventInterceptor,
                ReferenceBlockContextMenuEventsInterceptor);

            SetupModelChangedEvent();
            SetupSelectionChangedEvent();
            SetupBlockPositionChangedEvent(); 
            SetupAddedEvent();

        }
        public async Task<bool> AddBlock(Block newBlock)
        {
            var block = Palette.GetPaletteBlockByCategory(newBlock.Category);
            if (block == null)
                return false;

            await Diagram.AddBlockToJsModel(block);
            return true;
        }
        public async Task<bool> AddPortToBlock(Port newPort, int blockId)
        {
            string portId = "";
            var block = Diagram.GetBlockById(blockId);
            if (block == null)
                return false;

            var port = new PortModel
            {
                Name = newPort.Name,
                Color = newPort.Color,
                Description = newPort.Description
            };
            if (newPort.PortType == PortType.Output)
            {
                portId = "right" + block.OutputPorts.Count;
                port.Id = portId;
                block.OutputPorts.Add(port);
            }
            else if (newPort.PortType == PortType.Input)
            {
                portId = "left" + block.InputPorts.Count;
                port.Id = portId;
                block.InputPorts.Add(port);
            }
           
            await Diagram.UpdateBlockJsModel(block);
            return true;            
        }
        public async Task<bool> RemovePortFromBlock(string portId, int blockId)
        {
            var blockWithPort = Diagram.FindBlockWithPort(portId, blockId);
            if (blockWithPort == null)
                return false;
            await Diagram.RemovePort(portId, blockWithPort);
            return true;
        }

        public async Task<bool> UpdateBlock(Block blockUpdate, int blockId)
        {
            var block = Diagram.GetBlockById(blockId);
            if(block == null)   
                return false;
            block.Description = blockUpdate.Description;
            block.Name = blockUpdate.Name;
            block.Color = blockUpdate.Color;

            await Diagram.UpdateBlockJsModel(block);
            return true;

        }
        public async Task<bool> MoveBlock(Point newCoordinates, int blockId)
        {
            var block = Diagram.GetBlockById(blockId);
            if (block == null)
                return false;

            block.Coordinates = newCoordinates.X + " " + newCoordinates.Y;
            await Diagram.MoveBlockJsModel(block.Id, block.Coordinates);
            return true;
        }

        public async Task<bool> RemoveBlock(int blockId)
        {
            var block = Diagram.GetBlockById(blockId);
            if(block == null) 
                return false;

            await Diagram.RemoveBlockFromJsModel(block.Id);
            return true;
        }
        public async Task<bool> AddLink(Link newlink)
        {
            var block1 = Diagram.GetBlockById(newlink.FromBlock);
            var block2 = Diagram.GetBlockById(newlink.ToBlock);
            if( block1 != null && block2 != null)
            {
                var link = new LinkModel
                {
                    FromPort = newlink.FromPort,
                    ToPort = newlink.ToPort,
                    FromBlock = newlink.FromBlock,
                    ToBlock = newlink.ToBlock
                };
                await Diagram.AddLinkToJsModel(link);
                return true;
            }
            return false;
        }
        public async Task<bool> RemoveLink(Link link)
        {
            var findLink = Diagram.GetLinkByParams(link.FromBlock, link.ToBlock, link.FromPort, link.ToPort);
            if (findLink == null)
                return false;

            await Diagram.RemoveLinkFromJsModel(findLink);
            return true;
        }
        public async Task<bool> RemovePaletteBlock(int id)
        {
            var block = Palette.FindBlock(id);
            if( block == null) 
                return false;

            await Palette.RemoveBlockFromJsModel(block.Id);
            return true;
        }

        public async Task<bool> AddPaletteBlock(Block block, List<Port> ports)
        {
            var newPaletteBlock = new BlockModel
            {
                Category = block.Category,
                Color = block.Color,
                Name = block.Name,
                Description = block.Description,
                InputPorts = new List<PortModel>(),
                OutputPorts = new List<PortModel>(),
                Id = block.Id
            };
            foreach (var port in ports)
            {
                var newPort = new PortModel
                {
                    Color = port.Color,
                    Name = port.Name,
                    Description = port.Description,
                };
                string portId;
                if (port.PortType == PortType.Output)
                {
                    portId = "right" + newPaletteBlock.OutputPorts.Count;
                    newPort.Id = portId;
                    newPaletteBlock.OutputPorts.Add(newPort);
                }
                else if (port.PortType == PortType.Input)
                {
                    portId = "left" + newPaletteBlock.InputPorts.Count;
                    newPort.Id = portId;
                    newPaletteBlock.InputPorts.Add(newPort);
                }
            }
            if(Palette.ValidateNewBlock(newPaletteBlock))
            {
                await Palette.AddBlockToJsModel(newPaletteBlock);
                return true;
            }
            return false;
        }

        public DiagramModel SaveDiagramModel()
        {
            return Diagram.GetModel();
        }
        public async Task LoadDiagram(string jsModel)
        {
            await Diagram.Clear();
            var diagramModel = Diagram.ParseDiagramModel(jsModel);
            if(diagramModel != null) 
            { 
                foreach (var modelBlock in diagramModel.Blocks.ToList())
                {
                    await Diagram.AddBlockToJsModel(modelBlock);
                }

                foreach (var link in diagramModel.Links.ToList())
                {
                    await Diagram.AddLinkToJsModel(link);
                }
            }

            DiagramLoaded?.Invoke();
        }
        public Block GetPaletteBlockById(int blockId)
        {
            if (blockId == null)
                return null;

            var blockModel = Palette.FindBlock(blockId);
            if (blockModel == null)
                return null;
            Block block = new Block
            {
                Category = blockModel.Category,
                Description = blockModel.Description,
                Color = blockModel.Color,
                Name = blockModel.Name,
                Id = blockModel.Id
            };
            return block;
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
        public void SetupAddedEvent()
        {
            ReferenceAddedEventInterceptor = DotNetObjectReference.Create(AddedEventInterceptor);
            _jsRuntime.InvokeVoidAsync("subscribeAddedEvent", ReferenceAddedEventInterceptor);
        }
    }
}