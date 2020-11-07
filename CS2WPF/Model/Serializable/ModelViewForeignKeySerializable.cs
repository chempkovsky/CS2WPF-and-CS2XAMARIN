using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewForeignKeySerializable
    {
        public string NavigationName { get; set; }
        public string InverseNavigationName { get; set; }
        public string EntityName { get; set; }
        public string EntityFullName { get; set; }
        public string EntityUniqueProjectName { get; set; }
        public string NavigationEntityName { get; set; }
        public string NavigationEntityFullName { get; set; }
        public string NavigationEntityUniqueProjectName { get; set; }
        public NavigationTypeEnum NavigationType { get; set; }
        public InfoSourceEnum ForeignKeySource { get; set; } = InfoSourceEnum.ByConvention;
        public InfoSourceEnum PrincipalKeySource { get; set; } = InfoSourceEnum.ByConvention;
        public InfoSourceEnum InverseNavigationSource { get; set; } = InfoSourceEnum.ByConvention;
        public List<ModelViewKeyPropertySerializable> PrincipalKeyProps { get; set; }
        public List<ModelViewKeyPropertySerializable> ForeignKeyProps { get; set; }
        public bool IsCascadeDelete { get; set; }
        public string ViewName { get; set; }
        public string ForeignKeyPrefix { get; set; }
        public List<ModelViewPropertyOfFkSerializable> ScalarProperties { get; set; }
    }
}
