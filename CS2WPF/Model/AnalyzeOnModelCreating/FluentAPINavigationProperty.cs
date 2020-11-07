using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.AnalyzeOnModelCreating
{
    public class FluentAPINavigationProperty: FluentAPIProperty
    {
        public bool IsCollection { get; set; }
        public string UnderlyingTypeName { get; set; }
        public string FullTypeName { get; set; }
    }
}
