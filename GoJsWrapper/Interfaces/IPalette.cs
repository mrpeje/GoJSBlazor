using GoJsWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Interfaces
{
    internal interface IPalette
    {
        public Task AddBlock(UnitModel block);
        public Task RemoveBlock(string blockId);

    }
}
