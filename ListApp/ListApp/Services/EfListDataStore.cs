using ListApp.Models;
using ListApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListApp.Services
{
    public class EfListDataStore : IDataStore<List>
    {
        private ILogger _logger;

        ListContext _context;

        public EfListDataStore(ILogger logger, ListContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> AddItemAsync(List item)
        {
            try
            {
                if (item == null) throw new ArgumentNullException("item");

                await _context.AddAsync(item);
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

        public async Task<List> GetItemAsync(string id)
        {
            try
            {
                return await _context.Lists
                    .Include(x => x.ListItems)
                    .FirstAsync(l => l.ListId == id);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return null;
            }
        }

        public async Task<IEnumerable<List>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                return await _context.Lists
                    .Include(x => x.ListItems)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return Enumerable.Empty<List>();
            }

        }

        public async Task<bool> UpdateItemAsync(List list)
        {
            try
            {
                _context.Lists.Update(list);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return false;
            }
        }
    }
}