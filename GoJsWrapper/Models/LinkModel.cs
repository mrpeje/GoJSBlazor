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

        [JsonProperty(PropertyName = "from")]
        public int FromBlock { get; set; }
        [JsonProperty(PropertyName = "to")]
        public int ToBlock { get; set; }
        [JsonProperty(PropertyName = "fromPort")]
        public string FromPort { get; set; }
        [JsonProperty(PropertyName = "toPort")]
        public string ToPort { get; set; }
        [JsonProperty(PropertyName = "key")]
        public int Id { get; set; } = -1;
        //[JsonProperty(PropertyName = "points")]
        //public double[] Points {  get; set; }
    }
}
