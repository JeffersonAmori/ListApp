using System.Collections.Generic;

namespace ListApp.Models
{
    public class GroupedList
    {
        public string Name { get; set; }
        public IEnumerable<List> Lists { get; set; }
    }
}
