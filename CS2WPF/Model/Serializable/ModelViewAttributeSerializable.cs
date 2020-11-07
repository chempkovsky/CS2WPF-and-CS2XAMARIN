using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewAttributeSerializable
    {
        public string AttrName { get; set; }
        public string AttrFullName { get; set; }
        public List<ModelViewAttributePropertySerializable> VaueProperties { get; set; }
    }
}
