using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewSerializable
    {
        public string ViewName { get; set; }
        public string PageViewName { get; set; }
        public string RootEntityDbContextPropertyName { get; set; }
        public string RootEntityClassName { get; set; }
        public string RootEntityFullClassName { get; set; }
        public string RootEntityUniqueProjectName { get; set; }
        public string ViewProject { get; set; }
        public string ViewDefaultProjectNameSpace { get; set; }
        public System.String ViewFolder { get; set; }
        public System.Boolean GenerateJSonAttribute { get; set; }
        public List<ModelViewPropertyOfVwSerializable> ScalarProperties { get; set; }
        public List<ModelViewForeignKeySerializable> ForeignKeys { get; set; }
        public List<ModelViewKeyPropertySerializable> PrimaryKeyProperties { get; set; }
        public List<ModelViewKeyPropertySerializable> AllProperties { get; set; }
        public string WebApiServiceName { get; set; }
        public string WebApiServiceProject { get; set; }
        public string WebApiServiceDefaultProjectNameSpace { get; set; }
        public string WebApiServiceFolder { get; set; }
        public bool IsWebApiSelectAll { get; set; }
        public bool IsWebApiSelectManyWithPagination { get; set; }
        public bool IsWebApiSelectOneByPrimarykey { get; set; }
        public bool IsWebApiAdd { get; set; }
        public bool IsWebApiUpdate { get; set; }
        public bool IsWebApiDelete { get; set; }
        public List<CommonStaffSerializable> CommonStaffs { get; set; }
        public List<ModelViewUIFormPropertySerializable> UIFormProperties { get; set; }
        public List<ModelViewUIListPropertySerializable> UIListProperties { get; set; }
    }
}
