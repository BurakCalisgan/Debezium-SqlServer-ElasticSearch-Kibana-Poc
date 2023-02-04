using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Indices
{
    public class IndexErrorLog : ElasticBaseIndex
    {
        public string ErrorTitle { get; set; }
        public string Description { get; set; }
        public DateTime? ErrorDate { get; set; }
        public string ErrorCorelationId { get; set; }
        public int ErrorLevel { get; set; }
    }
}
