using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewAttributePropertySerializable
    {
        public string PropName { get; set; }
        public string PropValue { get; set; }
    }
}
