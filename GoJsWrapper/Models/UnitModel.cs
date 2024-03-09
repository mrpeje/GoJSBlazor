using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Models
{
    public class UnitModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name {get; set;}

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "key")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "leftArray")]
        public List<Port> InputPorts { get; set; }

        [JsonProperty(PropertyName = "rightArray")]
        public List<Port> OutputPorts { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public string Coordinates { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
    public class Port
    {
        [JsonProperty(PropertyName = "portId")] 
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "portColor")]
        public string Color { get; set; }
    }

}
