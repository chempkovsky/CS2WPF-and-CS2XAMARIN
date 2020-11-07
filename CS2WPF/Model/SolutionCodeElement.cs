using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class SolutionCodeElement
    {
        public int Order { get; set; }
        public string CodeElementName { get; set; }
        public string CodeElementFullName { get; set; }
        public CodeElement CodeElementRef { get; set; }
    }
}
