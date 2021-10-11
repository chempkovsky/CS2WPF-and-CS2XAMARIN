using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewEntityPropertySerializable: ModelViewKeyPropertySerializable
    {
        public List<ModelViewAttributeSerializable> Attributes { get; set; }
        public List<ModelViewFAPIAttributeSerializable> FAPIAttributes { get; set; }
    }
}
