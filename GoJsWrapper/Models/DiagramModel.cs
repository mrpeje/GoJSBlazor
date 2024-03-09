using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Models
{
    public class DiagramModel
    {
        [JsonProperty(PropertyName = "nodeDataArray")]
        public IEnumerable<UnitModel> Blocks { get; private set; }
        [JsonProperty(PropertyName = "linkDataArray")]
        public IEnumerable<LinkModel> Links { get; private set; }
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
            if (newBlock.Id == null)
                newBlock.Id = "1";
                return newBlock;
        }

        internal UnitModel MoveBlock(string blockId, string newCoordinates)
        {
            var block = Blocks.FirstOrDefault(e => e.Id == blockId);
            if (block == null)
                return null;
            block.Coordinates = newCoordinates;
            return block;
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
