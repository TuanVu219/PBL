using PBL3Hos.Migrations;

namespace PBL3Hos.ViewModel
{
    public class NotificationViewModel
    {
        public List<AppointmentCancel> Notifications { get; set; }
        public int UnseenCount { get; set; }
    }
}
