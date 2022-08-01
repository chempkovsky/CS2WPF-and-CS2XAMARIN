using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    public class ModelViewUniqueKeyOfVwSerializable
    {
        public string UniqueKeyName { get; set; }
        public bool IsPrimary { get; set; }
        public List<ModelViewPropertyOfVwSerializable> UniqueKeyProperties { get; set; }
    }
}
