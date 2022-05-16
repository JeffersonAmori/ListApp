using System.Collections.Generic;

namespace ListApp.Models
{
    public interface IListVisualItem { }
    public class ListHeader : IListVisualItem { }
    public class ListFooter : IListVisualItem { }
    public class ListGroupHeader : IListVisualItem { public string Name { get; set; } }

    public class List : IListVisualItem
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<ListItem> ListItems { get; set; } = new List<ListItem>();
    }
}
