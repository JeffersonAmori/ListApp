using ListApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Services.Interfaces;

namespace ListApp.Services
{
    public class EfListDataStore : IDataStore<List>
    {
        ListContext _context = new ListContext();

        public async Task<bool> AddItemAsync(List item)
        {
            if (item == null) throw new ArgumentNullException("item");

            await _context.AddAsync(item);
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

        public async Task<List> GetItemAsync(string id)
        {
            List list = await _context.Lists.FirstAsync(l => l.ListId == id);
            list.ListItems = _context.ListItems.Where(x => x.ListId == list.ListId).ToList();
            return list;
        }

        public async Task<IEnumerable<List>> GetItemsAsync(bool forceRefresh = false)
        {
            return await _context.Lists.ToListAsync();
        }

        public async Task<bool> UpdateItemAsync(List list)
        {
            _context.Lists.Update(list);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}