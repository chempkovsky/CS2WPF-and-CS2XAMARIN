using System.Collections.Generic;

namespace CS2ANGULAR.Model
{
    public class LookUpViewModel
    {
        public string LookUpViewModelName { get; set; }
        public string JSLookUpViewModelName { get; set; }
        public IList<LookUpClassField> LookUpClassFields { get; set; }
    }
}
