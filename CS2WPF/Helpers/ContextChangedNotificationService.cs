using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Helpers
{
    public class ContextChangedNotificationService
    {
        public delegate void ContextChangedNotification(Object sender);
        public class ContextChangedService
        {
            public event ContextChangedNotification ContextChanged;
            public void DoNotify(Object sender)
            {
                if (ContextChanged != null)
                    ContextChanged(sender);
            }
        }
    }
}
