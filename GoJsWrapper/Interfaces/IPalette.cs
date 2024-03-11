using GoJsWrapper.Models;
using GoJsWrapper.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Interfaces
{
    public interface IPalette
    {
        public Task<bool> AddPaletteBlock(Block block, List<Port> ports);
        public Task<bool> RemovePaletteBlock(string id);

    }
}
