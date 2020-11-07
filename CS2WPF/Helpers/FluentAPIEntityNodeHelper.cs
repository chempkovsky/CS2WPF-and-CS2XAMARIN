using CS2WPF.Model.AnalyzeOnModelCreating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Helpers
{
    public static class FluentAPIEntityNodeHelper
    {
        public static bool IsSatisfiedTheFilter(this FluentAPIEntityNode faen, List<FluentAPIEntityNode> filter)
        {
            if (faen == null) return false;
            if (filter == null) return true;
            bool isSatisfied = false;
            foreach (FluentAPIEntityNode flt in filter)
            {
                bool isBroken = false;
                if (flt.Methods != null)
                {
                    if ((flt.Methods.Count > 0) && (faen.Methods == null))
                    {
                        isBroken = true;
                    }
                    else
                    {
                        if (flt.Methods.Count > faen.Methods.Count)
                        {
                            isBroken = true;
                        }
                        else
                        {
                            for (int i = 0; i < flt.Methods.Count; i++)
                            {
                                if (!string.Equals(flt.Methods[i].MethodName, faen.Methods[i].MethodName))
                                {
                                    isBroken = true;
                                    break;
                                }
                                if (flt.Methods[i].MethodArguments != null)
                                {
                                    if((flt.Methods[i].MethodArguments.Count > 0) && (faen.Methods[i].MethodArguments == null))
                                    {
                                        isBroken = true;
                                        break;
                                    }
                                    if (flt.Methods[i].MethodArguments.Count > faen.Methods[i].MethodArguments.Count)
                                    {
                                        isBroken = true;
                                        break;
                                    }
                                    for (int k = 0; k < flt.Methods[i].MethodArguments.Count; k++)
                                    {
                                        if (!string.Equals(flt.Methods[i].MethodArguments[k], faen.Methods[i].MethodArguments[k]))
                                        {
                                            isBroken = true;
                                            break;
                                        }
                                    }
                                    if (isBroken) break;
                                }
                            }
                        }
                    }
                }
                if (!isBroken)
                {
                    isSatisfied = true;
                    break;
                }
            }
            return isSatisfied;
        }
    }
}
