using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewPropertyOfVwSerializable : ModelViewPropertySerializable
    {
        public bool IsUsedByfilter { get; set; }
        public bool IsUsedBySorting { get; set; }
        public List<ModelViewAttributeSerializable> Attributes { get; set; }
        public List<ModelViewFAPIAttributeSerializable> FAPIAttributes { get; set; }
    }
}
