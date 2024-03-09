using GoJsWrapper.Interfaces;
using GoJsWrapper.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace GoJsWrapper
{
    public class Palette : IPalette
    {
        [JsonProperty(PropertyName = "nodeDataArray")]
        public IEnumerable<UnitModel> Model { get; private set; }
        private readonly IJSRuntime _jsRuntime;
        public Palette(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Model = new List<UnitModel>();
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
            catch (Exception ex)
            {

            }
        }
        public async Task AddBlock(UnitModel block)
        {
            if (ValidateNewBlock(block))
            {
                var validatedBlockJson = JsonConvert.SerializeObject(block);
                await _jsRuntime.InvokeAsync<string>("addNewPaletteBlock", validatedBlockJson);
            }
        }

        private bool ValidateNewBlock(UnitModel newBlock)
        {
            if(newBlock == null || newBlock.Category == null || newBlock.Id == null)
                return false;

            if (!Model.Any(e=>e.Category == newBlock.Category))
            {
                return true;    
            }
            return false;
        }

        public async Task RemoveBlock(string blockId)
        {
            var block = Model.FirstOrDefault(e => e.Id == blockId);
            if (block != null)
            {
                await _jsRuntime.InvokeAsync<string>("removePaletteBlock", block.Id);
            }
        }
    }
}