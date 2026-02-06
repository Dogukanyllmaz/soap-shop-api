using Application.DTOs;
using Application.DTOs.Conversions;
using Application.Interfaces;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet("get-products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            // get all products from database
            var products = await productInterface.GetAllAsync();
            if (products is null || !products.Any())
            {
                return NotFound("No product detected in the database");
            }
            //convert products to productDto
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product detected in the database");
        }

        [HttpGet("get-product/{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            // get product by id from database
            var product = await productInterface.FindByIdAsync(id);
            if (product is null || product.Id <= 0)
            {
                return NotFound($"Product with Id {id} not found");
            }
            // convert product to productDto
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound($"Product with Id {id} not found");
        }
        [HttpPost("add-product")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDto productDto)
        {
            //check if model state is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //convert productDto to product entity
            var getEntity = ProductConversion.ToEntity(productDto);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut("update-product")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDto productDto)
        {
            //check if model state is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //convert productDto to product entity
            var getEntity = ProductConversion.ToEntity(productDto);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("delete-product")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDto productDto)
        {
            var getEntity = ProductConversion.ToEntity(productDto);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

    }
}
