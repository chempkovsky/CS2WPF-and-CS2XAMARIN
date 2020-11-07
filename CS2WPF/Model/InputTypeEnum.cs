using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public enum InputTypeEnum : int
    {
        Default = 0,
        ReadOnly = 1,
        Combo = 2,
        Typeahead = 3,
        SearchDialog = 4,
        Hidden = 5,
    }
}
