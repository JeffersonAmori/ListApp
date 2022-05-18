using ListApp.Models;
using ListApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListApp.Services
{
    public class EfListItemDataStore : IDataStore<ListItem>
    {
        private ILogger _logger;

        ListContext _context;

        public EfListItemDataStore(ILogger logger, ListContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> AddItemAsync(ListItem listItems)
        {
            try
            {
                if (listItems == null) throw new ArgumentNullException("listItems");

                await _context.AddAsync(listItems);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            try
            {
                var itemToDelete = await _context.Lists.FirstAsync(list => list.ListId == id);
                _context.Remove(itemToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return false;
            }

            return true;
        }

        public async Task<ListItem> GetItemAsync(string id)
        {
            try
            {
                return await _context.ListItems.FirstAsync(list => list.Id == id);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ListItem>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                return await _context.ListItems.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return Enumerable.Empty<ListItem>();
            }
        }

        public async Task<bool> UpdateItemAsync(ListItem item)
        {
            try
            {
                _context.ListItems.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return false;
            }

            return true;
        }
    }
}
