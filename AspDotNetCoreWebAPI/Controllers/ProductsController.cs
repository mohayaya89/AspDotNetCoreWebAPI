using AspDotNetCoreWebAPI.Models;
using AspDotNetCoreWebAPI.Models.Dto;
using AspDotNetCoreWebAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspDotNetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllAsync();

            var dtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IsAvailable = p.IsAvailable,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null) return NotFound();
            
            var dto = new ProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Sku = createDto.Sku,
                Name = createDto.Name,
                Description = createDto.Description ?? string.Empty,
                Price = createDto.Price,
                IsAvailable = createDto.IsAvailable,
                CategoryId = createDto.CategoryId
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            var dto = new ProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };

            return CreatedAtAction(nameof(GetProduct), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Sku = updateDto.Sku;
            product.Name = updateDto.Name;
            product.Description = updateDto.Description ?? string.Empty;
            product.Price = updateDto.Price;
            product.IsAvailable = updateDto.IsAvailable;
            product.CategoryId = updateDto.CategoryId;

            _repository.Update(product);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}