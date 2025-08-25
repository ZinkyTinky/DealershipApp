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
        // GET LIST
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
        // GET SINGLE
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

            return File(image.ImageBinary, "image/jpeg");
        }

        // =========================
        // CREATE
        // =========================

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStockItem([FromForm] StockItemCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Manually deserialize newAccessories JSON string from form
            if (Request.Form.TryGetValue("newAccessories", out var newAccJson))
            {
                dto.NewAccessories = System.Text.Json.JsonSerializer.Deserialize<List<StockAccessoryDto>>(
                    newAccJson.ToString(),
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<StockAccessoryDto>();
            }

            var stockItem = await StockItemMapper.ToEntityAsync(dto);

            var createdItem = await _stockRepo.AddStockItemAsync(stockItem);
            return Ok(StockItemMapper.ToDto(createdItem));
        }


        // =========================
        // UPDATE
        // =========================

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStockItem([FromRoute] int id, [FromForm] StockItemUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _stockRepo.GetStockItemByIdAsync(id);
            if (entity == null) return NotFound();

            // Deserialize newAccessories from form JSON string if present
            if (Request.Form.TryGetValue("newAccessories", out var newAccJson))
            {
                var deserialized = JsonSerializer.Deserialize<List<StockAccessoryDto>>(
                    newAccJson.ToString(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (deserialized != null) dto.NewAccessories.AddRange(deserialized);
            }

            await StockItemMapper.UpdateEntityAsync(entity, dto);

            var removeImageIds = JsonSerializer.Deserialize<List<int>>(dto.RemoveImageIds ?? "[]") ?? new();
            var removeAccessoryIds = JsonSerializer.Deserialize<List<int>>(dto.RemoveAccessoryIds ?? "[]") ?? new();

            var success = await _stockRepo.UpdateStockItemAsync(entity, removeImageIds, removeAccessoryIds);
            if (!success) return NotFound();

            return Ok(StockItemMapper.ToDto(entity));
}



        // =========================
        // DELETE
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
