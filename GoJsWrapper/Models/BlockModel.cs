using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Models
{
    public class BlockModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name {get; set;}

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "key")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "leftArray")]
        public List<PortModel> InputPorts { get; set; }

        [JsonProperty(PropertyName = "rightArray")]
        public List<PortModel> OutputPorts { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public string Coordinates { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }


}
