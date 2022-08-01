using System;
using System.Collections.Generic;

namespace CS2WPF.Model.Serializable
{
    [Serializable]
    public class ModelViewUniqueKeySerializable
    {
        public string UniqueKeyName { get; set; }
        public bool IsPrimary { get; set; }
        public InfoSourceEnum KeySource { get; set; } = InfoSourceEnum.ByOnModelCreating;
        public List<ModelViewKeyPropertySerializable> UniqueKeyProperties { get; set; }
    }
}
