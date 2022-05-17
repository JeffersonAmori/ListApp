namespace ListApp.Api.Database.Models
{
    public class ListItem
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public bool Checked { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public long ListId { get; set; }
        public int Index { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastChangedDate { get; set; }
    }
}