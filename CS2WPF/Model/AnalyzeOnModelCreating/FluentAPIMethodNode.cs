using System;
using System.Collections.Generic;

namespace CS2WPF.Model.AnalyzeOnModelCreating
{
    public class FluentAPIMethodNode
    {
        public string MethodName { get; set; }
        public string GenericName { get; set; }
        public List<String> MethodArguments { get; set; }
    }
}
