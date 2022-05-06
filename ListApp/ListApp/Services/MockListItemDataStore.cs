using ListApp.Models;
using ListApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListApp.Services
{
    public class MockListItemDataStore : IDataStore<ListItem>
    {
        readonly List<ListItem> items;

        public MockListItemDataStore()
        {
            items = new List<ListItem>()
            {
                new ListItem { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                new ListItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                new ListItem { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                new ListItem { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                new ListItem { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                new ListItem { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(ListItem item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(ListItem item)
        {
            var oldItem = items.Where((ListItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((ListItem arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<ListItem> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<ListItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}