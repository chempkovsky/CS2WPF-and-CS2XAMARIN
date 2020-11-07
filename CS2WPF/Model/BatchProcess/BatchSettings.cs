using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.BatchProcess
{
    [Serializable]
    public class BatchSettings
    {
        public List<string> Description { get; set; }
        public List<BatchItem> BatchItems { get; set; }
    }
}
