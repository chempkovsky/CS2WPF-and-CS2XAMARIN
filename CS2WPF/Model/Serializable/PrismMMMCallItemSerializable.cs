using System;
using System.Collections.Generic;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class PrismMMMCallItemSerializable
    {
        public string[] Description { get; set; }
        public string MethodName { get; set; }
        public string DestProjectName { get; set; }
        public string DestImplementedInterface { get; set; }
        public string DestMethodName { get; set; }
        public string[] DestMethodParamTypes { get; set; }
        public string DestMethodAccessType { get; set; }
        public string DestMethodParamTypeForVar { get; set; }
        public string InvocationVarType { get; set; }
        public string InvocationParamType { get; set; }
        public string InvocationClassType { get; set; }
        public string InvocationMethodName { get; set; }
        public string[] InvocationGenerics { get; set; }
        public string[] InvocationParams { get; set; }
        public string StepDescription { get; set; }
        public string Result { get; set; }
    }
}
