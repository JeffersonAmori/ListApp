using ListApp.Models;
using ListApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListApp.Services
{
    public class EfListItemDataStore : IDataStore<ListItem>
    {
        ListContext _context = new ListContext();

        public async Task<bool> AddItemAsync(ListItem listItems)
        {
            if (listItems == null) throw new ArgumentNullException("listItems");

            await _context.AddAsync(listItems);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var itemToDelete = await _context.Lists.FirstAsync(list => list.ListId == id);
            _context.Remove(itemToDelete);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ListItem> GetItemAsync(string id)
        {
            return await _context.ListItems.FirstAsync(list => list.Id == id);
        }

        public async Task<IEnumerable<ListItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await _context.ListItems.ToListAsync();
        }

        public async Task<bool> UpdateItemAsync(ListItem item)
        {
            _context.ListItems.Update(item);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
