using GoJsWrapper.Interfaces;
using GoJsWrapper.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace GoJsWrapper
{
    public class Palette
    {
        [JsonProperty(PropertyName = "nodeDataArray")]
        private IEnumerable<BlockModel> Model { get; set; }
        private readonly IJSRuntime _jsRuntime;
        public Palette(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Model = new List<BlockModel>();
        }
        internal BlockModel FindBlock(string id)
        {
            return Model.FirstOrDefault(e => e.Id == id);
        }
        internal BlockModel? GetPaletteBlockByCategory(string category)
        {
            return Model.FirstOrDefault(e => e.Category == category);
        }
        internal void UpdatePaletteModel(string model)
        {
            try
            {
                var parsedModel = JsonConvert.DeserializeObject<Palette>(model);
                if (parsedModel != null)
                {
                    Model = parsedModel.Model;
                }
            }
            catch (JsonSerializationException ex)
            {

            }
        }

        internal async Task AddBlockToJsModel(BlockModel block)
        {
            var validatedBlockJson = JsonConvert.SerializeObject(block);
            await _jsRuntime.InvokeAsync<string>("addNewPaletteBlock", validatedBlockJson);
        }

        internal bool ValidateNewBlock(BlockModel newBlock)
        {
            if(newBlock == null || newBlock.Category == null || newBlock.Id == null)
                return false;

            if (!Model.Any(e=>e.Category == newBlock.Category))
            {
                return true;    
            }
            return false;
        }

        internal async Task RemoveBlockFromJsModel(string blockId)
        {
            await _jsRuntime.InvokeAsync<string>("removePaletteBlock", blockId);
        }
    }
}