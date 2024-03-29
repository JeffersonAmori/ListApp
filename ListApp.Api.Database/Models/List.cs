﻿namespace ListApp.Api.Database.Models
{
    public class List
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public string OwnerEmail { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastChangedDate { get; set; }
        public List<ListItem> ListItems { get; set; } = new List<ListItem>();
    }
}
