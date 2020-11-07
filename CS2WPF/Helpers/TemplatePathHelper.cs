using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Helpers
{
    public static class TemplatePathHelper
    {
        public static string GetTemplatePath()
        {
            string str = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(str), "Templates");
        }
    }
}
