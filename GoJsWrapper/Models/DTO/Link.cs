using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.Models.DTO
{
    public class Link
    {
        public string FromBlock { get; set; }

        public string ToBlock { get; set; }

        public string FromPort { get; set; }

        public string ToPort { get; set; }
    }
}
