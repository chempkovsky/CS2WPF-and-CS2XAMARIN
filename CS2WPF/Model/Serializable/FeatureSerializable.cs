using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class FeatureSerializable
    {
        public string FeatureName { get; set; }
        public List<FeatureItemSerializable> FeatureItems { get; set; }
        public List<CommonStaffSerializable> CommonStaffs { get; set; }
    }
}
