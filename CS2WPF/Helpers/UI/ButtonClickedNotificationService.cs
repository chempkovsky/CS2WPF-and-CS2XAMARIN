using System;

namespace CS2WPF.Helpers.UI
{
    public delegate void ButtonClickedNotification(Object sender);
    public class ButtonClickedNotificationService
    {
        public event ButtonClickedNotification ButtonClickedEvent;
        public void DoNotify(Object sender)
        {
            if (ButtonClickedEvent != null)
                ButtonClickedEvent(sender);
        }
    }
}
