using CS2ANGULAR.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CS2ANGULAR.ViewModel
{
    public class ClassFiledSelectorViewModel: PropertySelectorViewModel
    {
        public ClassFiledSelectorViewModel() : base()
        {
        }
        public ClassFiledSelectorViewModel(bool updateDependent) : base(updateDependent)
        {
        }
        public override void DoUpdateNested()
        {
            if (ForeigKeyParentProperties == null) return;
            foreach (PropertySelectorViewModel itm in ForeigKeyParentProperties)
            {
                itm.ChildForeignKeyPrefix = ForeignKeyPrefix;
            }
        }

        public bool HasRequiredAttribute { get; set; }
        public override string DisplayName
        {
            get
            {
                return OriginalPropertyName;
            }
        }
        public string ViewModelFieldName { get; set; }
        public string JsonPropertyFieldName { get; set; }
        public override void UpdateViewModelFieldName()
        {
            ViewModelFieldName = _foreignKeyPrefix + _originalPropertyName;
            OnPropertyChanged("ViewModelFieldName");
            JsonPropertyFieldName = _foreignKeyPrefix + _originalPropertyName;
            OnPropertyChanged("JsonPropertyFieldName");
        } 
        public bool IsKeyField { get; set; }
        public int FieldOrder { get; set; }
        public bool IsForeignKeyField { get; set; }
        public ViewModelForeigKeyUITypeEnum ForeignKeyUIType { get; set; }
        public bool IsUIHidden { get; set; }
        public string LookUpViewName { get; set; }
        public int LookUpId { get; set; }
        public string LookUpFieldName { get; set; }
        public override void UpdateNestedIncludeInView(bool newVal)
        {
        }

    }
}
