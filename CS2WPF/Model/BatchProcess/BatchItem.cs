using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.BatchProcess
{
    [Serializable]
    public class BatchItem
    {
        public string DestinationFolder { get; set; }
        public string GeneratorType { get; set; }
        public string GeneratorSript { get; set; }
        public string ViewModel { get; set; }
    }
}
