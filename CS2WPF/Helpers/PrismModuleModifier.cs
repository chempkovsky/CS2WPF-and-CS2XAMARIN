using EnvDTE;
using EnvDTE80;
using System;
using System.Linq;

namespace CS2WPF.Helpers
{
    [Serializable]
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
            if (codeClass == null) return "Error: Cannot find project class by destImplementedInterface=[" + destImplementedInterface + "]";
            CodeFunction codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
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
            if (codeClass == null) return "Error: Cannot find project class by destImplementedInterface=[" + destImplementedInterface + "]";
            CodeFunction codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
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
            if (codeClass == null) return "Error: Cannot find project class by destImplementedInterface=[" + destImplementedInterface + "]";
            CodeFunction codeFunction = codeClass.CodeClassMethodByName(destMethodName, methodAccess, destMethodParamTypes);
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
    }
}
