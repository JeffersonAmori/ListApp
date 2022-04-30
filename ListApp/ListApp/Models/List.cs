﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ListApp.Models
{
    public class List
    {
        public string ListId { get; set; }
        public string Name { get; set; }
        public List<ListItem> ListItems { get; set; } = new List<ListItem>();
    }
}
