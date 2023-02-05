using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Cdc
{
    public class CdcSchema
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
