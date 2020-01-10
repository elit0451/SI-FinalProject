namespace NotificationService.DTOs
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Content { get; set; }

        internal AppDb Db { get; set; }

        public NotificationDTO() { }
        public NotificationDTO(AppDb db)
        {
            Db = db;
        }
    }
}