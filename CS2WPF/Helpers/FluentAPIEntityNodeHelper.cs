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
        // "faen" matches the filter if "faen" matches one of the list items (== List<FluentAPIEntityNode> filter)
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
                                    if ((flt.Methods[i].MethodArguments.Count > 0) && (faen.Methods[i].MethodArguments == null))
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
        public static bool HoldsIsRequired(this FluentAPIEntityNode faen, string propName, out bool IsReq)
        {
            IsReq = false;
            if ((faen == null) || string.IsNullOrEmpty(propName)) return false;
            if (faen.Methods == null) return false;
            bool checkProp = true;
            bool result = false;
            foreach (FluentAPIMethodNode m in faen.Methods)
            {
                if (checkProp)
                {
                    if (m.MethodName == "Property")
                    {
                        if (m.MethodArguments != null)
                            if (m.MethodArguments.Any(p => p == propName))
                                checkProp = false;
                            else
                                return false;
                    }
                    continue;
                }
                if (m.MethodName == "IsRequired")
                {
                    result = true;
                    if (m.MethodArguments != null)
                    {
                        IsReq = m.MethodArguments.Any(a => (a == System.Boolean.TrueString) || (a == "\"" + System.Boolean.TrueString + "\""));
                    }
                    else
                    {
                        IsReq = false;
                    }
                }
            }
            return result;
        }
        public static bool HoldsIsRequired(this IList<FluentAPIEntityNode> faens, string propName, out bool IsReq)
        {
            IsReq = false;
            bool result = false;
            if (faens == null) return result;
            foreach (FluentAPIEntityNode faen in faens)
            {
                bool locrslt = false;
                if (faen.HoldsIsRequired(propName, out locrslt))
                {
                    result = true;
                    IsReq = locrslt;
                }
            }
            return result;
        }
        public static bool HoldsIgnore(this FluentAPIEntityNode faen, string propName)
        {
            if ((faen == null) || string.IsNullOrEmpty(propName)) return false;
            if (faen.Methods == null) return false;
            foreach (FluentAPIMethodNode m in faen.Methods)
            {
                if (m.MethodName == "Ignore")
                {
                    if (m.MethodArguments != null)
                        if (m.MethodArguments.Any(a => (a == propName) || (a == "\"" + propName + "\"")))
                            return true;
                }
            }
            return false;
        }
        public static bool HoldsIgnore(this IList<FluentAPIEntityNode> faens, string propName)
        {
            if (faens == null) return false;
            foreach (FluentAPIEntityNode faen in faens)
            {
                if (faen.HoldsIgnore(propName))
                {
                    return true;
                }
            }
            return false;
        }
        public static FluentAPIEntityNode HoldsHasName(this IList<FluentAPIEntityNode> faens, string nameToFilter)
        {
            if ((faens == null) || (nameToFilter == null)) return null;
            int cnt = faens.Count;

            for (int i = cnt - 1; i >= 0; i--)
            {
                FluentAPIEntityNode faen = faens[i];
                if (faen.Methods == null) continue;
                FluentAPIMethodNode mthdNd = faen.Methods.FirstOrDefault(e => e.MethodName == "HasName");
                if (mthdNd == null) continue;
                if (mthdNd.MethodArguments == null) continue;
                if (mthdNd.MethodArguments.Any(e => nameToFilter.Equals(e))) return faen;
            }
            return null;
        }
    }
}
