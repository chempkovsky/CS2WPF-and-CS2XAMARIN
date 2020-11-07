using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class CommonStaffSerializable
    {
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string FileProject { get; set; }
        public string FileDefaultProjectNameSpace { get; set; }
        public string FileFolder { get; set; }
        public string Extension { get; set; }
        //public string FileTypeData { get; set; }
    }
}
