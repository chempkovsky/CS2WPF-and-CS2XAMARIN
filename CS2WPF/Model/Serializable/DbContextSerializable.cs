using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class DbContextSerializable
    {
        public string DbContextClassName { get; set; }
        public string DbContextFullClassName { get; set; }
        public string DbContextProjectUniqueName { get; set; }
        public List<ModelViewSerializable> ModelViews { get; set; }
        public List<CommonStaffSerializable> CommonStaffs { get; set; }
    }
}
