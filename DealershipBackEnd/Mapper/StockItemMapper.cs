using DealershipBackEnd.DTOs;
using DealershipBackEnd.Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DealershipBackEnd.Helpers
{
    public static class StockItemMapper
    {
        // Generates the API URL for an image by its ID
        private static string GetImageUrl(int imageId) => $"/Stock/image/{imageId}";

        /// <summary>
        /// Maps a StockItemCreateDto to a new StockItem entity.
        /// Adds new accessories and images if provided.
        /// </summary>
        /// <param name="dto">Data Transfer Object for creating stock item</param>
        /// <returns>New StockItem entity</returns>
        public static async Task<StockItem> ToEntityAsync(StockItemCreateDto dto)
        {
            var entity = new StockItem
            {
                RegNo = dto.RegNo,
                Make = dto.Make,
                Model = dto.Model,
                ModelYear = dto.ModelYear,
                KMS = dto.KMS,
                Colour = dto.Colour,
                VIN = dto.VIN,
                RetailPrice = dto.RetailPrice,
                CostPrice = dto.CostPrice,
                DTCreated = DateTime.UtcNow
            };

            // Add new accessories from DTO
            if (dto.NewAccessories != null)
            {
                foreach (var acc in dto.NewAccessories.Where(a => !string.IsNullOrWhiteSpace(a.Name)))
                {
                    entity.Accessories.Add(new StockAccessory
                    {
                        Name = acc.Name,
                        Description = acc.Description
                    });
                }
            }

            // Add new images from DTO
            if (dto.NewImages != null)
            {
                foreach (var file in dto.NewImages)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);

                        entity.Images.Add(new Image
                        {
                            Name = file.FileName,
                            ImageBinary = ms.ToArray()
                        });
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Maps a StockItem entity to a StockItemDto for API responses.
        /// Converts accessories and images to their respective DTOs.
        /// </summary>
        /// <param name="entity">StockItem entity</param>
        /// <returns>StockItemDto</returns>
        public static StockItemDto ToDto(StockItem entity)
        {
            return new StockItemDto
            {
                Id = entity.Id,
                RegNo = entity.RegNo,
                Make = entity.Make,
                Model = entity.Model,
                ModelYear = entity.ModelYear,
                KMS = entity.KMS,
                Colour = entity.Colour,
                VIN = entity.VIN,
                RetailPrice = entity.RetailPrice,
                CostPrice = entity.CostPrice,
                DTCreated = entity.DTCreated,
                DTUpdated = entity.DTUpdated,
                Accessories = entity.Accessories
                    .Select(a => new StockAccessoryDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description
                    })
                    .ToList(),
                Images = entity.Images.Select(i => new ImageDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    ImageUrl = GetImageUrl(i.Id)
                }).ToList()
            };
        }

        /// <summary>
        /// Updates an existing StockItem entity with data from StockItemUpdateDto.
        /// Adds new accessories and images if provided.
        /// </summary>
        /// <param name="entity">Existing StockItem entity to update</param>
        /// <param name="dto">DTO containing updated data</param>
        public static async Task UpdateEntityAsync(StockItem entity, StockItemUpdateDto dto)
        {
            // Update scalar properties
            entity.RegNo = dto.RegNo;
            entity.Make = dto.Make;
            entity.Model = dto.Model;
            entity.ModelYear = dto.ModelYear;
            entity.KMS = dto.KMS;
            entity.Colour = dto.Colour;
            entity.VIN = dto.VIN;
            entity.RetailPrice = dto.RetailPrice;
            entity.CostPrice = dto.CostPrice;
            entity.DTUpdated = DateTime.UtcNow;

            // Add new accessories
            if (dto.NewAccessories != null)
            {
                foreach (var acc in dto.NewAccessories.Where(a => !string.IsNullOrWhiteSpace(a.Name)))
                {
                    entity.Accessories.Add(new StockAccessory
                    {
                        Name = acc.Name,
                        Description = acc.Description
                    });
                }
            }

            // Add new images
            if (dto.NewImages != null)
            {
                foreach (var file in dto.NewImages)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);

                        entity.Images.Add(new Image
                        {
                            Name = file.FileName,
                            ImageBinary = ms.ToArray()
                        });
                    }
                }
            }
        }
    }
}
