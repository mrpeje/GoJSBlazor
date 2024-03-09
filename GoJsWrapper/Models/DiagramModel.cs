using GoJsWrapper.Interfaces;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Models
{
    public class DiagramModel : IDiagramModel
    {
        private readonly IJSRuntime _jsRuntime;
        public DiagramModel(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Blocks = new List<UnitModel>();
            Links = new List<LinkModel>();
        }

        [JsonProperty(PropertyName = "nodeDataArray")]
        public IEnumerable<UnitModel> Blocks { get; private set; }

        [JsonProperty(PropertyName = "linkDataArray")]
        public IEnumerable<LinkModel> Links { get; private set; }

        internal void UpdateDiagramModel(string model)
        {
            try
            {
                var parsedModel = JsonConvert.DeserializeObject<DiagramModel>(model);
                if (parsedModel != null)
                {
                    Blocks = parsedModel.Blocks;
                    Links = parsedModel.Links;
                }
            }
            catch (Exception ex)
            {

            }
        }







        public void AddBlock()
        {

        }
        public void ValidateNewLink(LinkModel newLink)
        {
            var from = Blocks.FirstOrDefault(e => e.Id.Equals(newLink.From));
            if(from == null)
            {
                throw new BlockNotFoundException($"Block {newLink.From} not found"); 
            }
            var to = Blocks.FirstOrDefault(e => e.Id.Equals(newLink.To));
            if (to == null)
            {
                throw new BlockNotFoundException($"Block {newLink.To} not found");
            }
            var portFrom = from.OutputPorts.FirstOrDefault(e=>e.Id.Equals(newLink.fromPort));
            if (portFrom == null)
            {
                throw new PortNotFoundException($"Port {newLink.fromPort} not found");
            }
            var portTo = to.InputPorts.FirstOrDefault(e => e.Id.Equals(newLink.toPort));
            if (portTo == null)
            {
                throw new PortNotFoundException($"Port {newLink.toPort} not found");
            }
        }
        public LinkModel? GetLink(LinkModel link)
        {
           return Links.FirstOrDefault(/*e => e.From == link.From &&
                                        e.To == link.To &&
                                        e.fromPort == link.fromPort &&
                                        e.toPort == link.toPort*/);           
        }
        public UnitModel? GetBlock(string blockId)
        {
            return Blocks.FirstOrDefault(/*e => e.Id.Equals(blockId)*/);
        }
        public UnitModel ValidateNewBlock(UnitModel newBlock)
        {
            if (newBlock.InputPorts == null)
                newBlock.InputPorts = new List<Port>();
            if (newBlock.OutputPorts == null)
                newBlock.OutputPorts = new List<Port>();
            if (String.IsNullOrEmpty(newBlock.Category))
                newBlock.Category = "Category";
            return newBlock;
        }


        public async Task AddBlock(UnitModel newBlock)
        {
            var validatedBlock = ValidateNewBlock(newBlock);
            if (validatedBlock != null)
            {
                var validatedBlockJson = JsonConvert.SerializeObject(validatedBlock);
                await _jsRuntime.InvokeAsync<string>("addNewBlock", validatedBlockJson);
            }
        }

        public async Task RemoveBlock(string id)
        {
            var block = Blocks.FirstOrDefault(e => e.Id == id);
            if (block != null)
            {
                var blockJson = JsonConvert.SerializeObject(block);
                await _jsRuntime.InvokeAsync<string>("removeBlock", blockJson);
            }

        }

        public async Task UpdateBlock(UnitModel block)
        {
            var blockToUpdate = Blocks.FirstOrDefault(e => e.Id == block.Id);
            if (blockToUpdate != null)
            {
                var blockToUpdateJson = JsonConvert.SerializeObject(blockToUpdate);
                await _jsRuntime.InvokeAsync<string>("updateBlock", blockToUpdateJson);
            }
        }


        public async Task MoveBlock(string blockId, string newCoordinates)
        {
            var block = Blocks.FirstOrDefault(e=>e.Id == blockId);
            if (block != null)
            {
                var blockJson = JsonConvert.SerializeObject(block);
                block.Coordinates = newCoordinates;
                await _jsRuntime.InvokeAsync<string>("updateBlock", blockJson);
            }
        }

        public async Task AddLink(LinkModel newlink)
        {
            try
            {               
                var newlinkJson = JsonConvert.SerializeObject(newlink);
                await _jsRuntime.InvokeAsync<string>("addLink", newlinkJson);
            }
            catch (BlockNotFoundException exeptionBlockNotFound)
            {

            }
            catch (PortNotFoundException exeptionPortNotFound)
            {

            }
        }

        public async Task RemoveLink(string linkId)
        {
            var foundLink = Links.FirstOrDefault(e=>e.Id == linkId);
            if (foundLink != null)
            {
                var foundLinkJson = JsonConvert.SerializeObject(foundLink);
                await _jsRuntime.InvokeAsync<string>("deleteLink", foundLinkJson);
            }
        }


    }
    public class BlockNotFoundException : Exception
    {
        public BlockNotFoundException()
        {
        }

        public BlockNotFoundException(string message)
            : base(message)
        {
        }

        public BlockNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class PortNotFoundException : Exception
    {
        public PortNotFoundException()
        {
        }

        public PortNotFoundException(string message)
            : base(message)
        {
        }

        public PortNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
