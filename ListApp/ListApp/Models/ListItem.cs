using System;

namespace ListApp.Models
{
    public class ListItem
    {
        public string Id { get; set; }
        public bool Checked { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
}