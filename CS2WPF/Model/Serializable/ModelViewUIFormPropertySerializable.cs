using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewUIFormPropertySerializable : ModelViewUIPropertySerializable
    {
        public InputTypeEnum InputTypeWhenAdd { get; set; }
        public InputTypeEnum InputTypeWhenUpdate { get; set; }
        public InputTypeEnum InputTypeWhenDelete { get; set; }
        public string ForeifKeyViewNameForAdd { get; set; }
        public string ForeifKeyViewNameForUpd { get; set; }
        public string ForeifKeyViewNameForDel { get; set; }
    }
}
