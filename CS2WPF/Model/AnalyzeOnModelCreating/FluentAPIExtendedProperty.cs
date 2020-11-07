using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.AnalyzeOnModelCreating
{
    public class FluentAPIExtendedProperty: FluentAPIProperty
    {
        public bool IsNullable { get; set; }
        public bool IsRequired { get; set; }
        public string UnderlyingTypeName { get; set; }
        public string TypeFullName { get; set; }
    }
}
