using DealershipBackEnd.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealershipBackEnd.Interfaces
{
    public interface IStockInterface
    {
        /// <summary>
        /// Get all stock items with optional pagination
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedResult<StockItem>> GetAllStockItemsAsync(int pageNumber = 1, int pageSize = 10);
        /// <summary>
        /// Get a paginated, sortable, searchable list of stock items
        /// </summary>
        /// <param name="searchTerm">Search by RegNo, Make, Model, or VIN</param>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="sortBy">Column to sort by (e.g., Make, ModelYear)</param>
        /// <param name="sortDesc">Sort descending if true</param>
        /// <returns>Paginated list of StockItems</returns>
         Task<PagedResult<StockItem>> GetStockItemsAsync(
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool sortDesc = false);

        /// <summary>
        /// Get stock image by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Image?> GetImageByIdAsync(int id);


        /// <summary>
        /// Get a single stock item by Id
        /// </summary>
        Task<StockItem?> GetStockItemByIdAsync(int id);

        /// <summary>
        /// Add a new stock item with up to 3 images
        /// </summary>
        Task<StockItem> AddStockItemAsync(StockItem stockItem);

        /// <summary>
        /// Update an existing stock item and optionally remove images that are not kept
        /// </summary>
        /// <param name="stockItem">The stock item entity to update</param>
        /// <param name="removeImageIds">Optional list of image IDs to remove; any other images will be kept</param>
        Task<bool> UpdateStockItemAsync(StockItem stockItem, List<int>? removeImageIds = null, List<int>? removeAccessoryIds = null);


        /// <summary>
        /// Delete an existing stock item along with its images
        /// </summary>
        Task<bool> DeleteStockItemAsync(int id);
    }
}
