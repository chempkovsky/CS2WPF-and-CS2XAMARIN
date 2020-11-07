using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.BatchProcess
{
    public class GeneratorBatchStep
    {
        public string GenerateText { get; set; }
        public string GenerateError { get; set; }
        public string FileExtension { get; set; }
        public string T4TempatePath { get; set; }
    }
}
