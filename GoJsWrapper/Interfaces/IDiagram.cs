using GoJsWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Interfaces
{
    internal interface IDiagram
    {
        public Task AddBlock(BlockModel block);
        public Task RemoveBlock(string blockId);
        public Task UpdateBlock(BlockModel block);
        public Task MoveBlock(string blockId, string coordinates);

        public Task AddLink(LinkModel link);
        public Task RemoveLink(string linkId);

    }
}
