using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Models
{
    public class DiagramModel
    {
        [JsonProperty(PropertyName = "nodeDataArray")]
        public List<BlockModel> Blocks { get; set; }

        [JsonProperty(PropertyName = "linkDataArray")]
        public List<LinkModel> Links { get; set; }
    }
}
