using System.Text.Json;
using DealershipBackEnd.DTOs;
using DealershipBackEnd.Helpers;
using DealershipBackEnd.Interfaces;
using DealershipBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealershipBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockInterface _stockRepo;

        public StockController(IStockInterface stockRepo)
        {
            _stockRepo = stockRepo;
        }

        // =========================
        // GET LIST (search + pagination)
        // =========================

        [HttpGet("search")]
        public async Task<IActionResult> GetStockItems(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false)
        {
            var result = await _stockRepo.GetStockItemsAsync(search, pageNumber, pageSize, sortBy, sortDesc);

            // Map entities to DTOs
            var dtoResult = result.Items.Select(StockItemMapper.ToDto).ToList();

            return Ok(new { result.TotalCount, Items = dtoResult });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStockItems(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _stockRepo.GetAllStockItemsAsync(pageNumber, pageSize);
            var dtoResult = result.Items.Select(StockItemMapper.ToDto).ToList();
            return Ok(new { result.TotalCount, Items = dtoResult });
        }

        // =========================
        // GET SINGLE ITEM
        // =========================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockItem(int id)
        {
            var item = await _stockRepo.GetStockItemByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(StockItemMapper.ToDto(item));
        }

        // =========================
        // GET IMAGE
        // =========================

        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _stockRepo.GetImageByIdAsync(id);
            if (image == null) return NotFound();

            return File(image.ImageBinary, "image/jpeg"); // Serve raw binary as JPEG
        }

        // =========================
        // CREATE NEW STOCK ITEM
        // =========================

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStockItem([FromForm] StockItemCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Deserialize 'newAccessories' from form if present
            if (Request.Form.TryGetValue("newAccessories", out var newAccJson))
            {
                dto.NewAccessories = JsonSerializer.Deserialize<List<StockAccessoryDto>>(
                    newAccJson.ToString(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<StockAccessoryDto>();
            }

            var stockItem = await StockItemMapper.ToEntityAsync(dto);

            var createdItem = await _stockRepo.AddStockItemAsync(stockItem);
            return Ok(StockItemMapper.ToDto(createdItem));
        }

        // =========================
        // UPDATE EXISTING STOCK ITEM
        // =========================

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStockItem([FromRoute] int id, [FromForm] StockItemUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _stockRepo.GetStockItemByIdAsync(id);
            if (entity == null) return NotFound();

            // Deserialize newAccessories from form JSON string
            if (Request.Form.TryGetValue("newAccessories", out var newAccJson))
            {
                var deserialized = JsonSerializer.Deserialize<List<StockAccessoryDto>>(
                    newAccJson.ToString(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (deserialized != null) dto.NewAccessories.AddRange(deserialized);
            }

            // Update entity scalars and add new images/accessories
            await StockItemMapper.UpdateEntityAsync(entity, dto);

            // Deserialize removed IDs
            var removeImageIds = JsonSerializer.Deserialize<List<int>>(dto.RemoveImageIds ?? "[]") ?? new();
            var removeAccessoryIds = JsonSerializer.Deserialize<List<int>>(dto.RemoveAccessoryIds ?? "[]") ?? new();

            var success = await _stockRepo.UpdateStockItemAsync(entity, removeImageIds, removeAccessoryIds);
            if (!success) return NotFound();

            return Ok(StockItemMapper.ToDto(entity));
        }

        // =========================
        // DELETE STOCK ITEM
        // =========================

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteStockItem(int id)
        {
            var success = await _stockRepo.DeleteStockItemAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
