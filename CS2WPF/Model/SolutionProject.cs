using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class SolutionProject
    {
        
        public string ProjectName { get; set; }
        public string ProjectUniqueName { get; set; }
        public Project ProjectRef { get; set; }
    }
}
