using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Cdc
{
    public class CdcPayload
    {
        [JsonProperty("op")]
        public string op { get; set; }
        [JsonProperty("before")]
        public CdcErrorLog Before { get; set; }
        [JsonProperty("after")]
        public CdcErrorLog After { get; set; }
    }
}
