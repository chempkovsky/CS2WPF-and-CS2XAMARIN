using CS2WPF.Model.Serializable;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace CS2WPF.Helpers
{

    public class PrismModuleModifier
    {
        protected DTE2 _Dte;
        public PrismModuleModifier(DTE2 Dte)
        {
            _Dte = Dte;
        }

        public string UpdateMethodWithParamIdentifier(
            string destProjectName, string destImplementedInterface, string destMethodName, string[] destMethodParamTypes, string destMethodAccessType,
            string invocationParamType, string invocationMethodName, string[] invocationGenerics, string[] invocationParams)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(destProjectName)) return "Error: destProjectName is not defined";
            if (string.IsNullOrEmpty(destImplementedInterface)) return "Error: destImplementedInterface is not defined";
            if (string.IsNullOrEmpty(destMethodName)) return "Error: destMethodName is not defined";
            if (destMethodParamTypes == null) return "Error: destMethodParamTypes is not defined";
            if (string.IsNullOrEmpty(destMethodAccessType)) return "Error: destMethodAccessType is not defined";
            if (string.IsNullOrEmpty(invocationParamType)) return "Error: invocationParamType is not defined";
            if (string.IsNullOrEmpty(invocationMethodName)) return "Error: invocationMethodName is not defined";
            if (invocationGenerics == null) return "Error: invocationGenerics is not defined";
            if (invocationParams == null) return "Error: invocationParams is not defined";
            foreach (string s in destMethodParamTypes)
            {
                if (string.IsNullOrEmpty(s)) return "Error: destMethodParamTypes contains  NullOrEmpty item";
            }
            foreach (string s in invocationGenerics)
            {
                if (string.IsNullOrEmpty(s)) return "Error: invocationGenerics contains  NullOrEmpty item";
            }
            foreach (string s in invocationParams)
            {
                if (string.IsNullOrEmpty(s)) return "Error: invocationGenerics contains  NullOrEmpty item";
            }
            if (!destMethodParamTypes.Any(s => s.Equals(invocationParamType))) return "Error: invocationParamType must be in the list of destMethodParamTypes";

            vsCMAccess methodAccess = vsCMAccess.vsCMAccessPublic;
            switch (destMethodAccessType)
            {
                case "public":
                    methodAccess = vsCMAccess.vsCMAccessPublic;
                    break;
                case "protected":
                    methodAccess = vsCMAccess.vsCMAccessProtected;
                    break;
                default:
                    return "Error: destMethodAccessType is incorrect. Expected values: [public, protected]";
            }

            Project project = _Dte.ProjectByName(destProjectName);
            if (project == null) return "Error: Cannot find project by destProjectName=[" + destProjectName + "]";
            CodeClass codeClass = project.ProjectClassByImplementedInterfaceName(destImplementedInterface);
            if (codeClass == null)
            {
                if (!project.Saved) project.Save();
                codeClass = project.ProjectClassByImplementedInterfaceName(destImplementedInterface);
            }
            if (codeClass == null) return "Error: Cannot find project class by destImplementedInterface=[" + destImplementedInterface + "]";
            CodeFunction codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
            if (codeFunction == null)
            {
                if (codeClass.ProjectItem != null)
                {
                    codeClass.ProjectItem.Save();
                }
                codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
            }
            if (codeFunction == null) return "Error: Cannot find project class method by destMethodParamTypes, destMethodAccessType, destMethodName";
            string[] prms = codeFunction.GetMethodParameterNames();
            if (prms == null) return "Error: internal error. Can not get destination method params";
            if (prms.Length != destMethodParamTypes.Length) return "Error: Number of destination method params is not equal to destMethodParamTypes";
            string prmName = null;
            for (int i = 0; i < destMethodParamTypes.Length; i++)
            {
                if (invocationParamType.Equals(destMethodParamTypes[i])) { prmName = prms[i]; break; }
            }
            if (codeFunction.IsExist(prmName, invocationMethodName, invocationGenerics, invocationParams)) return "Ok";
            string lineOfCode = PrismModuleAnalyzerHelper.GenerateLineOfCode(prmName, invocationMethodName, invocationGenerics, invocationParams);
            if (codeFunction.InsertToBody(lineOfCode, true)) return "Ok";
            return "Error: internal error. Can not insert line of code into the end of body of the destMethodName";
        }
        public string UpdateMethodWithClassIdentifier(
            string destProjectName, string destImplementedInterface, string destMethodName, string[] destMethodParamTypes, string destMethodAccessType,
            string invocationClassType, string invocationMethodName, string[] invocationGenerics, string[] invocationParams)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(destProjectName)) return "Error: destProjectName is not defined";
            if (string.IsNullOrEmpty(destImplementedInterface)) return "Error: destImplementedInterface is not defined";
            if (string.IsNullOrEmpty(destMethodName)) return "Error: destMethodName is not defined";
            if (destMethodParamTypes == null) return "Error: destMethodParamTypes is not defined";
            if (string.IsNullOrEmpty(destMethodAccessType)) return "Error: destMethodAccessType is not defined";
            if (string.IsNullOrEmpty(invocationClassType)) return "Error: invocationParamType is not defined";
            if (string.IsNullOrEmpty(invocationMethodName)) return "Error: invocationMethodName is not defined";
            if (invocationGenerics == null) return "Error: invocationGenerics is not defined";
            if (invocationParams == null) return "Error: invocationParams is not defined";
            foreach (string s in destMethodParamTypes)
            {
                if (string.IsNullOrEmpty(s)) return "Error: destMethodParamTypes contains  NullOrEmpty item";
            }
            foreach (string s in invocationGenerics)
            {
                if (string.IsNullOrEmpty(s)) return "Error: invocationGenerics contains  NullOrEmpty item";
            }
            foreach (string s in invocationParams)
            {
                if (string.IsNullOrEmpty(s)) return "Error: invocationGenerics contains  NullOrEmpty item";
            }
            vsCMAccess methodAccess = vsCMAccess.vsCMAccessPublic;
            switch (destMethodAccessType)
            {
                case "public":
                    methodAccess = vsCMAccess.vsCMAccessPublic;
                    break;
                case "protected":
                    methodAccess = vsCMAccess.vsCMAccessProtected;
                    break;
                default:
                    return "Error: destMethodAccessType is incorrect. Expected values: [public, protected]";
            }
            Project project = _Dte.ProjectByName(destProjectName);
            if (project == null) return "Error: Cannot find project by destProjectName=[" + destProjectName + "]";
            CodeClass codeClass = project.ProjectClassByImplementedInterfaceName(destImplementedInterface);
            if (codeClass == null)
            {
                if (!project.Saved) project.Save();
                codeClass = project.ProjectClassByImplementedInterfaceName(destImplementedInterface);
            }
            if (codeClass == null) return "Error: Cannot find project class by destImplementedInterface=[" + destImplementedInterface + "]";
            CodeFunction codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
            if (codeFunction == null)
            {
                if (codeClass.ProjectItem != null)
                {
                    codeClass.ProjectItem.Save();
                }
                codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
            }
            if (codeFunction == null) return "Error: Cannot find project class method by destMethodParamTypes, destMethodAccessType, destMethodName";
            if (codeFunction.IsExist(invocationClassType, invocationMethodName, invocationGenerics, invocationParams)) return "Ok";
            string lineOfCode = PrismModuleAnalyzerHelper.GenerateLineOfCode(invocationClassType, invocationMethodName, invocationGenerics, invocationParams);
            if (codeFunction.InsertToBody(lineOfCode, true)) return "Ok";
            return "Error: internal error. Can not insert line of code into the end of body of the destMethodName";
        }
        public string UpdateMethodWithVarIdentifier(
            string destProjectName, string destImplementedInterface, string destMethodName, string[] destMethodParamTypes, string destMethodAccessType, string destMethodParamTypeForVar,
            string invocationVarType, string invocationMethodName, string[] invocationGenerics, string[] invocationParams)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(destProjectName)) return "Error: destProjectName is not defined";
            if (string.IsNullOrEmpty(destImplementedInterface)) return "Error: destImplementedInterface is not defined";
            if (string.IsNullOrEmpty(destMethodName)) return "Error: destMethodName is not defined";
            if (destMethodParamTypes == null) return "Error: destMethodParamTypes is not defined";
            if (string.IsNullOrEmpty(destMethodAccessType)) return "Error: destMethodAccessType is not defined";
            if (string.IsNullOrEmpty(invocationVarType)) return "Error: invocationVarType is not defined";
            if (string.IsNullOrEmpty(invocationMethodName)) return "Error: invocationMethodName is not defined";
            if (invocationGenerics == null) return "Error: invocationGenerics is not defined";
            if (invocationParams == null) return "Error: invocationParams is not defined";
            foreach (string s in destMethodParamTypes)
            {
                if (string.IsNullOrEmpty(s)) return "Error: destMethodParamTypes contains  NullOrEmpty item";
            }
            foreach (string s in invocationGenerics)
            {
                if (string.IsNullOrEmpty(s)) return "Error: invocationGenerics contains  NullOrEmpty item";
            }
            foreach (string s in invocationParams)
            {
                if (string.IsNullOrEmpty(s)) return "Error: invocationGenerics contains  NullOrEmpty item";
            }
            vsCMAccess methodAccess = vsCMAccess.vsCMAccessPublic;
            switch (destMethodAccessType)
            {
                case "public":
                    methodAccess = vsCMAccess.vsCMAccessPublic;
                    break;
                case "protected":
                    methodAccess = vsCMAccess.vsCMAccessProtected;
                    break;
                default:
                    return "Error: destMethodAccessType is incorrect. Expected values: [public, protected]";
            }
            if (!destMethodParamTypes.Any(s => s.Equals(destMethodParamTypeForVar))) return "Error: destMethodParamTypeForVar must be in the list of destMethodParamTypes";

            Project project = _Dte.ProjectByName(destProjectName);
            if (project == null) return "Error: Cannot find project by destProjectName=[" + destProjectName + "]";
            CodeClass codeClass = project.ProjectClassByImplementedInterfaceName(destImplementedInterface);
            if (codeClass == null)
            {
                if (!project.Saved) project.Save();
                codeClass = project.ProjectClassByImplementedInterfaceName(destImplementedInterface);
            }
            if (codeClass == null) return "Error: Cannot find project class by destImplementedInterface=[" + destImplementedInterface + "]";
            CodeFunction codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
            if (codeFunction == null)
            {
                if (codeClass.ProjectItem != null)
                {
                    codeClass.ProjectItem.Save();
                }
                codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
            }
            if (codeFunction == null) return "Error: Cannot find project class method by destMethodParamTypes, destMethodAccessType, destMethodName";
            string varName = codeFunction.GetVariableOfType(invocationVarType);
            if (string.IsNullOrEmpty(varName))
            {
                varName = "regionManager";
                string[] prms = codeFunction.GetMethodParameterNames();
                if (prms == null) return "Error: internal error. Can not get destination method params";
                if (prms.Length != destMethodParamTypes.Length) return "Error: Number of destination method params is not equal to destMethodParamTypes";
                string prmName = null;
                for (int i = 0; i < destMethodParamTypes.Length; i++)
                {
                    if (destMethodParamTypeForVar.Equals(destMethodParamTypes[i])) { prmName = prms[i]; break; }
                }
                if (!codeFunction.InsertToBody(invocationVarType + " " + varName + " = " + prmName + ".Resolve(typeof(" + invocationVarType + ")) as " + invocationVarType + ";", false))
                    return "Error: internal error. Can not insert var declaration into destination method";
            }
            if (codeFunction.IsExist(varName, invocationMethodName, invocationGenerics, invocationParams)) return "Ok";
            string lineOfCode = PrismModuleAnalyzerHelper.GenerateLineOfCode(varName, invocationMethodName, invocationGenerics, invocationParams);
            if (codeFunction.InsertToBody(lineOfCode, true)) return "Ok";
            return "Error: internal error. Can not insert line of code into the end of body of the destMethodName";
        }

        public StringBuilder FormatOutput(StringBuilder sb, PrismMMMCallItemSerializable stepItm)
        {
            StringBuilder result = null;
            if (sb == null) result = new StringBuilder(); else result = sb;
            result.AppendLine("");
            if (stepItm != null)
            {
                if (stepItm.Description != null)
                {
                    foreach (string s in stepItm.Description) result.AppendLine(s);
                }
                if (stepItm.StepDescription != null) result.AppendLine(stepItm.StepDescription);
                result.AppendLine("Result:");
                if (string.IsNullOrEmpty(stepItm.Result)) result.AppendLine("Result is not defined"); else result.AppendLine(stepItm.Result);
            }
            return result;
        }
        public string ExecuteJsonScript(string jsonScript)
        {
            StringBuilder sb;
            if (string.IsNullOrEmpty(jsonScript)) sb = new StringBuilder(); else sb = new StringBuilder(jsonScript);
            sb.AppendLine("==============================================================================");
            sb.AppendLine("Deserialize json Script");
            PrismMMMCallsListSerializable steps = null;
            try
            {
                steps = JsonConvert.DeserializeObject<PrismMMMCallsListSerializable>(jsonScript);
                sb.AppendLine("Deserialize json Script: Done");
                if (steps == null)
                {
                    sb.AppendLine("Result:");
                    sb.AppendLine(" The list of PrismMMMCallsListSerializable steps is epmty.");
                    return sb.ToString();
                }
                if (steps.PrismMMMCallItems == null)
                {
                    sb.AppendLine("Result:");
                    sb.AppendLine(" The list of PrismMMMCallsListSerializable steps is epmty.");
                    return sb.ToString();
                }
                if (steps.PrismMMMCallItems.Count < 1)
                {
                    sb.AppendLine("Result:");
                    sb.AppendLine(" The list of PrismMMMCallsListSerializable steps is epmty.");
                    return sb.ToString();
                }
            }
            catch (Exception e)
            {
                sb.AppendLine("Error:");
                sb.AppendLine(e.Message);
                return sb.ToString();
            }
            foreach (PrismMMMCallItemSerializable stepItm in steps.PrismMMMCallItems)
            {
                switch (stepItm.MethodName)
                {
                    case "UpdateMethodWithParamIdentifier":
                        try
                        {
                            stepItm.Result =
                                UpdateMethodWithParamIdentifier(
                                    stepItm.DestProjectName, stepItm.DestImplementedInterface, stepItm.DestMethodName,
                                    stepItm.DestMethodParamTypes == null ? new string[] { } : stepItm.DestMethodParamTypes,
                                    stepItm.DestMethodAccessType,
                                    stepItm.InvocationParamType, stepItm.InvocationMethodName,
                                    stepItm.InvocationGenerics == null ? new string[] { } : stepItm.InvocationGenerics,
                                    stepItm.InvocationParams == null ? new string[] { } : stepItm.InvocationParams);
                        }
                        catch (Exception e)
                        {
                            stepItm.Result = "Exception thrown: " + e.Message;
                        }
                        break;
                    case "UpdateMethodWithClassIdentifier":
                        try
                        {
                            stepItm.Result =
                            UpdateMethodWithClassIdentifier(
                                stepItm.DestProjectName, stepItm.DestImplementedInterface, stepItm.DestMethodName,
                                stepItm.DestMethodParamTypes == null ? new string[] { } : stepItm.DestMethodParamTypes,
                                stepItm.DestMethodAccessType,
                                stepItm.InvocationClassType, stepItm.InvocationMethodName,
                                stepItm.InvocationGenerics == null ? new string[] { } : stepItm.InvocationGenerics,
                                stepItm.InvocationParams == null ? new string[] { } : stepItm.InvocationParams);
                        }
                        catch (Exception e)
                        {
                            stepItm.Result = "Exception thrown: " + e.Message;
                        }
                        break;
                    case "UpdateMethodWithVarIdentifier":
                        try
                        {
                            stepItm.Result =
                            UpdateMethodWithVarIdentifier(
                                stepItm.DestProjectName, stepItm.DestImplementedInterface, stepItm.DestMethodName,
                                stepItm.DestMethodParamTypes == null ? new string[] { } : stepItm.DestMethodParamTypes,
                                stepItm.DestMethodAccessType,
                                stepItm.DestMethodParamTypeForVar, stepItm.InvocationVarType,
                                stepItm.InvocationMethodName,
                                stepItm.InvocationGenerics == null ? new string[] { } : stepItm.InvocationGenerics,
                                stepItm.InvocationParams == null ? new string[] { } : stepItm.InvocationParams);
                        }
                        catch (Exception e)
                        {
                            stepItm.Result = "Exception thrown: " + e.Message;
                        }
                        break;
                    default:
                        stepItm.Result = "Error: Unknown MethodName";
                        break;
                }
                sb = FormatOutput(sb, stepItm);
            }

            return sb.ToString();
        }
    }
}
