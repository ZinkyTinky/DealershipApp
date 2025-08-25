using DealershipBackEnd.Data;
using DealershipBackEnd.Interfaces;
using DealershipBackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealershipBackEnd.Repositories
{
    /// <summary>
    /// Repository for managing StockItems, Accessories, and Images in the database.
    /// Implements IStockInterface for abstraction.
    /// </summary>
    public class StockRepository : IStockInterface
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all stock items with paging. No search or sorting applied.
        /// </summary>
        public async Task<PagedResult<StockItem>> GetAllStockItemsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.StockItems
                .Include(s => s.Images)
                .Include(s => s.Accessories);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<StockItem>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get stock items with optional search, sorting, and paging.
        /// </summary>
        public async Task<PagedResult<StockItem>> GetStockItemsAsync(
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool sortDesc = false)
        {
            var query = _context.StockItems
                .Include(s => s.Images)
                .Include(s => s.Accessories)
                .AsQueryable();

            // --- Search ---
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerTerm = searchTerm.ToLower();
                query = query.Where(s =>
                    s.RegNo.Trim().ToLower().Contains(lowerTerm) ||
                    s.Make.ToLower().Contains(lowerTerm) ||
                    s.Model.ToLower().Contains(lowerTerm) ||
                    s.VIN.ToLower().Contains(lowerTerm));
            }

            var totalCount = await query.CountAsync();

            // --- Sorting ---
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "retailprice":
                        query = sortDesc 
                            ? query.OrderByDescending(e => e.RetailPrice)
                            : query.OrderBy(e => e.RetailPrice);
                        break;
                    case "make":
                        query = sortDesc 
                            ? query.OrderByDescending(e => e.Make)
                            : query.OrderBy(e => e.Make);
                        break;
                    case "modelyear":
                        query = sortDesc
                            ? query.OrderByDescending(e => e.ModelYear)
                            : query.OrderBy(e => e.ModelYear);
                        break;
                    default:
                        query = query.OrderBy(e => e.Id); // fallback
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<StockItem>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get a single image by its ID.
        /// </summary>
        public async Task<Image?> GetImageByIdAsync(int id)
        {
            return await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
        }

        /// <summary>
        /// Get a single stock item by its ID, including images and accessories.
        /// </summary>
        public async Task<StockItem?> GetStockItemByIdAsync(int id)
        {
            return await _context.StockItems
                .Include(s => s.Images)
                .Include(s => s.Accessories)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Add a new stock item to the database.
        /// </summary>
        public async Task<StockItem> AddStockItemAsync(StockItem stockItem)
        {
            _context.StockItems.Add(stockItem);
            await _context.SaveChangesAsync();
            return stockItem;
        }

        /// <summary>
        /// Update an existing stock item, including handling of images and accessories.
        /// </summary>
        public async Task<bool> UpdateStockItemAsync(
            StockItem stockItem, 
            List<int>? removeImageIds = null,
            List<int>? removeAccessoryIds = null)
        {
            var existingItem = await _context.StockItems
                .Include(s => s.Images)
                .Include(s => s.Accessories)
                .FirstOrDefaultAsync(s => s.Id == stockItem.Id);

            if (existingItem == null) return false;

            // --- Update scalar fields ---
            existingItem.RegNo = stockItem.RegNo;
            existingItem.Make = stockItem.Make;
            existingItem.Model = stockItem.Model;
            existingItem.ModelYear = stockItem.ModelYear;
            existingItem.KMS = stockItem.KMS;
            existingItem.Colour = stockItem.Colour;
            existingItem.VIN = stockItem.VIN;
            existingItem.RetailPrice = stockItem.RetailPrice;
            existingItem.CostPrice = stockItem.CostPrice;
            existingItem.DTUpdated = stockItem.DTUpdated;

            // --- Remove selected accessories ---
            if (removeAccessoryIds != null && removeAccessoryIds.Any())
            {
                var accessoriesToRemove = existingItem.Accessories
                    .Where(a => removeAccessoryIds.Contains(a.Id))
                    .ToList();

                foreach (var acc in accessoriesToRemove)
                {
                    existingItem.Accessories.Remove(acc);
                    _context.StockAccessories.Remove(acc);
                }
            }

            // --- Add new accessories ---
            var newAccessories = stockItem.Accessories
                .Where(a => a.Id == 0)
                .ToList();

            foreach (var acc in newAccessories)
            {
                acc.StockItemId = existingItem.Id;
                _context.StockAccessories.Add(acc);
            }

            // --- Remove selected images ---
            if (removeImageIds != null && removeImageIds.Any())
            {
                var imagesToRemove = existingItem.Images
                    .Where(i => removeImageIds.Contains(i.Id))
                    .ToList();

                foreach (var img in imagesToRemove)
                {
                    existingItem.Images.Remove(img);
                    _context.Images.Remove(img);
                }
            }

            // --- Add new images ---
            var newImages = stockItem.Images
                .Where(i => i.Id == 0)
                .ToList();

            foreach (var img in newImages)
            {
                img.StockItemId = existingItem.Id;
                existingItem.Images.Add(img);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Delete a stock item and all its related images and accessories.
        /// </summary>
        public async Task<bool> DeleteStockItemAsync(int id)
        {
            var existingItem = await _context.StockItems
                .Include(s => s.Images)
                .Include(s => s.Accessories)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingItem == null) return false;

            _context.Images.RemoveRange(existingItem.Images);
            _context.StockAccessories.RemoveRange(existingItem.Accessories);
            _context.StockItems.Remove(existingItem);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
