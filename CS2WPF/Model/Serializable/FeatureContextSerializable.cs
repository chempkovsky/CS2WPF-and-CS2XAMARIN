using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class FeatureContextSerializable
    {
        public List<FeatureSerializable> Features { get; set; }
    }
}
