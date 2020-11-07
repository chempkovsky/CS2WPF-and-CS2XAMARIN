using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewUIPropertySerializable
    {
        public string OriginalPropertyName { get; set; }
        public string ForeignKeyName { get; set; }
        public string ForeignKeyNameChain { get; set; }
        public string ViewPropertyName { get; set; }
        public string JsonPropertyName { get; set; }
        public bool IsShownInView { get; set; }
        public bool IsNewLineAfter { get; set; }
    }
}
