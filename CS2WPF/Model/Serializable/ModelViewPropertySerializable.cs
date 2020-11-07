using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewPropertySerializable
    {
        public string OriginalPropertyName { get; set; }
        public string TypeFullName { get; set; }
        public bool IsNullable { get; set; }
        public bool IsRequired { get; set; }
        public bool IsRequiredInView { get; set; }
        public string UnderlyingTypeName { get; set; }
        public bool IsSelected { get; set; }
        public string ForeignKeyName { get; set; }
        public string ForeignKeyNameChain { get; set; }
        public string ViewPropertyName { get; set; }
        public string JsonPropertyName { get; set; }
    }
}
