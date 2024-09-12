using GoJsWrapper.Interfaces;
using GoJsWrapper.Models;
using GoJsWrapper.Models.DTO;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Drawing;
using System.Reflection;


namespace GoJsWrapper
{
    public class Diagram 
    {
        private readonly IJSRuntime _jsRuntime;
        private string _jsModel;
        public string JsModel {  get { return _jsModel; } }
        private DiagramModel Model { get; set; }
        public Diagram(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Model = new DiagramModel();
            Model.Blocks = new List<BlockModel>();
            Model.Links = new List<LinkModel>();
        }


        internal void UpdateDiagramModel(string model)
        {
            var parsedModel = ParseDiagramModel(model);           
            if (parsedModel != null)
            {
                _jsModel = model;
                Model.Blocks = parsedModel.Blocks;
                Model.Links = parsedModel.Links;
            }
        }
        internal DiagramModel? ParseDiagramModel(string model)
        {
            DiagramModel? parsedModel = null;
            try
            {
                parsedModel = JsonConvert.DeserializeObject<DiagramModel>(model);
            }
            catch (JsonSerializationException ex)
            {

            }
            return parsedModel;
        }
        public BlockModel GetBlockById(int id)
        {
            return Model.Blocks.FirstOrDefault(e => e.Id == id);
        }

        public async Task RemoveBlockFromJsModel(int blockId)
        {
               await _jsRuntime.InvokeAsync<string>("removeBlock", blockId);
        }
        public async Task<bool> UpdateBlock(Block blockUpdate, int blockId)
        {
            var block = GetBlockById(blockId);
            if (block == null)
                return false;
            block.Description = blockUpdate.Description;
            block.Name = blockUpdate.Name;
            block.Color = blockUpdate.Color;

            await UpdateBlockJsModel(block);
            return true;

        }
        public async Task UpdateBlockJsModel(BlockModel block)
        {
            var blockToUpdateJson = JsonConvert.SerializeObject(block);
            await _jsRuntime.InvokeAsync<string>("updateBlock", blockToUpdateJson, block.Id);

        }
        BlockModel TryDeleteInputPort(string portId, BlockModel block)
        {
            PortModel port = block.InputPorts.FirstOrDefault(e => e.Id == portId);
            if (port == null)
                return null;
            block.InputPorts.Remove(port);
            return block;
        }
        BlockModel TryDeleteOutputPort(string portId, BlockModel block)
        {
            PortModel port = block.OutputPorts.FirstOrDefault(e => e.Id == portId);
            if (port == null)
                return null;
            block.OutputPorts.Remove(port);
            return block;
        }
        public async Task RemovePort(string portId, BlockModel block)
        {
            BlockModel updatedBlock;
            updatedBlock = TryDeleteInputPort(portId, block);
            if (updatedBlock == null)
                updatedBlock = TryDeleteOutputPort(portId, block);

            await UpdateBlockJsModel(updatedBlock);
        }

        public async Task MoveBlockJsModel(int blockId, string newCoordinates)
        {
            await _jsRuntime.InvokeAsync<string>("updateBlockPosition", blockId, newCoordinates);
        }

        public LinkModel GetLinkByParams(int fromBlock, int toBlock, string fromPort, string toPort)
        {
            return Model.Links.FirstOrDefault(e => e.ToBlock == toBlock &&
                                            e.FromBlock == fromBlock &&
                                            e.ToPort == toPort &&
                                            e.FromPort == fromPort);

        }
        public async Task AddLinkToJsModel(LinkModel newlink)
        {
            var newlinkJson = JsonConvert.SerializeObject(newlink);
            await _jsRuntime.InvokeAsync<string>("addLink", newlinkJson);
        }

        public async Task<bool> RemoveLinkFromJsModel(LinkModel link)
        {
            var foundLinkJson = JsonConvert.SerializeObject(link);
            await _jsRuntime.InvokeAsync<string>("deleteLink", foundLinkJson);
            return true;
        }

        public async Task AddBlockToJsModel(BlockModel block)
        {
            var validatedBlockJson = JsonConvert.SerializeObject(block);
            await _jsRuntime.InvokeAsync<string>("addNewBlock", validatedBlockJson);
        }
        public BlockModel FindBlockWithPort(string portId, int blockId)
        {
            return Model.Blocks.FirstOrDefault(e => e.Id == blockId &&
                    (e.InputPorts.Any(e => e.Id == portId) || e.OutputPorts.Any(e => e.Id == portId)));
        }
        public DiagramModel GetModel()
        {
            var tmp = JsonConvert.SerializeObject(Model);
            var model = JsonConvert.DeserializeObject<DiagramModel>(tmp);
            return model;
        }

        public async Task Clear()
        {
            foreach(var block in Model.Blocks.ToList())
            {
                await RemoveBlockFromJsModel(block.Id);
            }
            foreach(var link in Model.Links.ToList())
            {
                await RemoveLinkFromJsModel(link);
            }
        }
    }
}
