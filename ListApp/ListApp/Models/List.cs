using System;
using System.Collections.Generic;

namespace ListApp.Models
{
    public interface IListVisualItem { }
    public class ListHeader : IListVisualItem { }
    public class ListFooter : IListVisualItem { }
    public class ListGroupHeader : IListVisualItem { public string Name { get; set; } }

    public class List : IListVisualItem
    {
        public string ListId { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPermanentlyDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastChangedDate { get; set; }
        public List<ListItem> ListItems { get; set; } = new List<ListItem>();
    }
}
