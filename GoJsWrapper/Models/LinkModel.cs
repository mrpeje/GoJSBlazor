using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Models
{
    public class LinkModel
    {
        [JsonProperty(PropertyName = "key")]
        public string Id { get; set; } = "1";
        [JsonProperty(PropertyName = "from")]
        public string FromBlock { get; set; }
        [JsonProperty(PropertyName = "to")]
        public string ToBlock{ get; set; }
        [JsonProperty(PropertyName = "fromPort")]
        public string FromPort { get; set; }
        [JsonProperty(PropertyName = "toPort")]
        public string ToPort { get; set; }
        [JsonProperty(PropertyName = "points")]
        public float[] Points {  get; set; }
    }
}
