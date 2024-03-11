using GoJsWrapper.Models;
using GoJsWrapper.Models.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Interfaces
{
    public interface IDiagram
    {
        public Task<bool> AddBlock(Block block);
        public Task<bool> RemoveBlock(string blockId);
        public Task<bool> UpdateBlock(Block blockUpdate, string blockId);
        public Task<bool> MoveBlock(Point newCoordinates, string blockId);

        public Task<bool> AddLink(Link link);
        public Task<bool> RemoveLink(Link link);

        Task<bool> AddPortToBlock(Port newPort, string blockId);
        Task<bool> RemovePortFromBlock(string portId, string blockId);
    }
}
