using GoJsWrapper.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.EventInterceptors
{
    public class AddRemoveEventInterceptor
    {
        public delegate void AddRemoveLinkHandler(string id);

        public delegate void AddRemoveBlockHandler(List<BlockModel> deletedBlocks);

        public event AddRemoveLinkHandler LinkAddedEvent;
        public event AddRemoveLinkHandler LinkRemoveEvent;

        public event AddRemoveBlockHandler BlockAddedEvent;
        public event AddRemoveBlockHandler BlocksRemoveEvent;


        [JSInvokable]
        public void OnLinkAddedEvent(string id) => LinkAddedEvent?.Invoke(id);
        [JSInvokable]
        public void OnBlockAddedEvent(string jsonNewdBlocks)
        {
            var newBlocks = JsonConvert.DeserializeObject<List<BlockModel>>(jsonNewdBlocks);
            if (newBlocks != null)
                BlockAddedEvent?.Invoke(newBlocks);
        }
        [JSInvokable]
        public void OnBlocksRemoveEvent(string jsonDeletedBlocks)
        {
            var deletedBlocks = JsonConvert.DeserializeObject<List<BlockModel>>(jsonDeletedBlocks);
            if(deletedBlocks!= null)
                BlocksRemoveEvent?.Invoke(deletedBlocks);
        }
        [JSInvokable]
        public void OnLinkRemoveEvent(string id) => LinkRemoveEvent?.Invoke(id);        
    }
}
