using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Cdc
{
    public class CdcModel
    {
        [JsonProperty("schema")]
        public CdcSchema Schema { get; set; }
        [JsonProperty("payload")]
        public CdcPayload Payload { get; set; }
    }
}
