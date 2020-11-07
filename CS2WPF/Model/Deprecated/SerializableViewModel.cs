using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;

namespace CS2ANGULAR.Model
{
    [Serializable]
    public class SerializableViewModel
    {
        public string ViewModelName { get; set; }
        public string ViewModelNameSpace { get; set; }
        public string ViewModelProjectName { get; set; }
        public string ViewModelFolderChain { get; set; }

        public string RootNodeClassName { get; set; }
        public string RootNodeNameSapce { get; set; }
        public string RootNodeProjectName { get; set; }
        public List<SerializableViewModelProperty> PrimKeys { get; set; }
        public List<SerializableViewModelProperty> Properties { get; set; }
        public List<SerializableViewModelForeignKey> ForeignKeys { get; set; }
        public bool GenerateJSonAttribute { get; set; }

        public string DestinationWebApiProjectName { get; set; }
        public string DestinationWebApiNamespace { get; set; }
        public string DestinationWebApiServiceName { get; set; }

        public string RequiredRootFields { get; set; }

        public bool IsWebApiSelectAllMethod { get; set; }
        public bool IsWebApiSelectManyWithPaginationMethod { get; set; }
        public bool IsWebApiSelectOneByPrimarykeyMethod { get; set; }
        public bool IsWebApiAddItemMethod { get; set; }
        public bool IsWebApiUpdateItemMethod { get; set; }
        public bool IsWebApiDeleteItemMethod { get; set; }


    }
}
