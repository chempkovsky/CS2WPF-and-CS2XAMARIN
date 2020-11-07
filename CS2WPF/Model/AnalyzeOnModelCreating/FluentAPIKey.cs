using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.AnalyzeOnModelCreating
{
    public class FluentAPIKey
    {
        public int SourceCount { get; set; } = 0;
        public InfoSourceEnum KeySource { get; set; } = InfoSourceEnum.ByConvention;
        public string KeySourceDisplay { get { return KeySource.ToString("g"); } }
        public List<FluentAPIProperty> KeyProperties { get; set; }
    }
}
