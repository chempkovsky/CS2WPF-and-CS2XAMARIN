using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.AnalyzeOnModelCreating
{
    public class FluentAPIForeignKey
    {
        public string EntityName { get; set; }
        public string EntityFullName { get; set; }
        public string NavigationName { get; set; }
        public string NavigationEntityName { get; set; }
        public string NavigationEntityFullName { get; set; }
        public string InverseNavigationName { get; set; }
        public string GenericForeignKeyClassName { get; set; }
        public List<FluentAPIProperty> PrincipalKeyProps { get; set; }
        public List<FluentAPIProperty> ForeignKeyProps { get; set; }
        public InfoSourceEnum ForeignKeySource { get; set; } = InfoSourceEnum.ByConvention;
        public InfoSourceEnum PrincipalKeySource { get; set; } = InfoSourceEnum.ByConvention;
        public InfoSourceEnum InverseNavigationSource { get; set; } = InfoSourceEnum.ByConvention;
        public int PrincipalKeySourceCount { get; set; } = 0;
        public int ForeignKeySourceCount { get; set; } = 0;
        public int SourceCount { get; set; } = 0;
        public CodeElement CodeElementEntityRef { get; set; }
        public CodeElement CodeElementNavigationRef { get; set; }
        public NavigationTypeEnum NavigationType { get; set; }
        public bool HasErrors { get; set; }
        public string ErrorsText { get; set; }
        public bool IsCascadeDelete { get; set; }
    }
}
