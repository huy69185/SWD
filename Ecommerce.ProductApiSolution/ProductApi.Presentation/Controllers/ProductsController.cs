using eCommerce.ShareLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            //Get all product from repo
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No products detected in the database");
            //Convert data from entity to DTO and return
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(string id)
        {
            //Get single product from repo
            if (Guid.TryParse(id, out var guid))
            {
                return BadRequest("Invalid product ID format");
            }
            var product = await productInterface.FindByIdAsync(guid);
            if (product is null)
                return NotFound("Product requested not found");
            //Convert from entity to DTO and return
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound("Product not found");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            //Check model state is all data annotations are pass
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            //Conver to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.AddAsync(getEntity);
            return response.Flag is true? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(string id, [FromBody] UpdateProductDTO updateProductDTO)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest("Invalid product ID format");
            }
            var existingProduct = await productInterface.FindByIdAsync(guid);
            if (existingProduct is null)
            {
                return NotFound($"Product with ID {id} not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingProduct.Name = updateProductDTO.Name;
            existingProduct.Quantity = updateProductDTO.Quantity;
            existingProduct.Price = updateProductDTO.Price;

            var response = await productInterface.UpdateAsync(existingProduct);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest("Invalid product ID format");
            }
            //Check Model
            var product = await productInterface.FindByIdAsync(guid);
            if (product is null)
            {
                return NotFound($"Product with ID {id} not found");
            }
            //Convert to entity
            var response = await productInterface.DeleteAsync(product);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

    }
}
