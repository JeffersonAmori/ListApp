using ListApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListApp.Services
{
    public class MockListDataStore : IDataStore<List>
    {
        readonly List<List> lists;

        public MockListDataStore()
        {
            lists = new List<List>()
            {
                new List { ListId = Guid.NewGuid().ToString(), Name = "First item" },
                new List { ListId = Guid.NewGuid().ToString(), Name = "Second item" },
                new List { ListId = Guid.NewGuid().ToString(), Name = "Third item" },
                new List { ListId = Guid.NewGuid().ToString(), Name = "Fourth item" },
                new List { ListId = Guid.NewGuid().ToString(), Name = "Fifth item" },
                new List { ListId = Guid.NewGuid().ToString(), Name = "Sixth item" }
            };
        }

        public async Task<bool> AddItemAsync(List item)
        {
            lists.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(List item)
        {
            var oldItem = lists.Where((List arg) => arg.ListId == item.ListId).FirstOrDefault();
            lists.Remove(oldItem);
            lists.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = lists.Where((List arg) => arg.ListId == id).FirstOrDefault();
            lists.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<List> GetItemAsync(string id)
        {
            return await Task.FromResult(lists.FirstOrDefault(s => s.ListId == id));
        }

        public async Task<IEnumerable<List>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(lists);
        }
    }
}