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
        public int Key { get; set; }
        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
        [JsonProperty(PropertyName = "fromPort")]
        public string fromPort { get; set; }
        [JsonProperty(PropertyName = "toPort")]
        public string toPort { get; set; }
        [JsonProperty(PropertyName = "points")]
        public float[] Points {  get; set; }
    }
}
